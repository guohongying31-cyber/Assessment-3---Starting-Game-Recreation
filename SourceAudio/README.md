# Audio authoring sources

These offline Python tools synthesize waveform-based audio for Shan Hai Spirit Trail.
They are production tools outside the Unity runtime.
The tools are outside Assets and are not compiled into the game.

The sounds contain no downloaded recordings, soundfonts, voice samples, or music excerpts.
Provenance is documented in Documentation/Audio/Audio-Provenance.md.

Requirements: Python 3 and NumPy. Render music with:

    python SourceAudio/render_music.py

Output: 44,100 Hz, 16-bit PCM WAV files in Assets/Audio Clips/Music.
The score, waveform instruments, arrangements, room reflections, and export settings
are preserved in the script. A fixed seed is used wherever noise is synthesized.

Render the six interaction effects with:

    python SourceAudio/render_effects.py

Effects are mono, 44,100 Hz, 16-bit PCM in Assets/Audio Clips/SFX.
The footsteps form a 0.8-second loop; the other five effects are one-shots.
