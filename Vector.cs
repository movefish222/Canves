﻿using System;
using System.Drawing;
using System.Collections.Generic;

namespace Canves.Core {
    public class Vector2 {
        public float x;
        public float y;
        public float Length {
            get { return (float)Math.Sqrt(x * x + y * y); }
        }
        public Vector2 Drection {
            get {
                if(Length != 0) {
                    return new Vector2(x / Length, y / Length);
                } else {
                    return Vector2.Zero();
                }
            }
        }
        public override string ToString() {
            return this.x.ToString() + " , " + this.y.ToString();
        }
        public Vector2(float x, float y) {
            this.x = x;
            this.y = y;
        }
        public Vector2(float[] f) {
            this.x = f[0];
            this.y = f[1];
        }
        public Vector2(List<float> f) {
            this.x = f[0];
            this.y = f[1];
        }
        public Vector2() {
            this.x = 0;
            this.y = 0;
        }
        public static Vector2 Zero() {
            return new Vector2(0, 0);
        }
        public float Dot(Vector2 v) {
            return x * v.x + y * v.y;
        }
        public static Vector2 operator+ (Vector2 v1, Vector2 v2) {
            Vector2 v = new Vector2();
            v.x = v1.x + v2.x;
            v.y = v1.y + v2.y;
            return v;
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2) {
            Vector2 v = new Vector2();
            v.x = v1.x - v2.x;
            v.y = v1.y - v2.y;
            return v;
        }
        public static Vector2 operator* (Vector2 v, float n) {
            Vector2 r = new Vector2();
            r.x = v.x * n;
            r.y = v.y * n;
            return r;
        }
        public static Vector2 operator *(float n, Vector2 v) {
            Vector2 r = new Vector2();
            r.x = v.x * n;
            r.y = v.y * n;
            return r;
        }
        public static Vector2 operator /(Vector2 v, float n) {
            Vector2 r = new Vector2();
            r.x = v.x / n;
            r.y = v.y / n;
            return r;
        }
    }

    public static class RandF {
        private static Random r = new Random();
        public static float FloRand(int a, int b) {
            int x = r.Next(a,b);
            return x + (float)r.NextDouble();
        }
        public static float[] FloRandArray(int a, int b, int length) {
            float[] result = new float[length];
           
            for (int i = 0; i < length; i++) {
                result[i] = r.Next(a, b) + (float)r.NextDouble();
            }
            return result;
        }
        public static List<float> FloRandList(int a, int b, int length) {
            List<float> result = new List<float>();
            
            for (int i = 0; i < length; i++) {
                result[i] = r.Next(a, b) + (float)r.NextDouble();
            }
            return result;
        }
    }

    public static class Plot {
        public static Form1 _mainForm;
        public static BufferedGraphicsContext bgc = new BufferedGraphicsContext();
        public static Graphics graphics;
        //
        //未使用双缓冲  逻辑未更新 慎用
        //
        public static void GDraw(Graphics graphics ,Vector2 vector, Color color, float width = 1) {
            SolidBrush brush = new SolidBrush(color);
            RectangleF r = new RectangleF(vector.x, vector.y, width, width);
            
            graphics.FillRectangle(brush, r);
        }
        public static void GDraw(Graphics graphics, Vector2[] vectors, Color color, float width) {
            SolidBrush brush = new SolidBrush(color);
            RectangleF[] rectangles = new RectangleF[vectors.Length];
            for (int i = 0; i < vectors.Length; i++) {
                rectangles[i] = new RectangleF(vectors[i].x, vectors[i].y, width, width);
            }
            
            graphics.FillRectangles(brush, rectangles);
        }
        public static void GDraw(Graphics graphics, List<Vector2> vectors, Color color, float width) {
            SolidBrush brush = new SolidBrush(color);
            RectangleF[] rectangles = new RectangleF[vectors.Count];
            for (int i = 0; i < vectors.Count; i++) {
                rectangles[i] = new RectangleF(vectors[i].x, vectors[i].y, width, width);
            }
            
            graphics.FillRectangles(brush, rectangles);
        }
        public static void GBall(Graphics graphics, Vector2 v, float size, Color color) {
            SolidBrush brush = new SolidBrush(color);
            RectangleF r = new RectangleF(v.x - size / 2, v.y - size / 2, size, size);
            
            graphics.FillEllipse(brush, r);
        }
        public static void GBalls(Graphics g, Vector2[] vs, float size, Color color) { 
            SolidBrush brush = new SolidBrush(color);
            RectangleF[] rectangleFs = new RectangleF[vs.Length];
            for (int i = 0; i < vs.Length; i++) {
                rectangleFs[i] = new RectangleF(vs[i].x - size / 2, vs[i].y - size / 2, size, size);
                g.FillEllipse(brush, rectangleFs[i]);
            }
        }
        public static void GText(Graphics graphics, string text, Font font, Color color, Vector2 postion) {
            SolidBrush brush = new SolidBrush(color);
            Font font1 = font;
            PointF p = new PointF(postion.x, postion.y);
            
            graphics.DrawString(text, font, brush, p);
        }
        public static void GCla(Bitmap bitmap, Graphics g) {
            g.DrawImage(bitmap, 0, 0);
            //bitmap.Dispose();
        }
        //
        //使用双缓冲
        //
        public static void Cla(BufferedGraphics display) { 
            display.Render(graphics);
            //display.Graphics.Dispose();
            display.Dispose();
        }
        public static void Ball(Graphics g, Vector2 v, float size, Color color) {
            SolidBrush brush = new SolidBrush(color);
            RectangleF r = new RectangleF(v.x - size / 2, v.y - size / 2, size, size);
            g.FillEllipse(brush, r);
        }
        public static void Balls(Graphics g, List<Vector2> vs, float size, Color color) {
            SolidBrush brush = new SolidBrush(color);
            Vector2[] mvs = vs.ToArray();
            RectangleF[] rectangleFs = new RectangleF[vs.Count];
            for (int i = 0; i < rectangleFs.Length; i++) {
                if(mvs[i] == null) continue;
                rectangleFs[i] = new RectangleF(mvs[i].x - size / 2, mvs[i].y - size / 2, size, size);
                g.FillEllipse(brush, rectangleFs[i]);
            }
        }
        public static BufferedGraphics GetBufferedGraphics(Color color) {
            BufferedGraphics display = bgc.Allocate(Plot._mainForm.CreateGraphics(),
                    new Rectangle(new Point(0, 0), Plot._mainForm.Size));
            display.Graphics.Clear(color);
            return display;
        }
        public static void Draw(Graphics g, Vector2 v, float width, Color color) {
            SolidBrush brush = new SolidBrush(color);
            RectangleF r = new RectangleF(v.x, v.y, width, width);
            g.FillRectangle(brush, r);
        }
        public static void Draw(Graphics g, List<Vector2> vs, float width, Color color) {
            SolidBrush brush = new SolidBrush(color);
            Vector2[] mvs = vs.ToArray();
            RectangleF[] rectangleFs = new RectangleF[mvs.Length];
            for (int i = 0; i < rectangleFs.Length; i++) {
                if(mvs[i] == null) continue;
                rectangleFs[i] = new RectangleF(mvs[i].x, mvs[i].y, width, width);
            }
            g.FillRectangles(brush, rectangleFs);
        }
        public static void Text(Graphics g, string text, Font font, Color color, Vector2 postion) {
            SolidBrush brush = new SolidBrush(color);
            Font font1 = font;
            PointF p = new PointF(postion.x, postion.y);
            g.DrawString(text, font, brush, p);
        }
    } 
}
