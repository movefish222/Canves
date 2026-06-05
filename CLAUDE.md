# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this project is

Canves is a .NET 6.0 **Windows Forms** desktop app — a **2D scene-graph engine** with a transform
hierarchy (position/rotation/scale), reflection-based object registration, and GDI+ rendering. The current
demo is a multi-level orbital animation (sun→earth→moon→satellite) using involute gears, arrows, and text.

Treat `README.md` as aspirational — it references types that do not exist. Source files are ground truth.

## Build & run

```bash
dotnet restore
dotnet build                              # Debug; targets net6.0-windows
.\bin\Debug\net6.0-windows\Canves.exe     # run (Windows only, GUI app)
```

No test suite. Verification = builds with 0 errors + app launches + visual check.
Stale `obj/Debug/net10.0/` artifacts may exist — authoritative target is `net6.0-windows`.
`Nullable` and `ImplicitUsings` are both enabled.

## File layout

```
├── Core/                    # namespace Canves.Core — engine primitives
│   ├── GObject.cs           # Base scene node (id, parent, Children, transform)
│   ├── GTransform.cs        # Transform: position, rotation (degrees), scale
│   ├── Scens.cs             # Scene (root node, recursive RenderNode with transform propagation)
│   ├── Tree.cs              # MultiwayTree (full CRUD, cycle detection via IsAncestor)
│   ├── Vector.cs            # Vector2, RandF
│   ├── Plot.cs              # Static GDI+ facade (double-buffered rendering)
│   ├── Time.cs              # Stopwatch-based deltaTime/time (Unity-style)
│   └── ManagedAtrribute.cs  # [Managed] attribute for reflection registration
├── Classes/                 # namespace Canves — concrete renderable types
│   ├── Arrow.cs             # Arrow with degree-based RotateByTail/RotateByWaist
│   ├── Body.cs              # N-body gravity particle
│   ├── Gear.cs              # Involute gear (cached PointF[] profile)
│   ├── GText.cs             # Text label
│   ├── Spiry.cs             # Image sprite (extends Arrow)
│   └── Effect.cs            # Effect base
├── CanvObject.cs            # Renderable GObject base (namespace Canves.Core)
├── Painting.cs              # Demo scene + Update() loop (partial class, namespace Canves)
├── PaintingPartial.cs       # Draw() + reflection registration _Start() (partial class)
├── Form1.cs                 # WinForms threading setup (namespace Canves)
└── Program.cs               # Entry point
```

## Architecture

### Runtime flow (Start button → `Form1.button1_Click`)

1. `Scene` created with reference to comboBox, assigned to `Painting.scene`, centered on form.
2. `Painting._Start()` — reflection registration: scans all static fields of `Painting`, collects
   `GObject`s (or `IEnumerable<GObject>`) where the field or runtime type carries `[Managed]`.
3. `Painting.Start()` — builds scene hierarchy (parent-child relationships via `scene.Addchild`).
4. Two threads: **MUpdate** (`Time.Tick()` + `Painting.Update()`) and **Draw** (`Painting.Draw()`).
   Both run `while(true)` with `Thread.Sleep(1)`. No lock-based synchronization — thread safety
   relies on `Children.ToArray()` snapshots in `Scene.RenderNode`.

### `Painting` partial class (split across two files)

- `Painting.cs` — demo scene objects + `Start()` (hierarchy setup) + `Update()` (per-frame logic).
- `PaintingPartial.cs` — `Draw()` + reflection startup (`_Start`, `Register`, `IsManagedType`) +
  private `gObjects` accumulator.

### Transform hierarchy (`Scene.RenderNode`)

Each node has a `GTransform` (local position, rotation in degrees, uniform scale). During rendering,
`RenderNode` recursively accumulates world transform:
- `worldRot = parentRot + node.transform.rotation`
- `worldScale = parentScale * node.transform.scale`
- `worldPos = parentPos + Rotate(node.position * parentScale, parentRot)`

This gives correct multi-level reference frames (child orbits parent).

### Reflection-based registration (`PaintingPartial._Start`)

- Scans all static fields (public + non-public) of `Painting`.
- Collects `GObject` values where the field carries `[Managed]` OR the runtime type does (inherit:true).
- Uses `Attribute.IsDefined(...)` (not `GetCustomAttribute`) to avoid `AmbiguousMatchException`
  when subclasses inherit `[Managed]`.
- Skips `gObjects` itself via `ReferenceEquals` to prevent collection-modified exceptions.
- `[Managed("...")]` string is informational only.

### Adding new renderable types

1. Subclass `CanvObject`, override `Render(Graphics g, Vector2 position, float rotation, float scale)`.
2. Mark the class `[Managed("Name")]`.
3. Declare a static field in `Painting` (base-class-typed fields work via runtime type checking).

### Two namespaces

- `Canves.Core` — engine: GObject, GTransform, CanvObject, Scene, MultiwayTree, Vector2, Plot, Time.
- `Canves` — app layer: Painting, Form1, concrete classes (Arrow, Body, Gear, GText, Spiry, Effect).

### Drawing layer

`Plot` is a static GDI+ facade. Rendering uses **double buffering** (`BufferedGraphicsContext`).
Per-frame clear uses `Color.FromArgb(4, Color.Black)` — near-transparent black producing a fading
motion-trail effect rather than fully erasing each frame.

## Known constraints / gotchas

- **Thread safety**: Update and Draw threads share mutable state with no locking. Use `ToArray()`
  snapshots when iterating `Children`. Be careful with collection mutations across threads.
- **Abort button** calls `Thread.Abort()` — throws `PlatformNotSupportedException` on .NET 6.
  Closing the window is the only reliable exit.
- **GTransform.Rotate** uses `% 360f` to prevent float drift on long-running rotations.
- **Hierarchy mutation**: Always use `scene.Addchild(parent, child)` or `scene.Add(obj)`.
  Do not set `GObject.Parent` directly — it doesn't sync the `Children` list.
- **No cycle protection in render** — circular parent/child would `StackOverflow`.
  `MultiwayTree.AddChild` checks `IsAncestor` to prevent cycles at insertion time.
- Source files use CRLF line endings; some have UTF-8 BOM.
