using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Turtle
{
    public class Turtle
    {
        private List<Vector2> positions;
        private float MoveLength, RotationSize;
        private Vector2 Position;
        private float Rotation;
        private string Command;
        private int comm;

        public Turtle(float moveLength, float rotationSize, Vector2 startingPosition, float startingAngle, string command)
        {
            positions = new List<Vector2>();
            Position = startingPosition;
            positions.Add(Position);

            MoveLength = moveLength;

            RotationSize = rotationSize;
            Rotation = MathHelper.ToRadians(startingAngle);

            //ExecuteCommand(command);

            comm = 0;
            Command = command;
        }

        public void Update()
        {
            if (Scripts.KeyIsPressed(Keys.Space))
            {
                ExecuteCommand(Command, comm++);
            }
        }

        private void ExecuteCommand(string comm, int i)
        {
            if (i < comm.Length - 1 && i >= 0)
            {
                switch (comm[i])
                {
                    case 'F':
                        Vector2 dir = MathAid.AngleToVector(Rotation);
                        Position += dir * (MoveLength);
                        positions.Add(Position);
                        break;

                    case 'L':
                        Rotation -= MathHelper.ToRadians(RotationSize);
                        break;

                    case 'R':
                        Rotation += MathHelper.ToRadians(RotationSize);
                        break;

                    default:
                        break;
                }
            }
        }

        private void ExecuteCommand(string command)
        {
            for (int i = 0; i < command.Length; i++)
            {
                ExecuteCommand(command, i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (positions.Count > 1)
            {
                for (int i = 0; i < positions.Count - 1; i++)
                {
                    spriteBatch.DrawLine(positions[i], positions[i + 1], Color.Red, 1);
                }
            }

            //for (int i = 0; i < positions.Count; i++)
            //{
            //    spriteBatch.PutPixel(positions[i], Color.Green);
            //}

            //spriteBatch.PutPixel(Position, Color.Blue);
        }
    }
}