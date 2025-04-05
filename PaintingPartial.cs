using Canves.Core;
using System.Reflection;

namespace Canves {
    static partial class Painting{
        private static List<GObject> gObjects = new List<GObject>();
        public static Scene scene;
        static int frame = 0;
        static Font font = new Font("Arial", 16);
        public static void Draw() {
            BufferedGraphics bg = Plot.GetBufferedGraphics(Color.FromArgb(4,Color.Black));
            Graphics g1 = bg.Graphics;
            scene.Render(g1);
            Plot.Cla(bg);
        }
        public static void _Start() {
            Type t_Painting = typeof(Painting);
            Type t_scene = typeof(Scene);
            Type t_G = typeof(GObject);
            FieldInfo[] fields = t_Painting.GetFields(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Static
            );
            foreach (FieldInfo field in fields) {
                if(field.GetCustomAttribute<ManagedAttribute>() != null) {
                    ManagedAttribute attribute = field.GetCustomAttribute<ManagedAttribute>();
                    if(attribute.name == "Array") {
                        GObject[] array = (GObject[])field.GetValue(t_Painting);
                        foreach(GObject obj in array) {
                            if(obj != null) {
                                gObjects.Add(obj);
                            }
                        }
                    }
                }else{
                    if(field.GetType().GetCustomAttribute<ManagedAttribute>() != null){
                        gObjects.Add((GObject)field.GetValue(null));
                    }
                }
            }
            scene.Add(gObjects);
        }
    }
}