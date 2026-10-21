# Open Waters modular art kit

45 independent prefabs live beside their FBX files in named folders. Drag a `.prefab` into a scene; do not import the entire workshop as one model. Each prefab has a placement root and a `Visuals` child. Static geometry is consolidated for rendering; animated parts and named sockets remain separate. Edit detailed geometry in the corresponding `Sources~/*.blend`, or create Unity prefab variants for placement/material/component changes.

## Inventory

- Dock, Warehouse, Fuel, Office, Repair: separate T1 and T2 prefabs (10).
- Freighter: one CoastalFreighter with CargoSocket, WakeSocket, DamageSocket and BowSocket.
- Cargo_Grain, Cargo_Textiles, Cargo_MachineParts: three interchangeable cargo clusters.
- Props: bollard, barrel, crate, container, lamp, fence.
- Coast: rock cluster, coast tile, quay tile. Vegetation: coastal pine.
- MapMarkers: Beacon Bay, Copperport, Meridian Quay; map miniatures, not destination scenes.
- Effects: selection, wake, arrival, upgrade, loading, damage smoke.
- Icons: ten original event/HUD emblems; transparent PNGs. Cargo and destination PNGs are included in their folders (16 PNG icons total).
- Water: OceanSurface and independent ShoreFoam.

`asset-manifest.json` records exact paths, triangle counts and animation flags. All dimensions are metres. Blender sources are Z-up, ship forward -Y; exported Unity models are Y-up, ship/building front +Z. Place buildings with yaw 180 when viewed from the preview camera on negative Z. Building roots are footprint-centered; the ship root is at the waterline.

## Animation

18 prefabs have imported four-second clips at 30 FPS. Their `Visuals` child uses Unity's Animation component, with automatic looping for ambient movement. Office flags flutter, the freighter bobs/rolls, the dock hoist moves, the repair trolley traverses, shoreline wash breathes, and mesh effects/icons animate. Upgrade is a manually triggered one-shot. Disable the Animation component for reduced motion or trigger `Animation.Play` when appropriate.

`Freighter/Animations/Departure.anim` and `Arrival.anim` are reusable four-second local-Z travel examples, 0 to 10 metres and back. Apply them to a dedicated travel parent, keeping the ship's independent idle animation underneath. Actual routes and gameplay triggers are not implemented. Parent Unity cargo/effect prefabs to CargoAttachment, WakeAttachment, DamageAttachment or BowAttachment with local position/rotation zero and the intended local scale. These children correct the raw Blender socket basis; do not parent Y-up prefabs directly to raw *Socket transforms. Wake should normally be enabled during travel, smoke only when damaged, loading during preparation, and upgrade only during the tier transition. The preview keeps the wake visible for inspection.

## Water

`Water/HarborWater.mat` uses the supplied URP shader: three periodic directional vertex swells, analytic normals, a deliberately curved seamless ripple/foam field, two detail scales, restrained highlights and distance fog. It is opaque and uses no realtime reflection or depth-texture requirement. Adjust wave height, speed, foam amount and colors in the material. Set Speed to 0 for reduced motion. Use ShoreFoam around CoastTile, scaling both together. No automatic arbitrary-shoreline detection is claimed.

OceanSurface is a flat 32-metre, 64-by-64 grid in the FBX; Unity owns its deformation, preventing double displacement. The Blender source has animated swell shape keys and a visual approximation of the shader. Keep adjoining tile edges aligned; the default world-space wave period is 32 metres. The water remains a presentation surface, not buoyancy physics.

## Editing and preview

Open `Preview/Harbor_ArtPreview.unity` or use **Open Waters > Art > Open modular harbor preview**. It is separate from Official_Game and is not added to build settings. Enter Play Mode to see automatic idle clips and water. Every scene element is an individual prefab instance.

Each `Sources~` directory contains a directly openable Blender file; the tilde prevents Unity from importing duplicate source assets. `_Production/Sources~/HarborWorkshop.blend` contains the whole workshop and assembled Blender preview. Existing Test.blend and Untitled.blend were preserved.

Shared/HarborPalette.png and HarborPalette.mat provide consistent colors. Flat faces sample palette swatches; preserve the PaletteUV layer when editing. Shader/material adjustments apply across the kit. Prefab variants are preferred for project-specific overrides. **Rebuild modular prefabs and preview** regenerates the authored base prefabs and preview, so do not use it to preserve custom edits to those generated files.

## Verification and limits

Unity 6000.3.23f1 imported 45 prefabs, 18 animation clips and two travel clips; material/reference checks passed and the water shader reported zero errors. `_Production/unity-validation.json`, `visual-validation.txt` and `Previews/Unity_Harbor_*.png` hold evidence. Full-kit source triangle count is approximately 81k; this is not a runtime performance measurement.

At the asset-only delivery, no gameplay wiring, sound production, player build, usability test or profiling had been completed. Gameplay integration now lives in `Assets/OpenWaters`; see `progression.md` and `docs/validation-summary.md` for current engine/build evidence and remaining acceptance gates. The original zero-byte Test.blend is unrelated pre-existing content. The Python files in _Production record authoring operations; native sources and exported prefabs are the deliverables, and scripts should be run only in a disposable Blender session after review.

### Socket fix verified

The apparent scale problem was a 90-degree orientation mismatch. The ship prefab now includes corrected *Attachment children beneath the original animated sockets. Unity checks at 0 and 1 seconds verify upright cargo, horizontal wake extending behind the stern, and unit attachment scale. Updated preview renders show the fix. All 45 prefab/FBX/source entries and importable-file metadata passed the inventory audit.

