# Generator validation

## Coordinate foundation

Unity 6000.4.11f1 compiled the map and coordinate classes and ran
[the geometry checks](Geometry-Validation.json). Twenty-five source-dimension
combinations covered rows 1, 2, 7, 15 and 22 against columns 1, 3, 9, 14 and 21.
Every source cell reflected to the expected full-grid coordinate exactly once;
the shared center row was neither lost nor duplicated. Input-array changes did
not mutate an existing layout snapshot. Null, empty and unknown-category arrays
were rejected.

The numeric default map reproduced the expected dimensions and category counts.
These are coordinate/data checks, not wall-topology or Play tests. Some small
test arrays intentionally exercise only geometry and are not valid maze designs.
Runtime replacement, wall orientation, camera fitting and alternate-map Play
validation remain pending at this milestone.

## Neighbor rules and runtime replacement

The [wall-rule report](Wall-Rules-Validation.json) records an actual Unity run.
All 110 default wall angles match the manually authored placement worksheet.
Four independently designed [replacement maps](Validation-Maps.json) also match
their expected source-cell angles exactly: Small (9x8), Wide (9x20), Tall (21x8)
and AllTAndGateOrientations (15x14). They cover every corner and T orientation,
both seal orientations and adjacent parallel walls. A deliberately ambiguous
cluster produces the same valid solution on repeated calls. Four invalid solver
inputs were rejected with the expected diagnostics.

The [runtime core Play report](RuntimeCore-Play-Default.json) verifies the actual
Start/Instantiate/Destroy path. All 660 generated objects match expected world
positions, transformed basis vectors and sprite sources. The center row is
unique, all four elixirs show both animation frames and no physics or missing
script components were found. The manual level disappears during Play and is
restored after Stop with the same scene hash. The original patrol continues at
2.5 units/s with movement audio.

Two early harness attempts assumed the EnteredPlayMode callback ran before
Start and the first movement update. They failed before validation completed.
Map substitution and starting-position evidence were moved to sceneLoaded,
before Start. Only the subsequent passing report is counted here.

This short core session does not establish complete music/showcase coverage,
alternate-map Play behavior or camera adaptation. Those checks follow in the
presentation milestone.

## Adaptive presentation and alternate-map Play

Five actual Play sessions exercise the generated scene. The default session
runs for at least 28 seconds; each alternate session observes both elixir frames
and the unchanged patrol before additional regeneration and camera checks.

| Map | Full columns x rows | Objects | Matching wall seams | Play evidence |
| --- | --- | --- | --- | --- |
| Default | 28 x 29 | 660 | 438 | [Default](Final-Play-Default.json) |
| Small | 16 x 17 | 220 | 146 | [Small](Final-Play-Small.json) |
| Wide | 40 x 17 | 364 | 194 | [Wide](Final-Play-Wide.json) |
| Tall | 16 x 41 | 364 | 194 | [Tall](Final-Play-Tall.json) |
| All T and gate orientations | 28 x 29 | 440 | 306 | [Orientation coverage](Final-Play-AllTAndGateOrientations.json) |

Every generated object's position, reflected basis vectors and source sprite
are compared with the independent fixture, rather than reusing the generator's
angle calculation as the expected result. The [pixel seam check](Seam-Validation.json)
compares the source wall edges after those verified transforms. All active edges
connect with identical alpha profiles; color differences occur only at the
intentional purple seal transitions. Four boundary wall ends form the side exits
in each map.

Each session rejects an incompatible-map retry and a missing-prefab retry while
preserving the current valid level. Repeated valid calls replace the previous
roots, with only one generated hierarchy remaining after deferred destruction.
The display is also disabled during a different-map generation and re-enabled
without changing aspect, checking that both camera and inventory catch up.

The default session observes all 46 showcase states, three completed patrol
laps, intro music, the normal music transition and one full normal loop. Other
maps keep the original patrol start, path, speed and footstep audio, even where
that fixed route crosses a replacement wall. This behavior is required by the
assessment. Stop restores the same saved scene hash and all 660 manual objects
after every session.

Camera checks cover 1920x1080 (16:9), 1024x768 (4:3), 720x1280 (9:16) and
1680x720 (21:9) for every map: 20 map/aspect combinations. Each checks the entire
cell envelope, fifteen showcase sprites, all text rectangles and caption offsets.
Ten representative camera renders are saved. Examples: [default landscape](Default-1920x1080.png),
[default portrait](Default-720x1280.png), [wide map](Wide-1680x720.png) and
[tall portrait](Tall-720x1280.png).

Visual review of the first camera run found that the stacked layout made 4:3
labels unnecessarily small. Stacking now applies only when width is less than
height. A missed refresh after re-enabling the view was also corrected. The
linked final reports and captures include both corrections.
