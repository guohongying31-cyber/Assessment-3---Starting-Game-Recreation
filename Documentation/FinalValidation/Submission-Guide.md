# Submission guide

Submit `26151833_Assess3.zip` as the Assessment 3 file upload. No upload is performed
by this project workflow. Use the current Canvas assignment page for the deadline.

## Open the project

1. Extract the ZIP into a new folder. Keep the hidden `.git` directory and
   `.gitignore` beside Assets, Packages and ProjectSettings.
2. In Unity Hub, add that extracted folder and open it with Unity 6000.4.11f1.
3. Allow the initial package import, compilation and Editor startup to finish.
4. Open `Assets/Scenes/RecreatedLevel.unity`. The manual maze is saved before Play.
5. Press Play. Runtime generation replaces the maze, the cultivator patrols
   automatically, the gallery cycles through animation states and music starts.
6. Watch for at least 28 seconds to see every showcase state and a full normal
   music loop. Stop restores the saved manual maze.

For a clear preview, select a fixed 1920x1080 or 2560x1440 Game view resolution,
disable Low Resolution Aspect Ratios when available, and inspect at 1x zoom.
Editor layout preferences are local and intentionally excluded from the archive.

## Contents and Git

The archive contains the full tracked Unity project, production sources,
documentation, `.git` and `.gitignore`. Library, Temp, Logs, UserSettings, local
reference documents, generated executables and temporary verification helpers
are excluded. Empty Unity asset folders are retained with their metadata.

The release procedure packages with Main checked out after validated Development
has been merged into it. Required feature branches and their genuine milestone
history are retained. The authorized remote is the repository linked in README.
The adjacent `26151833_Assess3-Validation.json` delivery receipt identifies the
actual release commit, archive SHA-256 and completed extracted-archive checks.
That receipt is verification support; the assignment upload is the ZIP.

To inspect alternate maps, replace the `int[,] levelMap` literal in
`Assets/Scripts/Level/LevelGenerator.cs`, as described in the generator guide.
Restore the supplied map after testing. The fixed patrol deliberately keeps its
original world route when a replacement map is used.
