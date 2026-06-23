# WallpaperThing
Wall &amp; Paper


Wallpaper Program
-----------------

This program purpose is to let the user to set custom background pictures, or videos that are changing in a given interval. This will be multi-platform (Windows & Linux), and currently can be ran on windows.

This program is currently in development, and in it's early stage.
What does work: 
---------------
- you can select the folder of your pictures
- you can select the interval of the picture change.

Future features (hopefully):
----------------------------
- video background integration
- effects for changing background
- url based backgrounds
- "workshop" where users can upload/download their own pictures/videos

The stack it is built on:
-------------------------
- The Framework: Avalonia UI (C#/.NET)
- Image processing (currently): SixLabors.ImageSharp
- Because it is currently developed for Windows, it utilizes the Windows API-s (user32.dll ie: WorkerW)


How it works:
-------------
This program uses a clever "layering" technique to turn your desktop background into a dynamic, changing canvas without interfering with your files or shortcuts.

- Invisible Layer: The program creates a special, borderless window that acts as a hidden layer.

- Desktop Integration: It uses Windows system commands to find the "WorkerW" layer—a hidden area located exactly behind your desktop icons.

- Parenting: It "attaches" itself to this layer, making the program a part of the desktop background itself.

- Z-Order Management: Finally, it forces itself to the very back, ensuring your desktop icons stay visible while your images or videos play smoothly behind them.


System Tray & Settings
----------------------
The application runs in the background to keep your desktop clean.

- System Tray: You can find the app icon in the system tray (the area next to your clock). If it’s not immediately visible, check the "hidden icons" menu (the small arrow pointing up).

- Control: Right-click the icon to:
- Open Settings: Configure your wallpaper folder and transition interval.


Disclaimer
----------
Note: This project is an independent, open-source utility and is not affiliated with, authorized, maintained, or endorsed by the official Wallpaper Engine application or its developers.

