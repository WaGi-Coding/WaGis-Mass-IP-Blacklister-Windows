import './index.css';

const api = window.electronAPI;
const automaticUpdates = document.getElementById('automaticUpdates');
const checkUpdatesButton = document.getElementById('checkUpdatesBtn');
const updateStatus = document.getElementById('updateStatus');
const themeStatus = document.getElementById('themeStatus');
const runAtStartup = document.getElementById('runAtStartup');
const startupStatus = document.getElementById('startupStatus');
const systemTheme = window.matchMedia('(prefers-color-scheme: dark)');

let currentTheme = 'system';

function applyTheme(theme) {
  currentTheme = ['system', 'light', 'dark'].includes(theme) ? theme : 'system';
  const effectiveTheme =
    currentTheme === 'system'
      ? systemTheme.matches
        ? 'dark'
        : 'light'
      : currentTheme;
  document.documentElement.dataset.theme = effectiveTheme;
  document.querySelectorAll('input[name="theme"]').forEach((input) => {
    input.checked = input.value === effectiveTheme;
  });
  if (themeStatus) {
    themeStatus.textContent =
      currentTheme === 'system'
        ? `Following system theme (${systemTheme.matches ? 'Dark' : 'Light'}).`
        : `${currentTheme === 'dark' ? 'Dark' : 'Light'} theme selected.`;
  }
}

systemTheme.addEventListener('change', () => {
  if (currentTheme === 'system') applyTheme('system');
});
api.onThemeChanged(applyTheme);

function setStatus(message, state = 'ready') {
  updateStatus.textContent = message;
  updateStatus.dataset.state = state;
}

async function checkForUpdates() {
  checkUpdatesButton.disabled = true;
  setStatus('Checking for updates...', 'running');

  try {
    const result = await api.checkForUpdates();
    if (!result.success && result.error) setStatus(result.error, 'error');
  } catch (error) {
    setStatus(error.message, 'error');
  } finally {
    checkUpdatesButton.disabled = false;
  }
}

async function loadSettings() {
  try {
    const settings = await api.loadSettings();
    automaticUpdates.checked = settings.automaticUpdates;
    runAtStartup.checked = settings.runAtStartup;
    applyTheme(settings.theme);
    startupStatus.textContent = settings.runAtStartup
      ? 'Starts at Windows sign-in with highest privileges.'
      : 'Does not start automatically with Windows.';
    setStatus(
      settings.automaticUpdates
        ? 'Automatic updates are enabled.'
        : 'Automatic updates are disabled.',
    );
  } catch (error) {
    setStatus(error.message, 'error');
  }
}

async function saveRunAtStartup() {
  runAtStartup.disabled = true;
  startupStatus.textContent = runAtStartup.checked
    ? 'Registering elevated startup task...'
    : 'Removing startup task...';
  startupStatus.dataset.state = 'running';

  try {
    const settings = await api.setRunAtStartup(runAtStartup.checked);
    runAtStartup.checked = settings.runAtStartup;
    startupStatus.textContent = settings.runAtStartup
      ? 'Will start at Windows sign-in with highest privileges.'
      : 'Windows startup disabled.';
    startupStatus.dataset.state = 'success';
  } catch (error) {
    startupStatus.textContent = error.message;
    startupStatus.dataset.state = 'error';
    const settings = await api.loadSettings();
    runAtStartup.checked = settings.runAtStartup;
  } finally {
    runAtStartup.disabled = false;
  }
}

async function saveTheme(event) {
  const previousTheme = currentTheme;
  const selectedTheme = event.currentTarget.value;
  try {
    const settings = await api.loadSettings();
    settings.theme = selectedTheme;
    await api.saveSettings(settings);
    applyTheme(selectedTheme);
    themeStatus.dataset.state = 'success';
  } catch (error) {
    applyTheme(previousTheme);
    themeStatus.textContent = error.message;
    themeStatus.dataset.state = 'error';
  }
}

async function saveAutomaticUpdates() {
  automaticUpdates.disabled = true;
  try {
    const settings = await api.loadSettings();
    settings.automaticUpdates = automaticUpdates.checked;
    await api.saveSettings(settings);
    setStatus(
      automaticUpdates.checked
        ? 'Automatic updates enabled. Checking now...'
        : 'Automatic updates disabled.',
      'success',
    );
    if (automaticUpdates.checked) await checkForUpdates();
  } catch (error) {
    setStatus(error.message, 'error');
    await loadSettings();
  } finally {
    automaticUpdates.disabled = false;
  }
}

async function resetAllSettings() {
  const confirmed = window.confirm(
    'Reset all saved settings to their defaults? Existing firewall rules will not be changed.',
  );
  if (!confirmed) return;

  automaticUpdates.disabled = true;
  runAtStartup.disabled = true;
  checkUpdatesButton.disabled = true;
  try {
    const settings = await api.resetSettings();
    automaticUpdates.checked = settings.automaticUpdates;
    runAtStartup.checked = settings.runAtStartup;
    applyTheme(settings.theme);
    setStatus(
      'Settings reset. Existing firewall rules were not changed.',
      'success',
    );
    themeStatus.dataset.state = 'ready';
  } catch (error) {
    setStatus(error.message, 'error');
  } finally {
    automaticUpdates.disabled = false;
    runAtStartup.disabled = false;
    checkUpdatesButton.disabled = false;
  }
}

checkUpdatesButton.addEventListener('click', checkForUpdates);
automaticUpdates.addEventListener('change', saveAutomaticUpdates);
runAtStartup.addEventListener('change', saveRunAtStartup);
document
  .getElementById('resetSettingsBtn')
  .addEventListener('click', resetAllSettings);
document.querySelectorAll('input[name="theme"]').forEach((input) => {
  input.addEventListener('change', saveTheme);
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
  checkUpdatesButton.disabled = [
    'checking',
    'downloading',
    'installing',
  ].includes(status.status);
});

void loadSettings();
