# Project validation record

Date: 2026-09-16 (Australia/Sydney). These are actual checks, not final assessment validation.

## Foundation checks performed

| Check | Result and scope |
|---|---|
| Required editor | Unity 6000.4.11f1, revision b0a1d6caadd2; confirmed by the user |
| First restricted-environment launch | Stalled while connecting to the licensing client; stopped and not counted as a pass |
| Project creation in the normal user environment | Unity logged a successful exit with return code 0 |
| Temporary Editor configuration | Saved 2D mode, text serialization, an orthographic camera, and four empty groups; Unity logged `ASSESSMENT_FOUNDATION_OK` |
| Reimport after removing the setup helper | Unity process exited with code 0; no C# compilation errors, compilation failures, or exceptions |
| Project settings | Visible Meta Files, 1920 x 1080, and RecreatedLevel in the startup scene list |
| Dependencies | Removed the generated Multiplayer Center dependency; retained only built-in Unity modules |
| Static asset checks | 25 unique GUIDs; a camera and four empty groups; no gameplay/helper C# or DLL files; local documentation links resolve |
| Numeric CSV block | 15 x 14; matches the PDF page 11 array and CSV symbol table element by element |
| Reference mirroring calculation | 29 rows x 28 columns without a duplicated center row; 218 normal pickups and four power pickups; see Reference/Map-Validation.json |
| Initial remote inspection | Set origin to the user's GitHub repository; its first inspection returned no existing branches |

Original local logs are in the ignored `tmp` directory: `unity-create.log`,
`unity-create-retry.log`, `unity-foundation.log`, and `unity-verify-foundation.log`.
They are not included in commits. Some successful runs contain license reconnection
messages at startup and a Curl callback-aborted message at shutdown. No claim is made
that all logs are warning-free. Passing results use process exit, import completion,
and compilation status.

## Current implementation limits

- The scene contains the audio rig, a complete manual maze, an automatically patrolling
  cultivator and a visual showcase with five characters, four items and six wall samples.
- Six visual controllers provide 46 animation states, in addition to the audio opening
  controller. Every visual state has been observed in two full Play sessions.
- Three runtime scripts implement linear tweening, the fixed patrol and animation/audio
  presentation. Temporary authoring and verification helpers are excluded from commits
  and removed before feature integration.
- Audio Play checks are recorded in [audio validation](Audio/Audio-Validation.md).
  Movement frame-rate tests are recorded in [movement validation](Movement/Movement-Validation.md).
  Player builds and generator tests have not run.
- Map data checks cannot replace manual-layout or LevelGenerator assessment evidence.
- No final ZIP exists, and Development has not been merged into Main.

## Foundation Git milestone

The foundation and design documents were committed as separate genuine milestones.
The table above records checks before the Feature-Setup merge. Consult Git history
and remote branches for actual merge and synchronization results. Each later feature
must record its own Unity compilation and relevant gameplay checks.

## English-only documentation update

On 2026-09-16, Feature-EnglishOnly translated the six existing documentation files
containing non-English text and added the language requirement to AGENTS.md.
Prompt excerpts were preserved as explicitly labeled English translations.

- Scanned all 77 tracked files: filenames and UTF-8 text contained only ASCII characters.
- Also scanned the existing, untracked PackageManagerSettings.asset without adding it to the change.
- All local Markdown links resolved; source links use ASCII URL paths or percent encoding.
- Reviewed the translated text for English wording and retention of the assessment requirements.
- Unity 6000.4.11f1 completed the pre-merge import/compilation check with exit code 0.
  The local log is tmp/unity-premerge-english.log and is not committed.
- This change affects seven documentation/rule files and does not add gameplay or alter assets.

The language check covers the current project files. Existing Git history and retained
feature branches are preserved without rewriting previous commits.

## Audio stage

Five music cues and six interaction effects were produced as two separate milestones.
Scene integration adds a reusable audio prefab and a built-in Animator opening sequence.
The revised music follows the user's request for a more ethereal sound and a weaker rhythm.
The [audio validation record](Audio/Audio-Validation.md) and its JSON evidence describe
actual Unity Play checks. Production sources and configuration are described in the audio
documentation. This stage does not establish completion of later bands.

## Visual stage

Seven sprite sheets provide 101 named frames, including four directions and distinct
frightened, recovering and dead appearances. Fifteen reusable prefabs form an English
showcase in the saved scene. Six controllers contain 46 visual clips with two different
frames each. Two full Play sessions verified state timing, immediate direction selection,
disabled showcase transitions, audio startup, normal-music looping and restart behavior.

The [visual validation record](Visual/Visual-Validation.md) links the import, pixel, seam
and actual playback evidence. The [animation guide](Visual/Animation-Guide.md) explains
how to inspect the preview and use the controllers in the next stages.

## Manual level stage

The saved scene now contains the specified mirrored maze: 28 columns, 29 rows, 660
nonempty placements, 218 spirit motes and four animated elixirs. The top-left worksheet
records explicit cell rotations. The lower groups omit the center row; all side-tunnel
cells are empty. Pixel-edge checks verified 438 wall connections after rotation/reflection.

The [manual level validation record](ManualLevel/Manual-Level-Validation.md) covers scene
reload, exact transforms and references, two final 28-second Play sessions, all 46
showcase states, audio restart and loop behavior, and unchanged saved scenes after Stop.
Actual renders were inspected at 1080p and 720p, both 16:9. Runtime generation,
player builds and final submission validation remain separate future work.

## Movement stage

The patrol follows `(1,-1) -> (6,-1) -> (6,-5) -> (1,-5) -> (1,-1)` using
elapsed/duration interpolation. Segment durations are distance divided by 2.5 units/s,
and unused frame time carries across corners. Three final Play sessions at requested
30, 60 and 144 FPS each observed two laps, immediate directional states, all eight
walking frames and nonzero continuous footstep output. Each full lap took 7.2 game seconds.

Disabling movement, pausing time scale and resuming passed the presentation checks.
The final scene regression verified all 660 manual placements and prefab references.
The [movement validation record](Movement/Movement-Validation.md) includes actual measured
frame rates, failed attempts, corrections, screenshots and scope limits.
