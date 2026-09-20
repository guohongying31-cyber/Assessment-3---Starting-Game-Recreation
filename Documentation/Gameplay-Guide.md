# Shan Hai Spirit Trail: play guide

## Launch

Use Unity 6000.4.11f1. Open `Assets/Scenes/MainMenu.unity`, then press Play.
The build scene order is MainMenu, ClassicLevel, TalismanLevel. The original
RecreatedLevel scene remains available as the Assessment 3 demonstration.

A Windows preview is available at `output/PlayableBuild/ShanHaiSpiritTrail.exe`.
Keep its Data directory, UnityPlayer.dll and other build dependencies beside it.
This preview is not an Assessment 4 submission archive.

## Controls

| Input | Action |
| --- | --- |
| Arrow keys / WASD | Move; queue the next turn before an intersection |
| Opposite direction | Reverse immediately while moving |
| P / Escape | Pause or resume |
| Space | Activate the ward in Talisman Trial |
| Enter | Next round after clearing; restart after Game Over; select a menu button |
| M while paused / Ready / results | Return to the main menu |
| 1 / 2 on main menu | Start Classic Trail / Talisman Trial |
| Mouse | Select menu and pause/result buttons |

## Classic Trail

Collect 218 spirit motes and four elixirs to clear the maze. Motes give 10 points;
elixirs give 50 and temporarily make guardians edible. A chain of four guardian
captures gives 200, 400, 800 and 1,600 points. Returning spirits are harmless.

Start with three lives. Ordinary guardian contact removes one life while keeping
collected items and score. One extra life is granted at 10,000 points. Crimson
fruit appears twice per round for a limited time. Clear every pickup and press
Enter or Next Round to continue. Later rounds become faster with shorter elixir
effects. A separate best score is saved for each mode.

## Talisman Trial

User direction, translated into English: "Use Pac-Man's rules and add an actively
usable talisman skill to the innovation scene."

This mode uses the same maze and base rules so the skill's effect is easy to
compare with Classic Trail. Press Space during play for a three-second protective
ward. It permits contact with ordinary guardians without losing a life or gaining
points. Frightened guardians still award normal capture points. The ward recharges
15 seconds after activation, including its three active seconds. Repeated presses
do not extend protection. Pause freezes both timers. Each respawn or new round
restores a ready ward.

The intended decision is when to spend a short period of safety: crossing a
blocked junction, escaping a chase, or reaching an elixir. The visible aura and
countdown show when the protection will end. Remaining inside a guardian as the
ward expires is dangerous. Classic Trail never enables the skill.

## Development and validation

- [Maze foundation](Playable-Maze-Validation.md)
- [Classic gameplay](Classic-Rules-Validation.md)
- [Menu, ward and build checks](Menu-Ward-Validation.md)

All authored project text and UI use English. The current work continues the
original development repository without changing its earlier history or the
delivered Assessment 3 archive. The complete Assessment 4 rubric has not been
provided, so this guide does not claim a final grade or complete submission scope.
