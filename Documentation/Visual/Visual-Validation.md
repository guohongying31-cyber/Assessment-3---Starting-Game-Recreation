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
