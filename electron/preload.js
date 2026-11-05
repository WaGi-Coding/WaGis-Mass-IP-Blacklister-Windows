const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
    runScan: () => ipcRenderer.invoke('run-firewall-scan')
});