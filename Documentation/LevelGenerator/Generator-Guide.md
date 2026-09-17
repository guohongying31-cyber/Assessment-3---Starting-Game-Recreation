# Procedural level generator

The generator uses the replaceable `int[,] levelMap` in
`Assets/Scripts/Level/LevelGenerator.cs`. The literal array contains only the
supplied top-left quadrant and uses categories 0 through 8 from the assessment.

## Coordinate foundation

`LevelMapLayout` snapshots and validates the array. For a source with R rows and
C columns, the full layout has `2 * R - 1` rows and `2 * C` columns. Cell `(r,c)`
maps to world `(c,-r,0)`.

| Quadrant | Origin | Scale | Source rows |
| --- | --- | --- | --- |
| TopLeft | (0,0) | (1,1,1) | 0 through R-1 |
| TopRight | (2C-1,0) | (-1,1,1) | 0 through R-1 |
| BottomLeft | (0,-(2R-2)) | (1,-1,1) | 0 through R-2 |
| BottomRight | (2C-1,-(2R-2)) | (-1,-1,1) | 0 through R-2 |

The lower quadrants omit the shared final source row. Reflection is applied to
the quadrant transform, separately from each tile's local rotation. Category 0
has no object. The default map contains 660 nonempty cells, 218 spirit motes and
four elixirs.

## Wall orientation

`WallOrientationSolver` assigns a small set of possible rotations to each wall.
It compares north, east, south and west connection ports with adjacent cells:
zero means no connection, one means the wider outer-wall edge, and two means
the narrower inner-wall edge. The exit seal uses inner-wall ports. A T junction
has two outer arms and one inner stem.

Candidates are removed until every facing pair is compatible. This also permits
adjacent parallel walls whose facing edges are both empty. The guaranteed first
outer corner anchors the solution at zero degrees. Tiles on the shared final
source row must be vertically symmetric. The right edge meets its own reflected
copy, while map-boundary wall ends can form open side tunnels.

If a cluster still has more than one valid arrangement, the solver chooses
deterministically by candidate count, row, column and angle. Its iterative search
has a 100000-node limit and reports incompatible or excessively ambiguous input.
There is no coordinate-to-angle table and no Rule Tile dependency.

## Runtime replacement

`Systems/LevelGenerator` contains the script and references the manual level and
eight existing tile prefabs. On Play, `Start()` validates the map, resolves wall
angles and builds four reflected quadrant groups under `Level01_Generated`.
Only nonempty cells receive instances. Existing elixir prefabs retain their
Animator, and each tile keeps its source sprite and metadata.

The completed candidate replaces the manual level only during Play. Validation
or construction failures preserve the existing level. `GenerateLevel()` supports
another runtime generation and rejects Edit-mode calls. Stop restores the saved
manual layout. The patrol is separate and retains its original fixed world route.

Camera and gallery adaptation for replacement dimensions are the next milestone.

See [validation](Generator-Validation.md) for evidence and current scope.
