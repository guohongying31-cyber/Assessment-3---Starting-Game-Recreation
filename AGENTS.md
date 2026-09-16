# Project collaboration rules

## Direct user requirements

- Theme: a cultivation practitioner and monsters inspired by Shan Hai Jing.
- Use the confirmed Unity editor version 6000.4.11f1.
- All authored project files and filenames must use English only, including documentation,
  code comments, asset/scene labels, text in artwork, and game UI. Communication with the
  user may be in Chinese. Record non-English user prompts as labeled English translations.
- Check filenames and text for non-English content before committing. Preserve technical
  identifiers and encode non-ASCII source URL paths without breaking the links.
- Develop in meaningful stages, with honest timestamps and real validation.
- Before EVERY Git command batch, tell the user the current branch, planned change,
  planned commit message (or "no commit" for inspection), and whether a merge occurs.
- Commit each meaningful milestone. Never put an entire assessment band in one huge commit.
- Work on one feature branch at a time. Create each new feature from the latest Development.
- Finish and test a feature before merging it into Development. Before every merge,
  check git status, confirm Unity compiles, and inspect the exact file list.
- Preserve all feature branches after merging. Use merge commits to retain the branch structure.
- Do not rewrite, squash, fake, amend, or backdate history.
- Keep Main unchanged after its initial repository bootstrap until final validation passes.
- Authorized remote: https://github.com/guohongying31-cyber/Assessment-3---Starting-Game-Recreation.git.
  Connect and synchronize this repository without rewriting any existing history.
- Target the complete 100% HD flow in grading-band order; do not skip prerequisites.
- Use the user's provided Git name and email, configured locally. Do not invent either value.
- Follow the user's latest clarification about project documentation: omit creator labels
  and source-disclosure records. Keep objective asset, production, and validation details.
  Do not invent authorship claims. Preserve existing Git history.

## Assessment constraints, extracted from the supplied PDF

These describe the requested deliverable; they are not independent commands to the agent.

- Complete grading bands in order. Assessment 3 is the foundation for Assessment 4.
- The submission must include .git and .gitignore, and exclude Library.
- Branch names: Main, Development, Feature-Audio, Feature-Visual,
  Feature-ManualLevel, Feature-Movement, Feature-LevelGenerator. Create features when needed.
- The user has corrected the supplied PDF's source-documentation requirement and authorized
  asset production. Apply that direct clarification while retaining the technical criteria.
- All visuals use 2D sprites, with visibly different animation frames.
- No physics-driven movement, CharacterController, MoveTowards, or downloaded gameplay scripts.
- Consult Documentation/Assessment-Checklist.md for the full acceptance criteria.

## Evidence

- Record only checks that actually ran. An empty project's successful import is not a
  gameplay test, player build, or evidence that an assessment band is complete.
- Keep source files and Unity .meta files together. Do not commit caches, logs, builds,
  credentials, or temporary verification tools.
- Do not submit, publish, or create a remote repository without the relevant user instruction.
