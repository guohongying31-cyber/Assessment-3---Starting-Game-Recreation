# Final technical validation

Project: Shan Hai Spirit Trail. Student number: 26151833.
Editor: Unity 6000.4.11f1. Required archive: `26151833_Assess3.zip`.

All Assessment 3 implementation bands have supporting technical checks. This
record covers the release candidate before the final Git merges and ZIP creation.
The exact final Main commit, archive hash, extracted archive checks and remote
state are recorded in `26151833_Assess3-Validation.json`, delivered beside the ZIP.
Keeping that receipt outside the ZIP avoids a checksum referring to itself.

## Build and clean extraction

- The [Windows build](Windows-Build.json) succeeded with zero errors and warnings.
- The [uninstrumented player](Player-Smoke.json) remained running for 35 seconds
  with graphics enabled and no runtime exceptions or early exit.
- The candidate ZIP included the actual Git repository and 387 project files.
  Library, Temp, Logs, UserSettings, builds and temporary tools were absent.
- The [extracted project](Extracted-Project.json) imported and compiled with a
  fresh Library. Every packaged project file retained its hash after import and Play.
- Five actual Play sessions in that extracted copy verified the default, smaller,
  wider, taller and expanded-orientation maps at four aspect ratios each.
- The default session observed at least 28 seconds, three patrol laps, all 46
  showcase states, intro music, the normal transition and a complete normal loop.
- Each session checked exact generated transforms and sprite references, animated
  elixirs, invalid/repeated generation, camera re-enable and manual scene restoration.

The extracted Play reports are [Default](Extracted-Play-Default.json),
[Small](Extracted-Play-Small.json), [Wide](Extracted-Play-Wide.json),
[Tall](Extracted-Play-Tall.json) and
[all T/gate orientations](Extracted-Play-AllTAndGateOrientations.json).
One [actual extracted-copy render](Extracted-Default-1920x1080.png) is retained.
Alternate sessions check the initial patrol segment; full laps are established
by the default session and the earlier multi-frame-rate movement tests.

## Files, assets and repository

The [static audit](Static-Audit.json) checks English filenames/text, valid media,
asset/meta pairs, unique and resolved GUIDs, animation references and unchanged
source art/audio. The final stage changes documentation only; all 246 asset files
retain their pre-validation hashes. Seven runtime scripts have no Editor-only
dependencies, physics movement, CharacterController, MoveTowards or Rule Tiles.
Only the specified recreated-level scene is enabled for building.

The [repository review](Repository-Review.json) records the initial final-validation
snapshot: 36 actual commits, eight preserved feature merges and all seven required
branches. Feature bases match the preceding Development state. Git object checking
returned success; unreferenced trees/blobs were preserved and no pruning was done.
Final integration adds its own real milestone and merge commits to this history.

## Issues encountered during validation

The temporary build helper initially used the wrong numeric type for build error
counts; fixing that helper allowed the actual player build to run. During the first
fresh-copy Play attempt, entering Play immediately overlapped Unity Editor search
index startup and produced an internal indexing exception followed by a test timeout.
Waiting ten seconds for startup/import completion before entering Play resolved the
automated test sequence. Neither correction changes submitted game assets or settings.
Failed attempts are excluded from passing results.

## Assessment scope

Assessment 3 demonstrates assets, animation previews, a saved manual level,
automatic patrol and runtime generation. Keyboard input, collection, collisions,
moving enemies, menu logic and an innovation scene belong to subsequent work.
All eleven audio clips have technical full-playback and signal checks. Final
personal listening/balance approval has not been recorded and is not claimed.

Use the [submission guide](Submission-Guide.md) to open and inspect the delivered
project. The release procedure verifies Main checkout, retained branches, ZIP
contents and a clean import of the actual final archive after the technical checks.
