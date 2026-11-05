# 🛡️ WaGi's Mass IP Blacklister for Windows

![Latest GitHub release](https://img.shields.io/github/v/release/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows?label=latest%20release&style=for-the-badge)
![GitHub release downloads](https://img.shields.io/github/downloads/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/total?label=downloads&style=for-the-badge)
![SourceForge downloads](https://img.shields.io/sourceforge/dt/wagi-ip-blacklister?label=SourceForge%20downloads&style=for-the-badge)
![GitHub stars](https://img.shields.io/github/stars/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows?style=social)

<p align="center">
	<a href="https://github.com/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases/">⬇️ Download the latest Windows installer</a>
</p>

<p align="center">
	<img width="580" height="665" alt="img" src="https://github.com/user-attachments/assets/64c23d89-819f-4c4f-b5ee-6c938539bd11" />
</p>

---

**WaGi's Mass IP Blacklister** is a Windows desktop application for creating and managing Windows Defender Firewall block rules from IP address lists.

## 💡 Why I Built It

I needed a way to automatically block large numbers of IP addresses from blacklist feeds. My first solution was a simple batch script, but processing around 20,000 addresses could take roughly two hours. I built this app to make recurring blacklist updates much faster and easier to manage.

## ⚙️ How It Works

The app extracts IPv4 addresses, IPv6 addresses, and CIDR ranges from plain text, so lists do not need a special format or separator. It groups addresses into firewall rules according to the Windows version: up to 10,000 addresses per rule on Windows 10 and newer, and 1,000 on older Windows builds. This avoids creating one rule per address and makes large-list updates practical.

Rules created by the app use its own naming prefix. Delete operations target only those rules; unrelated Windows Firewall rules are left untouched. Duplicate addresses are retained, and exclusions match exact addresses rather than removing a CIDR range that contains an excluded address.

## ✨ Key Features

- **Bulk firewall rules:** Block large IP lists in batches, with support for TCP, UDP, inbound, and outbound traffic.
- **IPv4, IPv6, and CIDR:** Extract valid addresses and network ranges from plain-text lists.
- **IP exclusions:** Exclude exact addresses from a list before adding firewall rules.
- **Automatic updates:** Download a configured plain-text list and refresh matching rules on a schedule.
- **Windows startup:** Optionally start elevated at sign-in through Windows Task Scheduler.
- **System tray:** Keep the app available in the tray while background updates run.
- **Themes and in-app updates:** Choose System, Light, or Dark appearance and check GitHub Releases from Settings.
- **Targeted rule removal:** Delete this app's rules without changing unrelated Windows Firewall rules.

## 🚀 For Users

### Installation

Download and run the latest `WaGis-Mass-IP-Blacklister-Windows.Setup.<version>.exe` from [GitHub Releases](https://github.com/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases). The installer lets you choose the installation directory.

The installed app requires administrator privileges to change Windows Firewall rules. Launching it manually displays a Windows User Account Control (UAC) prompt. To start it elevated at sign-in, enable **Windows startup** in Settings while running the installed app as administrator. The installer is currently unsigned, so Windows SmartScreen may show an additional warning.

### Add and remove firewall rules

1. Paste an IP list into the text area or select **GET LIST** to download one. List URLs must use HTTP or HTTPS; no particular file extension is required.
2. Choose **TCP**, **UDP**, or **ALL**, then select inbound and/or outbound traffic.
3. Optionally add exact IP exclusions. Valid IPv4, IPv6, and CIDR entries are extracted; invalid address fragments are ignored and duplicates are retained.
4. Select **ADD** to create block rules. The app batches 10,000 addresses on Windows 10 and newer, or 1,000 on older Windows builds.
5. Select **DEL** to remove this app's rules for the selected protocol and directions, or **DELETE ALL** to remove this app's rules for all protocols in the selected directions. Unrelated firewall rules are not changed.

### Automatic mode and settings

Open **Settings** from the application menu. Automatic mode downloads the configured list, removes matching rules, and recreates them on the selected schedule. Its **ON / OFF** state is saved and resumes when the app starts. **Do once when starting** runs an update immediately when automatic mode is enabled.

- **Appearance:** Follow the Windows theme or choose Light or Dark.
- **Updates:** Check the latest GitHub release manually or enable automatic checks, downloads, and installation at startup.
- **Windows startup:** Register or remove the elevated Task Scheduler entry for the current Windows user.
- **Reset all settings:** Restore defaults and remove the startup task. Existing firewall rules are not changed.

Settings are stored in Electron's per-user `userData/settings.json` file.

### In-app updates

Update checks are available in the installed Windows app, not in the development build. A release must have a tag newer than the installed version and include an installer asset named exactly `WaGis-Mass-IP-Blacklister-Windows.Setup.<version>.exe`. For example, release tag `2.0.1` must include `WaGis-Mass-IP-Blacklister-Windows.Setup.2.0.1.exe`.

The updater downloads from GitHub over HTTPS and verifies the SHA-256 digest when GitHub supplies one. It launches the installer after the app exits. The latest release must be published before automatic updates can find a newer version.

## 💻 For Developers

### Prerequisites

- Windows x64 for firewall operations and NSIS installer creation
- Node.js compatible with the project's Vite and Electron Builder versions
- Administrator privileges for testing firewall operations or configuring the elevated startup task

### Development and build commands

Run these commands from the repository root:

```powershell
npm install
npm start
npm run lint
npm run package
npm run installer
```

`npm start` launches the Electron Forge development app. `npm run package` creates the packaged app under `out/`. `npm run installer` packages it with electron-builder and NSIS. The installer is written to `release/WaGis-Mass-IP-Blacklister-Windows.Setup.<version>.exe`.

### Project structure

- `src/main.js`: Firewall operations, settings persistence, startup registration, automatic list refresh, updates, tray, and native windows.
- `src/background-worker.mjs`: Background list processing, firewall work, and automatic-mode scheduling in an Electron Utility Process.
- `src/preload.js`: Isolated IPC API exposed to renderer processes.
- `src/renderer.js`: Main IP-list and firewall interface.
- `src/settings-renderer.js`: Appearance, startup, and update settings.
- `src/ip-list.mjs`: IPv4/IPv6/CIDR extraction, validation, and exclusions.
- `index.html`, `info.html`, `settings.html`: Separate Vite renderer entry points.
- `public/icon.ico` and `public/icon.png`: Windows app, installer, tray, and renderer icons.

The packaged Windows executable requests `requireAdministrator` through the Windows application manifest. The development command does not elevate automatically; run it from an elevated terminal to test firewall operations.

### 📦 Publishing a GitHub Release

1. Increment `version` in `package.json`, for example to `2.0.1`.
2. Run `npm run installer` and verify the generated Setup executable.
3. Create and publish a GitHub Release with the matching tag, such as `2.0.1`.
4. Upload the installer as `WaGis-Mass-IP-Blacklister-Windows.Setup.2.0.1.exe`.

The in-app updater compares the release tag with `app.getVersion()` and looks for that exact asset name. A release without the matching Setup executable cannot be installed by the in-app updater.
