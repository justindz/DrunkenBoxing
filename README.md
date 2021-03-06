# DrunkenBoxing - a Decal Plugin for Drunkenfell

## Overview

The Drunkenfell ACE server for Asheron's Call includes a quest that rewards a series of custom Life Magic spells intended to allow play as a specialized Life mage in a blended offensive/supportive role. Unfortunately, Virindi Tank is unaware of the existence of these spells. DrunkenBoxing was created as a replacement for the combat mode of Virindi Tank (but not buff, loot, nav, or meta) for players wanting to build around these Life spells as their offensive skill. Eventually, DrunkenBoxing may facilitate other forms of combat not handled by Virindi Tank, including my own battle alchemy content. DrunkenBoxing does not include any UI, relying instead on chat commands for interaction and a hot-reloadable, easy-to-read JSON configuration file per character.

## Major Features

* Detects and, if known, uses Incantation of Bloodstone Bolt, Ring of Death, Corrupted Touch, and Ward of Rebirth (requires fellowship of 2+ members)
* Auto-wields casters based on primary target, switching to magic combat mode as required
* Can configure a preferred wand for PVE (e.g. CS), bosses (e.g. CB), and an optional wand preference per enemy race (e.g. Slayers)
* Can prioritize targets at four different levels: Rage, Focus, Normal, Last, and Never
* Can set monsters as bosses to prioritize CB > CS
* Can set mosnters to be targeted with Corrupted Touch
* Can configure attackable enemy range, ring range, and minimum viable target count for rings
* Configurable through per-character JSON files

## Major Limitations

* Does not use Summoning
* Does not debuff enemies
* Does not cast Harm Self (you will have to die via the normal methods)

There are also a number of quirks relative to VT that are being tightened up and aren't included on the "major" list. For example, spell cast timings are not as tight as VT, and sometimes VT looting will partially trigger between casts, or heals will be slightly delayed due to cast timings between the two plugins.

## Usage

Most of the plugin's work occurs automatically when enabled, based on your spells, casters, and configuration settings (see Configuration, below). The following is a list of chat commands the plugin recognizes:

* /db on - enable the plugin
* /db off - disable the plugin
* /db dump - write the current settings for the character to a JSON file (useful to then modify and load with the next command)
* /db load - reload settings from the character-specific JSON config file, if found (note: will read from the same folder dump writes to, which may be the AC client folder)
* /db update spells - use this command to start using new Mountain Retreat Life Magic spells you have learned since logging in (or last update)
* /db set caster TYPE - set the selected caster to be the preferred caster for TYPE=pve, TYPE=boss, or TYPE=race where race is a string with or without initcaps (e.g. "Tumerok" or "shadow")
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
  "dots": [
    "Pyreal Target Drudge",
  ],
  "priorities": {
    "Lag Beast": "Never",
    "Stomper": "Normal",
  },
  "fightDistance": 6.0,
  "ringDistance": 4.0,
  "ringMinimumCount": 3,
  "pveCaster": -2146505185,
  "bossCaster": -2147483648,
  "slayerCasters": {
    "Olthoi": -2147483648,
    "Banderling": -2147483648,
    "Drudge": -2147483648,
    "Lugian": -2147483648,
    "Tumerok": -2147483648,
    "Golem": -2147483648,
    "Undead": -2147483648,
    "Tusker": -2147483648,
    "Virindi": -2147483648,
    "Wisp": -2147483648,
    "Shadow": -2147483648,
    "Zefir": -2147483648,
    "Skeleton": -2147483648,
    "Human": -2147483648,
    "Niffis": -2147483648,
    "Elemental": -2147483648,
    "Burun": -2147483648,
    "Ghost": -2147483648,
    "Viamontian": -2147483648,
    "Mukkir": -2147483648,
    "Anekshay": -2147483648,
    "None": -2147483648
  }
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

These are the configurable item IDs:
* pveCaster - the caster to use against general enemies (e.g. CS)
* bossCaster - the caster to use against anything flagged as a boss (e.g. CB)
* slayerCasters - the caster to use against an enemy with the selected race

The priority for caster auto-selection is: slayer -> boss -> pve

Note that you can set the caster in-game using a command and them dump the config file. You should not have to manually configure this section. Also, if a race is not listed, it's because it does not currently have a slayer. Create an issue if I missed any custom slayers that weren't in retail, and I'll add support for that race.

## Installation

* Download /PluginCore/bin/Debug/net2.0/x86/Decal.Adapter.dll, Newtonsoft.Json.dll, and PluginCore.dll (or a Release package)
* Place them in a good, happy folder together on your filesystem
* Open Decal, Select "Add", then "Browse" and select PluginCore.dll and hit "Save"
* Confirm DrunkenBoxing.PluginCore displays in the Plugins list in Decal

## License

The MIT License (MIT)
Copyright ?? 2021 Crane Mountain Gaming, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ???Software???), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ???AS IS???, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.