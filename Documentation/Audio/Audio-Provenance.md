# Audio provenance

Project: Shan Hai Spirit Trail. Prepared on 2026-09-16.

## Production

All listed recordings are newly synthesized within this project from mathematical
waveforms and deterministic noise, using the retained offline synthesis tools and
arrangements. No external recordings, soundfonts, samples,
Pac-Man music, or downloaded audio libraries were used.

Production sources are retained in SourceAudio. They are not Unity gameplay code.
External recording licenses and download URLs are not applicable because no third-party
recorded audio is incorporated.

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

## Interaction-effect inventory

All paths below are relative to Assets/Audio Clips/SFX. Source: SourceAudio/render_effects.py.

| Assessment role | File | Duration | Loop | Synthesis approach |
|---|---|---|---|---|
| Moving without a pellet | SFX_Move_SoftSteps.wav | 0.8 seconds | Yes | Two quiet filtered-noise footfalls with low body tones |
| Eating a pellet | SFX_Pellet_SpiritMote.wav | 0.24 seconds | No | One short jade chime |
| Eating a ghost | SFX_Ghost_SpiritSeal.wav | 0.85 seconds | No | Paper-like noise and falling seal tones |
| Eating a cherry | SFX_Cherry_CrimsonFruit.wav | 0.95 seconds | No | A short rising four-note chime |
| Hitting a wall | SFX_Wall_StoneTouch.wav | 0.19 seconds | No | Soft stone-like resonances and a brief noise transient |
| PacStudent death | SFX_Death_FadingSpirit.wav | 1.8 seconds | No | Falling tones and an airy fading envelope |

Effects are 44.1 kHz, 16-bit mono PCM. One-shots have short edge fades to avoid
abrupt waveform cuts. Exact hashes and measured levels are in Effects-Measurements.json.
