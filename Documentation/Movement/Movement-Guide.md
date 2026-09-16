# Cultivator patrol

The scene's `Characters/PacStudent` follows a fixed clockwise route around the
first inner block. Its saved starting point is `(1, -1, 0)`.

| Direction | Start | End | Distance | Duration at 2.5 units/s |
| --- | --- | --- | --- | --- |
| Right | (1, -1) | (6, -1) | 5 | 2.0 s |
| Down | (6, -1) | (6, -5) | 4 | 1.6 s |
| Left | (6, -5) | (1, -5) | 5 | 2.0 s |
| Up | (1, -5) | (1, -1) | 4 | 1.6 s |

One lap covers 18 world units in 7.2 seconds. Coordinates are world positions,
with the source array's first cell at `(0, 0)` and rows extending downward.
The route stays fixed when a later generator changes the map.

`LinearPositionTween` interpolates from a segment's start to its end using
`elapsed / duration`. `PacStudentPatrol` calculates each duration as distance
divided by speed, advances with `Time.deltaTime`, and carries unused frame time
into the next segment. It reaches the exact corner before changing direction.
There is no velocity integration, physics movement, keyboard input, collision
response, or pickup collection in this feature.

The `PacStudentPatrol` prefab is a variant of the existing cultivator prefab.
The gallery's cultivator remains a separate showcase instance. The patrol root
has unit scale and its sprite renders above the maze pickups.

## Animation and audio

`PacStudentPresentation` disables the patrol Animator's `Showcase` parameter and
selects `WalkingRight`, `WalkingDown`, `WalkingLeft`, or `WalkingUp` when the
patrol changes direction. It evaluates the new state immediately. The existing
two-frame clips keep looping between corners. The gallery still uses its
automatic preview cycles.

The patrol's dedicated AudioSource plays `SFX_Move_SoftSteps` as a continuous
0.8-second loop at volume 0.5. It uses 2D playback with no Doppler or reverb-zone
processing. Turns do not restart the sound. The original audio-library source
remains idle; intro and normal music continue through `Systems/LevelAudio`.

Animation uses scaled time. Setting `Time.timeScale` to zero or disabling the
patrol component freezes movement and animation and stops its audio. Resuming
continues the route and restarts the footstep loop. Disabling the presentation
component stops its animation and audio; it does not disable the separate patrol.

## Inspect in Unity

1. Open `Assets/Scenes/RecreatedLevel.unity` and press Play.
2. Watch the small cultivator in the maze's upper-left passage for at least two
   laps. The large cultivator at the left is the animation showcase.
3. Select `Characters/PacStudent` to inspect its patrol, Animator and AudioSource.
4. Disable and re-enable the patrol to check pause and resume. Stop and Play
   again to restart at `(1, -1)` facing right.

Set `Units Per Second` on the patrol prefab before Play to adjust the speed.
Each segment then receives a new duration based on its own distance. Keep the
root scale at one and retain the `Visual` child path used by the animation clips.

See [validation](Movement-Validation.md) for checks that actually ran.
