"""Offline interaction-effect synthesis, outside the Unity runtime."""

import json
import numpy as np

from render_music import ROOT, RATE, tone, export


SFX = ROOT / "Assets" / "Audio Clips" / "SFX"


def timeline(seconds):
    return np.zeros(round(seconds * RATE), dtype=np.float64)


def place(buffer, sound, start, gain=1):
    begin = round(start * RATE)
    count = min(len(sound), len(buffer) - begin)
    buffer[begin:begin + count] += sound[:count] * gain


def softened_noise(seconds, seed, width=40):
    rng = np.random.default_rng(seed)
    count = round(seconds * RATE)
    return np.convolve(rng.normal(size=count), np.ones(width) / width, mode="same")


def movement():
    audio = timeline(.8)
    t = np.arange(round(.18 * RATE)) / RATE
    for step, start in enumerate([.035, .435]):
        envelope = (1 - np.exp(-t * 650)) * np.exp(-t * 34)
        foot = softened_noise(.18, 410 + step, 55) * envelope
        foot += .12 * np.sin(2 * np.pi * (95 + step * 9) * t) * envelope
        foot[-441:] *= np.linspace(1, 0, 441)
        place(audio, foot, start, .85 if step == 0 else .7)
    return export(SFX / "SFX_Move_SoftSteps.wav", audio, loop=True,
                  peak_limit=.32, rms_target=.045)


def pellet():
    audio = timeline(.24)
    place(audio, tone(86, .24, "bell"), 0, .8)
    place(audio, tone(93, .18, "pluck"), .025, .12)
    return export(SFX / "SFX_Pellet_SpiritMote.wav", audio, peak_limit=.34, rms_target=.075)


def seal():
    audio = timeline(.85)
    t = np.arange(round(.5 * RATE)) / RATE
    paper = softened_noise(.5, 420, 12) * np.sin(np.pi * t / .5) ** 2 * np.exp(-t * 6)
    place(audio, paper, 0, .4)
    for note, start, gain in [(81, .04, .25), (74, .13, .32), (69, .23, .34)]:
        place(audio, tone(note, .55), start, gain)
    return export(SFX / "SFX_Ghost_SpiritSeal.wav", audio, peak_limit=.4, rms_target=.085)


def fruit():
    audio = timeline(.95)
    for note, start in [(74, 0), (77, .1), (81, .2), (86, .3)]:
        place(audio, tone(note, .65, "bell"), start, .35)
    return export(SFX / "SFX_Cherry_CrimsonFruit.wav", audio, peak_limit=.36, rms_target=.08)


def wall():
    audio = timeline(.19)
    t = np.arange(len(audio)) / RATE
    envelope = (1 - np.exp(-t * 700)) * np.exp(-t * 42)
    audio += (.7 * np.sin(2 * np.pi * 230 * t)
              + .3 * np.sin(2 * np.pi * 367 * t)
              + .17 * np.sin(2 * np.pi * 583 * t)) * envelope
    audio += softened_noise(.19, 430, 22) * envelope * .17
    return export(SFX / "SFX_Wall_StoneTouch.wav", audio, peak_limit=.32, rms_target=.07)


def death():
    audio = timeline(1.8)
    t = np.arange(len(audio)) / RATE
    envelope = np.sin(np.pi * t / 1.8) ** 1.7
    phase = 2 * np.pi * (660 * t - 125 * t * t)
    audio += .17 * np.sin(phase) * envelope * np.exp(-t * 1.3)
    audio += softened_noise(1.8, 440, 35) * envelope * .35
    for note, start, gain in [(74, .05, .3), (69, .32, .22), (62, .65, .18), (50, .95, .16)]:
        place(audio, tone(note, .8, "pluck"), start, gain)
    return export(SFX / "SFX_Death_FadingSpirit.wav", audio, peak_limit=.4, rms_target=.075)


def main():
    records = [movement(), pellet(), seal(), fruit(), wall(), death()]
    destination = ROOT / "Documentation" / "Audio" / "Effects-Measurements.json"
    destination.write_text(json.dumps(records, indent=2) + "\n", encoding="ascii")
    print(json.dumps(records, indent=2))


if __name__ == "__main__":
    main()
