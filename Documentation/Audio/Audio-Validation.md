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

## Scene playback milestone

Unity 6000.4.11f1 ran two actual Play sessions using the saved RecreatedLevel scene.
The local log is tmp/unity-audio-play-audition.log; the process exited with code 0.
[Playback-Validation.json](Playback-Validation.json) preserves the emitted measurements.

| Check | Observed result |
|---|---|
| Scene audio inventory | Eleven AudioSources with assigned clips |
| Opening in session 1 | Intro output present; normal source disabled and silent during intro |
| Handover in session 1 | 2.400013506 seconds after Play entry; expected 2.4 seconds |
| Opening source after handover | Disabled and stopped |
| Normal music | Output present, continuously playing, and sample time wraps after its full 24-second loop |
| Stop and enter Play again | Intro starts again; handover observed at 2.400009297 seconds |
| Individual clip onsets | All eleven advance sample positions and produce nonzero output on an isolated temporary source |

The probe measured time from EnteredPlayMode, excluding editor startup offset. It read
AudioSource state, timeSamples, and GetOutputData RMS. Session 2 finishes the opening
check before a second full normal loop; its normalLoopWrapped field is therefore false
and is not a failed loop check. Session 1 verifies that behavior.

### Complete clip playback

A separate Play session played all eleven clips to their natural ends with looping
disabled on an isolated temporary AudioSource. Every clip started, advanced through
its expected sample range, produced nonzero output, and stopped without truncation.
Observed durations differed from file durations by at most 0.034 seconds, within the
probe's audio-buffer and frame-observation tolerance.

[Full-Clip-Playback.json](Full-Clip-Playback.json) records expected/observed durations,
furthest sample positions, maximum output RMS, and natural-ending results for every clip.
The local log is tmp/unity-audio-full-audition.log; Unity exited with code 0.
Loop playback is verified separately for the normal scene music; this full-file check
does not claim that all other looping categories ran through multiple loops in Unity.

### Issues found and resolved before acceptance

- The first control-clip binding passed editor sampling but failed in Play. Unity reported
  the native AudioSource enabled property as a float binding; changing to that binding
  with constant tangents populated the runtime curve bindings and fixed the handover.
- The initial probe included editor startup time in its handover clock. Its baseline was
  corrected to Play entry, then the real scene was retested.
- Reusing an Animator-controlled source for the separate audition produced no output in
  the first audition attempt. Isolating the audition on a temporary source resolved the
  test conflict. The saved scene's opening sources were still tested independently.

Failed diagnostic runs are not counted as passing checks. The ignored local logs retain
the diagnostics; the successful run is identified above. Temporary Editor scripts and
test objects are excluded from the deliverable.

### Final project import

After removing all temporary Editor scripts and their metadata, Unity reopened and
completed its import/compilation check with exit code 0. The local log is
tmp/unity-audio-clean-import.log. The saved prefab, controller, animation curves,
scene instance, and all eleven imported clips remain; no runtime C# code was added.

The final static audit checked 117 current tracked/candidate files: 106 ASCII-only text
files and eleven valid PCM WAV files. Filenames passed the English-only check, local
Markdown links resolved, all 40 asset GUIDs were unique, and scene/prefab references
resolved. Each source has metadata; audio hashes match the production measurements.
No C#, DLL, or Python files remain under Assets. The pre-existing untracked package-manager
settings file was checked for unchanged content and excluded from the feature commit.

### Review scope

These are technical playback and signal checks, not a claim that a person listened to
and approved all final audio. The user heard the initial style and requested the ethereal
revision. Final listening and balance review remains available through Unity's clip
previews and the scene. No player build or gameplay interaction test is claimed here.
