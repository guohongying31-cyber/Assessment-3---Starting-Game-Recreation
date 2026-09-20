# Menu, ward and Windows preview validation

Date: 2026-09-21. Editor: Unity 6000.4.11f1.

## Completed behavior

MainMenu links ClassicLevel and TalismanLevel. It displays the animated cultivator
and four guardians, plays menu music, supports mouse and keyboard selection, and
shows separate saved best scores. Pause and result overlays provide continuation
and return-to-menu buttons. Quit operates in the Windows player.

Talisman Trial adds a three-second active shield with a 15-second recharge from
activation. The visible aura reuses the existing elixir visual at a larger scale.
No new third-party package or physics movement was introduced. The classic mode
rejects the ability, keeping the two modes distinct.

## Checks actually executed

- Compiled and saved all three scenes with their serialized references.
- Core maze regression: 3,372 assertions, including frame rates, turns, tunnels
  and complete collection. Guardian regression: 115,148 assertions including
  three minutes of path legality and frame-rate/timer comparisons.
- Actual Play rules regression: 31 assertions. Added a contact-at-power-expiry
  case after review found that timer ordering could turn an already edible
  contact into a death. Simulation steps now stop at effect boundaries and
  preserve the contact's power state within the step.
- Actual Play scene/ward flow: 19 assertions across menu, both playable scenes,
  and the preserved Assessment 3 scene. Checks include menu button navigation,
  rejection of skills in Classic/Ready/Paused/Dying, active protection without
  score, repeated-input rejection, timer freeze, protection expiry, one death,
  cooldown completion, respawn/new-game reset, and no duplicate EventSystem.
- RecreatedLevel still generates 660 sprite tiles and its original patrol moves.
- Captured and inspected 1920 by 1080 and 720 by 1280 menu frames, plus 1440 by
  1080 and 720 by 1280 gameplay/ward frames. Main labels, buttons, map and controls
  remain visible without overlapping.
- Windows x64 player build succeeded with zero errors and zero warnings;
  reported output size: 153,137,111 bytes.
- A hidden headless Windows-player smoke run stayed alive for 15 seconds with
  no detected script exceptions, then the harness terminated its own process.
  This checks startup, not manual keyboard play or visual/audio quality in the
  standalone window. Visual and interaction checks above ran in Unity Play.

Test helpers and logs are kept outside tracked deliverables. Automated scene-flow
tests restore prior best-score preferences so test scores are not presented as
the player's records. The previous Assessment 3 ZIP remains unchanged; its
SHA-256 is `1863a50e33023b53a32736228cad901d47797786ce72874ef1c3b7cdd59164e7`.

The new playable work is an Assessment 4 development milestone. A complete HD
compliance assessment and final submission package require the full new rubric.
