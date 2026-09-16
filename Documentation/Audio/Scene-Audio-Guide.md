# Scene audio guide

Open Assets/Scenes/RecreatedLevel.unity and press Play.
The 2.4-second intro is followed by the normal-state music on a continuous loop.
The scene remains visually empty because sprite and level production belong to later stages.

## Scene organization

Systems/LevelAudio is an instance of Assets/Prefabs/Audio/LevelAudio.prefab.

| Object | Purpose | Initial configuration |
|---|---|---|
| Intro | Music_Intro_JadeAwakening | Enabled, Play On Awake, not looping, volume 0.6 |
| NormalLoop | Music_Normal_ThroughTheSeal | Disabled, Play On Awake, looping, volume 0.5 |
| AudioLibrary/Menu | Future StartScene music | Play On Awake off |
| AudioLibrary/Scared | Frightened-state music | Play On Awake off |
| AudioLibrary/DeadBeast | Dead-beast music | Play On Awake off |
| AudioLibrary/Movement | Soft footstep loop | Play On Awake off |
| AudioLibrary/Pellet | Spirit-mote pickup | Play On Awake off |
| AudioLibrary/Seal | Beast sealing | Play On Awake off |
| AudioLibrary/Fruit | Bonus fruit pickup | Play On Awake off |
| AudioLibrary/Wall | Wall contact | Play On Awake off |
| AudioLibrary/Death | Cultivator death | Play On Awake off |

All AudioSources use 2D playback, no Doppler shift, and no reverb-zone processing.
The wave files already include their intended room reflections. Music is stereo and
effects are mono. Audio data is preloaded and kept as PCM to preserve predictable timing.
The remaining nine sources are available for later integration; they do not play together
or respond to interactions at this stage.

## Opening sequence

The built-in Animator on LevelAudio uses AudioOpening.controller and the single
IntroThenNormal state. AudioOpeningSequence.anim controls the enabled property of the
two opening AudioSources with constant-tangent curves:

- At time zero, Intro is enabled and NormalLoop is disabled.
- At 2.4 seconds, Intro is disabled and NormalLoop is enabled.
- The non-looping control clip holds these final values. NormalLoop's own Loop setting
  repeats the music; the opening sequence itself never restarts during the session.

The controller uses Unscaled Time and Always Animate. There are no runtime C# scripts
for this configuration. This audio-control clip is not counted as a character or power-pellet
sprite animation; all visual animation requirements remain for Feature-Visual.

For the supplied intro, the configured switch time is min(2.4 seconds, 3 seconds).
If replacing the intro clip, move both final control keys to the earlier of the new
clip's duration and three seconds. Preserve constant tangents and the source settings.
The key time is an authored setting, not a runtime calculation of a replacement clip's length.

## Audition and later integration

Select any WAV in Assets/Audio Clips and use Unity's audio preview to hear it separately.
Adjust source Volume on the prefab for balance; the audio files retain headroom.
For later gameplay, connect the prepared sources to student-authored interaction logic.
When replacing the opening Animator with a student-written music manager, disable the
Animator so only one system controls the sources.

## References

- [Unity 6.4: AudioSource Play On Awake](https://docs.unity3d.com/6000.4/Documentation/ScriptReference/AudioSource-playOnAwake.html)
- [Unity 6.4: animation curves](https://docs.unity3d.com/6000.4/Documentation/ScriptReference/AnimationUtility.SetEditorCurve.html)
- [Unity 6.4: unscaled Animator time](https://docs.unity3d.com/6000.4/Documentation/ScriptReference/AnimatorUpdateMode.UnscaledTime.html)

These references informed component configuration; no example gameplay scripts were copied.
