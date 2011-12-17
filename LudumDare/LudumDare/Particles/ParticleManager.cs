using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Gex.Util;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LudumDare.Characters;
using LudumDare.Levels;

namespace LudumDare.Particles
{
    class ParticleManager
    {
        Pool<Particle> particles;
        Texture2D texture;
        Rectangle[] sources;
        int rain = 0;

        public ParticleManager()
        {
            particles = new Pool<Particle>(4096);
            int cellSize = 16;
            sources = new Rectangle[] { 
                SourceRectangle.Create(0, 0, 1, 1, cellSize),
                SourceRectangle.Create(1, 0, 1, 1, cellSize),
                SourceRectangle.Create(2, 0, 1, 1, cellSize)
            };
        }

        public ParticleManager LoadContent(ContentManager content, LevelScript script)
        {
            texture = content.Load<Texture2D>(@"gfx/particles");
            rain = script.rain;
            return this;
        }

        public void SprayAir(Vector2 position, float direction, int power /*0 - 100*/)
        {
            for (int i = 0; i < Utility.Rand(2, 4); i++)
            {
                Particle p = particles.Pop();
                p.position.X = position.X;
                p.position.Y = position.Y;
                p.current = 0;
                p.duration = Utility.Rand(1000, 3000);
                p.source = 2;
                p.origin.X = sources[p.source].Width / 2f;
                p.origin.Y = sources[p.source].Height / 2f;
                float dir = direction + Utility.Rand(-0.1f, +0.1f);
                p.velocity.X = (float)Math.Cos(dir);
                p.velocity.Y = (float)Math.Sin(dir);
                p.position.X += p.velocity.X * 32; //Start offset
                p.position.Y += p.velocity.Y * 32;
                p.velocity.X *= power * 0.01f;
                p.velocity.Y *= power * 0.01f; 
                p.color = Color.White;
                p.scale = Utility.Rand(0.6f, 1f);
                p.rotation = dir;
                p.rotval = 0f;
            }                
        }

        float current = 0, interval = 300;
        public void Update(float time, Character character)
        {
            current += time;
            if (current > interval)
            {
                current = 0f;
                //Spawn leaves
                for (int i = 0; i < Utility.Rand(2, 12); i++)
                {
                    Particle p = particles.Pop();
                    p.position.X = character.position.X + Utility.Rand(-600, 600);
                    p.position.Y = character.position.Y + Utility.Rand(-400, 400);
                    p.current = 0;
                    p.duration = Utility.Rand(2000, 5000);
                    p.source = rain;
                    p.origin.X = sources[p.source].Width / 2f;
                    p.origin.Y = sources[p.source].Height / 2f;
                    p.velocity.X = Utility.Rand(-0.1f, -0.01f);
                    p.velocity.Y = Utility.Rand(0.01f, 0.1f);
                    p.color = Color.White;
                    p.scale = Utility.Rand(0.8f, 1f);
                    p.rotation = Utility.Rand(2f);
                    p.rotval = Utility.Rand(-0.01f, 0.01f);
                }
            }

            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                p.current += time;
                p.position.X += p.velocity.X * time;
                p.position.Y += p.velocity.Y * time;
                p.rotation += p.rotval * time;
                if (p.current > p.duration)
                {
                    particles.Push(i--);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                sb.Draw(texture, p.position, sources[p.source], p.color * MathHelper.SmoothStep(1f, 0f, p.current / p.duration), p.rotation, p.origin, p.scale, SpriteEffects.None, 1f);
            }

        }
    }
}
