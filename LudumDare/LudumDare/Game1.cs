using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Gex.Audio;
using Gex.Cameras;
using Gex.Input;
using Gex.Util;
using LudumDare.Characters;
using LudumDare.Levels;
using LudumDare.GUI;
using LudumDare.Particles;

namespace LudumDare
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        AudioManager audio;
        Camera2D cam;
        int width, height;

        Texture2D pixel;

        Texture2D overlay;
        Texture2D background, bgBlurred;
        float bgFlicker = 0;

        Level level;
        Hud hud;

        InputManager input;
        Character character;

        ParticleManager pMan;

        bool paused = false;
        int currentLevel = 2;

        float currentDeath = 0, deathWait = 3000f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            width = 800;
            height = 600;
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>(@"fonts/offbeatgames");
            audio = new AudioManager();
            audio.LoadContent(Content.RootDirectory + "/audio/");


            background = Content.Load<Texture2D>(@"gfx/sky");
            bgBlurred = Content.Load<Texture2D>(@"gfx/sky_blurred");
            overlay = Content.Load<Texture2D>(@"gfx/overlay");
            cam = new Camera2D(new Vector2(width, height) / 2f);

            input = new InputManager();

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            hud = new Hud().LoadContent(Content);

            Start();
        }

        public void Start()
        {
            level = new Level();
            pMan = new ParticleManager();
            character = new Character();
            currentLevel--;
            NextLevel();
        }
        public void NextLevel()
        {
            currentLevel++;
            level.LoadContent(Content, currentLevel);
            pMan.LoadContent(Content, level.script);
            character.LoadContent(Content);
            character.position = level.start * SourceRectangle.CELL_SIZE * 2f;
        }
        public void RestartLevel()
        {
            level.LoadContent(Content, currentLevel);
            pMan.LoadContent(Content, level.script);
            character.LoadContent(Content);
            character.position = level.start * SourceRectangle.CELL_SIZE * 2f;
        }

        protected override void UnloadContent()
        {
        }

        float blurFactor = 1f;
        protected override void Update(GameTime gameTime)
        {
            if (input.KeyClicked(Keys.Escape))
            {
                paused = !paused;
                //TODO: pause music etc...
            }

            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!paused)
            {
                bgFlicker += blurFactor * (0.0005f * time);
                if (bgFlicker < 0f)
                {
                    bgFlicker = 0f;
                    blurFactor *= -1f;
                }
                if (bgFlicker > 1f)
                {
                    bgFlicker = 1f;
                    blurFactor *= -1f;
                }

                level.Update(time, pMan, character);
                character.Update(time, input, level);
                if (!character.alive)
                {
                    //TODO: blood and stuff.. restart delay, wait for button press...
                    //Start();
                    currentDeath += time;
                    if (currentDeath > deathWait || input.KeyClicked(Keys.Space))
                    {
                        currentDeath = 0;
                        RestartLevel();
                    }
                }
                pMan.Update(time, character);
                if (level.Cleared(character, input))
                {
                    NextLevel();
                }
            }
            input.Update(time);
            cam.Move(character.position);
            cam.Update(time);
            hud.Update(time);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
            spriteBatch.Draw(background, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, cam.Matrix);
            level.Draw(spriteBatch, character, pixel);
            character.Draw(spriteBatch, pixel);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, cam.Matrix);
            pMan.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null);
            spriteBatch.Draw(bgBlurred, new Rectangle(0, 0, width, height), Color.White * bgFlicker);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(overlay, new Rectangle(0, 0, width, height), Color.Black);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, cam.Matrix);
            hud.DrawCamRelated(spriteBatch, character, level);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
            hud.Draw(spriteBatch, character, level);
            hud.DrawSignMessages(spriteBatch, character, level);
            spriteBatch.End();

            if (paused)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(pixel, new Rectangle(0, 0, width, height), Color.Black * 0.8f);
                spriteBatch.DrawString(font, "PAUSED", new Vector2(width, height) / 2f, Color.White, 0f, font.MeasureString("PAUSED") / 2f, 1f, SpriteEffects.None, 1f);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
