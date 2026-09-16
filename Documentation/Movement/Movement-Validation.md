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

Directional animation, movement audio, and additional frame rates were outside
that core milestone. No standalone player build has been tested.

## Direction animation and audio milestone

The patrol prefab variant now enables the shared Animator with scaled time and
adds a dedicated 2D footstep AudioSource. Its presentation component selects and
evaluates the matching direction at each corner without interrupting audio.

| Requested frame rate | Measured FPS | Observed frames | Measured lap period | Report |
| --- | --- | --- | --- | --- |
| 30 | 29.4578 | 442 | 7.199999976 s | [30 FPS Play](Presentation-Play-30fps.json) |
| 60 | 60.0464 | 901 | 7.199999996 s | [60 FPS Play](Presentation-Play-60fps.json) |
| 144 | 143.8818 | 2159 | 7.199999973 s | [144 FPS Play](Presentation-Play-144fps.json) |

Each session observed at least 15 game seconds, two completed laps and eight
corner events. Matching Animator states and sprite directions were checked at
the corner event itself and on every observed frame. All eight walking sprites
were observed in each session. Peak route-position error was below 0.000001
units. Arc-speed error remained below 0.000102 units/s.

The movement AudioSource stayed active throughout the measured patrol, wrapped
its loop 19 times and produced nonzero output samples. Intro and normal music
also played. The showcase sprites retained their positions and all five gallery
character Animators retained `Showcase = true`.

Each session then disabled and resumed the patrol, paused and restored time
scale, and disabled and re-enabled the presentation component. Position,
animation and sound paused together when the patrol stopped. Playback and the
correct facing state returned on resume. Stop restored the saved scene.

An initial 60 FPS check exposed a one-frame animation advance after disabling
the patrol. Playback synchronization was moved from LateUpdate to Update, before
Unity evaluates animation. The linked 60 FPS rerun and 144 FPS session passed
with this correction. The final 30 FPS rerun also passed. Its maximum game-time
frame delta was 0.333333 seconds; interpolation retained the correct route and
lap duration despite that longer frame. Frame-rate averages and lap periods use
Unity's game clock, not wall-clock time.

The final 30 FPS session rechecked every manual tile before Play and after Stop:
660 exact world positions, rotations/reflections and prefab references. The
[scene regression record](Scene-Regression.json) also confirms the patrol sprite
renders above map sprites.

When the working project was temporarily open in another Unity window, isolated
copy attempts hit a Unity Editor search-index exception and did not finish.
They are not passing tests. All three linked final reports were produced in the
working project after it became available.

The [full scene](Patrol-Play.png) and [patrol close-up](Patrol-Closeup.png) are actual
camera renders captured after the 60 FPS timing window. The close-up temporarily
changes the camera and hides the showcase labels; these changes were restored
without saving. Both renders were inspected for character visibility, passage
scale and English labels. Pickups remain present because collection is outside
this feature's scope.

## Pre-merge validation

After removing all temporary Editor and runtime observation components, Unity
6000.4.11f1 completed a clean import/compilation with exit code 0. Its log contains
no C# compilation errors, compilation failures or exceptions. All 238 asset file
hashes were unchanged by this import. Three runtime C# files remain in Assets.

The [static audit](Static-Audit.json) checks 331 authored files, including 303 ASCII
text files and 129 unique asset GUIDs. Local documentation links resolve, metadata
remains paired with assets, and all seven sprite sheets and eleven audio files
retain their previously validated hashes. The pre-existing untracked package
settings file is unchanged and excluded from this feature.

These checks complete the movement feature's technical criteria. Runtime level
generation, a standalone player build, final submission validation, the final
Main merge and ZIP packaging remain outside this stage. Automated audio output
checks do not replace a listening review.
