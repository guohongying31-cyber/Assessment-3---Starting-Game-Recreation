# Manual level layout

The scene is Assets/Scenes/RecreatedLevel.unity. Level01_Manual contains the level objects
saved before Play. The layout uses the supplied top-left quadrant, then horizontal and
vertical reflection. It does not use a runtime level generator.

## Placement worksheet

TopLeft-Placement.csv lists each nonempty source cell and its chosen rotation. It has
166 placements across 15 rows and 14 columns, matching the numeric reference in
Documentation/Reference/LevelMapTopLeft.csv. Empty cells have no scene object.

The cell at row zero, column zero is at world position (0, 0). A cell at row r, column c
is at (c, -r), with one world unit between neighboring cell centers. Row groups organize
the saved prefab instances; each instance is named for its source row and column.

| Category | Existing prefab | Default orientation |
|---|---|---|
| 1 | Wall_OuterCorner | Right and Down arms |
| 2 | Wall_OuterStraight | Left and Right arms |
| 3 | Wall_InnerCorner | Right and Down arms |
| 4 | Wall_InnerStraight | Left and Right arms |
| 5 | SpiritMote | Unrotated |
| 6 | Elixir | Unrotated, animated |
| 7 | Wall_TJunction | Left and Right outer arms, Down inner stem |
| 8 | Wall_ExitSeal | Left and Right arms |

Rotations are counterclockwise around Z in 90-degree steps. Corners use 0 degrees for
Right/Down, 90 for Right/Up, 180 for Left/Up and 270 for Left/Down. A vertical straight
uses 90 degrees. Rotation choices are explicit in the worksheet, with no neighbor-rule
or runtime orientation calculation in this feature.

The scene reuses the six base wall sprites and existing pickup prefabs. It does not add
rotated image variants or extra wall categories. The power pickup keeps its own Animator.
Prefab instance roots retain unit scale before quadrant reflection; the Visual child
normalizes source image dimensions as described in the visual production guide.

## Mirrored quadrants

| Scene group | Origin | Group scale | Source rows |
|---|---|---|---|
| TopLeft | (0, 0) | (1, 1, 1) | 0 through 14 |
| TopRight | (27, 0) | (-1, 1, 1) | 0 through 14 |
| BottomLeft | (0, -28) | (1, -1, 1) | 0 through 13 |
| BottomRight | (27, -28) | (-1, -1, 1) | 0 through 13 |

The bottom quadrants omit Row14. Its two source placements are already represented by
the top quadrants, producing four occupied cells on the single center row. Mirroring
acts on the group transforms, preserving the explicit rotations of the placed copies.
A rotation is not substituted for a reflection.

The complete level spans 28 columns and 29 rows, with 660 nonempty cells: 438 walls,
218 spirit motes and four animated elixirs. Row14 is the side-tunnel row; its outer six
columns on each side are empty. The upper and lower tunnel walls end at the map boundary.

## Open and inspect

Open RecreatedLevel in Unity 6000.4.11f1. The maze is visible in Scene View before Play.
Select Level01_Manual and press F in Scene View to frame all quadrants. Inspect its row groups and individual
prefab instances. Expand TopLeft/Row03 to find the source elixir placement at column 01.
The other three elixirs are reflected copies at world cells (3,26), (25,1) and (25,26).

The orthographic camera is centered at (13.5, -14, -10), with size 15.8 and a dark navy
background. The intended display is 1920 x 1080. The complete maze occupies the center;
the existing character, item and wall samples occupy the two side panels under AssetShowcase.
The camera, level root and scene objects have descriptive English names and labels.

Press Play and observe at least 28 seconds. Four elixirs pulse in the maze, five characters
cycle through their preview states, and the opening music switches to its normal loop.
The sidebar elixir also pulses. Stop returns to the same saved maze. This stage does not
add keyboard control, patrol movement, collision handling, pickup collection or a runtime
LevelGenerator. The separate Characters group is reserved for the movement stage.

To adjust a cell in the Inspector, use integer positions and 90-degree rotations. Update
the placement worksheet and repeat mirror/seam checks when changing the source design.
Do not add a second Row14 to the lower quadrants. Keep the side exits free of pickups.

See [validation](Manual-Level-Validation.md) for actual checks and saved evidence.
