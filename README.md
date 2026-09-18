# ER-Inspector

**ER-Inspector** is a Unity Editor plugin for visually designing the Inspector layout of scripts (`MonoBehaviour` & `ScriptableObject`) via drag-and-drop — without needing to write a manual `CustomEditor` class for each script.

Layout configurations are built through a dedicated window, saved as a per-target-type asset, and automatically applied to Unity's built-in Inspector for every script of that type.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![openupm](https://img.shields.io/npm/v/com.edenrompas.er-inspector?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.edenrompas.er-inspector/)
[![CodeFactor](https://www.codefactor.io/repository/github/edenrompas/er-inspector/badge)](https://www.codefactor.io/repository/github/edenrompas/er-inspector)

## Key Features

- **Field Configuration Window** (`Tools > Field Configuration`) — a drag-and-drop window for arranging inspector layouts.
- **Supported layout elements**: Field (automatic), Title, Info Box, Vertical Group, Horizontal Group (2 columns: left & right), Foldout, Tab Group.
- **Auto-applies** to all `MonoBehaviour` and `ScriptableObject` types via `CustomEditor(..., true)` — no need to create a separate Editor class per script.
- **Unplaced fields** automatically remain visible at the root of the layout, and any field not placed anywhere is still drawn after the custom layout (no field ever silently disappears).
- **The ✕ button** on a field doesn't delete it permanently — it will automatically reappear at the root on the next refresh (see `EnsureDefaultFieldsPresent`).
- **Extensible, reflection-based drawer system**: adding a new layout element type only requires 1 new class + the `[LayoutNodeDrawer(typeof(X))]` attribute, and it's auto-registered without touching the window or the editor (`LayoutNodeDrawerRegistry`).
- **Automatic persistence**: configuration is saved as an `InspectorLayoutConfig` (ScriptableObject) at `Assets/Resources/InspectorDesigner/<ScriptName>.asset`, and loaded via `Resources.Load` when the Inspector is drawn — so it works both in the Editor and in builds.

## Screenshots

### Field Configuration Window
![Field Configuration Window](images/images-2)
*The window used to pick a target script, add layout elements (Title, Group, Foldout, Tab Group), and reorder fields via drag & drop.*

### Resulting Inspector
![Generated Inspector](images/images-1)
*Unity's built-in Inspector automatically following the designed layout — no extra `CustomEditor` code required.*

> **Note:** the two image paths above are placeholders. Replace them with actual screenshots of your window, save them under `docs/images/`, or adjust the paths to match your repo structure.

## Installation

1. Copy the `Assets/Plugins/ER-Inspector` folder into your Unity project.
2. No additional setup is required — the `Assets/Resources/InspectorDesigner/` folder is created automatically the first time a script is configured.
3. Open the **Tools > Field Configuration** menu in the Unity Editor.

## Usage

1. Open **Tools > Field Configuration**.
2. Select a GameObject with the component you want to configure, or set it manually via the **Target Script** field. Check **Lock** to keep the window from switching targets when the Hierarchy selection changes.
3. All fields that qualify as "inspectable" automatically appear as nodes at the root.
4. Click **Add Attribute** to insert: Title, Info Box, Vertical Group, Horizontal Group, Foldout, or Tab Group.
5. Drag a node (using the `≡` handle) to move a field/group to a different position or group.
6. Click **Refresh** to manually reload the panel if needed.
7. Open the Inspector for a GameObject with the same script — the layout will be rendered automatically based on the saved configuration.

## Field Eligibility Rules

A field is considered *inspectable* (see `FieldReflectionUtility.GetInspectableFieldNames`) if:

- it is `public`, **and** does not have `[HideInInspector]`, **and** does not have `[NonSerialized]`; **or**
- it is non-public **and** has the `[SerializeField]` attribute.

## Folder Structure

| Folder | Contents |
|---|---|
| `Data/` | `LayoutNode.cs` — all layout node types (Field, Title, InfoBox, VerticalGroup, HorizontalGroup, Foldout, TabGroup); `InspectorLayoutConfig.cs` — the ScriptableObject that stores the design per target type |
| `Drawers/` | One drawer class per node type (implementing `ILayoutNodeDrawer`), `LayoutNodeDrawerRegistry` (reflection-based auto-discovery), `GroupBoundsStack` (helper so child elements follow the width of their enclosing group) |
| `Editors/` | `InspectorEditor.cs` — `ConfigurableMonoBehaviourEditor` & `ConfigurableScriptableObjectEditor`, which replace the default Inspector whenever a saved configuration exists |
| `Window/` | `FieldConfigurationWindow.cs` — a UI Toolkit-based drag-and-drop UI for designing the layout |
| `Utility/` | `FieldReflectionUtility` (detects inspectable fields), `LayoutTreeUtility` (tree traversal for nodes), `ConfigAssetUtility` (load/create/save the config asset) |

## Adding a New Layout Element Type

1. Add a new subclass of `LayoutNode` in `Data/LayoutNode.cs`.
2. Create a new class in `Drawers/` that implements `ILayoutNodeDrawer`, tagged with `[LayoutNodeDrawer(typeof(YourNewNodeType))]`.
3. Done — the new node will automatically appear in the **Add Attribute** menu (as long as `MenuLabel` isn't `null`), with no changes needed to `FieldConfigurationWindow` or `InspectorEditor`.

## Known Limitations

- No explicit Undo/Redo support for drag-and-drop actions or node removal within the window.
- Layout is stored **per script type**, not per instance/object — every object of the same type shares one layout.
- `Horizontal Group` is limited to exactly 2 columns (left/right) for layout consistency.
- Configuration assets live under the `Resources` folder, so they will be included in builds even though they're primarily used in the Editor — worth keeping in mind if build size is a concern.
- There's no UI validation preventing users from creating very deep/complex group structures, which could affect IMGUI rendering performance.

## Requirements

- Unity with UI Toolkit/UIElements support for Editor windows (Unity 2020.3 LTS or later is recommended — adjust to whatever minimum version you've actually tested against).

## License

Add your project's license here (e.g. MIT, proprietary/internal, etc.).
