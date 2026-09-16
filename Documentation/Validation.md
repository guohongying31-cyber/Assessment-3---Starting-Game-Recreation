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

- The scene contains the audio rig and a visual showcase with five characters, four items
  and six wall samples. The manual maze and gameplay character group are still empty.
- Six visual controllers provide 46 animation states, in addition to the audio opening
  controller. Every visual state has been observed in two full Play sessions.
- No gameplay C# scripts have been committed. Temporary authoring and verification helpers
  are excluded from commits and removed before feature integration.
- Audio Play checks are recorded in [audio validation](Audio/Audio-Validation.md).
  Player builds, frame-rate movement tests, and generator tests have not run.
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
