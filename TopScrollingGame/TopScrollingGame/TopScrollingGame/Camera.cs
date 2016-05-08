using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopScrollingGame
{
    public class Camera
    {
        protected Vector2 zoom;
        public Matrix transform;
        public Vector2 pos;
        public Vector2 zeroPos;
        protected float rotation;
        protected float ShakeAmount;
        protected float ShakeMax;
        protected int dir;
        private bool shaking;
        private int shakes;

        public Camera()
        {
            zoom = new Vector2(1.0f);
            rotation = 0.0f;
            pos = new Vector2(Main.width / 2, Main.height / 2);
            zeroPos = pos;
        }

        public Vector2 Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                if (zoom.X < 0.1f) zoom.X = 0.1f;
                if (zoom.Y < 0.1f) zoom.Y = 0.1f;
            }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Position
        {
            get { return pos - zeroPos; }

            set
            {
                pos = value;
            }
        }

        public void Move(Vector2 amount)
        {
            pos += amount;
        }

        public void ShakeCamera(float amount)
        {
            ShakeMax = amount;
            ShakeAmount = amount / 2;
            dir = 1;
            shaking = true;
            shakes = 0;
        }

        public void Update()
        {
            if (shaking)
            {
                Rotation += ShakeAmount * dir;
                if (dir == 1)
                {
                    if (Rotation >= ShakeMax)
                    {
                        dir = -1;
                        shakes++;
                    }
                }
                else
                {
                    if (Rotation <= -ShakeMax)
                    {
                        dir = 1;
                        shakes++;
                    }
                }
                if (shakes >= 4)
                {
                    Rotation = 0f;
                    shaking = false;
                }
            }
        }

        public void HorizontalZoom(float setZoom)
        {
            zoom.X += setZoom;
        }

        public void VerticalZoom(float setZoom)
        {
            zoom.Y += setZoom;
        }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            transform =
              Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom.X, Zoom.Y, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return transform;
        }
    }
}