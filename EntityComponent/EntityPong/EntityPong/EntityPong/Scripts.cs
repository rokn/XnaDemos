using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Artemis.System;

namespace EntityPong
{
    public static class Scripts
    {
        public static Texture2D LoadTexture(string asset)
        {
            try
            {
                ContentManager Content = EntitySystem.BlackBoard.GetEntry("Content") as ContentManager;
                return Content.Load<Texture2D>(asset) as Texture2D;
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("Texture not found: " + asset);
                return null;
            }
        }

        public static bool CheckIfMouseIsOver(Rectangle rect)
        {
            if (rect.Contains(MyMouse.Position))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool KeyIsPressed(Keys key)
        {
            return MyKeyboard.JustPressed(key) || MyKeyboard.IsHeld(key);
        }

        public static bool KeyIsReleased(Keys key)
        {
            return MyKeyboard.IsReleased(key) || (!MyKeyboard.JustPressed(key) && !MyKeyboard.IsHeld(key));
        }

        public static void ForEach<T>(this IEnumerable<T> collection,Action<T> action)
        {

            foreach (var item in collection)
            {
                action(item);
            }

        }

        public static Rectangle InitRectangle(Vector2 Position, int Width,int Height)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        

        //public static bool CheckPixelPerfectCollision(Texture2D texture,Rectangle rect,Texture2D destructionTexture, Rectangle destructionnRectangle)
        //{
        //    Rectangle intersectRect = MathAid.GetIntersectingRectangle(destructionnRectangle, rect);

        //    Color[] colorData = new Color[texture.Width * texture.Height];
        //    texture.GetData(colorData);

        //    Color[] destructionTextureData = new Color[destructionTexture.Width * destructionTexture.Height];
        //    destructionTexture.GetData(destructionTextureData);

        //    Point startPos1 = new Point(intersectRect.X - destructionnRectangle.X, intersectRect.Y - destructionnRectangle.Y);
        //    Point startPos2 = new Point(intersectRect.X - rect.X, intersectRect.Y - rect.Y);

        //    for (int x = 0; x < intersectRect.Width; x++)
        //    {

        //        for (int y = 0; y < intersectRect.Height; y++)
        //        {

        //            if (destructionTextureData[(startPos1.X + x) + (startPos1.Y + y) * destructionTexture.Width].A != 0)
        //            {
        //                int X = startPos2.X + x;
        //                int Y = startPos2.Y + y;
        //                if (colorData[X + Y * texture.Width].A != 0)
        //                {
        //                    return true;
        //                }
        //            }

        //        }

        //    }
        //    return false;
        //}

        public static bool Contains(this Rectangle rect, Vector2 point)
        {
            float left = rect.X;
            float top = rect.Y;
            float right = rect.Right;
            float bottom = rect.Bottom;

            if (point.X > left && point.X < right && point.Y > top && point.Y < bottom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Rectangle> GetSourceRectangles(int startingId, int endId, int rectWidth, int rectHeight, Texture2D texture)
        {
            int rectsPerRow = texture.Width / rectWidth;
            List<Rectangle> result = new List<Rectangle>();

            for (int i = startingId; i <= endId; i++)
            {
                int sourceY = i / rectsPerRow;
                int sourceX = i - sourceY * rectsPerRow;
                result.Add(new Rectangle(sourceX * rectWidth, sourceY * rectHeight, rectWidth, rectHeight));
            }

            return result;
        }

        //public static Dictionary<Direction, Animation> LoadEntityWalkAnimation(Texture2D spriteSheet)
        //{
        //    int frameWidth = spriteSheet.Width / 4;
        //    int frameHeight = spriteSheet.Height / 4;

        //    Dictionary<Direction, Animation> walkingAnimation = new Dictionary<Direction, Animation>();
        //    List<Rectangle> Down = Scripts.GetSourceRectangles(0, 3, frameWidth, frameHeight, spriteSheet);
        //    Animation animation = new Animation(Down, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Down, animation);

        //    List<Rectangle> Left = Scripts.GetSourceRectangles(4, 7, frameWidth, frameHeight, spriteSheet);
        //    animation = new Animation(Left, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Left, animation);

        //    List<Rectangle> Right = Scripts.GetSourceRectangles(8, 11, frameWidth, frameHeight, spriteSheet);
        //    animation = new Animation(Right, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Right, animation);

        //    List<Rectangle> Up = Scripts.GetSourceRectangles(12, 15, frameWidth, frameHeight, spriteSheet);
        //    animation = new Animation(Up, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Up, animation);

        //    return walkingAnimation;
        //}        

        public static void VoidRemove<T>(this List<T> list, T item)
        {
            list.Remove(item);
        }

        public static Rectangle GetWalkingRect(Vector2 Position, int Width, int Height)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height - 24);
        }

        //internal static Vector2 GetWalkingOrigin(int EntityWidth, int EntityHeight)
        //{
        //    return new Vector2(EntityWidth / 2, EntityHeight / 2 - Math.Min(TileSet.tileHeight, EntityHeight / 2));
        //}

        //public static Texture2D GenerateWhitePixelTexture()
        //{
        //    Texture2D texture = new Texture2D(Main.GetGraphicsDevice(),1,1);

        //    Color[] data = { Color.White };
        //    texture.SetData(data);

        //    return texture;
        //}

        public static string GetMyIPAdress()
        {
            //WebRequest request = WebRequest.Create("http://checkip.dyndns.org");
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(
              "http://checkip.dyndns.org");
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            
            //string[] a = request.Credentials.ResponseBody.Split(':');
            //string a2 = a[1].Substring(1);
            //string[] a3=a2.Split('<');
            //string a4 = a3[0];
            //Console.WriteLine(a4);
            //Console.ReadLine();

            Match match = Regex.Match(responseFromServer, @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
            
            string ip = responseFromServer.Substring(match.Captures[0].Index,match.Captures[0].Length);

            return ip;
        }

        //public static void ShowBoundingBox(SpriteBatch spriteBatch, Rectangle rect)
        //{
        //    spriteBatch.Draw(Resources.WhitePixel, rect, rect, Color.Black * 0.4f, 0.0f, new Vector2(), SpriteEffects.None, Layers.BOUNDINGBOXES);
        //}

        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static float GetDiagonal(this Rectangle rect)
        {
            return (float)Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow(rect.Height, 2));
        }

        public static float Distance(this Vector2 v, Rectangle rect)
        {
            Vector2 nearest = v;

            if (v.X < rect.X)
               nearest.X = rect.X;
            else if (v.X > rect.Width + rect.X)
                nearest.X = rect.Width + rect.X;

            if (v.Y < rect.Y)
               nearest.Y = rect.Y;
            else if (v.Y > rect.Height + rect.Y)
                nearest.Y = rect.Height + rect.Y;

             return (nearest - v).Length();
        }

        public static void Update(this TimeSpan timeSpan, GameTime gameTime)
        {
            timeSpan = timeSpan.Subtract(gameTime.ElapsedGameTime);
        }
    }
}
