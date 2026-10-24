# Assessment 4 Validation Record

## Scene foundation

Unity 6000.4.11f1 compiled and ran the playable Level 1 foundation in batch Play Mode. The focused verification completed 12 assertions on 2026-10-24 (local development date).

Verified top-left stationary player spawn, four evenly spaced central ghost spawns, no automatic patrol component, disabled actor showcase cycles, all 660 generated A3 map sprites, 218 motes and four animated power pellets, and no movement without input after the ready period.

This is foundation evidence only. Menu, HUD, formal movement, collisions, prescribed ghost behavior, innovation and final player validation remain separate stages. The original A3 submission archive is preserved.

## Start screen

Unity compilation and batch Play Mode passed nine checks on 2026-10-24: first enabled scene StartScene, PacStudent title, exact Level 1 and Level 2 button labels, five world-space character sprites, animated Canvas border, centisecond formatting, looping menu music, and both level navigation paths. The test returned from Level 1 to StartScene before opening Level 2.

Rendered and visually inspected 1920x1080 and 1440x1080 menu captures. The border, roster, labels and buttons fit without overlap or cropping. Fresh records display zero time; older score-only records display an unknown associated time until a completed run supplies one. Full score-saving behavior will be validated with the lifecycle stage.

## HUD appearance

Unity compilation and Play Mode passed 17 assertions on 2026-10-24. Verified HUD Screen Space Canvas, initial six-digit score and zero timer, Level 1 label, three Soul Jade life sprites, and four correctly named child World Space Canvases with their corresponding single number. Rendered and inspected both 1920x1080 and 1440x1080 layouts; the maze and HUD do not overlap and ghost numbers remain readable. Elapsed-time progression and result transitions are intentionally validated in the later lifecycle stage.

The user deferred preparation of the submission declaration. It is outside the current delivery work until requested again.

## Player movement, moving cherry and effects

Unity compilation passed. Deterministic movement verification completed 57 assertions across nine scenarios: stationary start, linear speed, buffered invalid turns, latest-key retention, delayed reversal, first-contact wall feedback, both tunnels, gate exclusion, 30/60/144 FPS consistency, large updates and callback cancellation.

Integrated Play Mode verification completed 18 assertions covering the PacStudentController input fields, delayed reversal, dust emission, one impact burst per wall contact, death-particle emission/cleanup, five-second cherry spawn and respawn, three-unit-per-second linear travel through the exact map centre, new random entry, sorting/mask configuration, collection once and automatic exit disposal. A rendered centre-crossing capture was inspected.

The first run exposed Unity clamping the mask back sorting order to the old front order. Setting the front order first fixed the range; the successful rerun verified 49..51, with fruit at 50. Fruit uses an explicit SpriteMask and world-space movement. Particle simulation pauses with the game. Audio selects the pellet or empty-corridor loop from the upcoming target cell.

## Collision and state milestone

The first collision milestone compiled and passed 90 Play Mode assertions across ten scenarios on 2026-10-24. Checks exercised actual game stepping for 10-point motes, 50-point power pellets, fixed 300-point Scared/Recovering ghost captures, ten-second power with final-three-second recovery, destroyed collection objects, one wall impact per contact, and naturally spawned moving-fruit collection for 100 points. Death lasted 1.8 seconds with matching animation duration, particles, frozen moving ghosts and a stationary top-left respawn. Pause froze actors, fruit, power/ward timers and live particle lifetimes.

Four additional contact regressions covered the nonteleporting fruit and player portal seams. A separate fruit sweep prevents a cherry on the opposite map edge from being collected through a fictitious periodic copy.

This milestone verified the specified temporary three-second Dead timer. The later ghost-movement milestone must replace it with physical return to each ghost's own spawn, as explicitly required by the 85% band.

## Countdown, result flow and paired records

Unity compiled and the lifecycle suite passed 88 assertions across six scenarios. The four countdown labels each receive one second; actors and elapsed time remain frozen until GO ends. The first cherry appears at five scene seconds, including the countdown. Gameplay time advances during play and the 1.8-second death sequence, but not during pause or results. Respawn resumes play directly with a stationary right-facing player.

Both completion paths were exercised: collection arrival callbacks for all remaining pellets, and three consecutive ordinary-ghost deaths. Both enter a locked three-second Game Over state and automatically return to StartScene. The collection fixture is an integration test of arrival callbacks, not evidence of a human-played complete route. Verified score/time pairing, higher-score replacement, faster equal-score replacement, slower/lower-result rejection, independent level records and menu display after return. Test preferences were restored afterward.

Rendered result screens at both required aspect ratios after Unity completed object destruction. The capture helper preserves the production overlay ordering when rendering to a texture.
