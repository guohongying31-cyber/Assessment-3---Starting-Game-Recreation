# PacStudent: Shan Hai Spirit Trail

31263 / 32004 Introduction to Game Development, Assessments 3 and 4.
Student number: 26151833.

A 2D maze game featuring a cultivator in jade robes and four guardians inspired
by the Classic of Mountains and Seas. Assessment 4 extends the original maze,
sprites, animations and sound with playable rules and an active protective talisman.

## Open and play

1. Add this project folder in Unity Hub and open it with **Unity 6000.4.11f1**.
2. Allow package import and script compilation to finish.
3. Open `Assets/Scenes/StartScene.unity` and press Play.
4. Choose **Level 1** for the required recreation or **Level 2** for Talisman Trial.

The enabled scene order is `StartScene`, `ClassicLevel`, `InnovationScene`.
Use WASD or arrow keys to move, P or Escape to pause, and Space to activate the
Level 2 talisman. Input is buffered until the current grid cell is complete.
The HUD Exit button returns to StartScene without saving an unfinished run.

See the [play guide](Documentation/Gameplay-Guide.md) for complete controls,
scoring, guardian behavior and the distinction between ward protection and elixirs.

## Assessment 4 features

- Animated start-screen border, five character sprites, level selection and
  independent high-score/time records.
- A world-space maze with 218 spirit motes and four flashing ward elixirs;
  keyboard movement, player tunnels, collection, collision feedback and three lives.
- A four-second `3, 2, 1, GO!` countdown, six-digit score, centisecond game timer,
  sprite life indicators and numbered guardians.
- Moving crimson fruit that crosses the map centre and clips to its boundaries.
- Four specified guardian behaviors, ten-second scared effects with three-second
  recovery, and direct spirit returns to each guardian's own starting position.
- Game Over on maze completion or loss of all lives, followed by a locked
  three-second result display and automatic menu return. Completed runs store the
  score and its associated time; a faster equal score replaces the previous time.
- A Level 2 talisman that blocks Normal guardian contact for three seconds, with
  fifteen seconds between activations and clear protection/recharge feedback.

The project uses custom linear movement and world-space sprites. All authored
project text, filenames, code comments and game UI use English.

## Verification and release status

The [formal checklist](Documentation/Assessment4-Checklist.md) maps the supplied
Assessment 4 criteria. The [validation record](Documentation/Assessment4-Validation.md)
distinguishes checks actually executed from remaining work and historical milestones.

Windows player validation and cache-free project validation have passed. The
validation record describes their scope and the isolated full-route fixture.
The standalone output is `output/Assessment4Build/ShanHaiSpiritTrail.exe`;
its Data folder, UnityPlayer.dll and generated dependencies must remain beside it.

The Assessment 4 project archive is named **`26151833_A4_Project.zip`**. Package the
complete Unity project with `.git` and `.gitignore`, excluding Library, generated
caches, logs, local preview builds and temporary verification helpers. Complete
final validation before merging Development into Main and packaging with Main checked out.
The adjacent `Assessment4-Release-Receipt.json` records the delivered archive hash,
release commit and final archive checks; generated output files are not Git assets.
The separate submission declaration has been deferred by the user and is not part
of the current preparation work.

## Preserved Assessment 3 foundation

`Assets/Scenes/RecreatedLevel.unity` preserves the earlier Assessment 3 demonstration.
It contains the saved manual maze, automatic patrol and asset showcase; it is separate
from the three enabled Assessment 4 scenes.

That foundation includes eleven audio clips, 101 sprite frames, 46 visual animation
clips, six visual controllers and fifteen reusable sprite prefabs. The maze has
28 columns by 29 rows, four reflected quadrants and 660 placed objects. The original
patrol demonstrates a clockwise 7.2-second lap at 2.5 units per second, while the
runtime generator reconstructs the saved maze using neighboring wall connections.

Historical checks cover animation and audio playback, requested 30/60/144 FPS patrol
tests, five generator maps, twenty map/aspect combinations and an extracted-project
import. Those results belong to the Assessment 3 milestone. Its released archive
`26151833_Assess3.zip` is preserved. See the historical
[final validation](Documentation/FinalValidation/Final-Validation.md) and
[Assessment 3 opening guide](Documentation/FinalValidation/Submission-Guide.md).

## Development workflow and references

The repository uses Main, Development and preserved feature branches. Each meaningful
milestone is committed with its actual development timestamp. A feature is compiled
and tested before its merge into Development; the final release merge follows the
complete validation sequence.

- [Remote repository](https://github.com/guohongying31-cyber/Assessment-3---Starting-Game-Recreation)
- [Theme and asset design](Documentation/Theme-Brief.md)
- [Audio inventory and configuration](Documentation/Audio/Scene-Audio-Guide.md)
- [Visual production and import settings](Documentation/Visual/Visual-Production.md)
- [Generator layout and wall rules](Documentation/LevelGenerator/Generator-Guide.md)

The `.gitignore` follows the assessment-specified
[GitHub Unity template](https://github.com/github/gitignore/blob/main/Unity.gitignore),
with additional rules for local references and temporary verification files.
