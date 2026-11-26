# SLF Optimzr

```text
 ____   _      _____
/ ___| | |    |  ___|
\___ \ | |    | |_
 ___) || |___ |  _|
|____/ |_____||_|

  street league football // optimzr
```

a small tool i made to get more fps out of street league football on windows. it bumps the game's priority, stops windows from throttling it, speeds up the system timer, swaps the graphics profile and helps turn vsync off.

it doesn't touch the game files. it only edits the game's settings.cfg (makes a backup first) and can drop an override.cfg next to the exe.

## what you need

windows 10 or 11 and the game from itch: https://streetleaguefootball.itch.io/street-league-football

run the game once so it makes its settings file, then close it fully.

## how to use

1. put `SLF Optimzr.exe` and `Run SLF Optimzr.bat` in the same folder
2. double click the .bat and hit yes on the windows prompt (it needs admin for the network stuff)
3. use the arrow keys and enter to pick a row
4. first time, with the game closed, go top to bottom: pick a graphics profile, vsync off, fullscreen optimizations off
5. launch the game, then pick start tuner and leave that window open while you play

when it says "game tuned" it found the game. closing the window drops the faster timer.

to check it worked, open task manager > details and add the base priority column. the game should say high. the fps counter also shows in game now.

## profiles

| profile | what it does | good for |
|---|---|---|
| potato | shadows off, no aa, no extras, 70% render scale | old laptops, intel graphics |
| balanced | low shadows and detail, light aa, auto render scale | most pcs |
| beast | high detail and shadows, 2x aa, full resolution | a real graphics card |

these are the game's own presets so they still show up in its settings menu. the speed and looks bars in the picker are just a rough guide.

potato turns replays off and the other two turn them back on, same as the game does.

## menu rows

**graphics profile** left and right to switch, enter to apply. game has to be closed.

**vsync off** writes an override.cfg next to the game exe.

**fullscreen optimizations off** flips a windows flag for the game exe so fullscreen frames stop getting smoothed by windows.

**launch game** finds the game and starts it with vsync off.

**start tuner** raises priority, turns off power throttling and holds a 1 ms timer. ctrl+c goes back to the menu.

**ping test** pings 1.1.1.1 and shows min, avg, max and jitter.

**network tweaks on / undo** changes two windows network values and puts them back. needs admin and a restart.

## without the menu

    "SLF Optimzr.exe" --profile potato
    "SLF Optimzr.exe" --vsync-off
    "SLF Optimzr.exe" --fso-off
    "SLF Optimzr.exe" --launch
    "SLF Optimzr.exe" --tune
    "SLF Optimzr.exe" --ping
    "SLF Optimzr.exe" --net-tweaks
    "SLF Optimzr.exe" --net-undo
    "SLF Optimzr.exe" --no-banner

`--profile` takes potato, balanced or beast. `--fso-undo` reverses `--fso-off`. keep the quotes, the name has a space in it.

## undo everything

close the game, then:

- swap `settings.cfg` with `settings.cfg.bak`
- delete `override.cfg` (or put back `override.cfg.bak` if there was one)
- run `--fso-undo`
- run `--net-undo` as admin if you used the network tweaks
- delete `game-path.txt` if you want it to search for the game again

## if something breaks

**stuck on "waiting"** the process name doesn't match. check the name in task manager > details.

**settings.cfg not found** run the game once first.

**fps still stops at your refresh rate** your graphics driver is probably forcing vsync. turn it off for the game in intel graphics command center, nvidia control panel or amd adrenalin. test in fullscreen or borderless, windowed can still smooth frames.

**low fps on intel graphics** close chrome, plug the laptop in and set windows settings > graphics to high performance for the game. intel graphics use your ram so slow or single channel ram hurts a lot.

**tearing** normal with vsync off. borderless hides it best.

**menu looks cut off** make the window at least 90 columns wide.

**windows or antivirus warns about the exe** it's unsigned and it changes process priority and a few registry values, which looks sketchy to some scanners. the source is in this repo.

## notes

on my crappy intel iris xe pc, turning vsync off made my fps 120+. it includes waiting for each frame. it's not your route to the server, nothing on your pc changes that.

most of the fps comes from the graphics profile. the priority and timer stuff is small.

if your fps is way under 60 to begin with, vsync isn't your problem, your gpu is.
