using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Canves.Core;

namespace Canves {
    [Managed("Text")]
    class GText : CanvObject  {
        public string Text{get; set;}
        private Font font;
        public GText(string text, Vector2 position, Font font, Color color) {
            Text = text; this.position = position; this.font = font; this.colors.Add(color);
        }
        public GText(string text, Vector2 position, Font font, Color color, int _id) {
            Text = text; this.position = position; this.font = font; this.colors.Add(color);
            id = _id;
        }
        public override void Render(Graphics g, Vector2 position) {
            g.DrawString(Text, font, new SolidBrush(colors[0]), position.x, position.y); 
        }
    }
    [Managed("Arrow")]
    class Arrow : CanvObject {
        private Vector2 head;
        private Vector2 vector;
        public float scale;
        public float Length {
            get {
                return vector.Length;
            }
        }
        public Vector2 Head => head;
        public Vector2 Tail => position;
        public Arrow(Vector2 head, Vector2 position, float scale, Color color) {
            this.head = head; this.position = position; this.scale = scale;
            this.vector = head - position; this.colors.Add(color);
        }
        //
        //渲染
        //
        public override void Render(Graphics g, Vector2 position) {
            Pen pen = new Pen(colors[0]);
            pen.Width = scale;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            PointF pf1 = new PointF(position.x, position.y);
            PointF pf2 = new PointF(head.x, head.y);

            g.DrawLine(pen, pf1, pf2);
        }
        //
        //缩放与位移
        //
        public void Scale(float sx, float sy = 1) {
            scale = sx;
            vector *= sy;
            head = position + vector;
        }

        public void Translate(Vector2 vector) {
            position += vector;
            head += vector;
        }
        public void LookAt(Vector2 vector) {
            this.vector = vector;
            head = position + vector;
        }
        public void RotateByTail(float angle) { 
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float x0 = vector.x;
            float y0 = vector.y;

            vector.x = x0 * cos - y0 * sin;
            vector.y = x0 * sin + y0 * cos;

            head = position + vector;
        }
        public void RotateBywaist(float angle) {
            Vector2 p0 = position + vector / 2;

            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float x0 = vector.x;
            float y0 = vector.y;

            vector.x = x0 * cos - y0 * sin;
            vector.y = x0 * sin + y0 * cos;

            Vector2 p1 = position + vector / 2;
            position += p0 - p1;
            head = position + vector;
        }
    }

    [Managed("Body")]
    class Body : CanvObject{

        public List<Vector2> track;
        public Vector2 v;
        public float mass;
        public float size;
        public static float dt = 0.01f;
        public static int tLength = 10000;
        public Body(Vector2 pos, Vector2 v, int _id) {
            this.position = pos;
            this.v = v;
            mass = RandF.FloRand(0, 2);
            //this.mass = mass;
            size = (float)Math.Sqrt(mass);
            id = _id;
            track = new List<Vector2>();
            colors.Add(Color.White);
            colors.Add(Color.Yellow);
        }
        public Body(Vector2 pos, Vector2 v, int id, int mass) {
            this.position = pos;
            this.v = v;
            //mass = RandF.FloRand(0, 2);
            this.mass = mass;
            size = (float)Math.Sqrt(mass);
            this.id = id;
            track = new List<Vector2>();
            colors.Add(Color.White);
            colors.Add(Color.Yellow);
        }

        public void Move(Body[] bodies) {
            Vector2 a = new Vector2();
            foreach (var i in bodies) {
                if(i == this || !i.visal) {
                    continue;
                }
                Vector2 d = i.position - position;
                float Length = d.Length;
                if (Length <= i.size + size) {
                    if (i.mass <= mass) {
                        v = (i.mass *i.v+mass*v) / (i.mass + mass);
                        mass += i.mass;
                        size = (float)Math.Sqrt(mass);
                        i.visal = false;
                    }
                }
                a += i.mass * 100 * d / (Length * Length * Length);
            }
            position += v * dt + a * dt * dt / 2;
            v += a * dt;
            track.Add(position);
            if (track.Count > tLength) { 
                track.RemoveAt(0);
            }
        }
        public override void Render(Graphics g, Vector2 position) {
            Plot.Draw(g, track, 1, colors[1]);
            Plot.Ball(g, position, 2 * size, colors[0]);
        }
    }
    // [Managed("Spiry")]
    // class Spiry : Arrow {
    //     string resourcePath;
    //     public Spiry(string path, Vector2 position, Rectangle size) :
    //         base(position + new Vector2(size.X, 0), position, 1, Color.White)
    //     {
    //         resourcePath = path;
    //     }
    //     public override void Render(Graphics g, Vector2 position) {
    //         //TODO
    //         Bitmap bitmap = new Bitmap(resourcePath);
    //         g.DrawImage(bitmap, position.x - bitmap.Width / 2, position.y - bitmap.Height / 2, bitmap.Width, bitmap.Height);
    //     }
    // }
}
