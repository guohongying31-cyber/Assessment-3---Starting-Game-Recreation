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
