/**
 * This file will automatically be loaded by vite and run in the "renderer" context.
 * To learn more about the differences between the "main" and the "renderer" context in
 * Electron, visit:
 *
 * https://electronjs.org/docs/tutorial/process-model
 *
 * By default, Node.js integration in this file is disabled. When enabling Node.js integration
 * in a renderer process, please be aware of potential security implications. You can read
 * more about security risks here:
 *
 * https://electronjs.org/docs/tutorial/security
 */

import './index.css';

const api = window.electronAPI;

document.addEventListener('click', async (event) => {
  const button = event.target.closest('[data-copy-email]');
  if (!button) return;

  try {
    await api.copyText('wagi.coding@gmail.com');
    button.textContent = 'COPIED';
    setTimeout(() => {
      button.textContent = 'COPY';
    }, 1200);
  } catch {
    button.textContent = 'ERROR';
  }
});

const elements = {
  ipList: document.getElementById('ipList'),
  lineCount: document.getElementById('lineCount'),
  addressCount: document.getElementById('addressCount'),
  excludedCount: document.getElementById('excludedCount'),
  status: document.getElementById('statusMessage'),
  protocolGroup: document.getElementById('protocolGroup'),
  inbound: document.getElementById('inbound'),
  outbound: document.getElementById('outbound'),
  excludeList: document.getElementById('excludeList'),
  excludeInput: document.getElementById('excludeInput'),
  intervalHours: document.getElementById('intervalHours'),
  intervalMinutes: document.getElementById('intervalMinutes'),
  doOnce: document.getElementById('doOnce'),
  automaticUpdates: document.getElementById('automaticUpdates'),
  autoToggle: document.getElementById('autoToggle'),
  listUrl: document.getElementById('listUrl'),
};

let exclusions = [];
let busy = false;
let autoModeActive = false;
let currentTheme = 'system';
let previewSequence = 0;
let settingsSaveQueue = Promise.resolve();

const systemTheme = window.matchMedia('(prefers-color-scheme: dark)');

function applyTheme(theme) {
  currentTheme = ['system', 'light', 'dark'].includes(theme) ? theme : 'system';
  document.documentElement.dataset.theme =
    currentTheme === 'system'
      ? systemTheme.matches
        ? 'dark'
        : 'light'
      : currentTheme;
}

systemTheme.addEventListener('change', () => {
  if (currentTheme === 'system') applyTheme('system');
});
api.onThemeChanged(applyTheme);

function setStatus(message, state = 'ready') {
  elements.status.textContent = message;
  elements.status.dataset.state = state;
}

function getSettings() {
  return {
    protocol: document.querySelector('input[name="protocol"]:checked').value,
    inbound: elements.inbound.checked,
    outbound: elements.outbound.checked,
    excludeIPs: [...exclusions],
    intervalHours: Number(elements.intervalHours.value),
    intervalMinutes: Number(elements.intervalMinutes.value),
    doOnce: elements.doOnce.checked,
    automaticMode: elements.autoToggle.checked,
    listUrl: elements.listUrl.value.trim(),
  };
}

function saveSettingsAfterChange() {
  const snapshot = getSettings();
  settingsSaveQueue = settingsSaveQueue
    .catch(() => {})
    .then(() => api.saveSettings(snapshot))
    .then(() => setStatus('Settings saved automatically.', 'success'))
    .catch((error) => setStatus(error.message, 'error'));
  return settingsSaveQueue;
}

function getDirections() {
  const directions = [];
  if (elements.inbound.checked) directions.push('IN');
  if (elements.outbound.checked) directions.push('OUT');
  return directions;
}

function setBusy(value) {
  busy = value;
  document.querySelectorAll('[data-operation]').forEach((control) => {
    control.disabled = value || autoModeActive;
  });
  elements.autoToggle.disabled = value;
}

function setAutoLocked(value) {
  autoModeActive = value;
  document.querySelectorAll('[data-operation]').forEach((control) => {
    control.disabled = value || busy;
  });
  elements.autoToggle.disabled = busy;
}

async function updatePreview() {
  const sequence = ++previewSequence;
  try {
    const result = await api.previewList({
      text: elements.ipList.value,
      excludeIPs: exclusions,
    });
    if (sequence !== previewSequence) return;
    elements.lineCount.textContent = `Lines: ${result.lines.toLocaleString('en-US')}`;
    elements.addressCount.textContent = `Addresses: ${result.addressesRemaining.toLocaleString('en-US')}`;
    elements.excludedCount.textContent = `Excluded: ${result.excludedCount.toLocaleString('en-US')}`;
  } catch (error) {
    setStatus(error.message, 'error');
  }
}

function renderExclusions() {
  elements.excludeList.replaceChildren();
  if (exclusions.length === 0) {
    const empty = document.createElement('span');
    empty.className = 'empty-exclusions';
    empty.textContent = 'No excluded IPs';
    elements.excludeList.append(empty);
    return;
  }

  exclusions.forEach((address, index) => {
    const row = document.createElement('div');
    row.className = 'exclude-row';
    row.setAttribute('role', 'listitem');

    const value = document.createElement('span');
    value.textContent = address;

    const remove = document.createElement('button');
    remove.className = 'remove-exclusion';
    remove.type = 'button';
    remove.title = 'Remove exclusion';
    remove.setAttribute('aria-label', `Remove ${address}`);
    remove.textContent = '×';
    remove.addEventListener('click', () => {
      exclusions.splice(index, 1);
      renderExclusions();
      void updatePreview();
      void saveSettingsAfterChange();
    });

    row.append(value, remove);
    elements.excludeList.append(row);
  });
}

function applySettings(settings) {
  document.querySelector(
    `input[name="protocol"][value="${settings.protocol}"]`,
  ).checked = true;
  elements.inbound.checked = settings.inbound;
  elements.outbound.checked = settings.outbound;
  elements.intervalHours.value = settings.intervalHours;
  elements.intervalMinutes.value = settings.intervalMinutes;
  elements.doOnce.checked = settings.doOnce;
  elements.autoToggle.checked = settings.automaticMode;
  setAutoLocked(settings.automaticMode);
  applyTheme(settings.theme);
  elements.listUrl.value = settings.listUrl;
  exclusions = [...settings.excludeIPs];
  renderExclusions();
  void updatePreview();
}

async function loadSettings() {
  try {
    const settings = await api.loadSettings();
    applySettings(settings);
    setStatus('Settings loaded.');
    if (settings.automaticUpdates) {
      void api.checkForUpdates().then((result) => {
        if (!result.success && result.error) setStatus(result.error, 'error');
      });
    }
    if (settings.automaticMode) {
      setStatus('Resuming automatic mode...', 'running');
      void api
        .startAutoMode(settings)
        .then(() => {
          setStatus('Automatic mode resumed.', 'success');
        })
        .catch(async (error) => {
          elements.autoToggle.checked = false;
          setAutoLocked(false);
          await saveSettingsAfterChange();
          setStatus(error.message, 'error');
        });
    }
  } catch (error) {
    setStatus(error.message, 'error');
  }
}

async function addRules() {
  setBusy(true);
  setStatus('Adding firewall rules...', 'running');
  try {
    const result = await api.addRules({
      text: elements.ipList.value,
      settings: getSettings(),
    });
    elements.ipList.value = '';
    await updatePreview();
    setStatus(
      `Added ${result.addressesAdded.toLocaleString('en-US')} addresses in ${result.rulesAdded} rules; excluded ${result.exclusionsApplied}.`,
      'success',
    );
  } catch (error) {
    setStatus(error.message, 'error');
  } finally {
    setBusy(false);
  }
}

async function deleteRules() {
  setBusy(true);
  setStatus('Removing selected firewall rules...', 'running');
  try {
    const result = await api.deleteRules({
      protocol: getSettings().protocol,
      directions: getDirections(),
    });
    setStatus(`Removed ${result.rulesRemoved} firewall rules.`, 'success');
  } catch (error) {
    setStatus(error.message, 'error');
  } finally {
    setBusy(false);
  }
}

async function deleteAllRules() {
  if (
    !window.confirm(
      'Delete all WaGi IP-Blacklist rules for the selected directions?',
    )
  )
    return;
  setBusy(true);
  setStatus('Removing all WaGi firewall rules...', 'running');
  try {
    const result = await api.deleteAllRules(getDirections());
    setStatus(`Removed ${result.rulesRemoved} firewall rules.`, 'success');
  } catch (error) {
    setStatus(error.message, 'error');
  } finally {
    setBusy(false);
  }
}

async function downloadList() {
  setBusy(true);
  setStatus('Downloading IP list...', 'running');
  try {
    elements.ipList.value = await api.downloadList(
      elements.listUrl.value.trim(),
    );
    await updatePreview();
    setStatus('IP list downloaded.', 'success');
  } catch (error) {
    setStatus(error.message, 'error');
  } finally {
    setBusy(false);
  }
}

async function addExclusion(event) {
  event.preventDefault();
  const address = elements.excludeInput.value.trim();
  if (!(await api.validateExclusion(address))) {
    setStatus('Enter one valid IPv4 or IPv6 address.', 'error');
    return;
  }
  if (exclusions.some((item) => item.toLowerCase() === address.toLowerCase())) {
    setStatus('That IP is already excluded.', 'error');
    return;
  }
  exclusions.push(address);
  elements.excludeInput.value = '';
  renderExclusions();
  await updatePreview();
  void saveSettingsAfterChange();
}

async function toggleAutoMode() {
  if (elements.autoToggle.checked) {
    setAutoLocked(true);
    setStatus('Starting automatic mode...', 'running');
    try {
      const settings = getSettings();
      await api.saveSettings(settings);
      await api.startAutoMode(settings);
      setStatus('Automatic mode is running.', 'success');
    } catch (error) {
      elements.autoToggle.checked = false;
      setAutoLocked(false);
      await saveSettingsAfterChange();
      setStatus(error.message, 'error');
    }
    return;
  }

  try {
    await api.stopAutoMode();
    await api.saveSettings(getSettings());
  } catch (error) {
    setStatus(error.message, 'error');
  }
  setAutoLocked(false);
  setStatus('Automatic mode stopped.');
}

function normalizeDirectionSelection(changed) {
  if (elements.inbound.checked || elements.outbound.checked) return;
  if (changed === elements.inbound) elements.outbound.checked = true;
  else elements.inbound.checked = true;
}

document.getElementById('addBtn').addEventListener('click', addRules);
document.getElementById('deleteBtn').addEventListener('click', deleteRules);
document
  .getElementById('deleteAllBtn')
  .addEventListener('click', deleteAllRules);
document.getElementById('downloadBtn').addEventListener('click', downloadList);
document.getElementById('excludeForm').addEventListener('submit', addExclusion);
elements.ipList.addEventListener('input', () => void updatePreview());
elements.inbound.addEventListener('change', (event) => {
  normalizeDirectionSelection(event.currentTarget);
  void saveSettingsAfterChange();
});
elements.outbound.addEventListener('change', (event) => {
  normalizeDirectionSelection(event.currentTarget);
  void saveSettingsAfterChange();
});
document.querySelectorAll('input[name="protocol"]').forEach((input) => {
  input.addEventListener('change', () => void saveSettingsAfterChange());
});
elements.autoToggle.addEventListener('change', toggleAutoMode);
elements.intervalHours.addEventListener('change', () => {
  if (Number(elements.intervalHours.value) >= 24)
    elements.intervalMinutes.value = '0';
  void saveSettingsAfterChange();
});
elements.intervalMinutes.addEventListener('change', () => {
  if (
    Number(elements.intervalHours.value) === 0 &&
    Number(elements.intervalMinutes.value) === 0
  ) {
    elements.intervalMinutes.value = '1';
  }
  void saveSettingsAfterChange();
});
elements.doOnce.addEventListener(
  'change',
  () => void saveSettingsAfterChange(),
);
elements.listUrl.addEventListener(
  'change',
  () => void saveSettingsAfterChange(),
);

document.querySelectorAll('[data-operation]').forEach((control) => {
  control.disabled = false;
});
api.onAutoStatus((status) => {
  setStatus(
    status.message,
    status.state === 'complete' ? 'success' : status.state,
  );
});
api.onSettingsReset((settings) => {
  applySettings(settings);
  setStatus(
    'Settings reset. Existing firewall rules were not changed.',
    'success',
  );
});
api.onUpdateStatus((status) => {
  const state =
    status.status === 'error'
      ? 'error'
      : status.status === 'current'
        ? 'success'
        : status.status === 'checking' ||
            status.status === 'available' ||
            status.status === 'downloading' ||
            status.status === 'installing'
          ? 'running'
          : 'ready';
  setStatus(status.message, state);
});
void loadSettings();
