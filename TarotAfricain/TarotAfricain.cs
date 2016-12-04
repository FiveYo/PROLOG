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
        public const int CARTE_WIDTH = 90;
        public const int CARTE_HEIGHT = 113;
        public const int JOUEUR_WIDTH = (WINDOWS_WIDTH - 40 ) / 2;
        public const int JOUEUR_HEIGHT = CARTE_HEIGHT + 50;
        public GameState gameState = GameState.StartMenu;
        private MouseState oldMouseState;
        private KbHandler kb = new KbHandler();
        private EventsHandler eh;
        private GenerateEvents generateEvents;

        GameObject tapis;
        GameObject tapisParis;
        GameObject tableauPoints;
        GameObject newGameBtn;
        GameObject validerBtn;
        GameObject nbPlayerWindow;
        GameObject namePlayersWindow;
        Textbox nbPlayersTextbox;
        Textbox numManche;
        Textbox numTour;
        public List<Joueur> joueurs;
        public int nbJoueurs;
        public int nbCartes;
        public int manche;
        public int tour;
        public bool IsGameOver;

        public bool debugVariable = true;

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
            eh = new EventsHandler(this);
            generateEvents = new GenerateEvents();
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
            tapis = new Core.GameObject();
            tapisParis = new Core.GameObject();
            tableauPoints = new Core.GameObject();
            newGameBtn = new Core.GameObject();
            validerBtn = new Core.GameObject();
            nbPlayerWindow = new Core.GameObject();
            nbPlayersTextbox = new Core.Textbox();
            namePlayersWindow = new Core.GameObject();
            numManche = new Core.Textbox();
            numTour = new Core.Textbox();

            joueurs = new List<Joueur>();
            manche = 0;
            tour = 0;
            nbJoueurs = 0;
            nbCartes = 5;
            IsGameOver = false;

            this.IsMouseVisible = true;
            base.Initialize();

            InterfacePl pl = new InterfacePl();
            //pl.StartGame(new List<string> { "j1", "j2" }, new List<int> { 1, 1 }, 3);
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

            // Tapis des paris
            tapisParis.Texture = Content.Load<Texture2D>("tapisParis");
            //tapisParis.Position = new Rectangle(tapis.Position.Width / 2 - 50, 30, 250, tapis.Position.Height - 60);
            tapisParis.Position = new Rectangle(tapis.Position.Width / 2 - 50, 25, 100, tapisParis.Texture.Height);

            // Tableau des points
            tableauPoints.Texture = Content.Load<Texture2D>("tableauPoints");
            //tableauPoints.Position = new Rectangle(tapisParis.Position.X + tapisParis.Position.Width + 20, tapisParis.Position.Y, 100, tapisParis.Position.Height);
            tableauPoints.Position = new Rectangle(tapisParis.Position.X + tapisParis.Position.Width + 20, tapisParis.Position.Y, tableauPoints.Texture.Width, tableauPoints.Texture.Height);

            // Textbox pour la manche et le tour
            numManche.font = Content.Load<SpriteFont>("DefaultFont");
            numTour.font = Content.Load<SpriteFont>("DefaultFont");
            numManche.Position = new Vector2(tableauPoints.Position.X + tableauPoints.Position.Width + 50, 30);
            numTour.Position = new Vector2(numManche.Position.X, numManche.Position.Y + 30);

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

            // Bouton Valider
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
                                joueurs[i].nameField.text = joueurs[i].nom;
                                joueurs[i].nameField.font = Content.Load<SpriteFont>("DefaultFont");
                                joueurs[i].iaField.font = Content.Load<SpriteFont>("DefaultFont");
                                joueurs[i].font = Content.Load<SpriteFont>("DefaultFont");
                                joueurs[i].pointField.font = Content.Load<SpriteFont>("DefaultFont");
                                joueurs[i].parisField.font = Content.Load<SpriteFont>("DefaultFont");
                                joueurs[i].nameField.Texture = Content.Load<Texture2D>("textbox_large");
                                joueurs[i].iaField.Texture = Content.Load<Texture2D>("textbox_small");
                                joueurs[i].nameField.Position = new Rectangle(namePlayersWindow.Position.X + margeFenetre,
                                                                              namePlayersWindow.Position.Y + margeFenetre + margeEntete + (i * (heightNameField + 10)),
                                                                              namePlayersWindow.Position.Width - margeFenetre * 2 - 50,
                                                                              heightNameField);
                                joueurs[i].iaField.Position = new Rectangle(joueurs[i].nameField.Position.X + 5 + joueurs[i].nameField.Position.Width,
                                                                            joueurs[i].nameField.Position.Y,
                                                                            50,
                                                                            heightNameField);
                                joueurs[i].nameField.PositionText = new Vector2(joueurs[i].nameField.Position.X + 10, joueurs[i].nameField.Position.Y + 10);
                                joueurs[i].iaField.PositionText = new Vector2(joueurs[i].iaField.Position.X + 20, joueurs[i].iaField.Position.Y + 10);
                                if (nbJoueurs > 4)
                                {
                                    joueurs[i].Position = new Rectangle(50, (JOUEUR_HEIGHT + 10) * i + 30, JOUEUR_WIDTH, JOUEUR_HEIGHT);
                                } else
                                {
                                    joueurs[i].Position = new Rectangle(50, (JOUEUR_HEIGHT + 10) * (i+1) + 30, JOUEUR_WIDTH, JOUEUR_HEIGHT);
                                }
                                
                                joueurs[i].PositionText = new Vector2(joueurs[i].Position.X, joueurs[i].Position.Y + JOUEUR_HEIGHT / 3);
                                joueurs[i].parisField.Position = new Vector2(tableauPoints.Position.X + 50, joueurs[i].PositionText.Y);
                                joueurs[i].pointField.Position = new Vector2(tableauPoints.Position.X + 20 + (tableauPoints.Texture.Width / 2), joueurs[i].PositionText.Y);
                            }
                            gameState += 1;
                        }
                    }
                }

            // Écran Choix noms joueurs
            } else if (gameState == GameState.ChooseNames)
            {
                foreach (var j in joueurs)
                {
                    // Si click dans le champ du joueur
                    if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                    {
                        if (j.nameField.Position.X < x && x < (j.nameField.Position.X + j.nameField.Position.Width) &&
                            j.nameField.Position.Y < y && y < (j.nameField.Position.Y + j.nameField.Position.Height))
                        {
                            // On vide le champ cliqué
                            kb.text = "";
                            // On deselectionne tous les joueurs
                            foreach (Joueur jr in joueurs)
                            {
                                // Si le joueur etait precedemment selectionne on supprime le curseur de texte
                                if (jr.selectionne)
                                {
                                    jr.nameField.text = jr.nameField.text.Substring(0, jr.nameField.text.Length - 1);
                                }
                                jr.selectionne = false;
                            }
                            // On selectionne le bon
                            j.selectionne = true;
                        }
                    }

                    // Si click dans le champs IA d'un joueur
                    if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                    {
                        if (j.iaField.Position.X < x && x < (j.iaField.Position.X + j.iaField.Position.Width) &&
                            j.iaField.Position.Y < y && y < (j.iaField.Position.Y + j.iaField.Position.Height))
                        {
                            // Si le champ clické est vide on le set à x
                            if (j.iaField.text == "")
                            {
                                j.iaField.text = "x";
                            } else
                            {
                                j.iaField.text = "";
                            }
                        }
                    }

                    // Gestion de la saisie
                    if (kb.text.Length > 10)
                    {
                        kb.text = kb.text.Substring(0, 10);
                    }
                    if (j.selectionne)
                    {
                        j.nameField.text = kb.text.ToLower() + "|";
                    }
                    
                }
                
                // Si click sur le bouton valider
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    if (validerBtn.Position.X < x && x < (validerBtn.Position.X + validerBtn.Position.Width) &&
                        validerBtn.Position.Y < y && y < (validerBtn.Position.Y + validerBtn.Position.Height))
                    {
                        bool nomsValides = true;
                        List<String> noms = new List<string>();

                        foreach (var j in joueurs)
                        {
                            // On supprime le curseur de texte du dernier joueur selectionne
                            if (j.selectionne)
                            {
                                j.nameField.text = j.nameField.text.Substring(0, j.nameField.text.Length - 1);
                                j.selectionne = false;
                            }

                            // On verifie qu'aucun nom ne soit vide
                            if (j.nameField.text.Length == 0)
                            {
                                nomsValides = false;
                            }

                            // On ajoute le nom à la liste de noms
                            noms.Add(j.nameField.text);
                        }

                        // On verifie que tous les noms soient bien distincts
                        for (int i=0; i<noms.Count; i++)
                        {
                            string name = noms[i];
                            noms.RemoveAt(i);
                            if (noms.Contains(name))
                            {
                                nomsValides = false;
                            }
                            noms.Insert(i, name);
                        }

                        if (nomsValides)
                        {
                            foreach (Joueur j in joueurs)
                            {
                                j.nom = j.nameField.text;
                                j.text = j.nom;
                                if (j.iaField.text == "")
                                {
                                    j.IsIA = false;
                                }
                                else
                                {
                                    j.IsIA = true;
                                    j.text += "\n(IA)";
                                }
                            }
                            gameState += 1;
                        }
                        
                    }
                }

            // Écran de Jeu
            } else if (gameState == GameState.PlayGame)
            {
                // DEBUG : Test un envoi d'événements -------------------------------------------
                if (debugVariable)
                {
                    //GenerateEvents testEvents = new GenerateEvents();
                    eh.Subscribe(generateEvents);
                    generateEvents.Send();
                    generateEvents.mancheChanged(55);
                    debugVariable = false;
                }
                // ------------------------------------------------------------------------------
                
                if (!IsGameOver)
                {
                    // Redefinition de la zone texte correspondant au numero de tour et de manche
                    numManche.text = "Manche : " + manche.ToString();
                    numTour.text = "Tour : " + tour.ToString();

                    foreach (Joueur j in joueurs)
                    {
                        // Redefinition du dessin de chaque carte de la main
                        if (j.main != null)
                        {
                            foreach (Carte c in j.main)
                            {
                                int i = j.main.IndexOf(c);
                                c.Position = new Rectangle(j.Position.X + 150 + i * 25, j.Position.Y + 20, CARTE_WIDTH, CARTE_HEIGHT);
                                c.Texture = Content.Load<Texture2D>("Cartes/" + c.nom);
                            }
                        }
                        
                        // Redéfinition du dessin de la carte jouée
                        if (j.carteJouee != null)
                        {
                            j.carteJouee.Position = new Rectangle(tapisParis.Position.X + 5, j.Position.Y + 20, CARTE_WIDTH, CARTE_HEIGHT);
                            j.carteJouee.Texture = Content.Load<Texture2D>("Cartes/" + j.carteJouee.nom);
                        }
                        

                        // Redefinition de la zone texte correspondant au paris et au point
                        j.parisField.text = j.paris.ToString();
                        j.pointField.text = String.Format("{0} - {1}", j.pointsManche.ToString(), j.pointsGame.ToString());
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
                    j.iaField.DrawObject(spriteBatch);
                }
                validerBtn.Draw(spriteBatch);
            }
            else if (gameState == GameState.PlayGame)
            {
                tapisParis.Draw(spriteBatch);
                tableauPoints.Draw(spriteBatch);
                numManche.DrawString(spriteBatch);
                numTour.DrawString(spriteBatch);

                foreach (Joueur j in joueurs)
                {
                    j.DrawString(spriteBatch);
                    j.parisField.DrawWhiteString(spriteBatch);
                    j.pointField.DrawWhiteString(spriteBatch);
                    //TODO: afficher les mains, paris, points des joueurs
                    if (j.main != null)
                    {
                        foreach (Carte c in j.main)
                        {
                            c.Draw(spriteBatch);
                        }
                    }
                    if (j.carteJouee != null)
                    {
                        j.carteJouee.Draw(spriteBatch);
                    }
                    
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
