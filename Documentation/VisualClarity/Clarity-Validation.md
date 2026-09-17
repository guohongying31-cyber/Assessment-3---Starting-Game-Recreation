# Visual clarity improvement

The Game view screenshot showed enlarged preview pixels and small labels. The
local saved Editor layout confirmed that Low Resolution Aspect Ratios was
enabled and the preview scale was 1.5. The project also used small world-space
text and Point filtering for every sprite sheet.

## Changes

- Generated labels now use a minimum font size of 18, increased from 14 for the
  smallest captions. Muted captions have stronger contrast. Label widths expand
  only when the actual font requires more space; centered positions are retained.
- Dynamic font rasterization uses a Canvas scale factor between 2 and 4 based on
  the camera's pixel height and world size. Changing resolution without changing
  aspect refreshes font density without moving the labels or rebuilding layout.
- The five character sheets and pickup sheet use Bilinear filtering. Original
  PNG files, sprite slices, animation frames and source dimensions are unchanged.
- Wall tiles retain Point filtering. A trial with Bilinear walls produced faint
  atlas-edge fragments at 1080p, so it was reverted before the final tests.
  Mipmaps remain disabled because these sheets have no padding between frames.
- The [local saved preview settings](Preview-Settings.json) now disable Low
  Resolution Aspect Ratios and use 1x preview zoom. These are local Editor
  preferences, excluded from Git; the next interactive launch was not observed.

## Actual verification

Unity 6000.4.11f1 captured the same saved scene before and after the change at
1920x1080, 1536x740, 1280x720, 1024x768, 720x1280 and 2560x1440. The 1536x740
case approximates the user's saved Game view dimensions; it is not a measurement
of the uploaded image's render buffer.

[Before checks](Before-Validation.json) and [after checks](After-Validation.json)
verify all 660 generated cells, 26 label rectangles inside the view, complete text
and the unchanged manual scene after Stop. An additional same-aspect test changes
1280x720 to 2560x1440 and verifies increased glyph density with unchanged label
positions. All six after captures use a minimum font size of 18; density is 2
at the first five sizes and approximately 2.576 at 2560x1440.

| Resolution | Before | After |
| --- | --- | --- |
| 1920x1080 | [Before](Before-1920x1080.png) | [After](After-1920x1080.png) |
| 1536x740 | [Before](Before-1536x740.png) | [After](After-1536x740.png) |
| 1280x720 | [Before](Before-1280x720.png) | [After](After-1280x720.png) |
| 1024x768 | [Before](Before-1024x768.png) | [After](After-1024x768.png) |
| 720x1280 | [Before](Before-720x1280.png) | [After](After-720x1280.png) |
| 2560x1440 | [Before](Before-2560x1440.png) | [After](After-2560x1440.png) |

All twelve comparison images were visually inspected. The final captures show
clearer captions without clipping or overlaps, continuous wall runs and no visible
neighboring-frame fragments in the captured poses. Some text remains physically
small at 1024x768 because the entire composition shares the available screen.

The established generator regression also passed on [Default](Regression-Default.json),
[Small](Regression-Small.json), [Wide](Regression-Wide.json),
[Tall](Regression-Tall.json) and [all T/gate orientations](Regression-AllTAndGateOrientations.json).
These five sessions cover twenty map/aspect combinations, exact transforms and
sprite references, repeated generation, invalid retry preservation, view re-enable
and restoration of the saved manual scene. The default session runs for at least
28 seconds, observing three patrol laps, all 46 showcase states and the normal
music loop. Alternate sessions verify the initial patrol segment and animated
elixirs; they do not claim full patrol laps.

Temporary verification helpers were removed from Assets. A subsequent clean
Unity batch import succeeded. The [static audit](Static-Audit.json) checks English
text, metadata, source hashes and the final reports. Only one runtime script and
six texture metadata files changed among the 246 prior asset files; the remaining
239 files, including the saved scene and every PNG/audio file, are unchanged.

## Preview on another computer

Use a fixed Game view resolution such as 1920x1080 or 2560x1440, turn off Low
Resolution Aspect Ratios when available, and inspect at 1x zoom in a sufficiently
large Game panel. A small panel can still make an entire maze and two galleries
physically small; increasing source image resolution alone cannot recover that
screen space. These checks do not replace final build and submission validation.
