# Audio provenance

Project: Shan Hai Spirit Trail. Prepared with Codex assistance on 2026-09-16.

## Source and authorship

All listed recordings are newly synthesized within this project from mathematical
waveforms and deterministic noise. Codex authored the offline synthesis tools and
arrangements at the user's request. No external recordings, soundfonts, samples,
Pac-Man music, or downloaded audio libraries were used.

Production sources are retained in SourceAudio. They are not Unity gameplay code.
External recording licenses and download URLs are not applicable because no third-party
recorded audio is incorporated. This record does not claim student composition or performance.

## Music inventory

All paths below are relative to Assets/Audio Clips/Music.

| Assessment role | File | Duration | Loop | Intended character |
|---|---|---|---|---|
| Level intro | Music_Intro_JadeAwakening.wav | 2.4 seconds | No | Rising plucked notes and a fading jade bell |
| StartScene | Music_Menu_CloudTerrace.wav | 32 seconds | Yes | Sparse pentatonic notes above a soft drone |
| Ghost normal | Music_Normal_ThroughTheSeal.wav | 24 seconds | Yes | Floating plucked notes and sustained harmony, without drums |
| Ghost scared | Music_Scared_TalismanChase.wav | 20 seconds | Yes | Brighter high bells with gentle, sparse movement |
| Ghost dead | Music_Dead_ReturningSpirits.wav | 32 seconds | Yes | Low atmospheric harmony and scattered spirit chimes |

Music is 44.1 kHz, 16-bit stereo PCM. Loop tails and room reflections wrap across
the file boundary. The intro has a natural ending and is not intended to loop.
Measured duration, level, boundary difference, clipping count, and SHA-256 hashes
are in Music-Measurements.json. Measurements are not a claim of human listening approval.

## User direction

After hearing the first normal-state preview, the user requested a more ethereal sound
with a weaker rhythm. The final arrangements slow the tempo, add rests and sustained
harmony, remove the normal-state percussion, and extend the room reflections.
