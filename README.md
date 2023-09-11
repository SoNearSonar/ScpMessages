# ScpMessages
## A plugin for SCP: Secret Laboratory on NWAPI which displays messages based on player interactions (Inspired by SCP: Containment Breach interaction messages)

## Credits:
- [Northwood Studios](https://store.steampowered.com/developer/NWStudios) - [NWAPI](https://github.com/northwood-studios/NwPluginAPI)
- [JamesNK](https://github.com/JamesNK/) - [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Pardeike](https://github.com/pardeike/) - [Harmony](https://github.com/pardeike/Harmony)

## Features:
- Interactions for the following events:
	- Items (Tossing items, throwing projectiles, picking up armor and SCP-330 candy)
	- Map (Accessing doors, lockers, elevators, and generators (unlock only for this specific one))
	- Damage (Humans attacking other humans or SCP's, SCP's attacking humans)
	- Team (MTF and Chaos Insurgency respawning)
- Toggles for displaying messsages entirely and for the above events
- Tokens for dynamic message content in the above events
- Persistent individual user message toggling in-game

## Documentation:
[Link](https://github.com/SoNearSonar/ScpMessages/blob/main/Documentation.md)

## Install:
### LocalAdmin Install:
Installing this plugin can be done automatically by running the following command in your LocalAdmin console: ```p install SoNearSonar/ScpMessages```

### Manual Install:
Installing this plugin can be done manually by going to the [releases](https://github.com/SoNearSonar/ScpMessages/releases) page and following these steps:
1. Download the ScpMessages.dll file
2. Go to this directory: ```PluginAPI/plugins/(your_port)``` and place the above .dll file there
3. Go to this directory: ```PluginAPI/plugins/(your_port)/dependencies``` add the .dll's in the dependencies.zip in there
4. Restart your server

## Notes:
This plugin will create a file for storing user choices for the ScpMessages message displays at ```config/(your_port)/ScpMessages/Stored/toggles.json```. 
Do not delete or alter it unless you know what are you are doing!
