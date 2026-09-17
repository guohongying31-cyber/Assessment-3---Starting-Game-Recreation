"""Render editable geometric wall tiles and a pixel-aligned PNG atlas.

The source geometry uses a 32-unit grid. The SVG and PNG are rendered from the
same geometry; no existing raster artwork is modified. Requires Pillow and NumPy.
"""
from pathlib import Path
import hashlib
import json
import numpy as np
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[2]
CELL = 384
SCALE = CELL // 32
NAMES = ['Wall_OuterCorner', 'Wall_OuterStraight', 'Wall_InnerCorner',
         'Wall_InnerStraight', 'Wall_TJunction', 'Wall_ExitSeal']
COLORS = ['#101D2A', '#285958', '#72C8B1', '#E6C477']


def arms(x, y, directions, half):
    inside_x = abs(x - 15.5) < half
    inside_y = abs(y - 15.5) < half
    return ((inside_x and inside_y)
            or ('L' in directions and x < 16 and inside_y)
            or ('R' in directions and x >= 16 and inside_y)
            or ('U' in directions and y < 16 and inside_x)
            or ('D' in directions and y >= 16 and inside_x))


def color_at(index, x, y):
    color = None
    if index in (0, 1, 2, 3):
        directions = 'RD' if index in (0, 2) else 'LR'
        widths = [4, 3, 2, 1] if index < 2 else [3, 2, 1]
        for half, ink in zip(widths, COLORS):
            if arms(x, y, directions, half):
                color = ink
    elif index == 4:
        for outer, inner, ink in zip([4, 3, 2], [3, 2, 1], COLORS):
            if arms(x, y, 'LR', outer) or arms(x, y, 'D', inner):
                color = ink
        if arms(x, y, 'LR', 1):
            color = COLORS[3]
    else:
        if 13 <= y <= 18:
            color = '#40375D'
        if 14 <= y <= 17:
            color = '#8E7AAE'
        if 15 <= y <= 16:
            color = '#F0E6CA'
        if 13 <= x <= 18 and 13 <= y <= 18:
            color = '#E6C477' if x in (13, 18) or y in (13, 18) else '#8E7AAE'
    return color


def main():
    source = ROOT / 'SourceArt/Walls'
    source.mkdir(parents=True, exist_ok=True)
    target = ROOT / 'Assets/Art/Sprites/Walls/Wall_Tiles.png'
    if target.exists():
        raise SystemExit('Refusing to overwrite an existing wall atlas.')
    atlas = Image.new('RGBA', (CELL * 3, CELL * 2))
    draw = ImageDraw.Draw(atlas)
    for index, name in enumerate(NAMES):
        svg = [f'<svg xmlns="http://www.w3.org/2000/svg" width="384" height="384" viewBox="0 0 32 32" shape-rendering="crispEdges">',
               f'<title>{name}</title>']
        ox, oy = index % 3 * CELL, index // 3 * CELL
        for y in range(32):
            for x in range(32):
                ink = color_at(index, x, y)
                if ink is None:
                    continue
                svg.append(f'<rect x="{x}" y="{y}" width="1" height="1" fill="{ink}"/>')
                draw.rectangle((ox+x*SCALE, oy+y*SCALE, ox+(x+1)*SCALE-1, oy+(y+1)*SCALE-1), fill=ink)
        svg.append('</svg>')
        (source / (name + '.svg')).write_text('\n'.join(svg) + '\n', encoding='ascii')
    atlas.save(target)
    array = np.asarray(atlas)
    tiles = [array[i//3*CELL:(i//3+1)*CELL, i%3*CELL:(i%3+1)*CELL] for i in range(6)]
    assert np.array_equal(tiles[0][:,-1], tiles[1][:,0])
    assert np.array_equal(tiles[0][-1], tiles[1][:,0])
    assert np.array_equal(tiles[2][:,-1], tiles[3][:,0])
    assert np.array_equal(tiles[2][-1], tiles[3][:,0])
    assert np.array_equal(tiles[4][:,0], tiles[1][:,0])
    assert np.array_equal(tiles[4][:,-1], tiles[1][:,-1])
    assert np.array_equal(tiles[4][-1], tiles[3][:,0])
    assert np.array_equal(tiles[5][:,0,3], tiles[3][:,0,3])
    expected = [['R','D'], ['L','R'], ['R','D'], ['L','R'], ['L','R','D'], ['L','R']]
    result = []
    for name, tile, connections in zip(NAMES, tiles, expected):
        alpha = tile[:,:,3]
        actual = [side for side, edge in [('L',alpha[:,0]),('R',alpha[:,-1]),('U',alpha[0]),('D',alpha[-1])] if np.any(edge)]
        assert set(actual) == set(connections)
        result.append({'name':name, 'connections':connections})
    report = {'passed': True, 'tileSizePixels': CELL, 'atlasSizePixels': list(atlas.size),
              'matchingColorSeamChecks': 7, 'gateAlphaSeamCheck': True,
              'sha256': hashlib.sha256(target.read_bytes()).hexdigest(), 'tiles': result}
    (ROOT / 'Documentation/Visual/Wall-Connection-Validation.json').write_text(json.dumps(report,indent=2)+'\n',encoding='ascii')
    print(json.dumps({'passed': True, 'tiles': len(result), 'colorSeams':7}))


if __name__ == '__main__':
    main()
