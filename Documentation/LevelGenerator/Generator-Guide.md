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

At this foundation milestone the generator is not yet attached to the scene.
The existing manual level and fixed patrol continue to run as before. Neighbor
rotation rules, runtime replacement and camera fitting are subsequent milestones.

See [validation](Generator-Validation.md) for evidence and current scope.
