import { contextBridge, ipcRenderer } from 'electron';

contextBridge.exposeInMainWorld('electronAPI', {
  checkForUpdates: () => ipcRenderer.invoke('updates:check'),
  onUpdateStatus: (listener) => {
    const handler = (_event, status) => listener(status);
    ipcRenderer.on('update-status', handler);
    return () => ipcRenderer.removeListener('update-status', handler);
  },
  onThemeChanged: (listener) => {
    const handler = (_event, theme) => listener(theme);
    ipcRenderer.on('theme-changed', handler);
    return () => ipcRenderer.removeListener('theme-changed', handler);
  },
  onSettingsReset: (listener) => {
    const handler = (_event, settings) => listener(settings);
    ipcRenderer.on('settings-reset', handler);
    return () => ipcRenderer.removeListener('settings-reset', handler);
  },
  openSettingsWindow: () => ipcRenderer.invoke('open-settings-window'),
  setRunAtStartup: (enabled) =>
    ipcRenderer.invoke('set-run-at-startup', enabled),
  openInfoWindow: () => ipcRenderer.invoke('open-info-window'),
  copyText: (text) => ipcRenderer.invoke('copy-text', text),
  loadSettings: () => ipcRenderer.invoke('load-settings'),
  resetSettings: () => ipcRenderer.invoke('reset-settings'),
  saveSettings: (settings) => ipcRenderer.invoke('save-settings', settings),
  downloadList: (url) => ipcRenderer.invoke('download-list', url),
  previewList: (request) => ipcRenderer.invoke('preview-list', request),
  validateExclusion: (address) =>
    ipcRenderer.invoke('validate-exclusion', address),
  addRules: (request) => ipcRenderer.invoke('add-rules', request),
  deleteRules: (request) => ipcRenderer.invoke('delete-rules', request),
  deleteAllRules: (directions) =>
    ipcRenderer.invoke('delete-all-rules', directions),
  startAutoMode: (settings) => ipcRenderer.invoke('start-auto-mode', settings),
  stopAutoMode: () => ipcRenderer.invoke('stop-auto-mode'),
  minimizeToTray: () => ipcRenderer.invoke('minimize-to-tray'),
  onAutoStatus: (listener) => {
    const handler = (_event, status) => listener(status);
    ipcRenderer.on('auto-status', handler);
    return () => ipcRenderer.removeListener('auto-status', handler);
  },
});
