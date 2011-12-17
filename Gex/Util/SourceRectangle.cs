using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gex.Util
{
    public class SourceRectangle
    {
        public const int CELL_SIZE = 32;

        public static Rectangle Create(int col, int row, int columns, int rows)
        {
            return new Rectangle(col * CELL_SIZE, row * CELL_SIZE, columns * CELL_SIZE, rows * CELL_SIZE);
        }
        public static Rectangle Create(int col, int row, int columns, int rows, int cellSize)
        {
            return new Rectangle(col * cellSize, row * cellSize, columns * cellSize, rows * cellSize);
        }
    }
}