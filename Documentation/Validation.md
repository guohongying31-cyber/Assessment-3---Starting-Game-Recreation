# Project validation record

Record started: 2026-09-16 (Australia/Sydney). Latest feature validation: 2026-09-17.
This chronological record distinguishes each milestone from the completed
[final technical validation](FinalValidation/Final-Validation.md).

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
- Seven runtime scripts implement movement, procedural generation and adaptive scene
  presentation. Temporary authoring and verification helpers are excluded from commits
  and removed before feature integration.
- Audio Play checks are recorded in [audio validation](Audio/Audio-Validation.md).
  Movement frame-rate tests are recorded in [movement validation](Movement/Movement-Validation.md).
  Generator tests are recorded in [generator validation](LevelGenerator/Generator-Validation.md).
  A Windows player build, launch smoke check and clean extracted-candidate Play
  verification are recorded in the final validation section.
- Map data checks cannot replace manual-layout or LevelGenerator assessment evidence.
- Final Main integration and the delivered ZIP identity are recorded in the
  external delivery receipt described by the submission guide.

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
Actual renders were inspected at 1080p and 720p, both 16:9. These checks cover the
manual-stage implementation. Runtime generation is verified separately below;
a standalone player build and final submission validation were still pending at
that milestone. Their later results appear in the final validation section below.

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

## Procedural generator stage

On 2026-09-17, source dimensions and mirror coordinates passed 25 geometry cases.
Neighbor-based wall orientation matched all 110 manual wall angles, four independent
replacement-map oracles and a deterministic ambiguous layout. Runtime Start generated
the full default maze and preserved the manual scene after Stop.

The final five-map Play suite covers a 28-second default session and smaller, wider,
taller and expanded-orientation maps. It verifies generated transforms and sprites,
all default showcase states, music looping, original patrol motion, animated elixirs,
invalid-input preservation, repeated generation and re-enabled camera updates.
Twenty map/aspect checks and ten actual renders cover landscape and portrait displays.
Pixel comparisons verify every active wall seam after rotation and reflection.

The [generator validation record](LevelGenerator/Generator-Validation.md) contains the
reports, camera examples, corrections and clean-compilation scope. Final whole-project
review, a standalone build, the Main merge and ZIP inspection were reserved for
the following stage. The completed technical checks are recorded below, with
the final merge and ZIP identity recorded in the external delivery receipt.

## Visual clarity follow-up

The [clarity validation](VisualClarity/Clarity-Validation.md) records six before/after
resolution comparisons, larger and sharper captions, smoother character/pickup
sampling and a same-aspect font-density check. The five-map Play regression and
clean compilation passed again. The local saved Game view preference was changed
from low-resolution aspect rendering at 1.5x to normal-resolution rendering at 1x;
its next interactive Editor launch has not been observed. The saved scene, source
artwork and audio remain unchanged. Final build and submission review were still
pending at that milestone; their later results follow below.

## Final technical validation

The Windows build succeeded with zero errors/warnings and its uninstrumented player
ran for 35 seconds without exceptions or early exit. A candidate ZIP containing the
full project and Git history was extracted without Library, Temp, Logs or UserSettings.
All 387 candidate project files retained their hashes after fresh import and Play.
Five extracted-copy Play sessions passed, including twenty map/aspect combinations
and the default 28-second animation, patrol and music regression.

The [final validation record](FinalValidation/Final-Validation.md) contains build,
clean-copy and repository evidence and describes the two temporary-harness issues
encountered and corrected. The release procedure runs final Main/ZIP checks afterward
and records their exact hashes in the receipt delivered beside `26151833_Assess3.zip`.
