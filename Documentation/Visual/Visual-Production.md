# Visual production

## Sprite sheets

Sprite-Manifest.json maps each sheet's grid cells to English frame names in reading order.
Empty cells are excluded. Unity stores slicing rectangles in the sheet's metadata;
the original transparent PNG is preserved without pixel resampling or background replacement.

The cultivator uses ten frames: two each for walking Right, Left, Up, Down and Dead.
Up shows the back, Down shows the face, and the side poses face their named direction.
The death pair changes from a kneeling pose to a dissolving spirit.

NineTail, FlameCrane, WhiteHound and SerpentTortoise each use twenty frames: two each
for Walking Right/Left/Up/Down, Scared Right/Left/Up/Down, Recovering and Dead.
Normal, frightened and dissolving appearances are visibly different.

## Import settings

- Unity 6000.4.11f1 with the editor's bundled 2D Sprite package.
- Sprite (2D and UI), Multiple mode, consistent 384 pixels per unit and logical cell-center anchors.
- Point filtering, uncompressed RGBA, alpha from input, no mipmaps, Clamp wrapping.
- Preserve source dimensions; Full Rect sprite meshes; no generated physics shapes.
- For scene placement, use a visual-child scale of 384 divided by the source cell size.
  This makes each source cell one world unit while preserving consistent import settings.
  Keep the actor root at unit scale for later movement.

Some creature poses extend across a perfectly regular grid line. Their slicing boundaries
follow nearby transparent gaps, as recorded in columnEdges and rowEdges in the manifest.
Custom pivots preserve the original logical cell centers after these boundary adjustments.
The source pixels are unchanged, and neighboring poses remain in separate slices.

The earlier 32-pixel canvas suggestion was a drawing option, not the current source resolution.
The imported pixel-style illustrations retain their full source dimensions.

## Validation scope

Sprite-Import-Validation.json records actual Unity imports and frame counts.
Sprite-Pixel-Validation.json records source hashes, transparency, cell bounds and frame-pair
differences. Pixel differences support frame inspection; they do not replace visual checks
of facing direction and motion. Animation and scene-preview checks are recorded separately.

## Editing

Keep the PNG and its .meta together. Preserve existing frame names and sprite identifiers
when changing rectangles or replacing a sheet, so animation references remain valid.
Open the Sprite Editor to inspect the named slices. Do not rotate a side pose to create
an up/down pose, and do not count an unchanged copy as another animation frame.

Technical reference: [Unity Sprite Editor data provider API](https://docs.unity3d.com/Packages/com.unity.2d.sprite@1.0/manual/DataProvider.html).
