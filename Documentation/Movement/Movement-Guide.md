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
The gallery's cultivator remains a separate showcase instance. The core milestone
uses a stationary right-facing sprite while the transform follows the route;
directional animation and movement audio are the next milestone.

See [validation](Movement-Validation.md) for checks that actually ran.
