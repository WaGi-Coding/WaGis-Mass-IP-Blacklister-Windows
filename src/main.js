import {
  app,
  BrowserWindow,
  clipboard,
  globalShortcut,
  ipcMain,
  Menu,
  nativeImage,
  Tray,
} from 'electron';
import { createHash } from 'node:crypto';
import { spawn } from 'node:child_process';
import { createWriteStream } from 'node:fs';
import { mkdir, readFile, rm, writeFile } from 'node:fs/promises';
import https from 'node:https';
import { isIP } from 'node:net';
import { release as osRelease } from 'node:os';
import path from 'node:path';
import { applyAddressExclusions, extractAddresses } from './ip-list.mjs';

const GITHUB_LATEST_RELEASE_URL =
  'https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases/latest';
const UPDATE_ASSET_PREFIX = 'WaGis-Mass-IP-Blacklister-Windows.Setup.';
const UPDATE_DOWNLOAD_HOSTS = new Set([
  'github.com',
  'release-assets.githubusercontent.com',
  'objects.githubusercontent.com',
]);
const STARTUP_TASK_NAME = 'WaGis-Mass-IP-Blacklister-Windows-Startup';
const APP_TITLE = `WaGi's Mass IP-Blacklister Windows - v${app.getVersion()}`;

const DEFAULT_SETTINGS = {
  protocol: 'ALL',
  inbound: true,
  outbound: true,
  excludeIPs: [],
  theme: 'system',
  intervalHours: 0,
  intervalMinutes: 5,
  doOnce: true,
  automaticMode: false,
  automaticUpdates: false,
  runAtStartup: false,
  startMinimized: false,
  listUrl: 'https://lists.blocklist.de/lists/all.txt',
};

let updateCheckInProgress = false;

function sendUpdateStatus(status, message, progress) {
  const payload = { status, message, progress };
  for (const window of [mainWindow, settingsWindow]) {
    if (window && !window.isDestroyed()) {
      window.webContents.send('update-status', payload);
    }
  }
}

function parseReleaseVersion(version) {
  const match =
    /^v?(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?(?:-([0-9A-Za-z.-]+))?(?:\+[0-9A-Za-z.-]+)?$/.exec(
      String(version),
    );
  if (!match) throw new Error(`Unsupported release version: ${version}`);

  return {
    parts: [match[1], match[2], match[3], match[4] ?? '0'].map(Number),
    prerelease: match[5] ?? '',
  };
}

function compareReleaseVersions(left, right) {
  const leftVersion = parseReleaseVersion(left);
  const rightVersion = parseReleaseVersion(right);

  for (let index = 0; index < leftVersion.parts.length; index += 1) {
    if (leftVersion.parts[index] !== rightVersion.parts[index]) {
      return leftVersion.parts[index] - rightVersion.parts[index];
    }
  }

  if (!leftVersion.prerelease && rightVersion.prerelease) return 1;
  if (leftVersion.prerelease && !rightVersion.prerelease) return -1;
  return leftVersion.prerelease.localeCompare(rightVersion.prerelease);
}

async function getLatestRelease() {
  const response = await fetch(GITHUB_LATEST_RELEASE_URL, {
    headers: {
      Accept: 'application/vnd.github+json',
      'User-Agent': 'WaGis-Mass-IP-Blacklister-Windows',
    },
    signal: AbortSignal.timeout(15_000),
  });
  if (!response.ok) {
    throw new Error(`GitHub update check failed (HTTP ${response.status}).`);
  }
  return response.json();
}

function downloadUpdateAsset(asset, destination) {
  return new Promise((resolve, reject) => {
    let settled = false;
    let output;

    const fail = (error) => {
      if (settled) return;
      settled = true;
      output?.destroy();
      void rm(destination, { force: true }).finally(() => reject(error));
    };

    const download = (url, redirectCount = 0) => {
      let parsedUrl;
      try {
        parsedUrl = new URL(url);
      } catch {
        fail(new Error('GitHub returned an invalid installer URL.'));
        return;
      }

      if (
        parsedUrl.protocol !== 'https:' ||
        !UPDATE_DOWNLOAD_HOSTS.has(parsedUrl.hostname) ||
        redirectCount > 5
      ) {
        fail(new Error('The update download URL or redirect is not trusted.'));
        return;
      }

      const request = https.get(
        parsedUrl,
        { headers: { 'User-Agent': 'WaGis-Mass-IP-Blacklister-Windows' } },
        (response) => {
          if (
            [301, 302, 303, 307, 308].includes(response.statusCode) &&
            response.headers.location
          ) {
            const nextUrl = new URL(response.headers.location, parsedUrl);
            response.resume();
            download(nextUrl.href, redirectCount + 1);
            return;
          }

          if (response.statusCode !== 200) {
            response.resume();
            fail(
              new Error(
                `Update download failed (HTTP ${response.statusCode}).`,
              ),
            );
            return;
          }

          output = createWriteStream(destination);
          const hash = createHash('sha256');
          const totalBytes = Number(
            response.headers['content-length'] || asset.size || 0,
          );
          let downloadedBytes = 0;
          let lastPercent = -1;

          response.on('data', (chunk) => {
            hash.update(chunk);
            downloadedBytes += chunk.length;
            const percent = totalBytes
              ? Math.floor((downloadedBytes / totalBytes) * 100)
              : 0;
            if (percent !== lastPercent) {
              lastPercent = percent;
              sendUpdateStatus(
                'downloading',
                `Downloading update: ${percent}%`,
                percent,
              );
            }
          });
          response.once('error', fail);
          output.once('error', fail);
          output.once('finish', () => {
            output.close(() => {
              const expectedDigest = /^sha256:(.+)$/i.exec(
                asset.digest || '',
              )?.[1];
              const actualDigest = hash.digest('hex');
              if (
                expectedDigest &&
                actualDigest.toLowerCase() !== expectedDigest.toLowerCase()
              ) {
                fail(
                  new Error('The update failed its SHA-256 integrity check.'),
                );
                return;
              }
              if (!settled) {
                settled = true;
                resolve();
              }
            });
          });
          response.pipe(output);
        },
      );

      request.setTimeout(60_000, () =>
        request.destroy(new Error('Update download timed out.')),
      );
      request.once('error', fail);
    };

    download(asset.browser_download_url);
  });
}

async function launchInstallerAfterExit(installerPath) {
  const scriptPath = path.join(
    path.dirname(installerPath),
    `install-update-${process.pid}.vbs`,
  );
  const escapedInstallerPath = installerPath.replace(/"/g, '""');
  const script = [
    'Set shell = CreateObject("WScript.Shell")',
    'Set wmi = GetObject("winmgmts:\\\\.\\root\\cimv2")',
    `processId = ${process.pid}`,
    'For attempt = 1 To 60',
    '  Set processes = wmi.ExecQuery("SELECT * FROM Win32_Process WHERE ProcessId = " & processId)',
    '  If processes.Count = 0 Then Exit For',
    '  WScript.Sleep 500',
    'Next',
    `shell.Run """${escapedInstallerPath}""", 1, False`,
    'CreateObject("Scripting.FileSystemObject").DeleteFile WScript.ScriptFullName, True',
    '',
  ].join('\r\n');
  await writeFile(scriptPath, script, 'ascii');

  const launcher = spawn('wscript.exe', ['//B', '//NoLogo', scriptPath], {
    detached: true,
    windowsHide: true,
    stdio: 'ignore',
  });
  await new Promise((resolve, reject) => {
    launcher.once('error', (error) => {
      void rm(scriptPath, { force: true });
      reject(error);
    });
    launcher.once('spawn', resolve);
  });
  launcher.unref();
}

async function checkForUpdates() {
  if (!app.isPackaged) {
    const error = 'Update checks are available in the installed app.';
    sendUpdateStatus('error', error);
    return {
      success: false,
      error,
    };
  }
  if (process.platform !== 'win32') {
    const error = 'Updates are currently available for Windows only.';
    sendUpdateStatus('error', error);
    return {
      success: false,
      error,
    };
  }
  if (updateCheckInProgress) return { success: true };

  updateCheckInProgress = true;
  try {
    sendUpdateStatus('checking', 'Checking GitHub for updates...');
    const release = await getLatestRelease();
    const releaseVersion = String(release.tag_name ?? '').replace(/^v/, '');
    if (compareReleaseVersions(releaseVersion, app.getVersion()) <= 0) {
      sendUpdateStatus(
        'current',
        `WaGis-Mass-IP-Blacklister-Windows v${app.getVersion()} is up to date.`,
      );
      return { success: true, updateAvailable: false };
    }

    const installerName = `${UPDATE_ASSET_PREFIX}${releaseVersion}.exe`;
    const installerAsset = release.assets?.find(
      (asset) => asset.name === installerName,
    );
    if (!installerAsset) {
      throw new Error(
        `Release ${release.tag_name} has no ${installerName} asset.`,
      );
    }

    sendUpdateStatus(
      'available',
      `Update v${releaseVersion} found. Downloading...`,
    );
    const updateDirectory = path.join(
      app.getPath('temp'),
      'WaGis-Mass-IP-Blacklister-Windows',
      'updates',
    );
    await mkdir(updateDirectory, { recursive: true });
    const installerPath = path.join(updateDirectory, installerName);
    await downloadUpdateAsset(installerAsset, installerPath);

    sendUpdateStatus(
      'installing',
      `Installing v${releaseVersion}. The application will close first.`,
    );
    await launchInstallerAfterExit(installerPath);
    app.quit();
    return { success: true, updateAvailable: true };
  } catch (error) {
    const message =
      error instanceof Error ? error.message.split(/\r?\n/)[0] : String(error);
    sendUpdateStatus('error', `Update check failed: ${message}`);
    return { success: false, error: message };
  } finally {
    updateCheckInProgress = false;
  }
}

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

let mainWindow;
let infoWindow;
let settingsWindow;
let tray;
let trayUnavailableReason;
let autoTimer;
let autoRunning = false;

function getPublicAssetPath(fileName) {
  const basePath = app.isPackaged ? process.resourcesPath : app.getAppPath();
  return app.isPackaged
    ? path.join(basePath, fileName)
    : path.join(basePath, 'public', fileName);
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

function runWindowsCommand(command, args) {
  return new Promise((resolve, reject) => {
    const child = spawn(command, args, {
      windowsHide: true,
      stdio: ['ignore', 'pipe', 'pipe'],
    });
    const stdout = [];
    const stderr = [];
    child.stdout.on('data', (chunk) => stdout.push(chunk));
    child.stderr.on('data', (chunk) => stderr.push(chunk));
    child.once('error', reject);
    child.once('close', (code) => {
      const output = Buffer.concat(stdout).toString('utf8').trim();
      const errorOutput = Buffer.concat(stderr).toString('utf8').trim();
      if (code === 0) {
        resolve(output);
      } else {
        reject(
          new Error(
            errorOutput || output || `${command} exited with code ${code}.`,
          ),
        );
      }
    });
  });
}

async function setRunAtStartup(enabled) {
  if (process.platform !== 'win32') {
    throw new Error('Windows startup tasks are only available on Windows.');
  }
  if (enabled && !app.isPackaged) {
    throw new Error('Install the app before enabling Windows startup.');
  }

  if (enabled) {
    await runWindowsCommand('schtasks.exe', [
      '/Create',
      '/SC',
      'ONLOGON',
      '/TN',
      STARTUP_TASK_NAME,
      '/TR',
      `"${process.execPath}" --autostart`,
      '/RL',
      'HIGHEST',
      '/F',
      '/IT',
    ]);
  } else {
    const taskExists = await runWindowsCommand('schtasks.exe', [
      '/Query',
      '/TN',
      STARTUP_TASK_NAME,
    ])
      .then(() => true)
      .catch(() => false);

    if (taskExists) {
      await runWindowsCommand('schtasks.exe', [
        '/Delete',
        '/TN',
        STARTUP_TASK_NAME,
        '/F',
      ]);
    }
  }

  const settings = await loadSettings();
  return saveSettings({ ...settings, runAtStartup: enabled });
}

function getBlockSize() {
  const build = Number(osRelease().split('.')[2]);
  return build >= 10_240 ? 10_000 : 1_000;
}

function normalizeSettings(settings = {}) {
  const protocol = ['TCP', 'UDP', 'ALL'].includes(settings.protocol)
    ? settings.protocol
    : DEFAULT_SETTINGS.protocol;
  let intervalHours = Number(settings.intervalHours);
  let intervalMinutes = Number(settings.intervalMinutes);

  if (!Number.isInteger(intervalHours))
    intervalHours = DEFAULT_SETTINGS.intervalHours;
  if (!Number.isInteger(intervalMinutes))
    intervalMinutes = DEFAULT_SETTINGS.intervalMinutes;
  intervalHours = Math.max(0, Math.min(24, intervalHours));
  intervalMinutes = Math.max(0, Math.min(59, intervalMinutes));
  if (intervalHours === 24) intervalMinutes = 0;
  if (intervalHours === 0 && intervalMinutes === 0) intervalMinutes = 1;

  const excludeIPs = Array.isArray(settings.excludeIPs)
    ? settings.excludeIPs.filter(
        (address) => typeof address === 'string' && isIP(address),
      )
    : [];
  const inbound = settings.inbound !== false;
  const outbound = settings.outbound !== false || !inbound;

  return {
    protocol,
    inbound,
    outbound,
    excludeIPs,
    theme: ['light', 'dark'].includes(settings.theme)
      ? settings.theme
      : 'system',
    intervalHours,
    intervalMinutes,
    doOnce: settings.doOnce !== false,
    automaticMode: settings.automaticMode === true,
    automaticUpdates: settings.automaticUpdates === true,
    runAtStartup: settings.runAtStartup === true,
    startMinimized: settings.startMinimized === true,
    listUrl:
      typeof settings.listUrl === 'string'
        ? settings.listUrl
        : DEFAULT_SETTINGS.listUrl,
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

function getSettingsPath() {
  return path.join(app.getPath('userData'), 'settings.json');
}

async function loadSettings() {
  try {
    const content = await readFile(getSettingsPath(), 'utf8');
    return normalizeSettings(JSON.parse(content));
  } catch {
    return { ...DEFAULT_SETTINGS, excludeIPs: [] };
  }
}

async function saveSettings(settings) {
  const normalized = normalizeSettings(settings);
  const settingsPath = getSettingsPath();
  await mkdir(path.dirname(settingsPath), { recursive: true });
  await writeFile(settingsPath, JSON.stringify(normalized, null, 2), 'utf8');
  broadcastTheme(normalized.theme);
  return normalized;
}

async function resetSettings() {
  if (autoTimer) clearInterval(autoTimer);
  autoTimer = null;
  if (process.platform === 'win32') {
    await setRunAtStartup(false);
  }
  const settings = await saveSettings({ ...DEFAULT_SETTINGS, excludeIPs: [] });
  if (mainWindow && !mainWindow.isDestroyed()) {
    mainWindow.webContents.send('settings-reset', settings);
  }
  return settings;
}

function broadcastTheme(theme) {
  for (const window of [mainWindow, settingsWindow, infoWindow]) {
    if (window && !window.isDestroyed()) {
      window.webContents.send('theme-changed', theme);
    }
  }
}

function keepWindowTitle(window, title) {
  window.webContents.on('page-title-updated', (event) => {
    event.preventDefault();
    window.setTitle(title);
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
  if (Buffer.byteLength(text, 'utf8') > 25 * 1024 * 1024) {
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

function sendAutoStatus(status) {
  if (mainWindow && !mainWindow.isDestroyed()) {
    mainWindow.webContents.send('auto-status', status);
  }
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

function createTray() {
  if (tray && !tray.isDestroyed()) return true;

  try {
    const iconPath = getPublicAssetPath('icon.ico');
    const image = nativeImage.createFromPath(iconPath);
    if (image.isEmpty())
      throw new Error(`Tray icon could not be decoded: ${iconPath}`);

    tray = new Tray(image);
    tray.setToolTip('WaGis-Mass-IP-Blacklister-Windows');
    tray.setContextMenu(
      Menu.buildFromTemplate([
        { label: 'Open', click: showMainWindow },
        { type: 'separator' },
        { label: 'Exit', click: () => app.quit() },
      ]),
    );
    tray.on('click', showMainWindow);
    tray.on('double-click', showMainWindow);
    trayUnavailableReason = undefined;
    console.info('System tray icon created from', iconPath);
    return true;
  } catch (error) {
    tray = null;
    trayUnavailableReason =
      error instanceof Error ? error.message : String(error);
    console.error('Failed to create system tray icon:', error);
    return false;
  }
}

async function showMainWindow() {
  if (!mainWindow || mainWindow.isDestroyed()) return;
  mainWindow.show();
  mainWindow.focus();
  try {
    const settings = await loadSettings();
    if (settings.startMinimized) {
      await saveSettings({ ...settings, startMinimized: false });
    }
  } catch (error) {
    console.error('Failed to save the visible startup state:', error);
  }
  if (tray && !tray.isDestroyed()) {
    const oldTray = tray;
    tray = null;
    oldTray.destroy();
  }
}

async function minimizeMainWindow() {
  if (!mainWindow || mainWindow.isDestroyed()) return false;
  if (!createTray()) {
    mainWindow.webContents.send('auto-status', {
      state: 'error',
      message: `System tray unavailable: ${trayUnavailableReason || 'icon creation failed'}. The window was not hidden.`,
    });
    return false;
  }

  try {
    const settings = await loadSettings();
    await saveSettings({ ...settings, startMinimized: true });
  } catch (error) {
    mainWindow.webContents.send('auto-status', {
      state: 'error',
      message: `Could not save the tray startup state: ${error.message}`,
    });
    return false;
  }

  mainWindow.hide();
  return true;
}

function openInfoWindow() {
  if (infoWindow && !infoWindow.isDestroyed()) {
    infoWindow.show();
    infoWindow.focus();
    return;
  }

  infoWindow = new BrowserWindow({
    width: 640,
    theme: ['system', 'light', 'dark'].includes(settings.theme)
      ? settings.theme
      : DEFAULT_SETTINGS.theme,
    height: 484,
    minWidth: 640,
    maxWidth: 640,
    minHeight: 484,
    maxHeight: 484,
    resizable: false,
    show: false,
    parent: mainWindow,
    title: `Info - ${APP_TITLE}`,
    webPreferences: {
      preload: path.join(__dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  keepWindowTitle(infoWindow, `Info - ${APP_TITLE}`);
  infoWindow.removeMenu();
  infoWindow.once('ready-to-show', () => infoWindow?.show());

  if (MAIN_WINDOW_VITE_DEV_SERVER_URL) {
    infoWindow.loadURL(
      new URL('info.html', MAIN_WINDOW_VITE_DEV_SERVER_URL).toString(),
    );
  } else {
    infoWindow.loadFile(
      path.join(__dirname, `../renderer/${MAIN_WINDOW_VITE_NAME}/info.html`),
    );
  }

  infoWindow.on('closed', () => {
    infoWindow = null;
  });
}

function openSettingsWindow() {
  if (settingsWindow && !settingsWindow.isDestroyed()) {
    settingsWindow.show();
    settingsWindow.focus();
    return;
  }

  settingsWindow = new BrowserWindow({
    width: 460,
    height: 850,
    minWidth: 460,
    minHeight: 850,
    resizable: true,
    show: false,
    parent: mainWindow,
    title: `Settings - ${APP_TITLE}`,
    icon: getPublicAssetPath('icon.ico'),
    webPreferences: {
      preload: path.join(__dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  keepWindowTitle(settingsWindow, `Settings - ${APP_TITLE}`);
  settingsWindow.removeMenu();
  settingsWindow.once('ready-to-show', () => settingsWindow?.show());

  if (MAIN_WINDOW_VITE_DEV_SERVER_URL) {
    settingsWindow.loadURL(
      new URL('settings.html', MAIN_WINDOW_VITE_DEV_SERVER_URL).toString(),
    );
  } else {
    settingsWindow.loadFile(
      path.join(
        __dirname,
        `../renderer/${MAIN_WINDOW_VITE_NAME}/settings.html`,
      ),
    );
  }

  settingsWindow.on('closed', () => {
    settingsWindow = null;
  });
}

function createWindow({ startInTray = false } = {}) {
  mainWindow = new BrowserWindow({
    width: 600,
    height: 680,
    minWidth: 600,
    minHeight: 680,
    title: APP_TITLE,
    icon: getPublicAssetPath('icon.ico'),
    show: !startInTray,
    webPreferences: {
      preload: path.join(__dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  keepWindowTitle(mainWindow, APP_TITLE);
  mainWindow.setMenu(
    Menu.buildFromTemplate([
      {
        label: 'Settings',
        click: openSettingsWindow,
      },
      {
        label: 'Info',
        click: openInfoWindow,
      },
      {
        label: 'Minimize to System Tray',
        click: minimizeMainWindow,
      },
    ]),
  );

  if (startInTray && !createTray()) {
    mainWindow.show();
  }

  if (MAIN_WINDOW_VITE_DEV_SERVER_URL) {
    mainWindow.loadURL(MAIN_WINDOW_VITE_DEV_SERVER_URL);
  } else {
    mainWindow.loadFile(
      path.join(__dirname, `../renderer/${MAIN_WINDOW_VITE_NAME}/index.html`),
    );
  }

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.setAppUserModelId('com.wagicoding.wagismassipblacklisterwindows');

ipcMain.handle('load-settings', () => loadSettings());
ipcMain.handle('reset-settings', () => resetSettings());
ipcMain.handle('set-run-at-startup', (_event, enabled) =>
  setRunAtStartup(enabled === true),
);
ipcMain.handle('updates:check', () => checkForUpdates());
ipcMain.handle('open-settings-window', () => openSettingsWindow());
ipcMain.handle('open-info-window', () => openInfoWindow());
ipcMain.handle('copy-text', (_event, text) => clipboard.writeText(text));
ipcMain.handle('save-settings', async (_event, incoming) => {
  const current = await loadSettings();
  // runAtStartup only changes through setRunAtStartup, which also edits the scheduled task.
  return saveSettings({
    ...current,
    ...incoming,
    runAtStartup: current.runAtStartup,
  });
});
ipcMain.handle('download-list', (_event, url) => downloadList(url));
ipcMain.handle('preview-list', (_event, request) =>
  previewAddressList(request.text, request.excludeIPs ?? []),
);
ipcMain.handle(
  'validate-exclusion',
  (_event, address) => typeof address === 'string' && isIP(address) !== 0,
);
ipcMain.handle('add-rules', (_event, request) => addFirewallRules(request));
ipcMain.handle('delete-rules', (_event, request) =>
  deleteFirewallRules(request),
);
ipcMain.handle('minimize-to-tray', () => minimizeMainWindow());
ipcMain.handle('start-auto-mode', async (_event, settings) => {
  const normalized = normalizeSettings(settings);
  getSelectedDirections(normalized);
  if (autoTimer) clearInterval(autoTimer);
  if (normalized.doOnce) await runAutomaticCycle(normalized);
  const interval =
    (normalized.intervalHours * 60 + normalized.intervalMinutes) * 60_000;
  autoTimer = setInterval(() => {
    void runAutomaticCycle(normalized);
  }, interval);
  return { running: true };
});
ipcMain.handle('stop-auto-mode', () => {
  if (autoTimer) clearInterval(autoTimer);
  autoTimer = null;
  sendAutoStatus({ state: 'stopped', message: 'Automatic mode stopped.' });
  return { running: false };
});

ipcMain.handle('delete-all-rules', (_event, directions) =>
  deleteFirewallRules({
    directions,
    allProtocols: true,
  }),
);

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(async () => {
  const settings = await loadSettings();
  const startInTray =
    process.argv.includes('--autostart') && settings.startMinimized;
  createWindow({ startInTray });
  if (!globalShortcut.register('CommandOrControl+Alt+W', showMainWindow)) {
    console.warn('Could not register Ctrl+Alt+W to restore the main window.');
  }

  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('will-quit', () => globalShortcut.unregisterAll());

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and import them here.
