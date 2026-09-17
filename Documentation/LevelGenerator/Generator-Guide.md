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

## Camera and showcase

`GeneratedLevelView` fits the camera around the full map, the fifteen showcase
sprites and their English labels. Landscape and square displays keep the two
showcase panels beside the maze. Portrait displays place them below the maze.
The title and inventory follow the actual map dimensions and pickup counts.

During Play the label canvas uses world coordinates. Labels and their matching
sprites move by the same offsets, so resizing does not detach captions from
their examples. Aspect changes and later generation refresh the layout. The
component also refreshes after being disabled and re-enabled. Stop restores the
original saved camera and canvas configuration.

Runtime labels use a minimum font size of 18 and an adaptive glyph rasterization
density of 2-4. Changes to pixel resolution at the same aspect update text density
without shifting the layout. See the [clarity checks](../VisualClarity/Clarity-Validation.md).

## Try another map

1. Stop Play and edit the `levelMap` array literal in `LevelGenerator.cs`.
2. Use a rectangular quadrant with categories 0-8. Category 1 at `(0,0)` has the
   default right/down orientation, as guaranteed by the assessment.
3. Use smooth reciprocal wall connections. T junctions join outer walls to inner
   walls. The shared final source row must remain valid after vertical reflection.
4. Press Play. The generated map and camera adapt to the new dimensions.
5. Stop to recover the saved manual scene. Restore the supplied array after testing.

Unity does not serialize multidimensional arrays into the Inspector. The array
literal in this script is the replacement point. The supplied validation maps
include smaller, wider and taller examples with independent expected angles.
Changing a map intentionally does not move or reshape the original patrol.

See [validation](Generator-Validation.md) for evidence and current scope.
