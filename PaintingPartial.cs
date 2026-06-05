using Canves.Core;
using System.Reflection;

namespace Canves {
    static partial class Painting{
        private static List<GObject> gObjects = new List<GObject>();
        public static Scene scene;
        static int frame = 0;
        public static void Draw() {
            BufferedGraphics bg = Plot.GetBufferedGraphics(Color.FromArgb(4,Color.Black));
            Graphics g1 = bg.Graphics;
            scene.Render(g1);
            Plot.Cla(bg);
        }
        public static void _Start() {
            FieldInfo[] fields = typeof(Painting).GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static
            );
            // 统一规则：遍历所有静态字段，取出其值（单对象或集合）。
            // 字段本身带 [Managed] 时无条件收集其中的 GObject；
            // 否则按“运行时类型带 [Managed]”收集（接受子类）。
            foreach (FieldInfo field in fields) {
                object? value = field.GetValue(null);
                if (value == null) {
                    continue;
                }
                bool fieldAnnotated = field.GetCustomAttribute<ManagedAttribute>() != null;
                Register(value, fieldAnnotated);
            }
            scene.Add(gObjects);
        }
        // 自动区分单对象与集合（数组 / List 等任意 IEnumerable），仅做收集
        private static void Register(object value, bool fieldAnnotated) {
            // 跳过 gObjects 自身，避免在遍历中修改集合
            if (ReferenceEquals(value, gObjects)) {
                return;
            }
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
            return Attribute.IsDefined(type, typeof(ManagedAttribute), inherit: true);
        }
    }
}