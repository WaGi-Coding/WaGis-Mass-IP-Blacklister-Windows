# WaGis-Mass-IP-Blacklister-Windows

Windows desktop application for creating and managing Windows Defender Firewall block rules from IP address lists.

## Anwender

### Installation

Download and run the latest `WaGis-Mass-IP-Blacklister-Windows.Setup.<version>.exe` from the project's GitHub Releases. The installer lets you choose the installation directory.

The installed application requires administrator privileges to change Windows Firewall rules. Windows displays a User Account Control (UAC) prompt when you launch it manually. You can instead enable **Start with Windows as administrator** in Settings; Windows Task Scheduler then starts it with highest privileges at sign-in without a UAC prompt each time. The installer is currently unsigned, so Windows SmartScreen may show an additional warning.

### IP-Listen und Firewall-Regeln

Paste a list into the large text area or load one with **GET LIST**. A URL must use HTTP or HTTPS and end in `.txt`. Separators are flexible; the app extracts valid IPv4 and IPv6 addresses and CIDR ranges. Invalid address fragments are ignored. Duplicate addresses are retained. Exclusions match individual addresses exactly, without removing an enclosing CIDR range.

Choose TCP, UDP, or ALL, then choose inbound and/or outbound direction. **ADD** creates block rules in batches: 10,000 addresses on Windows 10 and newer, and 1,000 on older Windows builds. **DEL** removes this app's rules for the selected protocol and directions. **DELETE ALL** removes this app's rules for the selected directions and all protocols; unrelated firewall rules are not changed.

The automatic mode downloads the configured `.txt` list, removes the matching rules, then recreates them on the selected schedule. Its **ON / OFF** state is saved and resumes when the app starts. **Do once when starting** runs an update immediately when automatic mode is enabled.

### Settings

Open **Settings** from the native application menu.

- **Appearance**: System follows the current Windows theme until Light or Dark is selected. Manual choices are saved and applied to the open app windows.
- **Updates**: **Check for updates** checks the latest GitHub release. **Automatically check, download, and install updates when the app starts** saves the preference and enables the same check at startup.
- **Windows startup**: registers or removes a Task Scheduler entry for the current Windows user with highest privileges. Enable it from the installed app running as administrator; Windows then starts the app elevated at sign-in without recurring UAC prompts.
- Settings changes are saved automatically. **Reset all settings** restores defaults and removes the startup task, but does not change existing firewall rules.

User settings are stored in Electron's per-user `userData/settings.json` file.

### Updates

Update checks are available in the installed Windows application, not in the development build. A release must have a tag newer than the installed version and include an installer asset named exactly `WaGis-Mass-IP-Blacklister-Windows.Setup.<version>.exe`. For example, release tag `2.0.1` must include `WaGis-Mass-IP-Blacklister-Windows.Setup.2.0.1.exe`.

The updater downloads from GitHub over HTTPS, verifies the SHA-256 digest when GitHub supplies one, then launches the installer after the app exits. The latest release must be published before automatic updates can find a newer version.

## Developer

### Requirements

- Windows x64 for firewall operations and NSIS installer creation
- Node.js compatible with the project's Vite and Electron Builder versions
- Administrator privileges when testing firewall operations or configuring the elevated startup task

### Commands

Run from the repository root:

```powershell
npm install
npm start
npm run lint
npm run package
npm run installer
```

`npm start` launches the Electron Forge development app. `npm run package` creates the Forge-prepackaged app under `out/`. `npm run installer` then packages that app with electron-builder and NSIS. The installer is written to `release/WaGis-Mass-IP-Blacklister-Windows.Setup.<version>.exe`.

### Architecture

- `src/main.js`: Windows Firewall COM operations, settings persistence, Task Scheduler startup registration, automatic list refresh, GitHub update check/download/install, tray, and native windows.
- `src/preload.js`: isolated IPC API exposed to renderers.
- `src/renderer.js`: main IP-list and firewall interface.
- `src/settings-renderer.js`: theme and update preferences.
- `src/ip-list.mjs`: IPv4/IPv6/CIDR extraction, validation, and exclusions.
- `index.html`, `info.html`, `settings.html`: separate Vite renderer entry points.
- `src/index.css`: shared light/dark styling.
- `public/icon.ico`: Windows executable, installer, window, and tray icon.
- `public/icon.png`: renderer favicon.

The packaged Windows executable requests `requireAdministrator` through electron-builder's Windows manifest setting. The development command does not elevate automatically; firewall operations from a development build must be started from an elevated terminal.

### Publishing an update

1. Increment `version` in `package.json` using a three-part SemVer version, for example `2.0.1`.
2. Run `npm run installer` and verify the generated Setup executable.
3. Create and publish a GitHub Release with a matching tag such as `2.0.1`.
4. Upload the generated installer using the exact filename `WaGis-Mass-IP-Blacklister-Windows.Setup.2.0.1.exe`.

The updater compares the GitHub release tag with `app.getVersion()` and looks for that exact asset name. A release without the matching setup asset cannot be installed by the in-app updater.
