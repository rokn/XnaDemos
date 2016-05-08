using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG
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
            pos = new Vector2(Main.WindowWidth / 2, Main.WindowHeight / 2);
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
                if (Main.InEditMode)
                {
                    pos.X = MathHelper.Clamp(pos.X, zeroPos.X, zeroPos.X + (Main.currentLevel.tilemap.Width * TileSet.tileWidth) - Main.WindowWidth + TileSet.SpriteSheet[Editor.currTileset].Width);
                    pos.Y = MathHelper.Clamp(pos.Y, zeroPos.Y, zeroPos.Y + (Main.currentLevel.tilemap.Height * TileSet.tileHeight) - Main.WindowHeight);
                }
                else
                {
                    pos.X = MathHelper.Clamp(pos.X, zeroPos.X, zeroPos.X + (Main.currentLevel.tilemap.Width * TileSet.tileWidth) - Main.WindowWidth);
                    pos.Y = MathHelper.Clamp(pos.Y, zeroPos.Y, zeroPos.Y + (Main.currentLevel.tilemap.Height * TileSet.tileHeight) - Main.WindowHeight);
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

        public Matrix GetTransformation()
        {
            transform =
              Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom.X, Zoom.Y, 1)) *
                                         Matrix.CreateTranslation(new Vector3(Main.WindowWidth * 0.5f, Main.WindowHeight * 0.5f, 0));
            return transform;
        }

        public Vector2 GetRealMousePosition()
        {
            return MyMouse.Position + (Position);
        }

        public void FollowTarget(Vector2 target)
        {
            float posX = Main.camera.zeroPos.X + (target.X - (Main.WindowWidth / 2));
            float posY = Main.camera.zeroPos.Y + (target.Y - (Main.WindowHeight / 2));
            Main.camera.Position = new Vector2(posX, posY);
        }
    }
}