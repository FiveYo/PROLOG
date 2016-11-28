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
        Tapis tapis;
        List<Joueur> joueurs;
        int nbCartes;
        int manche;
        int tour;

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
                string filename = @"..\..\..\..\Prolog\prolog.pro";
                // Debug only :
                //FileStream fs = File.Open(filename, FileMode.Open);

                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);
                PlQuery.PlCall("assert(carte(1, '7 coeur', 0))");
                PlEngine.PlCleanup();
            }


            tapis = new Core.Tapis();
            joueurs = new List<Joueur>();
            manche = 0;
            tour = 0;
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
            tapis.Texture = Content.Load<Texture2D>("tapis");
            tapis.Position = new Vector2(0, 0);
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
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
