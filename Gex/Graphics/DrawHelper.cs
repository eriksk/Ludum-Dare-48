using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gex.Util;

namespace Gex.Graphics
{
    /// <summary>
    /// Extentions for drawing lines, etc.
    /// </summary>
    public static class DrawHelper
    {
        public static void DrawLine(this SpriteBatch sb, Texture2D pixel, Vector2 start, Vector2 end, Color color, int thickness)
        {
            int length = (int)Vector2.Distance(start, end) + 1;
            float angle = Utility.AngleFrom(start, end);
            sb.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, length, thickness), null, color, angle, Vector2.Zero, SpriteEffects.None, 1f);
        }
        public static void DrawRectangleOutline(this SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color, int thickness)
        {
            Vector2 start = new Vector2();
            Vector2 end = new Vector2();
            start.X = rect.X - thickness;
            start.Y = rect.Y;
            end.X = rect.X + rect.Width;
            end.Y = rect.Y;
            //TOP
            DrawLine(sb, pixel, start, end, color, thickness);

            start.X = rect.X;
            start.Y = rect.Y;
            end.X = rect.X;
            end.Y = rect.Y + rect.Height;
            //Left
            DrawLine(sb, pixel, start, end, color, thickness);

            start.X = rect.X + rect.Width;
            start.Y = rect.Y;
            end.X = rect.X + rect.Width;
            end.Y = rect.Y + rect.Height + thickness;
            //Right
            DrawLine(sb, pixel, start, end, color, thickness);

            start.X = rect.X - thickness;
            start.Y = rect.Y + rect.Height;
            end.X = rect.X + rect.Width;
            end.Y = rect.Y + rect.Height;
            //Bottom
            DrawLine(sb, pixel, start, end, color, thickness);
        }
    }
}
