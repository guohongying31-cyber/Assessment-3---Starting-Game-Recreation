# Final Windows build validation

Student number: 26151833. Required submission filename: `26151833_Assess3.zip`.
The submission is the complete Unity project with Git history; a standalone player
is additional validation and is not included in the assessment ZIP.

Unity 6000.4.11f1 built the sole enabled scene,
`Assets/Scenes/RecreatedLevel.unity`, for Windows x64 with the Mono backend and
normal, non-development build options. [Windows-Build.json](Windows-Build.json)
records success with zero errors and zero warnings. No temporary runtime probe
was present in the player. The Editor-only build helper was removed afterward.

The uninstrumented executable then ran with graphics enabled for 35 seconds.
It initialized Direct3D 12 and loaded its assemblies without an exception,
missing-script error, generation failure or early process exit. The test ended
the process after the observation window. [Player-Smoke.json](Player-Smoke.json)
records the actual duration and scope. This is a startup/log smoke check, not a
measurement of player frame rate, audio output or human interaction; detailed
behavior is checked separately in Editor Play.

All 246 project asset files kept their original hashes after building and helper
removal. The first temporary build-report helper used unsigned fields for signed
error/warning counts and failed compilation before building. The helper was
corrected; only the subsequent successful build is counted here.

The local validation executable is
`output/final-validation/Windows/ShanHaiSpiritTrail.exe`. Keep its entire Windows
folder together when running it locally. Generated binaries and local logs are
excluded from Git and the submission ZIP.
