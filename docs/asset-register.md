# Asset provenance register

All newly authored files under `Assets/Models` (excluding the pre-existing `Test.blend` and `Untitled.blend`) were created for Open Waters by Codex using original procedural mesh design in Blender on 2026-10-20. Shared palette, wave-field texture, URP water shader, mesh effects and icon renders were authored locally. No downloaded models, external texture packs, paid generators or extracted Clash of Clans content were used. Clash of Clans informed only broad silhouette/readability and visible-upgrade direction.

Project licensing governs these new project files; no third-party asset attribution requirements were introduced by this work. This register is provenance evidence, not a legal opinion. Existing source files retain their prior provenance.

Inventory: `Assets/Models/asset-manifest.json`. Editing/integration: `Assets/Models/README.md`. Native sources: per-group `Sources~` folders and the workshop master. No external publication occurred.
# Gameplay audio addition - 2026-10-20

The demo's harbor wash, UI bell, delivery chime, voyage alert and port-upgrade cue are generated locally from original sine/noise synthesis in `Assets/OpenWaters/Scripts/UI/DemoController.cs` (ambience seed 2701). No downloaded recordings, third-party samples or attribution-dependent audio sources were used. Source synthesis is included with the project. These are prototype cues; browser gesture behavior and listening-level acceptance remain platform validation gates.

## Interface branding addition - 2026-10-20

`Assets/OpenWaters/Resources/Branding/Logo.png` is an original placeholder sailing emblem, authorized by the user. Twenty consistent interface pictograms under `Resources/UI/Icons` are original procedural line/polygon artwork for buildings, dock, shipments, routes, operations, GSA, settings and controls. No third-party icon pack or downloaded logo was used. Reproducible authoring source: `Assets/OpenWaters/Editor/BrandingBuilder.cs`. The builder only creates missing PNGs, preserving user replacements. Replace the one Logo.png file while retaining its metadata, then rebuild to update menu/HUD/default application icons. `BrandingImporter` configures import and refreshes application icon settings automatically.

