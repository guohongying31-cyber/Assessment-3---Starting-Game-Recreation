# Audio validation record

## Music milestone

Date: 2026-09-16. Unity: 6000.4.11f1.

- Five music categories rendered to 44.1 kHz, 16-bit stereo PCM WAV.
- Revised the arrangements after the user requested a more ethereal sound and a weaker rhythm.
- No clipped samples in the exported files; every clip contains a non-silent signal.
- Loop boundary differences were checked against the normal sample-to-sample changes.
- Unity imported all five clips with their expected sample rates, channel counts, and lengths.
- Unity AudioImporter uses PCM, Decompress On Load, and the source sample rate.
- The revised music import completed with exit code 0.
- Local import log: tmp/unity-audio-music-revised.log (not committed).

Music-Measurements.json records measured levels and file hashes. The user reviewed the
initial style and requested the revision; this record does not claim that the user has
listened to and approved every final clip. A playable preview is provided separately.

## Interaction-effect milestone

- All six required effects rendered and imported successfully as mono PCM at 44.1 kHz.
- Movement is an 0.8-second loop; pellet, seal, fruit, wall, and death are one-shots.
- All six contain a non-silent signal and zero clipped samples.
- All six have matching first/last sample values after their fade or loop preparation.
- Unity reported the expected lengths and channel counts for every effect and exited with code 0.
- Local import log: tmp/unity-audio-effects-import.log (not committed).
- Exact levels and hashes are in Effects-Measurements.json.

Scene playback validation will be recorded when completed.
