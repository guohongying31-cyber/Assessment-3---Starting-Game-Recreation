# Sequential development plan

The suggested commit messages below describe future milestones. Use them only after the
corresponding work is complete. Do not pre-create every feature branch, manufacture empty
commits, assign artificial dates, or split a prebuilt assessment to fabricate progress.

All project content, filenames, comments, labels, and game text must use English.

## Foundation: Feature-Setup

1. Create Main, Development, and Feature-Setup; commit the Git workflow foundation.
2. Create and import a 2D project in the specified Unity version, with organized folders
   and empty scene groups.
   Suggested commit: `chore: create Unity 6000.4.11f1 2D project foundation`.
3. Review the supplied material and add the theme brief, grading checklist, map data checks,
   and AI assistance disclosure.
   Suggested commit: `docs: map assessment requirements to Shan Hai development milestones`.
4. Reimport the project and review Git status and the exact merge file list. After validation,
   merge Feature-Setup into Development and retain the feature branch. Main keeps only
   the initial repository bootstrap until final validation.

A successful empty-project import establishes a working foundation. It does not complete
the audio, artwork, animation, or manual-level grading bands.

## Next stages: create each branch from the latest Development

| Order and branch | Milestones and suggested commit messages | Evidence required before merging |
|---|---|---|
| 1 / Feature-Audio | `audio: add five synthesized music cues and provenance`; `audio: add six synthesized interaction effects and validation`; `feat: configure and verify intro-to-normal scene audio` | All 11 categories, provenance review, individual playback checks, intro transition within three seconds, looping, Unity compilation and Play checks; record human listening-review status separately |
| 2 / Feature-Visual | `art: import student-drawn cultivator directional and death frames`; `art: add four student-drawn Shan Hai creature sprite sets`; `art: add pickups life icon and six base wall sprites`; `anim: configure cultivator and power pellet controllers`; `anim: configure ghost state cycles and scene showcase` | Required frames and directions, visible scene examples, all ten states for every beast, correct controller names and preview durations |
| 3 / Feature-ManualLevel | `level: place and verify the top-left maze quadrant`; `level: mirror quadrants and fix center-row seams`; `level: frame the manual maze and place animated power pellets` | Manual scene layout, connected walls, 28-column by 29-row dimensions, four power pickups, a single center row, full camera coverage |
| 4 / Feature-Movement | `feat: tween cultivator clockwise around the first inner block`; `anim: synchronize direction changes with movement audio`; `fix: preserve tween speed across frame boundaries` only if that fix is needed | Equal speed across segments, immediate turns, lap timing at different frame rates, movement audio, no prohibited movement API |
| 5 / Feature-LevelGenerator | `feat: instantiate a quadrant from the numeric level map`; `feat: infer wall rotations from neighboring tiles`; `feat: mirror generated quadrants and fit camera bounds`; `fix: handle generator edge cases from alternate maps` with the actual issue named | Default layout matches the manual level; valid maps of different sizes; corners, T junctions, exits, and seams; manual level retained after Stop; no Rule Tiles |
| Final validation | `docs: record final play-mode and repository validation`; merge Development into Main only after all checks pass | Recheck every band in order, verify remote branches, exclude caches and private files, inspect the extracted ZIP |

Do not invent fixes to reach the example commit count. An unfinished feature may have genuine,
explainable partial milestone commits, but must not be marked complete or merged prematurely.
Continue its work on the same branch.

## Required checks before every merge

1. Announce the current branch, planned change, commit or merge message, and merge direction.
2. Inspect `git status` and the exact file list.
3. Compile/import in Unity 6000.4.11f1; also test actual gameplay in Play when applicable.
4. Exclude Library, Logs, UserSettings, builds, and temporary tools from the merge.
5. With a clean working tree, use a merge commit into Development and retain the feature branch.
6. Synchronize the actual remote branches; start the next feature from the latest Development.

## Student authorship and AI assistance

The PDF allows AI consultation but prohibits copying generated gameplay code wholesale.
AI can explain tweening, review the student's code, help investigate errors, check animations
and maps, propose test cases, and operate Git.

The student creates the gameplay code and final artwork. Record utilized advice in
AI-Assistance.md. This plan is not evidence of a completed submission.
