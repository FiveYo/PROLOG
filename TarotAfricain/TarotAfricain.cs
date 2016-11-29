using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TarotAfricain.Core;
using SbsSW.SwiPlCs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace TarotAfricain
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TarotAfricain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public const int WINDOWS_WIDTH = 983;
        public const int WINDOWS_HEIGHT = 888;
        public GameState gameState = GameState.StartMenu;
        private MouseState oldMouseState;

        Tapis tapis;
        GameButton newGameBtn;
        List<Joueur> joueurs;
        int nbCartes;
        int manche;
        int tour;

        public enum GameState
        {
            StartMenu = 0,
            GameOptions = 1,
            GameRunning =2
        }

        public TarotAfricain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WINDOWS_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOWS_HEIGHT;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Loading the prolog kbase
            if (!PlEngine.IsInitialized)
            {
                //string filename = @"C:\Users\Mathieu\Documents\Visual Studio 2015\Projects\TarotAfricain\TarotAfricain\Prolog\prolog.pro";
                string filename = Path.Combine(Content.RootDirectory + @"\Prolog\prolog.pro");
                // Debug only :
                // FileStream fs = File.Open(filename, FileMode.Open);



                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);
                PlQuery.PlCall("playGame([\"j1\",\"j2\"],3).");
                using (PlQuery q = new PlQuery("pointGame(X,Y)."))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Debug.Write(v["X"].ToString());
                        Debug.WriteLine(" : " + v["Y"].ToString());
                    }
                }
                PlEngine.PlCleanup();
            }


            tapis = new Core.Tapis();
            newGameBtn = new Core.GameButton("New Game");
            joueurs = new List<Joueur>();
            manche = 0;
            tour = 0;

            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here

            // Tapis (background)
            tapis.Texture = Content.Load<Texture2D>("tapis");
            tapis.Position = new Rectangle(0, 0, tapis.Texture.Width, tapis.Texture.Height);

            // Bouton nouveau jeu
            newGameBtn.Texture = Content.Load<Texture2D>("button");
            newGameBtn.font = Content.Load<SpriteFont>("DefaultFont");
            newGameBtn.Position = new Rectangle(WINDOWS_WIDTH / 2 - newGameBtn.Texture.Width / 2,
                                                WINDOWS_HEIGHT / 2 - newGameBtn.Texture.Height / 2,
                                                newGameBtn.Texture.Width, newGameBtn.Texture.Height);
            newGameBtn.PositionText = new Vector2(newGameBtn.Position.X + 180, newGameBtn.Position.Y + 50);

            //
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            graphics.PreferredBackBufferWidth = WINDOWS_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOWS_HEIGHT;

            MouseState mouseState = Mouse.GetState();
            KeyboardState kbState = Keyboard.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
            {
                int x = mouseState.X;
                int y = mouseState.Y;
                // Gestion click sur le bouton "New Game"
                if (newGameBtn.Position.X < x && x < (newGameBtn.Position.X + newGameBtn.Position.Width) &&
                    newGameBtn.Position.Y < y && y < (newGameBtn.Position.Y + newGameBtn.Position.Height) &&
                    gameState == GameState.StartMenu)
                {
                    newGameBtn.Dispose();
                    gameState += 1;
                    Actions.SetOptions();
                }

                // Gestion click sur le champ

            }

            oldMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            tapis.Draw(spriteBatch);
            newGameBtn.DrawObject(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
