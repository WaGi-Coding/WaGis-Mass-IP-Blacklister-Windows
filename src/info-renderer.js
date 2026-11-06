import './index.css';

const api = window.electronAPI;
const systemTheme = window.matchMedia('(prefers-color-scheme: dark)');
let currentTheme = 'system';

function applyTheme(theme) {
  currentTheme = ['system', 'light', 'dark'].includes(theme) ? theme : 'system';
  document.documentElement.dataset.theme =
    currentTheme === 'system'
      ? systemTheme.matches
        ? 'dark'
        : 'light'
      : currentTheme;
}

let themeUpdateVersion = 0;
api.onThemeChanged((theme) => {
  themeUpdateVersion += 1;
  applyTheme(theme);
});

const initialThemeVersion = themeUpdateVersion;
void api
  .loadSettings()
  .then((settings) => {
    if (themeUpdateVersion === initialThemeVersion) applyTheme(settings.theme);
  })
  .catch(() => applyTheme('system'));

systemTheme.addEventListener('change', () => {
  if (currentTheme === 'system') applyTheme('system');
});

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
