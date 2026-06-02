# Painting 反射启动修复说明

## 修复前的问题

原 `PaintingPartial.cs` 的 `_Start()` 方法存在以下缺陷：

### 1. **两套割裂的发现规则**
- **数组路径**：字段必须标注 `[Managed("Array")]`，靠魔法字符串 `"Array"` 判断
- **单对象路径**：字段**不能**标注，且 `field.FieldType` 必须**精确等于**带 `[Managed]` 的具体类型

结果：
- 写 `[Managed("Text")] static GText gText = ...` → 被静默丢弃（有注解但 name 不是 "Array"）
- 写 `static CanvObject obj = new Arrow(...)` → 永远匹配不上（FieldType 是 `CanvObject`，不在 gTypes 里）

### 2. **不支持多态**
`if(field.FieldType == t)` 要求声明类型精确相等，接受不了：
```csharp
static GObject   shape = new Arrow(...);   // FieldType = GObject ✗
static CanvObject body = new Body(...);     // FieldType = CanvObject ✗
```

### 3. **只渲染一层**
`Scene.Render` 只遍历 `Children[0]`，不递归。一旦 `Update()` 把 `gText` 重新挂到某个 `Body` 下（`scene.Addchild(bodies[i], gText)`），`gText` 就从场景直接子节点消失，不再渲染。

---

## 修复后的实现

### `PaintingPartial.cs::_Start()` — 统一的反射规则

```csharp
public static void _Start() {
    FieldInfo[] fields = typeof(Painting).GetFields(
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
    );
    // 统一规则：遍历所有静态字段，取出其值（单对象或集合）。
    // 字段本身带 [Managed] 时无条件收集其中的 GObject；
    // 否则按"运行时类型带 [Managed]"收集（接受子类）。
    foreach (FieldInfo field in fields) {
        object? value = field.GetValue(null);
        if (value == null) continue;
        
        bool fieldAnnotated = field.GetCustomAttribute<ManagedAttribute>() != null;
        Register(value, fieldAnnotated);
    }
    scene.Add(gObjects);
}

// 自动区分单对象与集合（数组 / List 等任意 IEnumerable）
private static void Register(object value, bool fieldAnnotated) {
    if (value is GObject obj) {
        if (fieldAnnotated || IsManagedType(obj.GetType())) {
            gObjects.Add(obj);
        }
    } else if (value is System.Collections.IEnumerable sequence) {
        foreach (object? item in sequence) {
            if (item is GObject g && (fieldAnnotated || IsManagedType(g.GetType()))) {
                gObjects.Add(g);
            }
        }
    }
}

// 用 [Managed] 判断类型是否受管理；inherit:true 让带注解基类的子类也算
private static bool IsManagedType(Type type) {
    return type.GetCustomAttribute<ManagedAttribute>(inherit: true) != null;
}
```

### `Scens.cs::Render()` — 递归渲染整棵子树

```csharp
public void Render(Graphics g){
    // 从场景根递归渲染整棵子树，世界坐标 = 各级父节点 position 之和。
    // 这样即便对象被 Addchild 重新挂到别的节点下，依旧会被绘制。
    foreach(GObject child in Children){
        RenderNode(g, child, position);
    }
}

private void RenderNode(Graphics g, GObject node, Vector2 parentWorld){
    Vector2 world = node.position + parentWorld;
    if(node is CanvObject co && co.visal){
        co.Render(g, world);
    }
    foreach(GObject child in node.Children){
        RenderNode(g, child, world);  // 递归子节点
    }
}
```

---

## 修复后的优势

### ✅ 统一的注册规则
- **字段带 `[Managed]`** → 无条件收集其中所有 `GObject`（单个或集合）
- **字段不带注解** → 检查对象**运行时类型**是否带 `[Managed]`
- 不再有"数组必须标 Array、单对象必须不标"的割裂规则

### ✅ 支持多态
```csharp
static GObject   shape = new Arrow(...);      // ✓ 运行时是 Arrow（带 [Managed]）
static CanvObject body = new Body(...);        // ✓ 运行时是 Body（带 [Managed]）
[Managed("Shapes")]
static CanvObject[] mixed = { new Arrow(...), new GText(...) };  // ✓ 字段注解覆盖全部
```

### ✅ 自动区分数组/单对象
- 用 `is IEnumerable` 判断，不依赖魔法字符串 `"Array"`
- 支持 `GObject[]`、`List<GObject>`、`IEnumerable<GObject>` 等任意集合

### ✅ 递归渲染子树
- `gText` 被 `Addchild` 挂到 `Body` 下依然能渲染
- 世界坐标正确累加（父 + 祖父 + ... + 自身）

### ✅ 构建通过
```
已成功生成。
    11 个警告
    0 个错误
```
警告从 16 降到 11（消除了旧代码中的 5 个空引用警告）。

---

## 使用示例

### 示例 1：标注字段（推荐）
```csharp
[Managed("Bodies")]
static Body[] bodies = new Body[550];

[Managed("UI")]
static GText title = new GText("Title", new Vector2(10, 10), font, Color.White);
```
→ `bodies` 全部 550 个元素 + `title` 都会被注册。

### 示例 2：不标注字段（靠类型判断）
```csharp
static Arrow arrow = new Arrow(...);   // Arrow 类带 [Managed("Arrow")]
static GText text  = new GText(...);   // GText 类带 [Managed("Text")]
```
→ 运行时检测到 `arrow`/`text` 是带注解类型，自动注册。

### 示例 3：混合基类声明（现在也能工作）
```csharp
static CanvObject obj1 = new Body(...);    // ✓ 运行时是 Body
static GObject    obj2 = new Arrow(...);   // ✓ 运行时是 Arrow
```
→ 不再要求声明类型精确等于带注解的具体类。

---

## 文件变更摘要
- **PaintingPartial.cs:15-52** → 重写 `_Start()` + 新增 `Register()` / `IsManagedType()` 辅助方法
- **Scens.cs:18-33** → 重写 `Render()` + 新增 `RenderNode()` 递归方法
- **构建结果**：0 error, 11 warnings（警告数减少 5 个）

修复完成。反射启动现在可以正确实现自动对象管理，支持多态、支持任意集合、递归渲染子树。
