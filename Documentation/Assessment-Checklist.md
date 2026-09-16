# Assessment 3: requirements and acceptance checklist

Based on the supplied 15-page PDF, PacMan Level Map.csv, Week 4 workshop transcript,
and Canvas screenshot, with the user's subsequent clarifications. This working checklist
is not the instructor's original wording or evidence that every grading item is complete.

## Confirmed details and open items

- Unity 6000.4.11f1: confirmed by the user.
- Git author name and email: supplied by the user and configured only for this repository.
- Remote: the user supplied `guohongying31-cyber/Assessment-3---Starting-Game-Recreation`.
  The local origin is configured. Consult Git history and remote branches for synchronization.
  The remaining required gameplay feature branches have not been developed.
- Student number: requires confirmation for the final ZIP filename.
- Deadline: the screenshot says Monday 23:59; PDF page 2 and the workshop say
  Friday of Week 8 at 23:59. The screenshot has no calendar date, so the current deadline
  cannot be inferred from these materials. Check the current Canvas page and any extension.
- Project language: English throughout files, filenames, comments, labels, and game text.

## Shared constraints (PDF pages 1-4)

- [ ] Use 2D sprites throughout the game.
- [ ] Clearly differ from Pac-Man's yellow circle and colored ghosts; do not copy other characters.
- [ ] Every required animation state has at least two visibly distinct frames.
- [ ] Audio is self-made or has a clear free/royalty-free license; do not use variants of the original.
- [ ] Do not use downloaded gameplay code, Asset Store artwork, or unapproved add-on plugins.
- [ ] Do not use physics-driven Rigidbody movement or CharacterController.
  Any Rigidbody2D used only for detection must remain Kinematic.
- [ ] Do not use the Animated Tile extension. Place animated power pickups manually.
- [ ] Complete grading bands in order; missing prerequisites can prevent later bands being marked.

Assessment 4 requires the menu, recreated level, innovation scene, and complete gameplay.
Assessment 3 does not require keyboard input, pellet or wall collisions, or enemy movement.

## 10%: Git (PDF pages 5-7)

- [x] Keep `.git` and `.gitignore` beside `Assets` and `ProjectSettings`.
- [x] Base `.gitignore` on the specified GitHub Unity template.
- [x] Connect the repository to a real GitHub, GitLab, or Bitbucket remote.
- [ ] Maintain genuine milestone commits throughout development; do not fabricate history or dates.
- [ ] Retain `Main`, `Development`, `Feature-Audio`, `Feature-Visual`,
  `Feature-ManualLevel`, `Feature-Movement`, and `Feature-LevelGenerator`.
- [ ] Develop one feature at a time from the latest Development, test, merge, and retain its branch.
- [ ] Pass final validation, merge Development into Main, and check out Main before packaging.

## 20%: project structure (PDF page 7)

- [x] Use clear folders, subfolders, and asset names; track resources with their `.meta` files.
- [x] Organize the Recreated Level hierarchy with parent groups and short, meaningful names.
- [x] Keep temporary scripts, duplicate examples, caches, and unrelated material out of the tracked project.

Recheck these shared structure requirements after each later feature and when packaging.

## 35%: audio (PDF page 8)

Import all **11 audio categories** into `Assets/Audio Clips`, with names indicating their purpose.

- [x] Intro: music when the level starts.
- [x] StartScene: future menu music; the clip is still required for Assessment 3.
- [x] Ghost Normal: looping music for the beasts' normal state.
- [x] Ghost Scared: music for the beasts' frightened state.
- [x] Ghost Dead: music while at least one beast is in its dead state.
- [x] Move: the cultivator moving without collecting a spirit mote.
- [x] Pellet: collecting a spirit mote.
- [x] Eat Ghost: sealing a beast.
- [x] Cherry: collecting a crimson fruit.
- [x] Wall: hitting a wall.
- [x] Death: the cultivator's death or dissolving spirit.
- [x] Include an AudioSource. On Play, start Intro, then switch to looping Ghost Normal
  when the clip ends or three seconds pass, whichever comes first.
- [ ] Audition every clip for playback, unintended silence, truncation, appropriate volume,
  and traceable source/license information.

The category checks confirm imported clips, not completed interaction logic or a StartScene.
See [audio validation](Audio/Audio-Validation.md) for measured levels, full playback results,
and the intro/loop checks. The final listening-review item remains open: automated checks
do not establish the student's approval of the sound, transitions, or relative volume.
This checklist does not award a grade.

## 50%: sprites (PDF pages 9 and 11-12)

- [x] PacStudent has distinct left, right, up, and down walking sequences, plus death frames.
  Rotating one sequence does not satisfy the directional requirement.
- [x] Each of the four beasts has normal frames for all four directions.
- [x] Include frightened frames for all four directions, recovering frames, and dead frames.
- [x] Include spirit motes, ward-breaking elixirs, crimson fruit, and a life icon.
- [x] Include six wall sprites: outside corner, outside wall, inside corner, inside wall,
  T junction, and beast exit seal.
- [x] Organize the layout around tile categories 1-8; do not export separate rotation variants.
  Category 0 uses no sprite. Power pellets must also meet the two-frame animation requirement.
- [x] Import all visual assets into Unity and provide visible scene examples.
- [x] Keep sizes, pixels per unit, and pivots consistent; characters must fit a one-cell passage.

## 65%: animators (PDF page 10)

- [x] PacStudent has one Animator Controller with four Walking directions and Dead.
- [x] Power pellets have their own Animator Controller.
- [x] Beast controller names start with `GhostAnimator_`. Each beast has four Walking
  directions, four Scared directions, Recovering, and Dead: ten states.
- [x] Use either one controller with three Override Controllers or four separate controllers.
- [x] In Play, show every required state for the cultivator and all beasts; power pickups pulse.
- [x] Preview looping animations for at least three cycles and at least two seconds,
  whichever is longer. Play non-looping animations fully once. Set Exit Time accordingly.
- [ ] Later movement instances change direction immediately; showcase cycling must not
  override their movement-driven animation state.

The direction-selection mechanism has been verified with Showcase disabled. The final
item stays open until it is exercised by the actual movement feature. See
[animation controls](Visual/Animation-Guide.md) and [visual validation](Visual/Visual-Validation.md).

## 75%: manual level (PDF pages 11-12)

- [x] Use only the first 15 rows by 14 columns of numeric CSV data, excluding the symbol table and legend.
- [x] Match the top-left quadrant to the PDF array and figure, with correctly joined wall rotations.
- [x] Mirror horizontally, then vertically, without duplicating the bottom row.
  The default complete level has 29 rows and 28 columns.
- [x] Keep a single center row and one-cell side exits, with no pellets in the exit tunnels.
- [x] Save the explicit manual placements in the scene, visible in Scene View **before** Play.
- [x] Show all four quadrants in Play; retain the manual level after Stop.
- [x] Use the specified mirrored-quadrant layout, not the original full Pac-Man Level 01.
- [x] Preplace animated elixirs; rotate and mirror the base wall sprites for reuse.

See [the manual level guide](ManualLevel/Manual-Level-Guide.md) for the saved placement
worksheet and mirror transforms, and [validation](ManualLevel/Manual-Level-Validation.md)
for actual scene, seam and Play checks. No runtime generator is part of this stage.

## 85%: movement (PDF page 13)

- [ ] Use the course's programmatic tween approach with frame-rate-independent motion.
- [ ] Loop clockwise around the first inner block at the top left. With the array origin
  at the top left, follow `(row 1,col 1) -> (1,6) -> (5,6) -> (5,1) -> (1,1)`.
- [ ] Use the same linear speed on every segment, regardless of segment length.
- [ ] Turn instantly, select the matching animation immediately, and play non-collecting movement audio.
- [ ] Check lap duration, direction, path, and audio at different frame rates.
- [ ] Do not substitute `Rigidbody.velocity`, `Vector3.MoveTowards()`, or similar methods for tweening.

## 100%: LevelGenerator (PDF pages 14-15)

- [ ] The student writes `LevelGenerator.cs`, containing an `int[,] levelMap` the marker can replace.
- [ ] In `Start()`, remove the manual Level01 only at runtime, then generate its replacement.
- [ ] Derive positions, rotations, and reflections from array dimensions and neighboring tiles;
  do not hardcode a coordinate table for the supplied map.
- [ ] Mirror horizontally, vertically, and in both directions, without repeating the bottom row.
- [ ] Handle reflection separately from rotation; a 180-degree rotation is not a reflection
  for an asymmetric tile.
- [ ] Fit the camera to any valid map dimensions and display aspect ratio.
- [ ] Use instantiated objects or Tilemap placement, but calculate connection rules in
  student-written code; do not use Rule Tiles.
- [ ] Test wider, taller, and smaller maps, all corner orientations, T junctions, exits, and seams.
- [ ] Match the manual layout with the default generated map; restore the manual scene after Stop.
- [ ] Keep PacStudent on its original manual-level patrol route when the test map changes;
  this assessment does not require adapting the route.

## Final delivery

- [ ] Reopen in the specified Unity version without compilation errors or missing references.
- [ ] Verify completed features in Play; documents, screenshots, and static checks cannot replace this.
- [ ] Review branches, milestone history, remote synchronization, and included files.
- [ ] Merge Development into Main only after all final checks, then check out Main.
- [ ] Include the complete project, `.git`, and `.gitignore` in a ZIP without `Library`.
- [ ] Use the student's confirmed number, inspect an extracted copy, and obtain student review.
