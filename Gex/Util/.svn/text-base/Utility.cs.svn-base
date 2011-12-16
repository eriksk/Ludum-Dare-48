using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gex.Util
{
    public class Utility
    {
        /// <summary>
        /// Atan2
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleFrom(Vector2 point1, Vector2 point2)
        {
            return (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        }
        /// <summary>
        /// Atan2
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleFrom(ref Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        /// <summary>
        /// Gets a normalized unit vector direction
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Vector2 DirectionFrom(Vector2 point1, Vector2 point2)
        {
            float angle = AngleFrom(point1, point2);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            direction.Normalize();
            return direction;
        }
        public static Vector2 DirectionFrom(Vector2 point1, Vector2 point2, out float angle)
        {
            angle = AngleFrom(point1, point2);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            direction.Normalize();
            return direction;
        }

        private static Random rand = new Random();

        /// <summary>
        /// Recreates the random object with a new seed
        /// </summary>
        /// <param name="seed"></param>
        public static void SetSeed(int seed)
        {
            rand = new Random(seed);
        }
        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Last value</param>
        /// <param name="weight">Weight</param>
        /// <returns>Returns the interpolated value</returns>
        public static float Lerp(float value1, float value2, float weight)
        {
            return value1 + (value2 - value1) * weight;
        }
        /// <summary>
        /// Linear interpolation for large angles
        /// </summary>
        /// <param name="from">From angle</param>
        /// <param name="to">To angle</param>
        /// <param name="weight">Weigth</param>
        /// <returns>Returns the interpolated value</returns>
        public static float Clerp(float from, float to, float weight)
        {
            float t = ((MathHelper.WrapAngle(to - from) * (weight)));
            return from + t;
        }
        /// <summary>
        /// Gets a random float.
        /// </summary>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random float value</returns>
        public static float Rand(float max)
        {
            return (float)rand.NextDouble() * max;
        }
        /// <summary>
        /// Gets a random float.
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random float value</returns>
        public static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
        /// <summary>
        /// Gets a random integer.
        /// </summary>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random integer</returns>
        public static int Rand(int max)
        {
            return rand.Next(max);
        }
        /// <summary>
        /// Gets a random integer.
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random integer</returns>
        public static int Rand(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
