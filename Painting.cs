using Canves.Core;

namespace Canves {
    static partial class Painting {
        static float t = 0;
        static Arrow arrow1 = new Arrow(new Vector2(100, 100), new Vector2(200, 200), 5, Color.Red);
        static Arrow arrow2 = new Arrow(new Vector2(0, 0), new Vector2(200, 200), 5, Color.Blue);
        static Font font = new Font("Arial", 24);
        static GText gText = new GText("", new Vector2(0, 0), font, Color.White);

        public static void Start() {
            // scene.Addchild(arrow1, arrow2);
        }

        public static void Update() {
            arrow1.RotateByTail(t);
            arrow2.RotateByTail(t);
            t += Time.deltaTime;
            gText.Text = t.ToString();
        }
        // static Font font = new Font("Arial", 16);
        // [Managed("Array")]
        // static Body[] bodies = new Body[550];
        // static float MAX = 0;
        // static GText gText = new GText("", new Vector2(0, 0), font, Color.White);
        // public static void Start() {
        //     for (int i = 0; i < bodies.Length; i++) {
        //         bodies[i] = new Body(new Vector2(RandF.FloRand(0, 2000), RandF.FloRand(0, 1200)), new Vector2(RandF.FloRandArray(-4, 4, 2)), i);
        //     }
        //     for (int i = 0; i < bodies.Length; i++) {
        //         if(bodies[i].mass > MAX){
        //             MAX = bodies[i].mass;
        //         }
        //     }
        // }
        // public static void Update() {
        //     for (int i = 0; i < bodies.Length; i++) {
        //         if (bodies[i].visal) {
        //             bodies[i].Move(bodies);
        //         }
        //     }
        //     for (int i = 0; i < bodies.Length; i++) {
        //         if(bodies[i].mass > MAX){
        //             bodies[i].colors[0] = Color.Red;
        //             MAX = bodies[i].mass;
        //             scene.Addchild(bodies[i], gText);
        //             gText.Text = MAX.ToString();
        //         }
        //     }
        // }
    }
}
