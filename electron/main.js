const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const { exec } = require('child_process');
const util = require('util');
const execPromise = util.promisify(exec);

let mainWindow;

app.whenReady().then(() => {
    mainWindow = new BrowserWindow({
        width: 600,
        height: 758,
        minWidth: 600,
        minHeight: 758,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: false,
            contextIsolation: true
        }
    });

    mainWindow.loadFile('index.html');
});

// Hilfsfunktion: Generiert einen String mit X Test-IPs (z.B. 192.168.1.1, ...)
function generateDummyIps(count) {
    let ips = [];
    for (let i = 0; i < count; i++) {
        let octet3 = Math.floor(i / 255) % 255;
        let octet4 = i % 255;
        ips.push(`10.99.${octet3}.${octet4}`);
    }
    return ips.join(',');
}

// Der Scan-Algorithmus (Binärsuche)
ipcMain.handle('run-firewall-scan', async (event) => {
    const ruleName = "_Temp_Firewall_Limit_Test_Rule";
    let low = 100;
    let high = 15000; // Obergrenze über dem Windows 11 Standard
    let maxSupported = 100;

    // Vorherige Leichen aufräumen falls vorhanden
    await execPromise(`netsh advfirewall firewall delete rule name="${ruleName}"`).catch(() => { });

    try {
        // Wir tasten uns per Binärsuche an das Maximum heran
        while (low <= high) {
            let mid = Math.floor((low + high) / 2);
            let testString = generateDummyIps(mid);

            // Test-Regel via netsh erstellen
            let cmd = `netsh advfirewall firewall add rule name="${ruleName}" dir=in action=block remoteip=${testString}`;

            try {
                await execPromise(cmd, { windowsHide: true });
                // Wenn erfolgreich, merken und höher testen
                maxSupported = mid;
                low = mid + 1;
            } catch (err) {
                // Wenn Fehler (z.B. Parameter ungültig / Puffer zu groß), nach unten korrigieren
                high = mid - 1;
            } finally {
                // Regel direkt wieder löschen für den nächsten Durchlauf
                await execPromise(`netsh advfirewall firewall delete rule name="${ruleName}"`).catch(() => { });
            }
        }
    } catch (globalErr) {
        console.error(globalErr);
    }

    return maxSupported;
});