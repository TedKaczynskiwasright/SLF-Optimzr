@echo off
setlocal
set "GAME=StreetLeagueFootball.exe"
set "DIR=%APPDATA%\Godot\app_userdata\Street League Football"
set "CFG=%DIR%\settings.cfg"
set "BAK=%DIR%\settings.cfg.bak"

tasklist /fi "imagename eq %GAME%" 2>nul | find /i "%GAME%" >nul
if not errorlevel 1 (
    echo close the game first, it rewrites settings.cfg when it exits.
    pause
    exit /b 1
)

if not exist "%BAK%" (
    echo no backup found at:
    echo %BAK%
    echo nothing was changed.
    pause
    exit /b 1
)

move /y "%BAK%" "%CFG%" >nul
if errorlevel 1 (
    echo restore failed, nothing was changed.
    pause
    exit /b 1
)

echo settings.cfg restored from backup.
pause