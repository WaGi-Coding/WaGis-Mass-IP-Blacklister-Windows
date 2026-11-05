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
