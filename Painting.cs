using Canves.Core;

namespace Canves {
    static partial class Painting {
        static Font font = new Font("Arial", 16);

        // === 多级参考系测试 ===
        static Gear sun = new Gear(new Vector2(0, 0), 50, 6f, Color.Yellow, true) { name = "sun" };
        static Gear earth = new Gear(new Vector2(30, 0), 24, 6f, Color.Cyan, true) { name = "earth" };
        static Gear moon = new Gear(new Vector2(40, 0), 17, 6f, Color.White, true) { name = "moon" };
        static GText label = new GText("SAT", new Vector2(0, -15), font, Color.LimeGreen) { name = "label" };
        static GText fps = new GText("FPS: 0", new Vector2(0, 0), font, Color.Black) { name = "fps" };

        static float timer = 0f;
        static bool added = false;

        public static void Start() {
            label.visal = false;

            earth.position = new Vector2(300, 0);
            moon.position = new Vector2(130, 0);
            scene.Addchild(sun, earth);
            scene.Addchild(earth, moon);
        }

        public static void Update() {
            float dt = Time.deltaTime;
            timer += dt;

            sun.transform.Rotate(30f * dt);
            earth.transform.Rotate(60f * dt);
            moon.transform.Rotate(120f * dt);

            fps.Text = $"FPS: {1f / dt:0.0}";

            if (!added && timer > 3f) {
                added = true;
                label.visal = true;
                scene.Addchild(moon, label);
            }
            if (added && timer > 6f) {
                label.transform.Translate(new Vector2(10f * dt, 0));
            }
            if(added && timer > 9f) {
                sun.transform.scale = (float)Math.Sin(timer) * 0.3f + 1f;
            }
        }
    }
}
