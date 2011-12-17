using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LudumDare.Characters;
using LudumDare.Levels;
using Microsoft.Xna.Framework;
using LudumDare.Entities;

namespace LudumDare.GUI
{
    class Hud
    {
        //Queue of messages
        SpriteFont font;
        Texture2D pixel;

        public Hud()
        {
        }

        public Hud LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>(@"fonts/offbeatgames");
            pixel = content.Load<Texture2D>(@"gfx/pixel");
            return this;
        }

        public void Update(float time)
        {

        }
        
        public void Draw(SpriteBatch sb, Character character, Level level)
        {
            if (!character.alive)
            {
                DrawMessage(sb, new Vector2(800 / 2, 600 / 2), "DEAD");
                DrawMessage(sb, new Vector2(800 / 2, 600 * 0.8f), "PRESS SPACE TO RESTART");
            }
        }
        public void DrawSignMessages(SpriteBatch sb, Character character, Level level)
        {
            if (character.alive)
            {
                foreach (Sign s in level.signs)
                {
                    if (Vector2.Distance(character.position, new Vector2(s.col * (Level.CELL_SIZE * 2f), s.row * (Level.CELL_SIZE * 2f))) < 100f)
                    {
                        DrawMessage(sb, new Vector2(800 / 2, 600 * 0.7f), s.text);
                    }
                }
            }
        }

        public void DrawCamRelated(SpriteBatch sb, Character character, Level level)
        {
            if (!character.alive)
            {
            }
            else
            {
                int[,] grid = level.grid;
                //Get current cell
                int col = (int)Math.Floor(character.position.X / (Level.CELL_SIZE * 2));
                int row = (int)Math.Floor(character.position.Y / (Level.CELL_SIZE * 2));

                for (int i = col - 2; i < col + 1; i++)
                {
                    for (int j = row - 2; j < row + 1; j++)
                    {
                        if (i > -1 && i < grid.GetLength(0) &&
                            j > -1 && j < grid.GetLength(1))
                        {
                            Cell cell = (Cell)grid[i, j];
                            Vector2 pos;
                            switch (cell)
                            {
                                case Cell.None:
                                    break;
                                case Cell.Start:
                                    pos = new Vector2(i * (Level.CELL_SIZE * 2) + Level.CELL_SIZE, j * (Level.CELL_SIZE * 2));
                                    DrawMessage(sb, pos, "START");
                                    break;
                                case Cell.End:
                                    pos = new Vector2(i * (Level.CELL_SIZE * 2) + Level.CELL_SIZE, j * (Level.CELL_SIZE * 2));
                                    DrawMessage(sb, pos, "PRESS SPACE");
                                    break;
                                case Cell.Spikes:
                                    break;
                                case Cell.Dirt:
                                    break;
                                case Cell.DirtFloor:
                                    break;
                                case Cell.DirtSky:
                                    break;
                                case Cell.Stone:
                                    break;
                                case Cell.StoneFloor:
                                    break;
                                case Cell.StoneSky:
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                }
            }

        }

        public void DrawMessage(SpriteBatch sb, Vector2 position, string message)
        {
            Vector2 size = font.MeasureString(message);
            sb.Draw(pixel, new Rectangle((int)(position.X - size.X / 2f) - 6, (int)(position.Y - size.Y / 2f) - 6, (int)size.X + 12, (int)size.Y + 6), Color.Black * 0.4f);
            sb.DrawString(font, message, position, Color.White, 0f, size / 2f, 1f, SpriteEffects.None, 1f);
        }
    }
}
