# Visual validation record

Unity version: 6000.4.11f1. Checks below describe completed work only.

## Cultivator milestone

- Imported ten named frames from a 1448 x 1086 transparent RGBA sheet.
- Confirmed two frames each for Right, Left, Up, Down and Dead.
- Inspected the side, back and front poses and the kneeling/dissolving death pair.
- Checked nonempty slices, frame-pair differences and boundaries.
- Unity import/compilation completed with exit code 0.
- Local log: tmp/unity-cultivator-import.log (excluded from commits).

## Creature milestone

- Imported four 1122 x 1402 RGBA sheets, with twenty named frames per creature.
- All four have four walking directions, four frightened directions, Recovering and Dead.
- Inspected the fox tail fan, the crane's single leg and wing changes, the hound's short
  tail and white head, and the tortoise's shell and serpentine tail.
- A regular-grid pixel check found content touching several slice boundaries. Adjusted
  the boundaries to transparent gaps and retained the original logical center anchors.
- The repeated pixel check passed for all 90 current character frames. No meaningful
  opaque edge pixels cross the accepted slice boundaries; frame pairs differ.
- Reimported all five sheets with stable frame names and sprite identifiers. Unity
  completed import/compilation with exit code 0.
- Local log: tmp/unity-creature-slices-verified.log (excluded from commits).

Sprite-Import-Validation.json records Unity results; Sprite-Pixel-Validation.json records
source hashes, transparency and frame bounds. These import checks do not establish
completed animation-controller or Play-mode showcase behavior.

## Pickup and wall milestone

- Imported five item frames, including two visibly different elixir pulse frames.
- Confirmed item transparency and an empty unused atlas cell.
- Rendered six geometric wall sprites from their retained source geometry.
- Compared seven connecting RGBA edge pairs and the gate's alpha edge profile; all matched.
- Verified the intended open edges for each wall. Wall edges deliberately reach their
  tile boundary; character/item boundary checks use a different rule.
- Imported all 101 sprite frames across seven sheets. Unity completed import/compilation
  with exit code 0. Local log: tmp/unity-complete-sprite-import.log.
- Wall-Connection-Validation.json records the seam checks and atlas hash.

## Static scene display

- Created fifteen reusable sprite prefabs: five characters, four items, and six walls.
- Placed their instances under AssetShowcase with English labels using the editor's
  bundled UI package. The manual-level and movement groups remain reserved for later stages.
- The first camera render showed labels only because the camera retained the future
  maze-center position. Aligned it to the showcase center and saved the correction.
- Inspected a new 1920 x 1080 render: all fifteen sprites are visible, transparent item
  backgrounds remain invisible, and labels fit without overlap or clipping.
- The corrected renderer run exited with code 0. Local log: tmp/unity-showcase-camera-aligned.log.
- Showcase-Static.png is an actual Unity camera render of the saved scene.

Animation behavior is still validated separately; a static render does not prove a state cycle.

## Cultivator and elixir animation milestone

- Created PacStudentAnimator with four walking states and one non-looping Dead state.
  Created ElixirAnimator with a continuous Pulse state. All six clips use two different sprites.
- Entered Play mode in the saved scene. Observed all six states and their actual sprite changes.
  Each walking preview lasted two seconds (four cycles); Dead completed once in 0.75 seconds.
  Pulse remained active for the nine-second observation window.
- Disabled the Showcase parameter, selected Right and Left immediately, and verified that
  the selected direction remained active for another 2.5 seconds.
- Confirmed opening music playback and its transition to the normal loop. This short run
  did not include a complete 24-second normal-music loop.
- Unity compilation and the Play verification exited with code 0.
  Local log: tmp/unity-cultivator-elixir-play.log.
- Cultivator-Elixir-Playback.json contains the measured state durations, cycles and frames.
  Animation-Manifest.json maps the clips, controllers, prefab instances and frame names.
