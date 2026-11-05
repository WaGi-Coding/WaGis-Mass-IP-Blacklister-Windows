import { spawn } from 'node:child_process';
import { isIP } from 'node:net';
import { release as osRelease } from 'node:os';
import { applyAddressExclusions, extractAddresses } from './ip-list.mjs';

const DEFAULT_LIST_URL = 'https://lists.blocklist.de/lists/all.txt';
const MAX_LIST_BYTES = 25 * 1024 * 1024;

const ADD_RULES_SCRIPT = String.raw`
$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
$addresses = @($payload.addresses)
$protocolName = [string]$payload.protocol
$protocol = switch ($protocolName) {
  'TCP' { 6 }
  'UDP' { 17 }
  'ALL' { 256 }
  default { throw 'Unknown protocol.' }
}
$directions = @($payload.directions)
$policy = New-Object -ComObject HNetCfg.FwPolicy2
$rules = $policy.Rules
$operationId = [Guid]::NewGuid().ToString('N')
$createdNames = [System.Collections.Generic.List[string]]::new()
$added = 0

try {
  foreach ($directionName in $directions) {
    if ($directionName -eq 'IN') { $direction = 1 }
    elseif ($directionName -eq 'OUT') { $direction = 2 }
    else { throw 'Unknown firewall direction.' }

    $baseName = "WaGi IP-Blacklist $directionName $protocolName"
    $blockNumber = 0
    for ($offset = 0; $offset -lt $addresses.Count; $offset += $payload.blockSize) {
      $end = [Math]::Min($offset + $payload.blockSize, $addresses.Count)
      $chunk = @($addresses[$offset..($end - 1)])
      $blockNumber++
      $ruleName = "$baseName $operationId-$blockNumber"
      $rule = New-Object -ComObject HNetCfg.FWRule
      $rule.Name = $ruleName
      $rule.Description = 'Created by WaGi IP-Blacklister'
      $rule.Enabled = $true
      $rule.Direction = $direction
      $rule.Action = 0
      $rule.Protocol = $protocol
      $rule.Profiles = 2147483647
      $rule.RemoteAddresses = $chunk -join ','
      $rules.Add($rule)
      [void]$createdNames.Add($ruleName)
      $added++
    }
  }
} catch {
  foreach ($name in $createdNames) {
    try { $null = $rules.Remove($name) } catch {}
  }
  [Console]::Error.WriteLine($_.Exception.Message)
  exit 1
}

$result = [ordered]@{ rulesAdded = $added }
[Console]::Out.WriteLine(($result | ConvertTo-Json -Compress))
`;

const DELETE_RULES_SCRIPT = String.raw`
$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
$policy = New-Object -ComObject HNetCfg.FwPolicy2
$rules = $policy.Rules
$prefixes = [System.Collections.Generic.List[string]]::new()
foreach ($direction in @($payload.directions)) {
  foreach ($protocol in @($payload.protocols)) {
    [void]$prefixes.Add("WaGi IP-Blacklist $direction $protocol")
  }
}

$names = [System.Collections.Generic.List[string]]::new()
foreach ($rule in $rules) {
  $name = [string]$rule.Name
  foreach ($prefix in $prefixes) {
    if ($name.Equals($prefix, [System.StringComparison]::OrdinalIgnoreCase) -or
        $name.StartsWith("$prefix ", [System.StringComparison]::OrdinalIgnoreCase)) {
      [void]$names.Add($name)
      break
    }
  }
}

$removed = 0
foreach ($name in $names) {
  try {
    $null = $rules.Remove($name)
    $removed++
  } catch {}
}
$result = [ordered]@{ rulesRemoved = $removed }
[Console]::Out.WriteLine(($result | ConvertTo-Json -Compress))
`;

let autoTimer;
let autoRunning = false;

function sendAutoStatus(status) {
  process.parentPort.postMessage({ type: 'auto-status', status });
}

function normalizeSettings(settings = {}) {
  const protocol = ['TCP', 'UDP', 'ALL'].includes(settings.protocol)
    ? settings.protocol
    : 'ALL';
  const intervalHours = Math.max(
    0,
    Math.min(
      24,
      Number.isInteger(Number(settings.intervalHours))
        ? Number(settings.intervalHours)
        : 0,
    ),
  );
  let intervalMinutes = Math.max(
    0,
    Math.min(
      59,
      Number.isInteger(Number(settings.intervalMinutes))
        ? Number(settings.intervalMinutes)
        : 5,
    ),
  );
  if (intervalHours === 24) intervalMinutes = 0;
  if (intervalHours === 0 && intervalMinutes === 0) intervalMinutes = 1;

  const inbound = settings.inbound !== false;
  return {
    protocol,
    inbound,
    outbound: settings.outbound !== false || !inbound,
    excludeIPs: Array.isArray(settings.excludeIPs)
      ? settings.excludeIPs.filter(
          (address) => typeof address === 'string' && isIP(address),
        )
      : [],
    intervalHours,
    intervalMinutes,
    doOnce: settings.doOnce !== false,
    listUrl:
      typeof settings.listUrl === 'string'
        ? settings.listUrl
        : DEFAULT_LIST_URL,
  };
}

function getSelectedDirections(settings) {
  const directions = [];
  if (settings.inbound) directions.push('IN');
  if (settings.outbound) directions.push('OUT');
  if (directions.length === 0) {
    throw new Error('Select at least one firewall direction.');
  }
  return directions;
}

function getBlockSize() {
  const build = Number(osRelease().split('.')[2]);
  return build >= 10_240 ? 10_000 : 1_000;
}

function runPowerShellJson(script, payload = {}) {
  const encodedScript = Buffer.from(script, 'utf16le').toString('base64');

  return new Promise((resolve, reject) => {
    const child = spawn(
      'powershell.exe',
      [
        '-NoLogo',
        '-NoProfile',
        '-NonInteractive',
        '-ExecutionPolicy',
        'Bypass',
        '-EncodedCommand',
        encodedScript,
      ],
      { windowsHide: true, stdio: ['pipe', 'pipe', 'pipe'] },
    );
    const stdout = [];
    const stderr = [];

    child.stdout.on('data', (chunk) => stdout.push(chunk));
    child.stderr.on('data', (chunk) => stderr.push(chunk));
    child.on('error', reject);
    child.on('close', (code) => {
      const output = Buffer.concat(stdout).toString('utf8').trim();
      const errorOutput = Buffer.concat(stderr).toString('utf8').trim();

      if (code !== 0) {
        reject(
          new Error(
            errorOutput || output || `PowerShell exited with code ${code}.`,
          ),
        );
        return;
      }

      try {
        resolve(JSON.parse(output.split(/\r?\n/).at(-1)));
      } catch {
        reject(new Error(output || 'PowerShell returned an invalid response.'));
      }
    });
    child.stdin.on('error', reject);
    child.stdin.end(JSON.stringify(payload), 'utf8');
  });
}

async function downloadList(url) {
  let parsedUrl;
  try {
    parsedUrl = new URL(url);
  } catch {
    throw new Error('Enter a valid .txt URL.');
  }

  if (!['http:', 'https:'].includes(parsedUrl.protocol)) {
    throw new Error('Only HTTP and HTTPS list URLs are supported.');
  }
  if (!parsedUrl.pathname.toLowerCase().endsWith('.txt')) {
    throw new Error('The IP list URL must end in .txt.');
  }

  const response = await fetch(parsedUrl, {
    headers: { 'User-Agent': 'WaGis-Mass-IP-Blacklister-Windows' },
    signal: AbortSignal.timeout(30_000),
  });
  if (!response.ok) {
    throw new Error(`List download failed: HTTP ${response.status}.`);
  }

  const text = await response.text();
  if (Buffer.byteLength(text, 'utf8') > MAX_LIST_BYTES) {
    throw new Error('The downloaded list is larger than 25 MB.');
  }
  return text;
}

function getFilteredAddresses(text, exclusions) {
  const addresses = extractAddresses(text);
  const filtered = applyAddressExclusions(addresses, exclusions);
  if (filtered.length === 0) {
    throw new Error('No valid IP addresses or CIDR ranges remain in the list.');
  }
  return { addresses: filtered, found: addresses.length };
}

function previewAddressList(text, exclusions) {
  const source = String(text ?? '');
  const addresses = extractAddresses(source);
  const filtered = applyAddressExclusions(addresses, exclusions);
  return {
    lines: source.trim() ? source.split(/\r\n|\r|\n/).length : 0,
    addressesFound: addresses.length,
    addressesRemaining: filtered.length,
    excludedCount: addresses.length - filtered.length,
  };
}

async function addFirewallRules({ text, settings }) {
  if (process.platform !== 'win32') {
    throw new Error('Firewall rules can only be changed on Windows.');
  }

  const normalized = normalizeSettings(settings);
  const directions = getSelectedDirections(normalized);
  const { addresses, found } = getFilteredAddresses(
    text,
    normalized.excludeIPs,
  );
  const result = await runPowerShellJson(ADD_RULES_SCRIPT, {
    addresses,
    protocol: normalized.protocol,
    directions,
    blockSize: getBlockSize(),
  });

  return {
    addressesAdded: addresses.length,
    exclusionsApplied: found - addresses.length,
    rulesAdded: result.rulesAdded,
  };
}

async function deleteFirewallRules({
  protocol,
  directions,
  allProtocols = false,
}) {
  if (process.platform !== 'win32') {
    throw new Error('Firewall rules can only be changed on Windows.');
  }

  const selectedDirections = directions.filter((direction) =>
    ['IN', 'OUT'].includes(direction),
  );
  if (selectedDirections.length === 0) {
    throw new Error('Select at least one firewall direction.');
  }

  const protocols = allProtocols
    ? ['TCP', 'UDP', 'ALL']
    : ['TCP', 'UDP', 'ALL'].includes(protocol)
      ? [protocol]
      : [];
  if (protocols.length === 0) {
    throw new Error('Select a valid protocol.');
  }

  return runPowerShellJson(DELETE_RULES_SCRIPT, {
    protocols,
    directions: selectedDirections,
  });
}

async function runAutomaticCycle(settings) {
  if (autoRunning) return;
  autoRunning = true;
  sendAutoStatus({ state: 'running', message: 'Getting list...' });

  try {
    const text = await downloadList(settings.listUrl);
    getFilteredAddresses(text, settings.excludeIPs);
    const directions = getSelectedDirections(settings);
    sendAutoStatus({
      state: 'running',
      message: 'Replacing firewall rules...',
    });
    await deleteFirewallRules({ protocol: settings.protocol, directions });
    const added = await addFirewallRules({ text, settings });
    sendAutoStatus({
      state: 'complete',
      message: `Updated ${added.addressesAdded.toLocaleString('en-US')} addresses in ${added.rulesAdded} rules.`,
    });
  } catch (error) {
    sendAutoStatus({ state: 'error', message: error.message });
  } finally {
    autoRunning = false;
  }
}

async function handleRequest(method, payload) {
  switch (method) {
    case 'download-list':
      return downloadList(payload);
    case 'preview-list':
      return previewAddressList(payload.text, payload.excludeIPs ?? []);
    case 'add-rules':
      return addFirewallRules(payload);
    case 'delete-rules':
      return deleteFirewallRules(payload);
    case 'delete-all-rules':
      return deleteFirewallRules({ directions: payload, allProtocols: true });
    case 'start-auto-mode': {
      const settings = normalizeSettings(payload);
      getSelectedDirections(settings);
      if (autoTimer) clearInterval(autoTimer);
      const interval =
        (settings.intervalHours * 60 + settings.intervalMinutes) * 60_000;
      autoTimer = setInterval(() => {
        void runAutomaticCycle(settings);
      }, interval);
      if (settings.doOnce) void runAutomaticCycle(settings);
      return { running: true };
    }
    case 'stop-auto-mode':
      if (autoTimer) clearInterval(autoTimer);
      autoTimer = null;
      sendAutoStatus({ state: 'stopped', message: 'Automatic mode stopped.' });
      return { running: false };
    default:
      throw new Error(`Unknown background task: ${method}`);
  }
}

process.parentPort.on('message', async ({ data }) => {
  const { id, method, payload } = data;
  try {
    const result = await handleRequest(method, payload);
    process.parentPort.postMessage({ id, result });
  } catch (error) {
    process.parentPort.postMessage({
      id,
      error: error instanceof Error ? error.message : String(error),
    });
  }
});
