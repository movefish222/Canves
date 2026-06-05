using Canves.Core;

namespace Canves {
    static partial class Painting {
        static Font font = new Font("Arial", 16);

        // === 多级参考系测试 ===
        static Gear sun = new Gear(new Vector2(0, 0), 24, 6f, Color.Yellow, true);
        static Arrow earth = new Arrow(new Vector2(30, 0), new Vector2(0, 0), 4, Color.Cyan);
        static GText moon = new GText("M", new Vector2(40, 0), font, Color.White);
        static Arrow satellite = new Arrow(new Vector2(15, 0), new Vector2(0, 0), 2, Color.Magenta);

        static GText label = new GText("SAT", new Vector2(0, -15), font, Color.LimeGreen);

        static float timer = 0f;
        static bool added = false;

        public static void Start() {
            label.visal = false;

            earth.position = new Vector2(120, 0);
            moon.position = new Vector2(50, 0);
            satellite.position = new Vector2(20, 0);

            scene.Addchild(sun, earth);
            scene.Addchild(earth, moon);
            scene.Addchild(moon, satellite);
        }

        public static void Update() {
            float dt = Time.deltaTime;
            timer += dt;

            sun.transform.Rotate(30f * dt);
            earth.transform.Rotate(60f * dt);
            moon.transform.Rotate(120f * dt);
            satellite.transform.Rotate(200f * dt);

            if (!added && timer > 3f) {
                added = true;
                label.visal = true;
                scene.Addchild(satellite, label);
            }
        }
    }
}
