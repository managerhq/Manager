# Manager

**Manager** is free accounting software for small businesses. It runs on Windows, macOS, and Linux, works offline, and keeps your accounting data on hardware you control.

This repository holds the source for both self-hosted editions:

- **Desktop** — one user, one computer. Fully featured and free, with no trial period, no ads, and no registration.
- **Server** — your own private accounting website. It runs on one computer you control, and your whole team works on the same records from a browser.

Both editions share the same data format, so you can move between them without losing anything. A **[Cloud edition](https://www.manager.cloud)** is also available — identical to Server in features, but hosted by us.

## What you can do with it

Manager covers the everyday accounting needs of a small business:

- Invoicing and quotes
- Expense tracking and purchase orders
- Bank reconciliation
- Inventory management
- Payroll
- Financial reports — profit and loss, balance sheet, tax reports, and more

It supports multiple businesses, multiple currencies, and dozens of languages including right-to-left ones. Dark mode is included.

## Download

Get the latest release for your platform from the [releases page](https://github.com/Manager-io/Manager/releases/latest), or from [manager.io](https://www.manager.io). Older versions stay available on the [releases page](https://github.com/Manager-io/Manager/releases).

New to the software? Start with the built-in guides at [manager.io/guides](https://www.manager.io/guides).

---

# Desktop edition

1. Download Manager and install it.
2. Open the app and create your first business.

Your data is saved automatically as you work. You can back up a business to a single file at any time and restore it on another computer — moving between Windows, macOS, and Linux works seamlessly.

The desktop edition is designed for a single user on one computer. If you need multi-user access, or want your data available from anywhere, use the Server edition below or the Cloud edition.

---

# Server edition

## What you need

- A computer running Windows, macOS, or Linux that can stay switched on while people need access.
- A web browser (Chrome, Edge, Firefox, or Safari) — on the same computer, or on any other computer, tablet, or phone on your network.

No database, no additional software, and no technical setup are required.

## Getting started

1. **Start the server.**
   - On Windows: double-click `ManagerServer.exe`.
   - On macOS or Linux: run the `ManagerServer` program.

   A window will appear confirming the server is running. Leave it open — closing it stops the server.

2. **Open Manager in your browser.**
   On the same computer, go to:

   > http://localhost:5000

3. **Sign in for the first time.**
   The first time you connect, the username will be pre-filled as `administrator` and you will be asked to create a password. Choose a strong password and keep it safe — this account has full access to everything.

4. **Add your business and your users.**
   Once signed in, create your business, then go to **Users** to add accounts for your team. You decide what each person is allowed to see and do.

## Connecting from other computers

Other people on your network connect using your server computer's name or address instead of `localhost` — for example:

> http://office-pc:5000

If the page doesn't load from another computer, the server computer's firewall is usually the reason — allow Manager Server (or port 5000) through the firewall, or ask whoever looks after your network to do so.

## Where your data is kept

Everything is stored in a data folder on the server computer. By default this is the `Manager.io` folder inside the Documents folder of the person who started the server. The exact location is shown in the server window every time it starts.

**Back up this folder regularly.** Copying it to an external drive or your backup system is all it takes — the folder contains all your businesses, users, and settings. You can also back up individual businesses from within Manager itself using the **Backup** button.

## Forgot the administrator password?

You can reset it on the server computer itself:

1. Stop the server.
2. Open the data folder (the location shown in the server window), then open the `Users` folder inside it.
3. Delete the file named `administrator`.
4. Start the server again and open Manager in your browser — you will be asked to create a new administrator password.

Passwords for other users can simply be reset by the administrator from the **Users** screen.

## Optional settings

Manager Server works out of the box, but you can adjust it by starting it with extra options:

| What you want | How to start the server |
|---|---|
| Use a different address or port (e.g. plain port 80) | `ManagerServer --urls http://*:80` |
| Keep your data in a different folder | `ManagerServer --path "D:\Accounting Data"` |
| Let users reset their own passwords by email | `ManagerServer --smtp smtp://user:pass@host:587?from=noreply@example.com` |

On Windows, these options can be added to a shortcut so the server always starts the same way.

---

# How it is built

| Path | What it is |
|---|---|
| `ManagerServer/` | The application itself — a single ASP.NET Core project that ships as one self-contained executable, with web assets, translations, and localizations embedded as resources. |
| `ManagerDesktop/` | The Electron shell for the desktop edition. It launches the server locally and connects to it. |

Each business is a single `.manager` file (SQLite) in the data folder, loaded into memory and served as plain HTML. There is no separate database server, no build step for the front end, and no runtime dependency beyond the executable itself.

## Building from source

You need the [.NET SDK 10.0](https://dotnet.microsoft.com/download) or later. Nothing else — the committed sources build as-is.

Run a development build from the `ManagerServer` directory:

```
dotnet run
```

Then open http://localhost:5000.

Produce a self-contained single-file build, the same way official releases are built:

```
dotnet publish ManagerServer.csproj --configuration Release --runtime win-x64 --self-contained true /p:PublishSingleFile=true
```

Releases are published for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64` and `osx-arm64`.

`Assets/Translations.json` and `Assets/Extensions.json` are committed, so a normal build does not touch the network. The `refresh-translations.cmd` and `refresh-extensions.cmd` scripts re-download them from manager.io when they need updating.

---

# Help and community

- **Guides** — [manager.io/guides](https://www.manager.io/guides)
- **Community forum** — [forum.manager.io](https://forum.manager.io), where users and the development team answer questions
- **Security** — see [SECURITY.md](SECURITY.md) to report a vulnerability

If something goes wrong with the server, the message shown in the server window is usually the quickest clue — include it when asking for help on the forum.

# License

Manager is **source-available, not open source**. It is licensed under the [Functional Source License, Version 1.1, Apache 2.0 Future License](LICENSE.md) (FSL-1.1-Apache-2.0), © NGSoftware Pty Ltd.

In short — read [LICENSE.md](LICENSE.md) for the terms that actually apply:

- You may use, copy, modify, and redistribute the software for any purpose **other than a competing use** — which includes running it internally, however many businesses or users you have, and using it in professional services you provide to clients.
- You may **not** use it to offer a commercial product or service that substitutes for Manager or for anything we offer using it.
- Two years after each version is released, that version becomes available to you under the **Apache License 2.0**, with no restrictions.

The license does not grant rights to the Manager name, logo, or other trademarks.

Third-party components bundled with Manager keep their own licenses and are listed in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
