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
