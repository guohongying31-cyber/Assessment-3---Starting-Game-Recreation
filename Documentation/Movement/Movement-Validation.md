# Movement validation

## Core milestone

Unity 6000.4.11f1 compiled the two runtime scripts and saved the patrol prefab
variant and scene instance. The numerical tween checks covered a partial segment,
exact completion, leftover frame time, advancing an already completed segment,
and invalid durations.

The [core Play report](Core-Play-30fps.json) records 451 observed frames over
15.003573 game seconds (30.0595 FPS measured, 30 requested). The cultivator
completed two laps, including eight exact corner turns. The measured full lap
period was 7.200000032 seconds. Maximum route-position error was below 0.000001
world units, and maximum arc-speed error was 0.0000191 units/s.

Speed was checked using distance along the rectangular route, including frames
that crossed a corner. The saved start position, prefab connection, unchanged
scene hash, and all 660 manual maze renderers were checked after leaving Play.

An earlier verification attempt did not complete because its observation
component was inside an Editor folder. The helper was moved to a temporary
runtime folder and given a timeout. That incomplete attempt is not a passing
test; the linked report is from the subsequent successful run.

Directional animation, movement audio, and additional frame rates are not yet
validated at this core milestone. No standalone player build has been tested.
