﻿using Canves.Core;

namespace Canves {
    static partial class Painting {
        static Font font = new Font("Arial", 16);
        [Managed("Array")]
        static Body[] bodies = new Body[550];
        static float MAX = 0;
        static GText gText = new GText("", new Vector2(0, 0), font, Color.White);
        public static void Start() {
            for (int i = 0; i < bodies.Length; i++) {
                bodies[i] = new Body(new Vector2(RandF.FloRand(0, 2000), RandF.FloRand(0, 1200)), new Vector2(RandF.FloRandArray(-4, 4, 2)), i);
            }
            for (int i = 0; i < bodies.Length; i++) {
                if(bodies[i].mass > MAX){
                    MAX = bodies[i].mass;
                }
            }
        }
        public static void Update() {
            for (int i = 0; i < bodies.Length; i++) {
                if (bodies[i].visal) {
                    bodies[i].Move(bodies);
                }
            }
            for (int i = 0; i < bodies.Length; i++) {
                if(bodies[i].mass > MAX){
                    bodies[i].colors[0] = Color.Red;
                    MAX = bodies[i].mass;
                    scene.Addchild(bodies[i], gText);
                    gText.Text = MAX.ToString();
                }
            }
        }
    }
}
