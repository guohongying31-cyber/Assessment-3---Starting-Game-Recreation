# Shan Hai Spirit Trail: theme and asset guidance

**Visual direction and production specifications. Completion is recorded separately.**

## Core premise

A cultivator in jade robes enters an ancient sealing maze, collecting scattered spirit energy
along its stone paths. Four mythical beasts affected by a dark influence roam the maze.
A ward-breaking elixir briefly empowers the cultivator to seal them; crimson fruit grants
bonus points. Preserve the original collection, pursuit, and temporary reversal mechanics,
using night colors, jade, talisman paper, and spirit light.

This is an original proposed story, not a plot from the Classic of Mountains and Seas.
Do not adopt character designs from modern games or animation.

## Character and interaction mapping

| Assessment role | Theme equivalent | Silhouette and production priorities |
|---|---|---|
| PacStudent | Jade-robed cultivator | Hair knot, short cape, narrow sleeves, pale jade belt; keep the sheathed sword within one cell |
| Ghost 1 | Nine-tailed fox | Fan-shaped tail cluster and pointed ears, with warm vermilion colors |
| Ghost 2 | One-legged flame crane | Long beak, raised wings, blue-green feathers and red markings |
| Ghost 3 | White-headed hound | Four-legged creature with a white head and dark body; distinguish it from the fox by face and gait |
| Ghost 4 | Serpent-tailed tortoise | Broad low shell, birdlike head, and slender snake tail; bronze green and amber |
| Standard pellet | Spirit mote | Small diamond of light that clearly marks the route |
| Power pellet | Ward-breaking elixir | Round elixir and flashing square talisman ring, larger than a spirit mote |
| Bonus cherry | Crimson fruit | One red fruit with two leaves; visually distinct from spirit motes |
| Life indicator | Soul jade pendant | A separate pendant silhouette that can later be repeated to show remaining lives |
| Outer walls | Guardian stones | Thick jade edges with an inner pale gold line; prioritize a clear geometric outline |
| Inner walls | Inner jade channels | Thinner jade lines, clearly distinct from outer walls |
| T junction | Three-way seal junction | One base T-shaped image reused through rotation and reflection |
| Ghost exit | Spirit-seal gate | A pale violet talisman strip, clearly different from a solid wall |

Use the classic only as a source of written inspiration. The fox and tortoise appear in
[Southern Mountains](https://zh.wikisource.org/zh-hans/%E5%B1%B1%E6%B5%B7%E7%B6%93/%E5%8D%97%E5%B1%B1%E7%B6%93);
the crane and hound appear in
[Western Mountains](https://ctext.org/shan-hai-jing/xi-shan-jing/zh).
The English creature labels above are descriptive design names, not formal translations.
The proposed colors, clothing, personalities, and mechanics are project design choices,
not claims about the original text.

## Consistent production specifications

- Current source sheets retain their native dimensions and use 384 pixels per unit.
  The visual child scales each logical source cell to one Unity unit; see Documentation/Visual.
- Keep the main silhouette inside its logical cell, leaving space for movement and tail motion.
  Do not let solid character shapes exceed the corridor width.
- Import textures as sprites with Point filtering and no lossy compression; use consistent pivots.
- Name files by role and state, such as `Cultivator_Walk_Right_00` and `NineTail_Scared_Up_01`.
- Draw front, back, and side walking poses separately; do not rotate a single side-view image.
- Use at least two frames per state; four walking frames are recommended. Animate limbs,
  sleeves, tails, or wings so the frame changes are visible.
- Save editable artwork in `SourceArt`, export PNG files to `Assets/Art/Sprites`,
  and preserve the actual creation process.
- Keep all filenames, asset labels, text in images, and game interface text in English.

| Color role | Suggested value | Use |
|---|---|---|
| Night | `#101D2A` | Background and negative space |
| Deep jade | `#285958` | Main wall surfaces |
| Light jade | `#72C8B1` | Cultivator and wall highlights |
| Warm gold | `#E6C477` | Spirit motes and seal markings |
| Vermilion | `#D96956` | Fox, fruit, and selected markings |
| Ivory | `#F0E6CA` | Faces, white head, and emphasis |
| Muted violet | `#8E7AAE` | Seals and dead-state spirit emblems |

Color supports recognition; all four beasts must also have distinct silhouettes.
Show fear through a crouched pose, trembling, and a moving forehead talisman.
Alternate normal and frightened appearances while recovering. For the dead state,
use a broken spirit emblem rather than the original game's floating eyes.

## Animation production checklist

The cultivator has five states, each of four beasts has ten, and the elixir has one:
46 preview states. Drawing each beast independently with only two frames per state
would require at least 92 state frames. This is an estimate for the separate-controller
plan; permitted reuse of shared states can reduce the number of unique images.

| Character or object | Required states | Suggested motion |
|---|---|---|
| Cultivator | Walk Left/Right/Up/Down | Alternating steps, sleeves, and belt movement |
| Cultivator | Dead | Spirit light fades and the body dissolves into jade fragments; non-looping |
| Each beast | Walk Left/Right/Up/Down | Steps or wingbeats suited to its anatomy |
| Each beast | Scared Left/Right/Up/Down | Crouching, trembling, and moving talisman paper |
| Each beast | Recovering | Clearly alternate normal and frightened appearances |
| Each beast | Dead | Two distinct frames of a recognizable spirit emblem breaking or reforming |
| Elixir | Pulse | Change the elixir and seal ring's brightness and size |

Organize layout resources by tile categories 1-8. Draw six base wall images without
additional rotation variants. Use one spirit-mote image and separate elixir animation frames.
Do not continually add new wall categories to solve connection problems.

## Audio direction

Use a short bell and plucked strings for the intro, sparse pentatonic notes for the menu,
floating sustained harmony without drums for normal pursuit, high chimes for frightened beasts, and a low atmospheric
tone for dead beasts. Use soft boot steps for movement, a short jade chime for spirit motes,
paper and seal sounds for trapping a beast, an ascending arpeggio for fruit, a short stone
impact for walls, and fading breath for the cultivator's death.

Eleven waveform-synthesized clips now implement this direction in Assets/Audio Clips.
After hearing the first preview, the user requested a more ethereal sound with a weaker
rhythm. The revised music uses slower, sparse notes, sustained pads, and longer reflections.
No external recordings are used.
See [audio provenance](Audio/Audio-Provenance.md) and [validation](Audio/Audio-Validation.md)
for the inventory, production sources, measurements, and actual playback checks.

## Visual production status

The sprite library now contains the cultivator, four beasts, pickups, a life icon and six
base wall tiles. Documentation/Visual records their dimensions, named slices and checks.
The scene displays all fifteen reusable sprite prefabs. Six visual controllers contain
46 clips, and two full Play sessions verified every required state. See
[the animation guide](Visual/Animation-Guide.md) for preview timings and direction control.
