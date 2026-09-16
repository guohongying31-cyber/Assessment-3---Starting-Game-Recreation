# Manual level validation

Unity version: 6000.4.11f1. This record covers actual checks as the feature progresses.

## Top-left quadrant

- Reviewed PDF pages 11-12 and compared the 15 x 14 numeric reference with the supplied CSV.
- Recorded 166 explicit placements and rotations: 55 spirit motes, one elixir, and 110
  wall pieces across the six base wall categories. Empty cells have no placed object.
- Verified every placement category against the numeric reference. Source pixel checks
  passed for 107 joined alpha edges and 106 joined color edges. The one gate-to-wall
  connection intentionally changes color while retaining the same alpha edge profile.
- Saved the instances under Level01_Manual/TopLeft with row groups. Inspected the actual
  Unity camera render against the reference quadrant: block shapes, corridor positions,
  T junction, exit seal and boundary arms match the intended layout.
- Reopened the saved scene and verified all 166 positions, rotations, prefab references,
  active sprite renderers, the elixir controller and complete camera coverage at 16:9.
- Ran a Play probe with a four-second minimum (9.44 seconds measured). The elixir
  displayed both frames repeatedly, the intro
  switched to normal music, and all 166 placements survived Play and Stop. The saved
  scene file hash remained unchanged.
- The first Play probe incorrectly requested Editor-only prefab linkage from runtime
  objects. The corrected probe checks sprite asset references during Play and retains
  prefab-instance checks before and after Play. The failed probe is not counted as a pass.
- Unity compilation and the corrected Play check exited with code 0.
  Local logs: tmp/manual-level/unity-top-left-build.log and
  tmp/manual-level/unity-top-left-play-verified.log. These logs are excluded from commits.

Evidence: TopLeft-Layout-Validation.json, TopLeft-Scene-Validation.json,
TopLeft-Play-Session1.json and TopLeft-Editor.png.

The top-left milestone alone does not complete the four-quadrant manual level.

## Four-quadrant mirror milestone

- Duplicated the saved top-left placements into the three remaining quadrant groups.
  Reflected X around x=13.5 and Y around y=-14 using group scales and offsets.
  Omitted Row14 from both lower quadrants.
- Verified 660 unique cells against the expected reflections: 438 wall pieces,
  218 spirit motes and four elixirs. Only four cells are occupied on the center row.
  All twelve outer tunnel cells on that row are empty.
- Compared 438 connected source alpha edges after rotation/reflection; all matched.
  434 color edges also matched. The four intentional color changes occur at the seals.
  The only boundary-ending wall arms are the four tunnel-wall ends.
- Inspected the rendered full map: four reflected quadrants, a single central corridor,
  joined middle walls and seals, and clear one-cell side exits. All quadrants fit the camera.
- Reopened and checked every world position, transform basis, prefab reference and sprite.
  The transform-basis check distinguishes true reflection from a substituted rotation.
- The Play check observed all four elixirs changing frames, intro-to-normal audio and
  unchanged placement counts before, during and after Play. The saved scene hash stayed
  unchanged after Stop. Unity compilation and the Play run exited with code 0.
- Local logs: tmp/manual-level/unity-mirrored-build.log and unity-mirrored-play.log.
  Evidence: Full-Layout-Validation.json, Mirrored-Scene-Validation.json,
  Mirrored-Play-Session1.json and Mirrored-Editor.png.
