using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MessingAroundwithXBOX
{
    public class Game1 : Game
    {
        enum ScreenState
        {
            Home = -1,
            Single = 0,
            Multi = 1
        }


        //Todo:
        //fire rate
        //dennis class
        //TODO: Have it so that it updates position with enemy

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random rand = new Random();

        Texture2D pixel;

        MainCharacter player1;
        MainCharacter player2;
        MainCharacter player3;
        MainCharacter player4;
        Texture2D projectileSprite;
        Texture2D characterTexture;
        Enemy Denis;

        GamePadState state;
        GamePadState state2;
        GamePadState state3;
        GamePadState state4;
        GamePadState prevState;
        Song backgroundSong;

        SpriteFont font1;

        bool paused = false;
        ScreenState screen = ScreenState.Home;
        Sprite[] stars = new Sprite[10];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("SpriteFont1");

            Texture2D denisTexture = Content.Load<Texture2D>("dennisthemenacingly");
            characterTexture = Content.Load<Texture2D>("GoldenKappa");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundSong = Content.Load<Song>("Heman Hey ya");
            player1 = new MainCharacter(characterTexture, new Vector2(0, 0), TimeSpan.FromSeconds(1), Content.Load<SoundEffect>("Laser Gun Sound Effect"), Content.Load<SpriteFont>("LoseFOnt"));
            player2 = new MainCharacter(characterTexture, new Vector2(GraphicsDevice.Viewport.Width - characterTexture.Width, GraphicsDevice.Viewport.Height - characterTexture.Height), TimeSpan.FromSeconds(1), Content.Load<SoundEffect>("Laser Gun Sound Effect"), Content.Load<SpriteFont>("LoseFOnt"));
            player2.Effects = SpriteEffects.FlipHorizontally;
            player3 = new MainCharacter(characterTexture, new Vector2(0, GraphicsDevice.Viewport.Height - characterTexture.Height), TimeSpan.FromSeconds(1), Content.Load<SoundEffect>("Laser Gun Sound Effect"), Content.Load<SpriteFont>("LoseFOnt"));
            player4 = new MainCharacter(characterTexture, new Vector2(GraphicsDevice.Viewport.Width - characterTexture.Width, 0), TimeSpan.FromSeconds(1), Content.Load<SoundEffect>("Laser Gun Sound Effect"), Content.Load<SpriteFont>("LoseFOnt"));
            player4.Effects = SpriteEffects.FlipHorizontally;

            projectileSprite = Content.Load<Texture2D>("Laser");
            Denis = new Enemy(denisTexture, new Vector2(rand.Next(player1.Texture.Width, GraphicsDevice.Viewport.Width - denisTexture.Width), rand.Next(player1.Texture.Height, GraphicsDevice.Viewport.Height - denisTexture.Height)), Content.Load<Texture2D>("DenisDoomExplosion"), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), player1);
            MediaPlayer.Play(backgroundSong);
            player1.Points = Denis.Points;
            // TODO: use this.Content to load your game content here

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            stars[0] = new Star();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public string checkForWinner()
        {
            if (!player1.Visible && !player2.Visible && !player3.Visible && player4.Visible)
            {
                return "player4";
            }
            else if (!player1.Visible && !player2.Visible && player3.Visible && !player4.Visible)
            {
                return "player3";
            }
            else if (!player1.Visible && player2.Visible && !player3.Visible && !player4.Visible)
            {
                return "player2";
            }
            else if (player1.Visible && !player2.Visible && !player3.Visible && !player4.Visible)
            {
                return "player1";
            }
            return null;
        }

        protected override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(backgroundSong);
            }
            prevState = state;
            state = GamePad.GetState(PlayerIndex.One);
            state2 = GamePad.GetState(PlayerIndex.Two);
            state3 = GamePad.GetState(PlayerIndex.Three);
            state4 = GamePad.GetState(PlayerIndex.Four);
            if (!state.IsConnected)
            {
                player1.Visible = false;
            }
            else if (!state2.IsConnected)
            {
                player2.Visible = false;
            }
            if (!state3.IsConnected)
            {
                player3.Visible = false;
            }
            if (!state4.IsConnected)
            {
                player4.Visible = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || state.Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here
            if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start != ButtonState.Pressed)
            {
                paused = !paused;
            }

            if (!paused)
            {
                if (screen == ScreenState.Home)
                {
                    if (state.Buttons.A == ButtonState.Pressed)
                    {
                        screen = ScreenState.Single;
                    }
                    else if (state.Buttons.B == ButtonState.Pressed)
                    {
                        screen = ScreenState.Multi;
                    }
                }
                else
                {
                    player1.Update(GraphicsDevice.Viewport.Bounds, projectileSprite, gameTime, PlayerIndex.One);
                    if (screen == ScreenState.Single)
                    {
                        for (int i = 0; i < Denis.projectiles.Count; i++)
                        {
                            if (Denis.projectiles[i].Hitbox.Intersects(player1.Hitbox))
                            {
                                player1.Visible = false;
                            }
                        }
                        Denis.Update(GraphicsDevice.Viewport.Bounds, gameTime, player1.projectiles, projectileSprite, player1.Position, 10);
                        player1.Points = Denis.Points;
                    }
                    else if (screen == ScreenState.Multi)
                    {
                        player2.Update(GraphicsDevice.Viewport.Bounds, projectileSprite, gameTime, PlayerIndex.Two);
                        player3.Update(GraphicsDevice.Viewport.Bounds, projectileSprite, gameTime, PlayerIndex.Three);
                        for (int i = 0; i < player1.projectiles.Count; i++)
                        {
                            if (player1.projectiles[i].Hitbox.Intersects(player2.Hitbox))
                            {
                                player2.Visible = false;
                            }
                            else if (player1.projectiles[i].Hitbox.Intersects(player3.Hitbox))
                            {
                                player3.Visible = false;
                            }
                            else if (player1.projectiles[i].Hitbox.Intersects(player4.Hitbox))
                            {
                                player4.Visible = false;
                            }
                        }
                        for (int i = 0; i < player2.projectiles.Count; i++)
                        {
                            if (player2.projectiles[i].Hitbox.Intersects(player1.Hitbox))
                            {
                                player1.Visible = false;
                            }
                            else if (player2.projectiles[i].Hitbox.Intersects(player3.Hitbox))
                            {
                                player3.Visible = false;
                            }
                            else if (player2.projectiles[i].Hitbox.Intersects(player4.Hitbox))
                            {
                                player4.Visible = false;
                            }
                        }
                        for (int i = 0; i < player3.projectiles.Count; i++)
                        {
                            if (player3.projectiles[i].Hitbox.Intersects(player1.Hitbox))
                            {
                                player1.Visible = false;
                            }
                            else if (player3.projectiles[i].Hitbox.Intersects(player2.Hitbox))
                            {
                                player2.Visible = false;
                            }
                            if (player4.projectiles[i].Hitbox.Intersects(player4.Hitbox))
                            {
                                player4.Visible = false;
                            }
                        }
                        for (int i = 0; i < player4.projectiles.Count; i++)
                        {
                            if (player4.projectiles[i].Hitbox.Intersects(player1.Hitbox))
                            {
                                player1.Visible = false;
                            }
                            else if (player4.projectiles[i].Hitbox.Intersects(player2.Hitbox))
                            {
                                player3.Visible = false;
                            }
                            else if (player4.projectiles[i].Hitbox.Intersects(player3.Hitbox))
                            {
                                player4.Visible = false;
                            }
                        }
                    }
                }
            }

            else
            {
                if (state.Buttons.Y == ButtonState.Pressed)
                {
                    paused = false;
                    screen = ScreenState.Home;
                }
            }

            if (screen == ScreenState.Single)
            {
                if ((state.Buttons.LeftShoulder == ButtonState.Pressed || state2.Buttons.LeftShoulder == ButtonState.Pressed) && !player1.Visible)
                {
                    player1.Position = Vector2.Zero;
                    player1.Visible = true;
                    player1.Points = 0;
                    Denis.Points = 0;
                }
            }

            else if (screen == ScreenState.Multi)
            {
                if ((state.Buttons.LeftShoulder == ButtonState.Pressed || state2.Buttons.LeftShoulder == ButtonState.Pressed || state3.Buttons.LeftShoulder == ButtonState.Pressed || state4.Buttons.LeftShoulder == ButtonState.Pressed) && (!player1.Visible || !player2.Visible || !player3.Visible || !player4.Visible) && screen == ScreenState.Multi)
                {
                    player1.Position = Vector2.Zero;
                    player2.Position = new Vector2(GraphicsDevice.Viewport.Width - characterTexture.Width, GraphicsDevice.Viewport.Height - characterTexture.Height);
                    player3.Position = new Vector2(0, GraphicsDevice.Viewport.Height - characterTexture.Height);
                    player4.Position = new Vector2(GraphicsDevice.Viewport.Width - characterTexture.Width, 0);
                    player1.Visible = true;
                    player2.Visible = true;
                    player3.Visible = true;
                    player4.Visible = true;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (paused)
            {
                spriteBatch.DrawString(font1, "The game is paused", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString("The game is paused").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString("The game is paused").Y / 2), Color.Black);
                spriteBatch.DrawString(font1, "Press Y for the home page", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString("Press Y for the home page").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString("Press Y for the home page").Y / 2 + font1.MeasureString("The game is paused").Y), Color.Black);
            }
            else
            {
                //spriteBatch.Draw(pixel, MainCharacter.Hitbox, Color.Red * 0.40f);
                if (screen == ScreenState.Single)
                {
                    player1.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
                    if (!player1.Visible)
                    {
                        spriteBatch.DrawString(font1, "YOU LOSE", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString("YOU LOSE").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString("YOU LOSE").Y), Color.Black);
                        spriteBatch.DrawString(font1, "Press LB to restart", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString("Press LB to restart").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString("YOU LOSE").Y + font1.MeasureString("Press LB to restart").Y), Color.Black);
                    }
                    Denis.Draw(spriteBatch);

                    spriteBatch.DrawString(font1, $"You have {player1.Points} points", new Vector2(GraphicsDevice.Viewport.Width - 300, 0), Color.Black);
                    /*foreach (var shot in Denis.projectiles)
                    {
                        spriteBatch.Draw(pixel, shot.Hitbox, Color.Red * 0.40f);
                    }*/
                }
                else if (screen == ScreenState.Home)
                {
                    spriteBatch.DrawString(font1, "Press A to play single player. Press B to play Multiplayer", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString("Press A to play single player. Press B to play Multiplayer").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString("Press A to play single player. Press B to play Multiplayer").Y), Color.Black);
                }
                else
                {
                    player1.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
                    player2.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
                    player3.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
                    player4.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
                    string w = checkForWinner();
                    if (w != null)
                    {
                        spriteBatch.DrawString(font1, $"{w} WINS", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString($"{w} WINS").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString($"{w} WINS").Y), Color.Black);
                        spriteBatch.DrawString(font1, "Press LB to restart", new Vector2(GraphicsDevice.Viewport.Width / 2 - font1.MeasureString("Press LB to restart").X / 2, GraphicsDevice.Viewport.Height / 2 - font1.MeasureString($"{w} WINS").Y + font1.MeasureString("Press LB to restart").Y), Color.Black);
                    }
                }
            }
            spriteBatch.End();
            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }
    }
}
