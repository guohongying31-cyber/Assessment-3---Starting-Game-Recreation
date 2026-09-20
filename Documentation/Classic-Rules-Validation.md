# Classic gameplay rules

Implemented and validated on 2026-09-21 in Unity 6000.4.11f1.

## Rules in this recreation

The classic scene keeps the supplied mirrored maze and the cultivation theme.
The core loop follows the [official Pac-Man game instructions](https://dlassets-ssl.xboxlive.com/public/content/467ce1a6-5f37-4556-87c4-bc141a91051f/GameManual/61e9a9f3-c73b-4241-a1b9-0eeec5f2e1f0/ru-RU/index.html):
collect the dots, avoid ordinary ghosts, eat frightened ghosts after a power
pickup, collect bonus fruit, and clear the maze to advance.

- Begin with three lives. A normal guardian contact removes one life.
- Death preserves score and collected pickups, then resets actor positions.
- An elixir grants six seconds of frightened time in round one. Its last two
  seconds show the recovering animation. Later rounds shorten the duration to
  a minimum of two seconds and gradually increase movement speed.
- Consecutive guardian rewards are 200, 400, 800 and 1,600 per elixir chain.
- Eaten guardians are harmless, return through the upper house gate, wait one
  second and rejoin. Recovered guardians need a fresh elixir to become frightened.
- Crimson fruit appears at 70 and 170 pickups, lasts ten seconds, and awards
  100 points in round one with increasing rewards in later rounds.
- One extra life is awarded on reaching 10,000 points in a game.
- All pickups cleared: Enter starts the next round. No lives: Enter restarts.

The four guardians use direct pursuit, an ahead-of-player target, a flank target
relative to the first guardian, and a distance-dependent chase/scatter target.
Scatter/chase intervals begin at 7/20/7/20/5/20/5 seconds then remain in chase.
These speeds, delays and target projections are explicit tuning choices for this
different maze; this project does not emulate original arcade hardware quirks.

## Executed checks

- Unity compilation and actual ClassicLevel Play completed successfully.
- Guardian tests passed 115,148 assertions across eight scenarios: four spawns,
  separate 298-cell house navigation, staggered releases, mode and power timers,
  eaten returns, recovery immunity, tunnel returns, complete resets, frame-rate
  comparisons and three minutes of legal movement with irregular frame steps.
- The Play integration suite passed 30 assertions: collection, power activation,
  pause, all four combo rewards, repeated-contact protection, three deaths,
  preserved round progress, Game Over, restart, both fruit thresholds, fruit
  collection/expiry, the one-time extra life, and swept head-on/tunnel contact.
- Rules tests place actors deliberately to isolate collision outcomes; the
  separate long-running navigation test exercises autonomous decisions.
- Captured 1440 by 1080 and 720 by 1280 frames were visually inspected. The HUD
  uses responsive columns and wrapped controls rather than overlapping labels.
- Pause stops gameplay timers, character movement, actor and elixir animations,
  music and effects. Intro playback restarts on each new round or respawn.

These checks establish the listed behavior. They do not replace hands-on tuning
feedback or the full Assessment 4 marking specification, which is still absent.
