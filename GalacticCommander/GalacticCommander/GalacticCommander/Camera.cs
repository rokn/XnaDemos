using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GalacticCommander
{
    public class Camera
    {
        protected Vector2 zoom;
        public Matrix transform;
        public Vector2 pos;
        public Vector2 zeroPos;
        protected float rotation;

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

        public void Move(Vector2 amount)
        {
            pos += amount;
        }

        public Vector2 Position
        {
            get { return pos - zeroPos; }

            set
            {
                pos = value;
                //pos.X = (float)Math.Round(value.X);
                //pos.Y = (float)Math.Round(value.Y);
                //pos.X = (float)Math.Round(MathHelper.Clamp(pos.X, zeroPos.X, zeroPos.X + (Main.tilemap.Width * TileSet.TileWidth) - Main.width));
                //pos.Y = (float)Math.Round(MathHelper.Clamp(pos.Y, zeroPos.Y, zeroPos.Y + (Main.tilemap.Height * TileSet.TileHeight) - Main.height));
            }
        }

        public void FollowTarget(Vector2 position)
        {
            Position = position;
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

        public Vector2 GetRealMousePosition()
        {
            return Main.mouse.Position + (Position);
        }
    }
}