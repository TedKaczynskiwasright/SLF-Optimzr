# SLF Optimzr

Small tool I made to get a few more frames out of Street League Football on Windows. It bumps the game's priority, stops Windows from throttling it, speeds up the system timer, drops the graphics to a lighter setup and starts the game with vsync off.

It doesn't touch any game files. Only the game's settings.cfg gets edited, and it makes a backup first.

## Before you start

You need Windows 10 or 11 and the game from itch.io. Run the game once so it makes its settings file, then close it.

Put `SLF Optimzr.exe` in a folder of its own. Open a terminal in that folder (type `cmd` in the address bar and hit Enter).

## Using it

Lighter graphics, once, with the game closed:

    "SLF Optimzr.exe" --write-settings

It should say the settings were patched and leave a `settings.cfg.bak` next to the original.

Then start the game through the tool:

    "SLF Optimzr.exe" --launch

It looks for the game on its own. If it can't find it, it asks for the path to the exe one time and remembers it in `game-path.txt`.

Leave the tool window open while you play. When it prints "tuned pid" it has found the game. If you close the window, the faster timer goes away with it.

To check it worked, open Task Manager, go to Details and add the Base priority column. The game should say High. The FPS counter will also show up in game.

## Extras

`--ping` tests your connection to 1.1.1.1 and prints min, average, max and jitter. Run it before and after to compare.

`--net-tweaks` changes two Windows network values. Needs an admin terminal and a restart. `--net-undo` puts them back.

## Putting everything back

Close the game and swap `settings.cfg` with `settings.cfg.bak`. If you used `--net-tweaks`, run `--net-undo` as admin. Delete `game-path.txt` if you want it to search for the game again.

## If something goes wrong

Stuck on "waiting": the process name doesn't match. Check the name in Task Manager > Details against the one in the code.

"settings.cfg not found": run the game once first.

FPS still capped at your refresh rate: your graphics driver is forcing vsync. Turn it off for the game in the NVIDIA or AMD control panel.

Screen tearing with vsync off is normal. Borderless mode hides it best.

## Honest notes

Most of the gain comes from the lighter graphics. The priority and timer tweaks are small. Nothing in here lowers your real ping, a wired connection does more than any tool.
