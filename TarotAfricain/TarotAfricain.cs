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
        private KbHandler kb = new KbHandler();
        private Interface itf = new Interface();

        Tapis tapis;
        GameButton newGameBtn;
        GameButton validerBtn;
        GameObject nbPlayerWindow;
        GameObject namePlayersWindow;
        Textbox nbPlayersTextbox;
        List<Joueur> joueurs;
        int nbJoueurs;
        int nbCartes;
        int manche;
        int tour;

        public enum GameState
        {
            StartMenu = 0,
            ChooseNbPlayers = 1,
            ChooseNames = 2,
            PlayGame = 3
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
            newGameBtn = new Core.GameButton();
            validerBtn = new Core.GameButton();
            nbPlayerWindow = new Core.GameObject();
            nbPlayersTextbox = new Core.Textbox();
            namePlayersWindow = new Core.GameObject();

            joueurs = new List<Joueur>();
            manche = 0;
            tour = 0;
            nbJoueurs = 0;
            nbCartes = 5;

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

            // Tapis (background)
            tapis.Texture = Content.Load<Texture2D>("tapis");
            tapis.Position = new Rectangle(0, 0, tapis.Texture.Width, tapis.Texture.Height);

            // Fenetre de selection du nombre de joueurs
            nbPlayerWindow.Texture = Content.Load<Texture2D>("nbPlayersWindow");
            nbPlayerWindow.Position = new Rectangle(WINDOWS_WIDTH / 2 - nbPlayerWindow.Texture.Width / 2,
                                                    WINDOWS_HEIGHT / 2 - nbPlayerWindow.Texture.Height / 2,
                                                    nbPlayerWindow.Texture.Width, nbPlayerWindow.Texture.Height);

            //Fenetre de selection des noms de joueurs
            namePlayersWindow.Texture = Content.Load<Texture2D>("nomPlayersWindow");
            namePlayersWindow.Position = new Rectangle(WINDOWS_WIDTH / 2 - namePlayersWindow.Texture.Width / 2,
                                                       WINDOWS_HEIGHT / 2 - namePlayersWindow.Texture.Height / 2,
                                                       namePlayersWindow.Texture.Width, namePlayersWindow.Texture.Height);

            // Bouton nouveau jeu
            newGameBtn.Texture = Content.Load<Texture2D>("newGameBtn");
            //newGameBtn.font = Content.Load<SpriteFont>("DefaultFont");
            newGameBtn.Position = new Rectangle(WINDOWS_WIDTH / 2 - newGameBtn.Texture.Width / 2,
                                                WINDOWS_HEIGHT / 2 - newGameBtn.Texture.Height / 2,
                                                newGameBtn.Texture.Width, newGameBtn.Texture.Height);
            //newGameBtn.PositionText = new Vector2(newGameBtn.Position.X + 180, newGameBtn.Position.Y + 50);

            validerBtn.Texture = Content.Load<Texture2D>("validerBtn");
            validerBtn.Position = new Rectangle(WINDOWS_WIDTH / 2 - validerBtn.Texture.Width / 2,
                                                namePlayersWindow.Position.Y + namePlayersWindow.Position.Width + 20,
                                                validerBtn.Texture.Width, validerBtn.Texture.Height);

            // Textbox pour saisir le nombre de joueurs
            nbPlayersTextbox.font = Content.Load<SpriteFont>("DefaultFont");
            nbPlayersTextbox.Position = new Vector2(nbPlayerWindow.Position.X + 190, nbPlayerWindow.Position.Y + 210);
            //nbPlayersTextbox.text = "test";
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
            kb.Update();

            int x = mouseState.X;
            int y = mouseState.Y;

            // Écran Nouveau Jeu
            if (gameState == GameState.StartMenu)
            {
                // Si click sur le bouton nouveau jeu
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    if (newGameBtn.Position.X < x && x < (newGameBtn.Position.X + newGameBtn.Position.Width) &&
                        newGameBtn.Position.Y < y && y < (newGameBtn.Position.Y + newGameBtn.Position.Height))
                    {
                        gameState += 1;
                    }
                }

            // Écran Choix nb joueurs
            } else if (gameState == GameState.ChooseNbPlayers)
            {
                // Gestion de la saisie
                if (kb.text.Length > 1)
                {
                    kb.text = kb.text[0].ToString();
                }
                nbPlayersTextbox.text = kb.text + "|";
                // Si click sur le bouton valider
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    if (validerBtn.Position.X < x && x < (validerBtn.Position.X + validerBtn.Position.Width) &&
                        validerBtn.Position.Y < y && y < (validerBtn.Position.Y + validerBtn.Position.Height))
                    {
                        // Si la chaine saisie est bien un entier (2, 3, 4 ou 5) on passe à l'étape suivante.
                        int testCast;
                        if (Int32.TryParse(kb.text, out testCast) &&
                            (testCast == 2 || testCast == 3 || testCast == 4 || testCast == 5))
                        {
                            nbJoueurs = testCast;
                            // Création des joueurs
                            int margeFenetre = 40;
                            int margeEntete = 50;
                            int heightNameField = Math.Min((namePlayersWindow.Position.Height - (margeEntete + 2 * margeFenetre + nbJoueurs * 10)) / nbJoueurs, 50);
                            for (int i=0; i<nbJoueurs; i++)
                            {
                                joueurs.Add(new Joueur("player" + (i+1)));
                                joueurs[i].nameField.font = Content.Load<SpriteFont>("DefaultFont");
                                joueurs[i].nameField.Texture = Content.Load<Texture2D>("textbox_large");
                                joueurs[i].nameField.Position = new Rectangle(namePlayersWindow.Position.X + margeFenetre,
                                                                              namePlayersWindow.Position.Y + margeFenetre + margeEntete + (i * (heightNameField + 10)),
                                                                              namePlayersWindow.Position.Width - margeFenetre * 2,
                                                                              heightNameField);
                                joueurs[i].nameField.PositionText = new Vector2(joueurs[i].nameField.Position.X + 10, joueurs[i].nameField.Position.Y + 10);
                            }
                            itf.creerPartie(joueurs, nbCartes);
                            gameState += 1;
                        }
                    }
                }

            // Écran Choix noms joueurs
            } else if (gameState == GameState.ChooseNames)
            {
                // Affichage des noms
                foreach (var j in joueurs)
                {
                    j.nameField.text = j.nom;
                }

                // Si click sur le bouton valider
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    if (validerBtn.Position.X < x && x < (validerBtn.Position.X + validerBtn.Position.Width) &&
                        validerBtn.Position.Y < y && y < (validerBtn.Position.Y + validerBtn.Position.Height))
                    {
                        bool nomsValides = true;
                        foreach (var j in joueurs)
                        {
                            if (j.nameField.text.Length == 0)
                            {
                                nomsValides = false;
                            }
                        }
                        if (nomsValides)
                        {
                            foreach (Joueur j in joueurs)
                            {
                                j.nom = j.nameField.text;
                            }
                            gameState += 1;
                        }
                        
                    }
                }

            // Écran de Jeu
            } else if (gameState == GameState.PlayGame)
            {
                if (!itf.isGameOver())
                {
                    manche = itf.getManche();
                    tour = itf.getTour();
                    foreach (Joueur j in joueurs)
                    {
                        j.main = itf.getMain(j.nom);
                        j.points = itf.getPoints(j.nom);
                        j.paris = itf.getParis(j.nom);
                        j.carteJouee = itf.carteJouee(j.nom);
                    }
                }
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
            if (gameState == GameState.StartMenu)
            {
                newGameBtn.Draw(spriteBatch);
            }
            else if (gameState == GameState.ChooseNbPlayers)
            {
                nbPlayerWindow.Draw(spriteBatch);
                nbPlayersTextbox.DrawString(spriteBatch);
                validerBtn.Draw(spriteBatch);
            }
            else if (gameState == GameState.ChooseNames)
            {
                namePlayersWindow.Draw(spriteBatch);
                foreach (var j in joueurs)
                {
                    j.nameField.DrawObject(spriteBatch);
                }
                validerBtn.Draw(spriteBatch);
            }
            else if (gameState == GameState.PlayGame)
            {
                foreach (Joueur j in joueurs)
                {
                    //TODO: afficher les mains, paris, points des joueurs
                }

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
