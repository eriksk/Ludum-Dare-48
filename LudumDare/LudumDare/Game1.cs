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
using System.Threading;

namespace LudumDare
{
    enum GameState
    {
        Splash,
        Loading,
        Loaded,
        None,
        GameOver
    }

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
        int currentLevel = 1;
        int maxLevels = 8;

        float currentDeath = 0, deathWait = 3000f;
        ProgressBar fadeIn = new ProgressBar(2000f);

        GameState state = GameState.Splash;

        Texture2D logoTex, gameOverTex;
        Color splashColor = Color.Cyan;
        Color nextSplashColor;
        float splashCurrent = 1000f, splashColorInterval = 100f;

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

            logoTex = Content.Load<Texture2D>(@"gfx/logo");
            input = new InputManager();
            audio = new AudioManager();
            audio.LoadContent(Content.RootDirectory + "/audio/");

            Thread loadThread = new Thread(LoadAssets);
            loadThread.Start();           

        }
        bool loaded = false;
        private void LoadAssets()
        {
            font = Content.Load<SpriteFont>(@"fonts/offbeatgames");
            
            background = Content.Load<Texture2D>(@"gfx/sky");
            bgBlurred = Content.Load<Texture2D>(@"gfx/sky_blurred");
            overlay = Content.Load<Texture2D>(@"gfx/overlay");
            gameOverTex = Content.Load<Texture2D>(@"gfx/gameOver");
            cam = new Camera2D(new Vector2(width, height) / 2f);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            hud = new Hud().LoadContent(Content);

            //Play song
            audio.PlaySong("Josh");
            audio.ResumeAll();
            loaded = true;
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
            if (currentLevel > maxLevels)
            {
                state = GameState.GameOver;
            }
            else
            {
                level.LoadContent(Content, currentLevel);
                pMan.LoadContent(Content, level.script);
                character.LoadContent(Content);
                character.position = level.start * SourceRectangle.CELL_SIZE * 2f;
                fadeIn.Reset();
            }
        }
        public void RestartLevel()
        {
            level.LoadContent(Content, currentLevel);
            pMan.LoadContent(Content, level.script);
            character.LoadContent(Content);
            character.position = level.start * SourceRectangle.CELL_SIZE * 2f;
            fadeIn.Reset();
        }

        protected override void UnloadContent()
        {
        }

        float blurFactor = 1f;
        protected override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            switch (state)
            {
                case GameState.Splash:
                    splashCurrent += time;
                    if (splashCurrent > splashColorInterval)
                    {
                        splashCurrent = 0f;
                        splashColor = nextSplashColor;
                        nextSplashColor = new Color(Utility.Rand(0f, 1f), Utility.Rand(0f, 1f), Utility.Rand(0f, 1f));
                    }
                    if (input.KeyClicked(Keys.Space))
                    {
                        state = GameState.Loading;
                    }
                    break;
                case GameState.Loading:
                    if (loaded)
                    {
                        state = GameState.Loaded;
                    }
                    break;
                case GameState.Loaded:
                    Start();
                    state = GameState.None;
                    break;
                case GameState.GameOver:
                    splashCurrent += time;
                    if (splashCurrent > splashColorInterval)
                    {
                        splashCurrent = 0f;
                        splashColor = nextSplashColor;
                        nextSplashColor = new Color(Utility.Rand(0f, 1f), Utility.Rand(0f, 1f), Utility.Rand(0f, 1f));
                    }
                    if (input.KeyClicked(Keys.Space) || input.KeyClicked(Keys.Enter) || input.KeyClicked(Keys.Escape))
                    {
                        this.Exit();
                    }
                    break;
                case GameState.None:
                    if (!paused)
                    {
                        if (input.KeyClicked(Keys.Escape))
                        {
                            paused = true;
                            audio.PlaySound("blip");
                            audio.PauseAll();
                        }
                    }
                    else if (paused)
                    {
                        if (input.KeyClicked(Keys.Escape))
                        {
                            this.Exit();
                        }
                        if (input.KeyClicked(Keys.Enter))
                        {
                            paused = false;
                            audio.ResumeAll();
                        }
                    }


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
                            if (!character.splatted)
                            {
                                pMan.BloodSplat(new Vector2(character.CollRect.Center.X, character.CollRect.Center.Y));
                                character.splatted = true;
                            }
                            currentDeath += time;
                            if (currentDeath > deathWait || input.KeyClicked(Keys.Space))
                            {
                                currentDeath = 0;
                                RestartLevel();
                            }
                        }
                        pMan.Update(time, character);
                    }
                    else
                    {

                    }
                    if (level.Cleared(character, input))
                    {
                        NextLevel();
                    }
                    else
                    {
                        fadeIn.Update(time);
                        cam.Move(character.position);
                        cam.Update(time);
                        hud.Update(time);
                    }
                    audio.Update();
                    break;
                default:
                    break;
            }
            input.Update(time);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (state == GameState.None)
            {
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
                    const string psMessage = "ENTER = RESUME";
                    spriteBatch.DrawString(font, psMessage, new Vector2(width / 2f, height / 2f + 60), Color.Green, 0f, font.MeasureString(psMessage) / 2f, 1f, SpriteEffects.None, 1f);
                    const string escMessage = "ESC = EXIT GAME";
                    spriteBatch.DrawString(font, escMessage, new Vector2(width / 2f, height / 2f + 90), Color.Red, 0f, font.MeasureString(escMessage) / 2f, 1f, SpriteEffects.None, 1f);
                    spriteBatch.End();
                }

                //Fade in
                spriteBatch.Begin();
                spriteBatch.Draw(pixel, new Rectangle(0, 0, width, height), Color.Black * (1f - fadeIn.Progress));
                spriteBatch.End();
            }
            if (state == GameState.GameOver)
            {
                GraphicsDevice.Clear(Color.White);
                spriteBatch.Begin();
                spriteBatch.Draw(gameOverTex, new Vector2(width / 2f, height / 2f), null, Color.Lerp(splashColor, nextSplashColor, splashCurrent / splashColorInterval), 0f, new Vector2(gameOverTex.Width, gameOverTex.Height) / 2f, 1f, SpriteEffects.None, 1f);
                spriteBatch.End();
            }

            
            //Fade logo
            if (state == GameState.Splash)
            {
                GraphicsDevice.Clear(Color.White);
                spriteBatch.Begin();
                spriteBatch.Draw(logoTex, new Vector2(width / 2f, height / 2f), null, Color.Lerp(splashColor, nextSplashColor, splashCurrent / splashColorInterval), 0f, new Vector2(logoTex.Width, logoTex.Height) / 2f, 1f, SpriteEffects.None, 1f);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
