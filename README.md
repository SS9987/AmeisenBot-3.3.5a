# AmeisenBot - WoW 3.3.5a Bot [![Build Status](https://jenkins.jnns.de/buildStatus/icon?job=AmeisenBot)](https://jenkins.jnns.de/job/AmeisenBot/) [![codecov](https://codecov.io/gh/Jnnshschl/WoW-3.3.5a-Bot/branch/master/graph/badge.svg)](https://codecov.io/gh/Jnnshschl/WoW-3.3.5a-Bot) [![HitCount](http://hits.dwyl.io/jnnshschl/WoW-3.3.5a-Bot.svg)](http://hits.dwyl.io/jnnshschl/WoW-3.3.5a-Bot)


A bot written in (at this time) C# only for World of Warcraft WotLK (3.3.5a 12340) (the best WoW :P).
This project will be developed like "Kraut und Rüben" (Herb and beet?) as we say here in Germany, its a synonym for messy, so deal with it 😎.

⚠️ Currently this thing is only a playground for me to try out memory-hacking related stuff, but maybe it turns into something useable in the near future...

## Credits

❤️ **Blackmagic** (Memory Editing) - https://github.com/acidburn974/Blackmagic

## Usage

Although i don't recommend to run this thing in this stage, **you can do it!**

🕹️ **How to use the Bot:**
Compile it, Start it, profit i guess?

🌵 **How to enable AutoLogin:**
Place the "WoW-LoginAutomator.exe" in the same folder as the bot, thats all...

🔪 **How to make a CombatClass:**
Template \*.cs file:
```c#
...
```

## Modules
**AmeisenBot.Combat**: CombatClass utilities & template

**AmeisenBot.Core**: Collection of some static object-reading/casting/lua functions

**AmeisenBot.Data**: "DataHolder" to hold things like our playerobject, our target object & active WoWObjects

**AmeisenBot.DB**: Database connection manager, from this thing the map is beeing read/saved

**AmeisenBot.FSM**: StateMachine of the Bot executing all actions

**AmeisenBot.GUI**: WPF GUI

**AmeisenBot.Logger**: Basic logging class

**AmeisenBot.Manager**: Create a new Bot instance here and manage it

**AmeisenBot.Mapping**: Mapping related stuff like loading/saving nodes

**AmeisenBot.Test**: Maybe some tests will appear in this module in the near future

**AmeisenBot.Utilities**: Memory offsets, data structs and a few math related funtions

**AmeisenPathLib**: Pathfinding using A*

**WoWLoginAutomator**: Auto-Login into WoW 3.3.5a

## Screenshots

**Maybe outdated!**

### Character selection without AutoLogin

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/charselect.PNG?raw=true "Character selection")

### Character selection with AutoLogin

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/charselect_auto.PNG?raw=true "Character selection Autologin")

### Main bot screen

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/mainscreen.PNG?raw=true "Mainscreen")

### Settings

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/settings.PNG?raw=true "Settings")

### Map

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/map.PNG?raw=true "Map")

### Debug UI

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/debug.PNG?raw=true "Debug GUI")
