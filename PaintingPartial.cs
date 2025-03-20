using Canves.Core;

namespace Canves {
    static partial class Painting{
        public static Scene scene;
        static int frame = 0;
        static Font font = new Font("Arial", 16);
        public static void Draw() {
            BufferedGraphics bg = Plot.GetBufferedGraphics(Color.FromArgb(4,Color.Black));
            Graphics g1 = bg.Graphics;
            scene.Render(g1);
            Plot.Cla(bg);
        }
    }
}