# Assessment 4 Formal Acceptance Matrix

Source: supplied `31263-32004 IGDev Assessment 4.pdf` and `Assessment4 Movement Visualization.pdf`.
This matrix records acceptance criteria and planned checks, not completed validation or a guaranteed grade.
Complete the bands in order; later features depend on the earlier bands being complete.

| Band | Acceptance criteria | Required evidence |
| --- | --- | --- |
| 10%: Git | Main and Development branches; at least one feature branch per completed band; multiple meaningful commits per feature; merge completed, tested features to Development; preserve branches. Remote reference present. Merge Development to Main only after final validation, and check out Main for submission. | Inspect branch graph, honest chronological milestones, clean status, exact file lists and remote configuration. |
| 20%: A3 catch-up | Complete A3 through the manual maze. Player starts top-left. Four ghosts start in the central area. Power pellets flash. Remove the automatic A3 player movement and ghost animation preview cycles from the playable level, and remove irrelevant showcase objects. | Inspect and play Level 1. Confirm the retained maze and assets work without gallery or patrol behavior. |
| 30%: Start screen | `StartScene` is the first scene. Title contains `PacStudent`. Display player and four ghost sprites. Animate a Canvas border. Provide `Level 1` and `Level 2` buttons with visual and/or audio feedback. Show separate previous high scores and associated times. Loop Start Scene music. | Launch build directly into StartScene. Test both level buttons, keyboard/mouse feedback, music, and layouts at 16:9 and 4:3 landscape. |
| 40%: HUD | Screen Space Canvas named `HUD`. Three life-indicator sprites, six-digit score starting `000000`, Game Timer starting `00:00:00` in `mm:ss:msms`, integer Ghost Scared Timer hidden at zero, Exit button returning to StartScene, and current Level name. Each ghost has a child World Space Canvas named `Ghost1Canvas` through `Ghost4Canvas`, with its single readable number. | Inspect object names and Canvas modes. Capture both aspect ratios. Verify lives are sprites, number labels follow ghosts, and Exit works. |
| 55%: Movement | `PacStudentController.cs` handles Update input and has persistent `lastInput` and `currentInput` members. At a grid center, try lastInput first, then currentInput, else stop. Input during a tween, including reverse input, takes effect at the next center. Custom linear interpolation at about 5 tiles/second; no physics movement or external tween library. Start facing right and stationary. Animate and play appropriate pellet/empty movement audio only while moving, with custom dust. Ghost gates block player and outside ghosts. | Test buffered invalid turn, later valid turn, multiple keys during a tween, delayed reversal, stopping against a wall, no-input start, animation/audio stopping, and movement consistency at 30/60/144 requested FPS. |
| 55%: Cherry | `CherryController.cs` spawns a bonus cherry 5 seconds after scene start and 5 seconds after the prior cherry is destroyed. Use a different random start just outside the level. Move in a straight line through the map center, ignoring walls/ghosts/pellets, above all sprites. Visible only within map bounds. Destroy after exiting the opposite side outside camera view. | Observe multiple timed spawns and different starting locations. Check center crossing, clipping at all boundaries, sprite order, and disposal. |
| 70%: Contacts | Wall contact prevents movement and emits a small impact effect and sound on its first frame. Player tunnels connect both sides and preserve inward movement. Destroy pellets for 10 points and cherries for 100 points plus sound. Power pills give 50 points and activate a 10-second scared timer, with Recovering during the final 3 seconds. Dead ghosts are not reset by power expiry. | Test each contact and score delta, both tunnels, wall feedback without repeated spam, timer refresh and expiry, and Dead ghosts during expiry. |
| 70%: Death and ghosts | Normal ghost contact costs one life, plays synchronized player death animation/audio and death particles, and freezes ghosts. Respawn top-left, facing right, waiting for input; ghosts reset to their initial positions and Normal state. Eating Scared/Recovering ghosts gives a fixed 300 points, Dead state, GhostEat sound and Dead music. At this band, a 3-second dead timer restores the current global state; 85% explicitly replaces this timer with physical return. | Test both edible states, exact score, death effects, frozen ghosts, state reset and stationary respawn. |
| 70%: Lifecycle and records | Show `3`, `2`, `1`, `GO!`, each for one second, with a semi-opaque blocking image and intro music. No movement/collisions or elapsed time during this initial countdown. After GO disappears, start timer and Normal music and enable play. On all pellets collected or zero lives, show `Game Over` with blocking image, freeze actors/timer, disable UI buttons, save score/time if score is higher or equal with a lower time, and return automatically to StartScene after 3 seconds. Use PlayerPrefs for independent per-level records. | Time all four countdown stages and the result delay. Test both end conditions, disabled Exit/retry controls during Game Over, restart persistence, better score, faster tie, slower tie, and separate Level 1/2 records. |
| 85%: Ghost movement | `GhostController.cs` controls four ghosts in an evenly spaced horizontal central row. Decide after each grid tween. Never stop, backtrack only when no alternative exists, obey corridor walls, and treat teleporters as walls. No Unity Pathfinding. Ghosts 1/3 exit the top gap; 2/4 exit the bottom gap. Living ghosts cannot re-enter the house. Dead ghosts move straight through walls at constant frame-independent speed to their own starting positions, then adopt the current global state and leave through their assigned exit. Disable the 70% dead timer. | Test each initial exit, no living re-entry, no portal traversal, dead-end reversal, uninterrupted decisions, direct Dead return through walls, own home position, current-state revival, and last-Dead music restoration. |
| 85%: Ghost decisions | Normal Ghost 1 randomly selects moves with next straight-line distance to the player greater than or equal to current distance. Ghost 2 uses less than or equal. Ghost 3 chooses randomly among valid non-backtracking moves. Ghost 4 follows the outside wall clockwise. Scared and Recovering ghosts all use Ghost 1 behavior. Normal speed is 90% of player speed. Scared, Recovering and Dead speeds are each 50% of Normal speed. | At player speed 5, verify speeds 4.5 and 2.25. Check labeled behavior at junctions and forced corridors, clockwise outer movement, and state-dependent behavior changes without an unsolicited reversal. |
| 100%: Innovation | `InnovationScene` is linked by `Level 2`. Add a meaningful innovation after the formal baseline is complete. The requested active talisman ability should create a distinct player decision and have clear controls, availability and feedback. Explain how it changes the experience in the declaration's Design Innovation section. | Play the complete Level 2 lifecycle. Test activation, effect boundaries, cooldown, reset, audio/visual feedback and interaction with power/death. Compare the Level 1 baseline. |

## Narrow distance-filter fallback

The specification does not define what happens when Ghost 1 or Ghost 2 has no move satisfying its distance condition while other nonreverse corridor moves exist. The no-stopping and no-backstep rules still apply.

1. Construct physically legal moves, excluding portal edges and living house entry.
2. Exclude reversal whenever any physically legal nonreverse move exists.
3. Apply the ghost's distance condition and randomly choose among matches.
4. If the distance filter has no match, randomly select among the physically legal nonreverse choices.
5. Reverse only when no physically legal nonreverse choice exists.

This is a documented interpretation of an underspecified case, not an additional official rule. Squared Euclidean distances preserve the required comparisons. Prefer the player's current world position at the decision instant. Include a forced-corridor case in validation.

## Cross-cutting validation

- Confirm Unity 6000.4.11f1 compiles, imports and runs without project Console errors; verify a Windows player build as well as Editor Play mode.
- Exercise StartScene, Level 1, InnovationScene, countdown, gameplay, pause if retained, death, Game Over and return at 1920x1080 (16:9) and 1440x1080 (4:3). Verify no overlap, cropping or unreadable ghost labels.
- Keep maze, characters, pellets and cherry in world space. Only menu/HUD elements and required ghost labels belong on Canvases.
- Do not use physics-driven movement, CharacterController, MoveTowards, downloaded gameplay scripts, external tween libraries or Unity Pathfinding.
- Avoid Resources folders and Resources.Load for ordinary assets. Built-in font access through Resources.GetBuiltinResource is distinct from Resources.Load.
- Confirm all authored filenames, code comments, scene labels, UI text and documentation use English.
- Record only tests actually executed; distinguish deterministic logic checks, Editor Play checks, player smoke tests and manual visual checks.

## Submission checklist

- [ ] All preceding bands complete and final validation recorded.
- [ ] Main contains the final validated work and is checked out.
- [ ] Feature branches and genuine commit history preserved; remote reference present.
- [ ] Final archive named `26151833_A4_Project.zip`.
- [ ] Include the complete Unity project, `.git` and `.gitignore`; exclude `Library` and other generated caches, temporary verification files, logs and local preview builds.
- [ ] Extract the ZIP into a fresh location without caches; confirm project import, correct first scene, gameplay and required assets.
- [ ] Complete `26151833_A4_Declaration.txt` using the actual Canvas template, submitted to the separate portal. The template must be obtained before preparing the final declaration.
- [ ] Explain the active talisman innovation in detail in the declaration's Design Innovation section.
- [ ] Resolve the deadline from Canvas: the supplied PDF and screenshot have inconsistent wording, so no exact final deadline is inferred here.
