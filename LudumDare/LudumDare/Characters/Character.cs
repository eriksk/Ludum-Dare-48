using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gex.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Gex.Util;
using Gex.Input;
using Microsoft.Xna.Framework.Input;
using LudumDare.Levels;

namespace LudumDare.Characters
{
    class Character : Entity
    {
        public Texture2D texture;
        private Dictionary<string, Rectangle[]> animations;

        int currentFrame;
        float interval;
        float current;
        string currentAnim;
        private Dictionary<string, float> intervals;
        float speed = 0.1f;
        public bool inAir = false;
        bool flipped = false;
        Rectangle collRect;
        public bool alive = true;
        public bool splatted = false;

        public Character()
            :base()
        {
            interval = 1000;
            collRect = new Rectangle(0, 0, SourceRectangle.CELL_SIZE * 2, SourceRectangle.CELL_SIZE * 2);
        }
        public void Reset()
        {
            alive = true;
            SetAnim("jump");
            inAir = true; 
            splatted = false;
        }

        public Character LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(@"gfx/characters");
            animations = new Dictionary<string,Rectangle[]>();
            int cellSize = 16;

            intervals = new Dictionary<string, float>();
            intervals.Add("idle", 1000);
            intervals.Add("walk", 175);
            intervals.Add("jump", 1000);

            animations.Add("idle", new Rectangle[] { 
                SourceRectangle.Create(0, 0, 1, 1, cellSize),
                SourceRectangle.Create(1, 0, 1, 1, cellSize),
            });

            animations.Add("walk", new Rectangle[] { 
                SourceRectangle.Create(2, 0, 1, 1, cellSize),
                SourceRectangle.Create(3, 0, 1, 1, cellSize)
            });

            animations.Add("jump", new Rectangle[] { 
                SourceRectangle.Create(4, 0, 1, 1, cellSize)
            });

            Reset();

            return this;
        }

        public Rectangle CollRect
        {
            get 
            {
                return collRect;
            }
        }

        public void SetAnim(string name)
        {
            if (currentAnim != name)
            {
                currentAnim = name;
                current = 0;
                currentFrame = 0;
                interval = intervals[name];
            }
        }

        public void Land()
        {
            velocity.Y = 0;
            SetAnim("idle");
            inAir = false;
        }

        public void Jump()
        {
            if (!inAir)
            {
                velocity.Y = -1.2f;
                SetAnim("jump");
                inAir = true;
            }
        }

        public void FallOff()
        {
            if (!inAir)
            {
                velocity.Y = 0f;
                SetAnim("jump");
                inAir = true;
            }
        }

        public void ApplyForce(Vector2 direction, float distance, float power)
        {
            //Clamp
            distance = distance > 300 ? 300 : distance;
            velocity += direction * Utility.Lerp(0.1f, 0f, distance / 300f) * (power * 0.1f);
        }

        public void Update(float time, InputManager input, Level level)
        {
            if (alive)
            {
                //Gravity 
                if (inAir)
                {
                    velocity.Y += 0.003f * time;
                }
                //Friction
                float maxSpeed = 0.4f;
                float friction = 0.003f;
                if (velocity.X > 0f)
                {
                    velocity.X -= friction * time;
                    if (velocity.X < 0f)
                    {
                        velocity.X = 0;
                        SetAnim("idle");
                    }
                }
                if (velocity.X < 0f)
                {
                    velocity.X += friction * time;
                    if (velocity.X > 0f)
                    {
                        velocity.X = 0;
                        SetAnim("idle");
                    }
                }
                position.X += velocity.X * time;
                collRect.X = (int)position.X;
                collRect.Y = (int)position.Y;
                level.DoXCollision(this);
                position.Y += velocity.Y * time;
                collRect.X = (int)position.X;
                collRect.Y = (int)position.Y;
                level.DoYCollision(this);

                if (!inAir)
                {
                    if (!level.HasFloor(this))
                    {
                        FallOff();
                    }
                }

                current += time;
                if (current > interval)
                {
                    current = 0;
                    currentFrame++;
                    if (currentFrame > animations[currentAnim].Length - 1)
                    {
                        currentFrame = 0;
                    }
                }
                               

                if (input.KeyDown(Keys.Left))
                {
                    flipped = true;
                    velocity.X -= speed;
                    if (!inAir)
                    {
                        SetAnim("walk");
                    }
                    if (velocity.X < -maxSpeed)
                    {
                        velocity.X = -maxSpeed;
                    }
                }
                if (input.KeyDown(Keys.Right))
                {
                    flipped = false;
                    velocity.X += speed;
                    if (!inAir)
                    {
                        SetAnim("walk");
                    }
                    if (velocity.X > maxSpeed)
                    {
                        velocity.X = maxSpeed;
                    }
                }
                if (input.KeyClicked(Keys.Space))
                {
                    Jump();
                }
            }
        }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            if (alive)
            {
                sb.Draw(texture, position, animations[currentAnim][currentFrame], Color.White, 0f, Vector2.Zero, 4f, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
            }
            //sb.Draw(pixel, collRect, Color.Green * 0.4f);
        }
    }
}
