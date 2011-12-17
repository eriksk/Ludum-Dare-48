using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gex.Util;
using LudumDare.Characters;
using System.IO;
using Gex.Input;
using Microsoft.Xna.Framework.Input;
using LudumDare.Entities;
using LudumDare.Particles;

namespace LudumDare.Levels
{
    enum Cell
    {
        None = 0,
        Start = 9,
        End = 8,
        Spikes = 7,
        Dirt = 1,
        DirtFloor = 2,
        DirtSky = 3,
        Stone = 4,
        StoneFloor = 5,
        StoneSky = 6

    }
    class Level
    {
        public int[,] grid;
        public const int CELL_SIZE = 32;
        Texture2D texture;
        Dictionary<Cell, Rectangle> sources;
        public LevelScript script;
        public Vector2 start, end;
        public List<Sign> signs;
        public List<Fan> fans;
        Texture2D signTex, fanTex;

        public Level()
        {

        }

        public Level LoadContent(ContentManager content, int level)
        {
            signs = new List<Sign>();
            fans = new List<Fan>();
            texture = content.Load<Texture2D>(@"gfx/grid");
            sources = new Dictionary<Cell,Rectangle>{
                {Cell.None, SourceRectangle.Create(0, 0, 0, 0)}, //Empty
                {Cell.Dirt, SourceRectangle.Create(0, 0, 1, 1)}, //2
                {Cell.DirtFloor, SourceRectangle.Create(1, 0, 1, 1)},
                {Cell.DirtSky, SourceRectangle.Create(2, 0, 1, 1)},
                {Cell.Stone, SourceRectangle.Create(3, 0, 1, 1)},
                {Cell.StoneFloor, SourceRectangle.Create(4, 0, 1, 1)},
                {Cell.StoneSky, SourceRectangle.Create(5, 0, 1, 1)},
                {Cell.Spikes,  SourceRectangle.Create(6, 0, 1, 1)},
                {Cell.Start, SourceRectangle.Create(7, 0, 1, 1)},
                {Cell.End, SourceRectangle.Create(8, 0, 1, 1)}
            };

            start = Vector2.Zero;
            end = Vector2.Zero;
            grid = LoadLevel(content, "level" + level, out start, out end);
            script = LoadScript(content, "level" + level);

            signTex = content.Load<Texture2D>(@"gfx/sign");
            fanTex = content.Load<Texture2D>(@"gfx/fan");
            
            return this;
        }

        private LevelScript LoadScript(ContentManager content, string level)
        {
            LevelScript s = new LevelScript();
            using (StreamReader r = new StreamReader(content.RootDirectory + @"/gfx/levels/" + level + ".txt"))
            {
                while (!r.EndOfStream)
                {
                    string[] line = r.ReadLine().Split('=');
                    switch (line[0].ToLower())
                    {
                        case "rain":
                            s.rain = int.Parse(line[1]);
                            break;

                        case "fan":
                            fans.Add(Fan.Parse(line[1]));
                            break;

                        case "sign":
                            signs.Add(Sign.Parse(line[1]));
                            break;

                        default:
                            break;
                    }
                }
            }
            return s;
        }

        public int[,] LoadLevel(ContentManager content, string level, out Vector2 start, out Vector2 end)
        {
            start = Vector2.Zero;
            end = Vector2.Zero;
            Texture2D levelData = content.Load<Texture2D>(@"gfx/levels/" + level);
            Color[] data = new Color[levelData.Width * levelData.Height];
            levelData.GetData<Color>(data);

            int[,] array = new int[levelData.Width, levelData.Height];
            //Copy to array
            for (int i = 0; i < levelData.Width; i++)
            {
                for (int j = 0; j < levelData.Height; j++)
                {
                    Cell val = 0;
                    Color col = data[i + j * levelData.Width];
                    if (col == Color.White)
                    {
                        val = Cell.None;
                    }
                    else if (col == Color.Black)
                    {
                        val = Cell.Dirt;
                    }
                    else if (col == Color.Yellow)
                    {
                        end = new Vector2(i, j);
                        val = Cell.End;
                    }
                    else if (col == Color.Cyan)
                    {
                        val = Cell.DirtSky;
                    }
                    else if (col == Color.Red)
                    {
                        val = Cell.Spikes;
                    }
                    else if (col == Color.Gray)
                    {
                        val = Cell.DirtFloor;
                    }
                    else if (col == Color.Green)
                    {
                        start = new Vector2(i, j);
                        val = Cell.Start;
                    }
                    array[i, j] = (int)val;
                }
            }
            return array;
        }


        public void Update(float time, ParticleManager pMan, Character character)
        {
            //Fan stuff
            Vector2 pos = Vector2.Zero;
            foreach (Fan s in fans)
            {
                pos = new Vector2(s.col * (CELL_SIZE * 2) + (CELL_SIZE), s.row * (CELL_SIZE * 2) + (CELL_SIZE));
                s.current += time;
                if (s.current > s.interval)
                {
                    s.current = 0f;
                    pMan.SprayAir(pos, s.Rotation, s.power);
                }

                //Check for collision
                //Broad phase
                float distance = Vector2.Distance(character.position, pos);
                if(distance < 500f)
                {
                    //Check if in area of airflow
                    bool applyForce = false;
                    switch (s.direction)
                    {
                        case Direction.Up:
                            if (character.CollRect.Center.X > pos.X - CELL_SIZE && character.CollRect.Center.X < pos.X + CELL_SIZE &&
                                character.CollRect.Center.Y <= pos.Y)
                            {
                                applyForce = true;
                            }
                            break;
                        case Direction.Down:
                            if (character.CollRect.Center.X > pos.X - CELL_SIZE && character.CollRect.Center.X < pos.X + CELL_SIZE &&
                                character.CollRect.Center.Y <= pos.Y)
                            {
                                applyForce = true;
                            }
                            break;
                        case Direction.Left:
                            if (character.CollRect.Center.Y > pos.Y - CELL_SIZE && character.CollRect.Center.Y < pos.Y + CELL_SIZE &&
                                character.CollRect.Center.X <= pos.X)
                            {
                                applyForce = true;
                            }
                            break;
                        case Direction.Right:
                            if (character.CollRect.Center.Y > pos.Y - CELL_SIZE && character.CollRect.Center.Y < pos.Y + CELL_SIZE &&
                                character.CollRect.Center.X >= pos.X)
                            {
                                applyForce = true;
                            }
                            break;
                        default:
                            break;
                    }
                    if (applyForce)
                    {
                        character.ApplyForce(Utility.AngleFrom(s.Rotation), distance, s.power);
                    }
                }

            }
        }

        #region Collision
        Rectangle tempRect;
        public void DoXCollision(Character character)
        {
            //Check surrounding cells.
            int col = (int)Math.Floor(character.position.X / (CELL_SIZE * 2f));
            int row = (int)Math.Floor(character.position.Y / (CELL_SIZE * 2f));
            
            for (int i = col-2; i < col+3; i++)
            {
                for (int j = row-2; j < row+3; j++)
                {
                    if (i > -1 && i < grid.GetLength(0) && 
                        j > -1 && j < grid.GetLength(1))
                    {
                        Cell cell = (Cell)grid[i, j];                     
                        tempRect.X = i * (CELL_SIZE * 2);
                        tempRect.Y = j * (CELL_SIZE * 2);
                        tempRect.Width = CELL_SIZE * 2;
                        tempRect.Height = CELL_SIZE * 2;
                        switch (cell)
                        {
                            case Cell.None:
                                //No collision
                                break;
                            case Cell.Start: //Door
                                break;
                            case Cell.End: //Door
                                break;
                            //case Cell.Spikes://Spikes Allow walking up to spikes, but not stepping on them

                            default: //Collidable cell found                                     
                                if (character.CollRect.Intersects(tempRect))
                                {
                                    //Collision found
                                    if (character.velocity.X < 0f)
                                    {
                                        //Move to right side
                                        character.velocity.X = 0;
                                        character.position.X = tempRect.Right;
                                        //collRects.Add(tempRect);
                                    }
                                    else if (character.velocity.X > 0f)
                                    {
                                        //Move to left side
                                        character.velocity.X = 0;
                                        character.position.X = tempRect.Left - character.CollRect.Width;
                                        //collRects.Add(tempRect);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void DoYCollision(Character character)
        {
            //Check surrounding cells.
            int col = (int)Math.Floor(character.position.X / (CELL_SIZE * 2f));
            int row = (int)Math.Floor(character.position.Y / (CELL_SIZE * 2f));

            for (int i = col - 2; i < col + 3; i++)
            {
                for (int j = row - 2; j < row + 3; j++)
                {
                    if (i > -1 && i < grid.GetLength(0) &&
                        j > -1 && j < grid.GetLength(1))
                    {
                        Cell cell = (Cell)grid[i, j];
                        tempRect.X = i * (CELL_SIZE * 2);
                        tempRect.Y = j * (CELL_SIZE * 2);
                        tempRect.Width = CELL_SIZE * 2;
                        tempRect.Height = CELL_SIZE * 2;
                        switch (cell)
                        {
                            case Cell.None:
                                //No collision
                                break;
                            case Cell.Start: //Door
                                break;
                            case Cell.End: //Door
                                break;
                            case Cell.Spikes://Spikes
                                if (character.CollRect.Intersects(tempRect))
                                {
                                    //Be gentle use a distance diff instead
                                    if (Vector2.Distance(new Vector2(character.CollRect.Center.X, character.CollRect.Center.Y), new Vector2(tempRect.Center.X, tempRect.Center.Y)) < 32)
                                    {
                                        character.alive = false;
                                    }
                                }
                                break;

                            default: //Collidable cell found         
                                //TODO: check for death.

                                if (character.CollRect.Intersects(tempRect))
                                {
                                    if (character.inAir)
                                    {
                                        //Collision found
                                        if (character.velocity.Y > 0f)
                                        {
                                            //Move to top side
                                            character.position.Y = tempRect.Top - character.CollRect.Height;
                                            character.Land();
                                            //collRects.Add(tempRect);
                                        }
                                        else if (character.velocity.Y < 0f)
                                        {
                                            //Move to bottom side, bump head into top.
                                            character.velocity.Y = 0;
                                            character.position.Y = tempRect.Bottom;
                                            character.SetAnim("jump");
                                            character.inAir = true;
                                            //collRects.Add(tempRect);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        public bool HasFloor(Character character)
        {
            Rectangle charCollRect = character.CollRect;
            charCollRect.Y += character.CollRect.Height + 5;
            //Check surrounding cells.
            int col = (int)Math.Floor(character.position.X / (CELL_SIZE * 2f));
            int row = (int)Math.Floor((character.position.Y + character.CollRect.Height + 5) / (CELL_SIZE * 2f));
            bool ret = false;
            for (int i = col - 3; i < col + 3; i++)
            {             
                if (i > -1 && i < grid.GetLength(0) &&
                    row > -1 && row < grid.GetLength(1))
                {
                    //Check collision first
                    tempRect.X = i * (CELL_SIZE * 2);
                    tempRect.Y = row * (CELL_SIZE * 2);
                    tempRect.Width = CELL_SIZE * 2;
                    tempRect.Height = CELL_SIZE * 2;

                    if (tempRect.Intersects(charCollRect))
                    {
                        //collRects.Add(tempRect);
                        Cell cell = (Cell)grid[i, row];
                        switch (cell)
                        {
                            case Cell.None:
                                ret = false;
                                break;//No floor
                            case Cell.Start:
                                ret = false;
                                break;//No floor
                            case Cell.End: ret = false;
                                break;//No floor
                            case Cell.Spikes:
                                ret = false;
                                break;
                            default:
                                return true;//Probably has floor
                        }
                    }
                }
            }
         
            //No map found, so no floor.
            return ret;
        }

        public bool Cleared(Character character, InputManager input)
        { 
            //Check surrounding cells.
            int col = (int)Math.Floor(character.position.X / (CELL_SIZE * 2f));
            int row = (int)Math.Floor(character.position.Y / (CELL_SIZE * 2f));

            if (col > -1 && col < grid.GetLength(0) &&
                row > -1 && row < grid.GetLength(1))
            {
                Cell cell = (Cell)grid[col, row];
                tempRect.X = col * (CELL_SIZE * 2);
                tempRect.Y = row * (CELL_SIZE * 2);
                tempRect.Width = CELL_SIZE * 2;
                tempRect.Height = CELL_SIZE * 2;
                switch (cell)
                {
                    case Cell.End: //Door
                        if (character.CollRect.Intersects(tempRect))
                        {
                            if (input.KeyClicked(Keys.Space))
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        #endregion

        List<Rectangle> collRects = new List<Rectangle>();
        public void Draw(SpriteBatch sb, Character character, Texture2D pixel)
        {
            int col = (int)Math.Floor(character.CollRect.Center.X / (CELL_SIZE * 2f));
            int row = (int)Math.Floor(character.CollRect.Center.Y / (CELL_SIZE * 2f));
            
            for (int i = col - 16; i < col + 16; i++)
            {
                for (int j = row - 16; j < row + 16; j++)
                {
                    if (i > -1 && i < grid.GetLength(0) &&
                        j > -1 && j < grid.GetLength(1))
                    {
                        sb.Draw(texture, new Vector2(i * (CELL_SIZE * 2), j * (CELL_SIZE * 2)), sources[(Cell)grid[i, j]], Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                    }
                }
            }

            foreach (Sign s in signs)
            {
                sb.Draw(signTex, new Vector2(s.col * (CELL_SIZE * 2), s.row * (CELL_SIZE * 2)), Color.White);
            }
            foreach (Fan s in fans)
            {
                sb.Draw(fanTex,
                    new Vector2(s.col * (CELL_SIZE * 2) + (CELL_SIZE), s.row * (CELL_SIZE * 2) + (CELL_SIZE)),
                    null,
                    Color.White, 
                    s.Rotation,
                    new Vector2(CELL_SIZE, CELL_SIZE),
                    1f, 
                    SpriteEffects.None, 1f);
            }

            /*

            foreach (Rectangle r in collRects)
            {
                sb.Draw(pixel, r, Color.Yellow * 0.8f);
            }
            collRects.Clear();
            */
        }
    }
}
