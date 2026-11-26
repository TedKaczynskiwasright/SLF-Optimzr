# SLF Optimzr

```text
 ____   _      _____
/ ___| | |    |  ___|
\___ \ | |    | |_
 ___) || |___ |  _|
|____/ |_____||_|

  street league football // optimzr
  [############################] ready
```

Small tool I made to get more frames out of Street League Football on Windows. It raises the game's priority, stops Windows from throttling it, speeds up the system timer, switches the graphics to a light profile and turns vsync off.

It never touches the game's own files. It edits the game's settings.cfg (backup first) and, if you ask for it, writes an override.cfg next to the exe.

## Before you start

You need Windows 10 or 11 and the game from itch.io: https://streetleaguefootball.itch.io/street-league-football

Run the game once so it creates its settings file, then close it completely.

Put `SLF Optimzr.exe` in a folder of its own and open a terminal there. Type `cmd` in the folder's address bar and hit Enter.

## Using it

Run these once, with the game closed.

Light graphics (shadows off, 70% render scale, no AA, no reflections):

    "SLF Optimzr.exe" --write-settings

Vsync off:

    "SLF Optimzr.exe" --vsync-off

Keep the quotes, the name has a space in it. The tool finds the game by itself. If it can't, it asks for the exe path one time and remembers it in `game-path.txt`.

Then start the game and leave the tuner running while you play:

    "SLF Optimzr.exe"

When it prints "tuned pid" it found the game. Closing the window drops the faster timer. You can also start the game through the tool with `--launch`.

To check it worked, open Task Manager, go to Details and add the Base priority column. The game should say High. The FPS counter shows in game, and with vsync off it can go past your monitor's refresh rate.

## All commands

`--write-settings` applies the light graphics profile.
`--vsync-off` writes override.cfg next to the game exe.
`--launch` starts the game for you.
`--ping` tests latency to 1.1.1.1 and prints min, average, max and jitter.
`--net-tweaks` and `--net-undo` change two Windows network values and put them back. Admin terminal and a restart needed.
`--no-banner` skips the boot animation.

## Putting everything back

Close the game, then:

- swap `settings.cfg` with `settings.cfg.bak`
- delete `override.cfg` (or restore `override.cfg.bak` if there was one)
- run `--net-undo` as admin if you used `--net-tweaks`
- delete `game-path.txt` to make it search for the game again

## If something goes wrong

Stuck on "waiting": the process name doesn't match. Compare it with the name in Task Manager > Details.

"settings.cfg not found": run the game once first.

FPS still stops at your refresh rate: the graphics driver is forcing vsync. Turn it off for the game in Intel Graphics Command Center, NVIDIA Control Panel or AMD Adrenalin. Use borderless or fullscreen when testing, windowed mode can still smooth frames.

Low FPS on Intel graphics: close Chrome and other apps, plug the laptop in, and set Windows Graphics settings to High performance for the game. Integrated graphics use system RAM, so memory speed matters.

Tearing with vsync off is normal. Borderless hides it best.

## Honest notes

Most of the gain comes from the light graphics profile, the priority and timer tweaks are small. Nothing in here lowers your real ping, a wired connection does more than any tool.
