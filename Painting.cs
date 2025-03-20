using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Canves.Core;

namespace Canves {
    static partial class Painting {
        // public static Body b0;
        // public static Body b1;
        // public static Body[] bs;
        // public static Arrow a;
        // public static void Start() {
        //     Body.dt = 0.001f;
        //     Body.tLength = 20000;
        //     b0 = new Body(new Vector2(800, 600), new Vector2(), 0, 3000);
        //     b1 = new Body(new Vector2(800,100), new Vector2(13, 0), 1, 100);
        //     a = new Arrow(b1.position, b1.position + b1.v*800, b1.v.length, Color.Red);
        //     bs = new Body[] { b0, b1 };
        //     scene.Add(bs);
        //     scene.Add(a);
        // }
        // public static void Update(){

        // }
        // public static void Draw() {
        //     frame++;
        //     a.Translate(b1.position - a.Tail);
        //     a.LookAt(b1.v * 5);
        //     a.Scale(b1.v.length / 4);
        //     b0.Move(bs);
        //     b1.Move(bs);

        //     if (frame == 200) { 
        //         frame = 0;
        //         BufferedGraphics display = Plot.GetBufferedGraphics(Color.Black);
        //         Graphics g1 = display.Graphics;
        //         scene.Render(g1);
        //         Plot.Text(g1, b1.v.length.ToString(), font, Color.Yellow, b1.position);
                    
                
        //         Plot.Cla(display);
        //     }
        // }
            static Body[] bodies = new Body[550];
            static float MAX = 0;
            public static void Start() {
                for (int i = 0; i < bodies.Length; i++) {
                    bodies[i] = new Body(new Vector2(RandF.FloRand(0, 2000), RandF.FloRand(0, 1200)), new Vector2(RandF.FloRandArray(-4, 4, 2)), i);
                }
                scene.Add(bodies);
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
                    }
                }
            }
        }
}
