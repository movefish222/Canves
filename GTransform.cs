using System;

namespace Canves.Core {
    public class GTransform {
        public Vector2 position;
        public float rotation;
        public float scale;

        public GTransform() {
            position = new Vector2();
            rotation = 0f;
            scale = 1f;
        }

        public GTransform(Vector2 pos, float rotation = 0f, float scale = 1f) {
            this.position = pos;
            this.rotation = rotation;
            this.scale = scale;
        }

        public void Translate(Vector2 delta) {
            position = position + delta;
        }

        public void Rotate(float degrees) {
            rotation += degrees;
            rotation %= 360f;
            if (rotation < 0) rotation += 360f;
        }

        public void ScaleBy(float factor) {
            scale *= factor;
        }
    }
}
