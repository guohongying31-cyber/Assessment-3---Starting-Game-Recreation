"""Offline audio authoring, outside the Unity runtime.

Creates original waveform-based music without downloaded recordings or soundfonts.
Requires Python and NumPy. Run from any directory: python SourceAudio/render_music.py
"""

from pathlib import Path
import hashlib
import json
import math
import wave

import numpy as np


ROOT = Path(__file__).resolve().parents[1]
RATE = 44100
MUSIC = ROOT / "Assets" / "Audio Clips" / "Music"


def frequency(midi):
    return 440.0 * 2.0 ** ((midi - 69.0) / 12.0)


def tone(midi, duration, voice="pluck", seed=0):
    t = np.arange(round(duration * RATE), dtype=np.float64) / RATE
    f = frequency(midi)
    attack = 1.0 - np.exp(-t / 0.005)
    if voice == "pluck":
        y = sum(np.sin(2 * np.pi * f * h * t) * np.exp(-t * (1.9 + h * 0.48))
                / h ** 1.55 for h in range(1, 8))
        y *= attack
    elif voice == "bell":
        y = sum(a * np.sin(2 * np.pi * f * ratio * t) * np.exp(-t * decay)
                for ratio, a, decay in [(1, 1, 1.7), (2.01, .35, 2.8),
                                        (2.76, .13, 4), (4.11, .05, 5.5)])
        y *= attack
    elif voice == "pad":
        y = (np.sin(2 * np.pi * f * t) + .22 * np.sin(2 * np.pi * f * 2 * t)
             + .09 * np.sin(2 * np.pi * f * 3 * t))
        y *= np.sin(np.pi * np.arange(len(t)) / max(1, len(t) - 1)) ** 1.6
    elif voice == "drum":
        phase = 2 * np.pi * (54 * t + 4 * (1 - np.exp(-t * 22)))
        y = np.sin(phase) * np.exp(-t * 17) * attack
    elif voice == "wood":
        rng = np.random.default_rng(seed)
        noise = np.convolve(rng.normal(0, .2, len(t)), np.ones(8) / 8, mode="same")
        y = (.7 * np.sin(2 * np.pi * 710 * t) + noise) * np.exp(-t * 60) * attack
    else:
        raise ValueError(voice)
    fade = min(round(.025 * RATE), len(t) // 4)
    y[-fade:] *= np.linspace(1, 0, fade)
    return y


def add(buffer, sound, start, gain=1, pan=0, loop=False):
    stereo = sound[:, None] * np.array([math.sqrt((1 - pan) / 2),
                                      math.sqrt((1 + pan) / 2)]) * gain
    begin = round(start * RATE)
    if loop:
        indices = (begin + np.arange(len(sound))) % len(buffer)
        np.add.at(buffer, indices, stereo)
    else:
        size = max(0, min(len(sound), len(buffer) - begin))
        buffer[begin:begin + size] += stereo[:size]


def room(buffer, loop):
    dry = buffer.copy()
    for delay, gain in [(.137, .18), (.293, .14), (.467, .10), (.719, .07), (.941, .045)]:
        shift = round(delay * RATE)
        if loop:
            reflected = np.roll(dry[:, ::-1], shift, axis=0)
        else:
            reflected = np.zeros_like(dry)
            reflected[shift:] = dry[:-shift, ::-1]
        buffer += reflected * gain
    return buffer


def export(path, audio, loop=False, peak_limit=.46, rms_target=.105):
    audio = np.asarray(audio, dtype=np.float64)
    if audio.ndim == 1:
        audio = audio[:, None]
    audio -= np.mean(audio, axis=0)
    if not loop:
        ramp = min(round(.015 * RATE), len(audio) // 4)
        audio[:ramp] *= np.linspace(0, 1, ramp)[:, None]
        audio[-ramp:] *= np.linspace(1, 0, ramp)[:, None]
    peak = float(np.max(np.abs(audio)))
    rms = float(np.sqrt(np.mean(audio * audio)))
    audio *= min(peak_limit / max(peak, 1e-9), rms_target / max(rms, 1e-9))
    pcm = np.rint(audio * 32767).astype("<i2")
    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), "wb") as stream:
        stream.setnchannels(pcm.shape[1])
        stream.setsampwidth(2)
        stream.setframerate(RATE)
        stream.writeframes(pcm.tobytes())
    return {
        "file": path.relative_to(ROOT).as_posix(),
        "duration_seconds": round(len(pcm) / RATE, 6),
        "sample_rate": RATE,
        "channels": int(pcm.shape[1]),
        "bit_depth": 16,
        "loop": loop,
        "peak_dbfs": round(20 * np.log10(max(np.max(np.abs(audio)), 1e-9)), 3),
        "rms_dbfs": round(20 * np.log10(max(np.sqrt(np.mean(audio * audio)), 1e-9)), 3),
        "boundary_step": round(float(np.max(np.abs(audio[0] - audio[-1]))), 8),
        "clipped_samples": int(np.count_nonzero(np.abs(pcm.astype(np.int32)) >= 32767)),
        "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
    }


def render_intro():
    audio = np.zeros((round(2.4 * RATE), 2))
    for start, note, gain in [(0, 62, .65), (.35, 69, .52), (.72, 74, .45)]:
        add(audio, tone(note, 1.7), start, gain, -.15 + start * .3)
    add(audio, tone(86, 1.55, "bell"), .8, .14, .25)
    add(audio, tone(50, 2.35, "pad"), 0, .14)
    return export(MUSIC / "Music_Intro_JadeAwakening.wav", room(audio, False))


def render_loop(name, bpm, melody, mood):
    beat = 60 / bpm
    audio = np.zeros((round(32 * beat * RATE), 2))
    for bar in range(8):
        base = bar * 4 * beat
        root = [50, 53, 48, 50][bar % 4]
        if mood == "dead":
            add(audio, tone(root - 12, 4 * beat, "pad"), base, .4, -.1, True)
            add(audio, tone(root - 5, 4 * beat, "pad"), base, .16, .2, True)
            if bar % 2 == 0:
                add(audio, tone(melody[bar // 2], 3.2, "bell"), base + beat, .15, -.25, True)
            add(audio, tone(root + 12, 2.0), base + 2.5 * beat, .1, .3, True)
            continue
        add(audio, tone(root, 4 * beat, "pad"), base, .24 if mood != "scared" else .2, 0, True)
        add(audio, tone(root + 7, 4 * beat, "pad"), base, .055, .25, True)
        for step in range(4):
            index = (bar * 4 + step) % len(melody)
            note = melody[index]
            if note is not None:
                voice = "bell" if mood == "scared" else "pluck"
                add(audio, tone(note, 2.4 if mood != "scared" else 1.65, voice),
                    base + step * beat, .22 if mood == "scared" else .34,
                    -.2 if step % 2 == 0 else .2, True)
            if mood == "scared" and step == 2:
                harmony = root + (12 if step % 2 == 0 else 19)
                add(audio, tone(harmony, 1.6), base + (step + .5) * beat, .075, .3, True)
        if mood == "menu" and bar % 2 == 0:
            add(audio, tone(81, 2.3, "bell"), base + 2.5 * beat, .08, .35, True)
    return export(MUSIC / name, room(audio, True), loop=True,
                  rms_target=.085 if mood == "dead" else .105)


def main():
    records = [render_intro()]
    records.append(render_loop("Music_Menu_CloudTerrace.wav", 60,
                   [62, None, None, 69, 74, None, None, None, 65, None, None, 69, 62, None, None, None], "menu"))
    records.append(render_loop("Music_Normal_ThroughTheSeal.wav", 80,
                   [62, None, 69, None, 67, None, None, 65, 62, None, 57, None, 60, None, 65, None], "normal"))
    records.append(render_loop("Music_Scared_TalismanChase.wav", 96,
                   [74, None, 81, 79, None, 74, None, 69, 74, None, 81, None, 79, None, 77, None], "scared"))
    records.append(render_loop("Music_Dead_ReturningSpirits.wav", 60, [74, 72, 69, 77], "dead"))
    destination = ROOT / "Documentation" / "Audio"
    destination.mkdir(parents=True, exist_ok=True)
    (destination / "Music-Measurements.json").write_text(json.dumps(records, indent=2) + "\n", encoding="ascii")
    print(json.dumps(records, indent=2))


if __name__ == "__main__":
    main()
