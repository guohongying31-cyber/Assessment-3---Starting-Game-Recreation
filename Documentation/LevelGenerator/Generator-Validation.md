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
