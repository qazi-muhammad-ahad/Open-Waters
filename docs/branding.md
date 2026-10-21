# One logo for Open Waters

Replace this single authoring file:

`Assets/OpenWaters/Resources/Branding/Logo.png`

Use a square PNG, ideally 512×512 or 1024×1024, with transparency if desired. Keep its existing `.meta` file when replacing the image so its GUID stays stable. The included sailing emblem is an original placeholder approved by the user.

Unity imports the PNG as a sprite and refreshes default application icon settings automatically. The menu, HUD and credits load that same resource. Windows builds embed it as the executable icon; the default engine splash is disabled. The custom WebGL loading screen and favicon receive a copy of the same source at build time. There is no second logo file to maintain in source.

After replacing the PNG, rebuild existing packages to update their embedded UI, executable icon and browser files. An already-installed executable cannot update its embedded icon without a new build. The editor menu **Open Waters > Branding > Apply logo and UI icons** reapplies configuration; it preserves existing PNG replacements.

Function icons live separately under `Assets/OpenWaters/Resources/UI/Icons/`. They use a consistent pictogram style and remain accompanied by readable labels. Their generator is `Assets/OpenWaters/Editor/BrandingBuilder.cs`; runtime lookup is `UIIcons`.

The future game-website repository is not connected yet. When that deployment is implemented, its build pipeline should copy this canonical logo along with the WebGL package. No external repository or hosting changes have been made.
