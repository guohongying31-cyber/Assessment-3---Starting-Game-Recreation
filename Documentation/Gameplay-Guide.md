# PacStudent: Shan Hai Spirit Trail

## Open and play

Use Unity 6000.4.11f1. Open `Assets/Scenes/StartScene.unity` and press Play.
The enabled scene order is StartScene, ClassicLevel, InnovationScene.
The separate RecreatedLevel scene preserves the Assessment 3 visual demonstration.

The Windows build is in `output/Assessment4Build`. Launch `ShanHaiSpiritTrail.exe`
with its Data folder, UnityPlayer.dll and other generated dependencies beside it.

## Controls

| Input | Action |
| --- | --- |
| WASD or arrow keys | Request a direction; the latest request is kept until valid |
| Opposite direction | Finish the current grid cell, then reverse |
| P or Escape | Pause or resume |
| Space in Level 2 | Activate the protective talisman when ready |
| Exit | Return to StartScene without recording an unfinished run |
| 1 or 2 in StartScene | Open Level 1 or Level 2 |
| Enter in StartScene | Activate the selected menu button |

## Level 1: the Jade Labyrinth

Collect 218 spirit motes and four ward elixirs. Begin with three Soul Jade life
indicators. The cultivator starts in the top-left corridor facing right and waits
for input after the four-second `3, 2, 1, GO!` countdown.

Movement uses one-cell linear interpolation at five tiles per second. A blocked
requested turn stays buffered while the cultivator continues in the current
direction. At the next valid junction the requested turn takes effect. A reversal
also waits for the current cell to finish. The side tunnels connect the two edges.

| Item or contact | Result |
| --- | --- |
| Spirit mote | 10 points; the mote is removed |
| Ward elixir | 50 points; guardians become Scared for ten seconds |
| Crimson fruit | 100 points; the fruit is removed |
| Scared or Recovering guardian | 300 points; its spirit returns home |
| Normal guardian | Lose one life and play the death sequence |

The last three seconds of the elixir effect use the Recovering animation.
The integer SCARED timer is hidden when the effect ends. Eating another elixir
refreshes the full ten seconds. Returning spirits remain harmless and cannot be
captured a second time before reviving.

Crimson fruit first appears after five unpaused scene seconds, including the
initial countdown and death sequences. It flies through the centre at three units
per second. It ignores the maze walls and is visible only inside the map boundary.
The next fruit appears five seconds after the previous one is collected or exits.

## The numbered guardians

All four guardians start evenly spaced in the centre. Numbers identify the
behaviors required by the assessment, independently of their themed sprites.

| Number | Guardian | Normal behavior | House exit |
| --- | --- | --- | --- |
| 1 | Nine-tailed fox | Random valid direction that increases or preserves distance from the player | Top |
| 2 | Flame crane | Random valid direction that decreases or preserves distance from the player | Bottom |
| 3 | White-headed hound | Random valid direction | Top |
| 4 | Serpent tortoise | Follows the outside wall clockwise | Bottom |

Living guardians stay in corridors, cannot teleport through side tunnels, and reverse only when
there is no other physical exit. If a distance filter leaves no eligible forward
route, a legal nonreverse route is selected so a guardian never stops or makes
an unnecessary backstep. The formal checklist explains this underspecified case.

Normal speed is 4.5 tiles per second. Scared, Recovering and Dead speed is 2.25.
All Scared or Recovering guardians use the first guardian's fleeing behavior.
Dead spirits move directly through walls to their own original spawn, then
adopt the current global state and leave through the assigned gate. There is no
fixed revival timer in the final ghost implementation.

## Results and records

The game clock begins after GO disappears. Death sequences count as elapsed play
time; pause does not. Death freezes all guardian movement, then restores the
cultivator to the top-left and resets the guardians. The player waits for fresh
input and the initial countdown is not repeated.

Clearing all pickups or losing all three lives shows `Game Over`. Movement, game
time and buttons are disabled for three seconds before an automatic menu return.
Each level stores its own highest score and the time achieved with that score.
An equal score replaces the record only if the time is lower. Leaving an unfinished
run does not save a result. Records persist on the local computer using PlayerPrefs.

## Level 2: Talisman Trial

The active talisman adds a timing decision to the same maze and scoring rules.
Press Space for three seconds of protection from Normal guardians. Protection
does not award points, eat a Normal guardian or change its route. Scared guardians
still award the regular 300 points when captured.

The talisman recharges fifteen seconds after activation, including its active
three seconds. Repeated presses do not extend it. The aura and HUD show its active
duration and recharge. The HUD reports CONTACT BLOCKED after a successful block.
Pause freezes both timers; a new life restores availability. Normal guardian
contact becomes dangerous immediately after protection expires.

Saving the talisman for a blocked junction or an elixir approach gives the player
control over one short risky passage. Spending it early leaves a twelve-second
unprotected interval, so it complements movement planning rather than replacing it.

## Verification

See `Documentation/Assessment4-Checklist.md` for formal acceptance criteria and
`Documentation/Assessment4-Validation.md` for checks actually executed. Historical
preview validation documents describe earlier development states; the formal
Assessment 4 record and this guide describe the final rules.
