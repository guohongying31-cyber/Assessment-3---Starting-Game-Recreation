# Visual animation guide

Open Assets/Scenes/RecreatedLevel.unity with Unity 6000.4.11f1 and press Play.
The AssetShowcase displays five characters, four items and six wall samples in side panels
beside the manual maze. See the [manual scene guide](../ManualLevel/Manual-Level-Guide.md).
Characters cycle through their states automatically; the elixir pulses continuously.
The opening music switches to the normal loop after its 2.4-second intro.

## Controllers and states

| Controller | States |
|---|---|
| PacStudentAnimator | WalkingRight, WalkingLeft, WalkingUp, WalkingDown, Dead |
| GhostAnimator_NineTail | Four Walking directions, four Scared directions, Recovering, Dead |
| GhostAnimator_FlameCrane | Four Walking directions, four Scared directions, Recovering, Dead |
| GhostAnimator_WhiteHound | Four Walking directions, four Scared directions, Recovering, Dead |
| GhostAnimator_SerpentTortoise | Four Walking directions, four Scared directions, Recovering, Dead |
| ElixirAnimator | Pulse |

There are 46 visual clips. Every clip switches between two distinct sprite frames.
Four separate creature controllers retain each creature's own frame references.
Controllers are in Assets/Animations/Controllers; clips are grouped by actor in
Assets/Animations/Clips. The manifest records the exact names and asset paths.

## Showcase timing

| State type | Clip duration | Exit Time | Time before transition |
|---|---:|---:|---:|
| Walking | 0.50 s | 4 cycles | 2.00 s |
| Scared | 0.60 s | 4 cycles | 2.40 s |
| Recovering | 0.75 s | 3 cycles | 2.25 s |
| Dead | 0.75 s | 1 cycle | 0.75 s |
| Pulse | 1.00 s | No transition | Continuous |

Walking, Scared, Recovering and Pulse loop. Dead has Loop Time disabled and plays fully
once per showcase pass. Transitions have zero duration. The default character state is
WalkingDown. It continues into the next state in the manifest sequence and eventually
returns to WalkingDown. One complete creature pass takes 20.6 seconds.

Each character prefab has an Animator on its root and a SpriteRenderer on the Visual
child. Clips animate only Visual's sprite property. Root motion is disabled. The Visual
child supplies the scale that normalizes the source cell to one world unit; leave actor
root scale at one for later movement work.

## Using the prefabs in the movement stage

All character controllers expose a Boolean parameter named Showcase, enabled by default.
Set Showcase to false on movement instances before selecting a direction. Then select the
matching WalkingRight, WalkingLeft, WalkingUp or WalkingDown state immediately through
Animator.Play. The disabled Showcase parameter prevents automatic preview transitions
from taking control again. The showcase instances can retain their automatic cycles.

This control configuration has a Play-mode verification, but patrol movement itself
belongs to a later feature. These prefabs do not yet implement movement or collisions.

## Inspecting and editing

Select a character under AssetShowcase/Characters and open Window > Animation > Animator
to watch the active state. Select a clip to preview its two frame references in the
Animation window. Edit Exit Time only with both the three-cycle and two-second minimum
in mind. Keep Dead non-looping and preserve the Visual child path and named sprite IDs.

See [visual validation](Visual-Validation.md) for actual checks and their scope.
