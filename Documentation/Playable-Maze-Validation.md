# Playable maze foundation

Validated on 2026-09-21 in Unity 6000.4.11f1. This is an implementation milestone,
not a claim that the complete Assessment 4 rubric has been met.

The new ClassicLevel scene uses the existing map, sprite prefabs and audio clips.
The Assessment 3 RecreatedLevel demonstration and delivered ZIP are preserved.

- Arrow keys or WASD queue turns. Reversing direction is immediate.
- Movement uses timed linear interpolation between adjacent cell centres.
- Only the centre-row side tunnel wraps. Walls and house gates block the player.
- Motes award 10 points; elixirs award 50. Each pickup scores only once per round.
- P or Escape pauses. Clearing all pickups allows Enter to start another round.

## Executed validation

The Unity Editor compiled all new scripts and saved ClassicLevel with serialized
prefab/audio references. Core checks passed 3,372 assertions covering the 276-cell
navigation graph, all 222 reachable pickups, blocked walls and gates, buffered
turns, immediate reversal, both tunnel directions, large deltas and callback stops.
Complete 550-edge traversals agreed at 30, 60 and 144 FPS and irregular steps.

An actual Play session traversed the scene, collected 218 motes and four elixirs
for 2,380 points, and reduced visible generated tiles from 660 to 438. Pause held
the player still; round two restored 222 pickups and kept the score. A captured
1440 by 1080 frame was inspected for maze, player and HUD visibility.

The first scene traversal harness attempted to issue turns at floating-point
time boundaries and did not complete the planned route. It was corrected to
issue the next turn on cell arrival, then the full traversal passed. The movement
implementation was not changed to conceal or bypass this failed harness attempt.

Ghost interactions and lives are the next feature and are not covered by this
foundation validation record.
