<h1 align="center">
    <img src="https://store-images.s-microsoft.com/image/apps.9803.13665982798840789.e082f07b-1ea3-4fde-b043-8dcc4e3a08c6.11ecdb51-2d2d-40f6-bed7-e3b959340fcc?mode=scale&q=90&h=300&w=300" width="50"/>
  <br>Game Bar Browser
</h1>

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/0ba0377d699a4a20a53829efe3e5d553)](https://app.codacy.com/manual/dynamiquel/Game-Bar-Browser?utm_source=github.com&utm_medium=referral&utm_content=dynamiquel/Game-Bar-Browser&utm_campaign=Badge_Grade_Dashboard)

<img src="https://store-images.s-microsoft.com/image/apps.31513.13665982798840789.ff7405c8-a553-4c46-885d-f4e1eb46b3e1.f3e3dad5-c6a3-4648-8a39-2935526c0e19?w=1399&h=787&q=90&format=jpg" width="800"/>

A basic web browser made for the Xbox Game Bar on Windows 10.

This web browser is really basic but one didn't exist for the Xbox Game Bar at the time, so it's better than nothing. I expect a superior web browser to appear in the coming months but this repo is open to suggestions.

Due to **Xbox Game Bar** only supporting an outdated **Edge WebView**, this browser will not run smoothly on many modern sites that rely on modern technologies.
Therefore, I find it kind of pointless to update this app until Xbox Game Bar supports **WinUI 3**.

## Current features
[Click here to view all features](https://github.com/dynamiquel/Game-Bar-Browser/blob/master/RELEASES.md)

## Download
[Click here to download the latest version of **Browser for Game Bar**](https://www.microsoft.com/en-gb/p/browser-for-xbox-game-bar/9nk1cnb0nccx?irgwc=1&OCID=AID2000142_aff_7593_159229&tduid=%28ir__ywh0qxotpckftjqnxka03fe3c22xnsta9kwxk6l900%29%287593%29%28159229%29%28%29%28UUwpUdUnU77533YYmYb%29&irclickid=_ywh0qxotpckftjqnxka03fe3c22xnsta9kwxk6l900&activetab=pivot:overviewtab) from the Microsoft Store.

## Installing older versions
To install an older version of Game Bar Browser, you must [**Sideload**](https://docs.microsoft.com/en-us/previous-versions/windows/apps/bg126232(v=win.10)?redirectedfrom=MSDN) it.

1. Download the desired version from [releases](https://github.com/dynamiquel/Game-Bar-Browser/releases/)
2. Right click on MSIX file
3. Click Properties
4. Click Digital Signatures
5. Select Signature from the list
6. Click Details
7. Click View Certificate
8. Click Install Certificate
9. Open the MSIX file


## Developers
The app uses Microsoft's EdgeHTML WebView, and so inherits limitations and bugs from it. I also wouldn't say the code is worthy to fork, but do as you will.

[Learn more about how to create an app for Xbox Game Bar](https://docs.microsoft.com/en-us/gaming/game-bar/).
