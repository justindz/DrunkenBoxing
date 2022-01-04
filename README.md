# DrunkenBoxing - a Decal Plugin for Drunkenfell

## Overview

The Drunkenfell ACE server for Asheron's Call includes a quest that rewards a series of custom Life Magic spells intended to allow play as a specialized Life mage in a blended offensive/supportive role. Unfortunately, Virindi Tank is unaware of the existence of these spells. DrunkenBoxing was created as a replacement for the combat mode of Virindi Tank (but not buff, loot, nav, or meta) for players wanting to build around these Life spells as their offensive skill. Eventually, DrunkenBoxing may facilitate other forms of combat not handled by Virindi Tank, including my own battle alchemy content. DrunkenBoxing does not include any UI, relying instead on chat commands for interaction and a hot-reloadable, easy-to-read JSON configuration file per character.

## Major Features

* Detects and, if known, uses Incantation of Bloodstone Bolt, Ring of Death, Corrupted Touch, and Ward of Rebirth (requires fellowship of 2+ members)
* Auto-wields no-wield or life casters based on primary target, switching to magic combat mode as required
* Can prioritize targets at four different levels: Rage, Focus, Normal, Last, and Never
* Can set monsters as bosses to prioritize CB > CS
* Can set mosnters to be targeted with Corrupted Touch
* Can configure attackable enemy range, ring range, and minimum viable target count for rings
* Configurable through per-character JSON files

## Major Limitations

* Does not use Summoning
* Does not debuff enemies
* Does not cast Harm Self (you will have to die via the normal methods)
* Caster auto-selection is a little basic (prioritizes slayer multiplier, then CB on bosses and CS on everything else)

There are also a number of quirks relative to VT that are being tightened up and aren't included on the "major" list. For example, spell cast timings are not as tight as VT, and sometimes VT looting will partially trigger between casts.

## Usage

Most of the plugin's work occurs automatically when enabled, based on your spells, casters, and configuration settings (see Configuration, below). The following is a list of chat commands the plugin recognizes:

* /db on - enable the plugin
* /db off - disable the plugin
* /db dump - write the current settings for the character to a JSON file (useful to then modify and load with the next command)
* /db load - reload settings from the character-specific JSON config file, if found (note: will read from the same folder dump writes to, which may be the AC client folder)
* /db update spells - use this command to start using new spells you have learned since logging in (or last update)
* /db update casters - use this command to refresh the plugin's knowledge of your no-wield or life-req casters
* /db test caster - select an enemy and use this command to test which caster the plugin would select for it
* /db test distance - select an enemy and use this command to test the distance from your character
* /db test enemies - list the enemies currently known to the plugin
* /db test combatants - list the enemies the plugin believes are in combat range
* /db test state - display the current state of the character in the internal state machine

The intent is that eventually you will be able to control all aspects of the plugin via chat commands, giving you flexibility to customize your behaviour in-game and choose whether or not to dump the update config to a file and re-use it going forward, or leave it as a session-only tweak.

## Configuration

Configuring the plugin is primarily done through character-specific JSON files. You can create these from scratch--the syntax is quite simple. Alternately, you can "/db dump" to produce a minimal file for your character. This also places it where it needs to be to "/db load" the file--this is likely the Asheron's Call folder--and generates the proper filename format.

A valid config file looks like this:

```json
{
  "bosses": [
    "Sir Bellas",
    "Tremendous Monouga",
  ],
  "priorities": {
    "Lag Beast": "Never",
    "Stomper": "Normal"
  },
  "dots": [
    "Oak Target Drudge",
  ],
  "fightDistance": 6.0,
  "ringDistance": 4.0,
  "ringMinimumCount": 3
}
```

Here is an explanation of each object:
* bosses - these are enemies where CB imbue will be favored over CS imbue when there isn't a relevant slayer available
* priorities - these entries instruct the plugin on how to choose targets when multiple are in range (see explanation below)
* dots - these are enemies where the plugin should periodically apply Corrupted Touch, rather than only use bolt and ring spells

These are the valid priorities:
* Rage - ignore other enemies and continue attacking Rage enemies until they are gone or out of range
* Focus - prioritize Focus enemies over Normal, Last, and Never, but not Rage
* Normal - prioritize below Focus enemies, but above Last, and Never (this is the default priority for all enemies not explicitly overriden)
* Last - only prioritize these enemies after Rage, Focus, and Normal enemies are gone or out of range
* Never - ignore these enemies entirely--though they can be affected by ring spells which trigger based on on ringMinimumCount (see below)

The bosses, priorities, and dots objects should be present, even if they are empty. Each object has a distinct function, and so the same enemy can be present in any of the three.

These are the configurable numerical values:
* fightDistance - the maximum distance a player can be from an enemy for that enemy to be considered "fightable"
* ringDistance - the maximum distance a player can be from an enemy for the enemy to be counted towards the minimum needed to use rings, which must be a lower value than fightDistance
* ringMinimumCount - the minimum number of enemies in ringDistance needed to prioritize casting a ring spell

## Installation

* Download /PluginCore/bin/Debug/net2.0/x86/Decal.Adapter.dll, Newtonsoft.Json.dll, and PluginCore.dll (or a Release package)
* Place them in a good, happy folder together on your filesystem
* Open Decal, Select "Add", then "Browse" and select PluginCore.dll and hit "Save"
* Confirm DrunkenBoxing.PluginCore displays in the Plugins list in Decal

## License

The MIT License (MIT)
Copyright © 2021 Crane Mountain Gaming, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.