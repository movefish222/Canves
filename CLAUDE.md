# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this project actually is

Canves is a single-project .NET 6.0 **Windows Forms** desktop app. Despite the ambitious framework
described in `README.md` (animation engine, video compositor, ECS, plugin system), the real code is a
**2D N-body gravity simulation demo** built on a small scene-graph + reflection-based registration core.
Treat `README.md` as aspirational/inaccurate — it references types that do not exist (`CanvasObject`,
`RenderContext`, `IComponent`). The source files below are ground truth.

## Build & run

```bash
dotnet restore
dotnet build                                  # Debug; targets net6.0-windows
.\bin\Debug\net6.0-windows\Canves.exe         # run (Windows only, GUI app)
```

There is **no test suite** and no test framework configured. "Verification" means it builds with 0 errors
and the app launches; the simulation is observed visually (red body with a trailing white mass label).

Note: stale `obj/Debug/net10.0/` artifacts may exist from prior experiments — the authoritative target is
`net6.0-windows` per `Canves.csproj`. `Nullable` and `ImplicitUsings` are both enabled.

## Architecture (the big picture)

Runtime flow, all kicked off from the **Start button** (`Form1.button1_Click`):
1. `new Scene(comboBox)` becomes the scene-graph root and is assigned to `Painting.scene`.
2. `Painting.Start()` — seeds simulation state (550 `Body` objects in the `bodies` array).
3. `Painting._Start()` — reflection registration (see below); populates the scene tree.
4. Two threads spin up: **MUpdate** (calls `Painting.Update()` — physics) and **Draw** (calls
   `Painting.Draw()` — rendering). They run in tight `while(true)` loops with no synchronization.

### The `Painting` class is split across two files
`static partial class Painting` lives in **both** `Painting.cs` and `PaintingPartial.cs`. This naming is a
known trap — when editing "Painting", confirm which half you mean:
- `Painting.cs` — simulation state (`bodies`, `font`, `gText`) + `Start()` / `Update()` (physics loop).
- `PaintingPartial.cs` — `Draw()` + the reflection startup (`_Start`, `Register`, `IsManagedType`) and the
  private `gObjects` accumulator.

### Reflection-based registration (`PaintingPartial._Start`)
The core mechanism that turns static fields into scene objects. Single unified rule:
- Scan **all** static fields of `Painting` (public + non-public).
- A `GObject` (or any `IEnumerable` of them) is collected when **either** the field carries `[Managed]`
  **or** the value's *runtime type* carries `[Managed]` (`inherit:true`, so subclasses count). This is what
  enables base-class-typed fields to still register polymorphically.
- Critical invariant: `Register` skips `gObjects` itself via `ReferenceEquals` — `gObjects` is a static
  field too, so iterating it as an `IEnumerable` while adding to it would throw
  `InvalidOperationException: Collection was modified`.
- The `[Managed("...")]` string name is **informational only** — no longer used for logic (legacy from an
  old magic-string scheme; e.g. `[Managed("Array")]` on `bodies` is just a label).

To add a new renderable type: subclass `CanvObject`, override `Render(Graphics, Vector2)`, mark the class
`[Managed("Name")]`, then declare a static field for it in `Painting` (base-class-typed fields are fine).

### Scene graph & rendering
- `GObject` (`GObject.cs`) — base node: `id`, `parent`, `Children`, `position` (local coords).
- `CanvObject` (`CanvObject.cs`) — renderable `GObject`: `colors`, `visal` flag, virtual
  `Render(Graphics, Vector2 worldPosition)`.
- Concrete nodes in `Classes.cs`: `GText`, `Arrow`, `Body` (the gravity body, owns its own physics in
  `Body.Move`). `Spiry` is commented out.
- `Scene` (`Scens.cs`) extends `GObject` and is the root. `Scene.Render` walks the **entire subtree
  recursively** via `RenderNode`, accumulating world position = sum of every ancestor's `position`. This is
  why reparenting (`Scene.Addchild`) still renders correctly. There is no cycle protection — a circular
  parent/child link would `StackOverflow`.
- `MultiwayTree` (`Tree.cs`) is a thin tree helper; its `AddChild` reparents (removes from old parent first).
  Mutate hierarchy through `Scene.Add` / `Scene.Addchild`, **not** by setting `GObject.Parent` directly
  (the setter doesn't sync the `Children` list).

### Drawing layer
`Canves.Core` namespace (in `Vector.cs`) holds `Vector2` (with operator overloads), `RandF` (random
helpers), and `Plot` — a static GDI+ facade. Rendering uses **double buffering** via
`Plot.GetBufferedGraphics` / `Plot.Cla` (`BufferedGraphicsContext`). The per-frame clear uses
`Color.FromArgb(4, Color.Black)` — a near-transparent black, which produces the fading motion-trail effect
rather than fully erasing the frame.

### Two namespaces
- `Canves` — app, scene graph, concrete objects, `Painting`.
- `Canves.Core` — math/drawing primitives (`Vector2`, `RandF`, `Plot`).

## Known constraints / gotchas
- The **Abort button** (`Form1.button2_Click`) calls `Thread.Abort()`, which throws
  `PlatformNotSupportedException` on .NET 6 — stopping the simulation cleanly is not actually supported;
  closing the window is the reliable exit.
- The update and draw threads share mutable state (`bodies`, the scene tree) with no locking. Be careful
  introducing collection mutations on one thread that the other iterates.
- Source files use CRLF line endings and some have a UTF-8 BOM — match exact bytes when doing string-based
  edits.
