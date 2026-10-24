# Windows player UI shader repair

Date: 2026-09-21. Unity: 6000.4.11f1.

## Reported defect and cause

The Windows preview displayed magenta rectangles instead of menu text and
buttons, while character sprites rendered correctly. The defect was reproduced
in the original executable with graphics enabled.

MainMenu and ArcadeHud create their Canvas, Text and Image components at runtime.
The scene dependency scan therefore did not retain their default UI shader, and
Graphics Settings had an empty Always Included Shaders list. The actual Canvas
material uses `UI/Default`. The installed UGUI Text component also draws with
that default material; its font supplies the text texture.

Unity documents this editor/player difference and shader retention options in
the [Shader.Find reference](https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Shader.Find.html).

The earlier preview checks included rendered Editor Play and a headless player
startup test. A headless startup cannot detect this graphical defect. The original
preview was therefore insufficiently validated for standalone display.

## Repair

- Retain `UI/Default` explicitly in Graphics Settings. Other rendering settings
  retain their previous values.
- Add `RuntimeUiShaderCheck` as a permanent Editor build preprocessor. It refuses
  to build when the actual default Canvas shader is absent from the retained list,
  explaining which Graphics Settings entry must be restored.
- Replace the Windows preview at `output/PlayableBuild/ShanHaiSpiritTrail.exe`.

## Executed verification

- Unity compilation passed, including after removal of the temporary repair helper.
- A deliberately empty retention list was rejected by the preprocessor. Restoring
  the shader passed the same validation. The test restored settings before saving.
- A clean-cache Windows x64 build succeeded: zero errors, zero warnings,
  153,143,191 reported bytes.
- Ran the rebuilt executable with real graphics on Direct3D 12 / RTX 5080.
  Inspected its native window, rather than an Editor render or headless process.
- Main menu: title, descriptions, character labels and button backgrounds rendered
  normally. Checked both windowed and fullscreen display.
- Classic Trail: keyboard scene selection, Ready overlay, HUD, pause menu and
  mouse return-to-menu action worked and displayed readable text without magenta.
- Talisman Trial: Ready and gameplay HUD rendered normally. Space activated the
  visible aura and the `WARD ACTIVE` countdown. The Game Over overlay and its
  buttons also rendered correctly; the menu action returned successfully.
- Player log contained no script exceptions in the observed checks. The existing
  best scores were not reset, and these display checks collected no points.

This change repairs rendering and build validation; it does not alter game rules
or replace the preserved Assessment 3 submission archive.
