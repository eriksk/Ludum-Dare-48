using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LudumDare.Entities
{
    class Fan
    {
        public int col, row;
        public Direction direction;
        public int power;
        public float current, interval = 50;
        
        public float Rotation 
        {
            get 
            {
                switch (direction)
                {
                    case Direction.Up: return MathHelper.ToRadians(270);
                    case Direction.Down: return MathHelper.ToRadians(90);
                    case Direction.Left: return MathHelper.ToRadians(180);
                    case Direction.Right: return 0;
                    default:
                        return 0;
                };
            }
        }

        public static Fan Parse(string line)
        {
            Fan s = new Fan();

            string[] values = line.ToLower().Split('&');
            for (int i = 0; i < values.Length; i++)
            {
                string key = values[i].Split(':')[0];
                string value = values[i].Split(':')[1];

                switch (key)
                {
                    case "col":
                        s.col = int.Parse(value);
                        break;
                    case "row":
                        s.row = int.Parse(value);
                        break;
                    case "dir":
                        value = value.ToLower();
                        #region
                        if (value == "up")
                        {
                            s.direction = Direction.Up;
                        }
                        if (value == "down")
                        {
                            s.direction = Direction.Down;
                        }
                        if (value == "left")
                        {
                            s.direction = Direction.Left;
                        }
                        if (value == "right")
                        {
                            s.direction = Direction.Right;
                        }
                        #endregion
                        break;
                    case "power":
                        s.power = int.Parse(value);
                        break;
                    default:
                        break;
                }

            }

            return s;
        }
    }
}
