using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TarotAfricain.Core;

namespace TarotAfricain
{
    public class NouveauTour : EventArgs
    {
        public int tour;
        public NouveauTour(int tour)
        {
            this.tour = tour;
        }
    }

    public class NouvelleManche : EventArgs
    {
        public int manche;
        public NouvelleManche(int manche)
        {
            this.manche = manche;
        }
    }

    public class NouvelleMain : EventArgs
    {
        public string joueur;
        public List<Carte> main;
        public NouvelleMain(string joueur, List<Carte> main)
        {
            this.joueur = joueur;
            this.main = main;
        }
    }

    public class NouveauParis : EventArgs
    {
        public string joueur;
        public int paris;
        public NouveauParis(string joueur, int paris)
        {
            this.joueur = joueur;
            this.paris = paris;
        }
    }

    public class NouvelleCarteJouee : EventArgs
    {
        public string joueur;
        public Carte carteJouee;
        public NouvelleCarteJouee(string joueur, Carte carteJouee)
        {
            this.joueur = joueur;
            this.carteJouee = carteJouee;
        }
    }

    public class NouveauxPoints : EventArgs
    {
        public string joueur;
        public int points;
        public NouveauxPoints (string joueur, int points)
        {
            this.joueur = joueur;
            this.points = points;
        }
    }

    public class NouveauGagnantTour : EventArgs
    {
        public string joueur;
        public NouveauGagnantTour(string joueur)
        {
            this.joueur = joueur;
        }
    }

    public class JoueurArg : EventArgs
    {
        public string nomJoueur;
        public Joueur joueur;
        public JoueurArg(string joueur)
        {
            this.nomJoueur = joueur;
            this.joueur = null;
        }
    }

    public class GenerateEvents
    {
        public delegate void ChangedMancheEventHandler(object sender, NouvelleManche e);
        public delegate void ChangedTourEventHandler(object sender, NouveauTour e);
        public delegate void ChangedMainEventHandler(object sender, NouvelleMain e);
        public delegate void ChangedParisEventHandler(object sender, NouveauParis e);
        public delegate void ChangedCarteJoueeEventHandler(object sender, NouvelleCarteJouee e);
        public delegate void ChangedPointsEventHandler(object sender, NouveauxPoints e);
        public delegate void GameOverEventHandler(object sender, EventArgs e);
        public delegate void ChangedGagnantTourEventHandler(object sender, NouveauGagnantTour e);
        public delegate void GetCarteJoueeJoueurEventHandler(object sender, JoueurArg e);
        public delegate void GetPariEventHandler(object sender, JoueurArg e);
        public delegate void MancheEndEventHandler(object sender, EventArgs e);
        public event ChangedMancheEventHandler OnMancheChanged;
        public event ChangedTourEventHandler OnTourChanged;
        public event ChangedMainEventHandler OnMainChanged;
        public event ChangedParisEventHandler OnParisChanged;
        public event ChangedCarteJoueeEventHandler OnCarteJoueeChanged;
        public event ChangedPointsEventHandler OnPointsMancheChanged;
        public event ChangedPointsEventHandler OnPointsGameChanged;
        public event GameOverEventHandler OnGameOver;
        public event ChangedGagnantTourEventHandler OnGagnantTour;
        public event GetCarteJoueeJoueurEventHandler OnGetCarteJouee;
        public event MancheEndEventHandler OnMancheEnd;
        public event GetPariEventHandler OnGetPari;

        public void mancheChanged()
        {
            if (OnMancheChanged != null)
            {
                OnMancheChanged(this, new NouvelleManche(0));
            }
        }

        public void tourChanged(int tour)
        {
            if (OnTourChanged != null)
            {
                OnTourChanged(this, new NouveauTour(tour));
            }
        }

        public void mainChanged(string joueur, List<string> main)
        {
            if (OnMainChanged != null)
            {
                List<Carte> cartes = new List<Carte>();
                foreach (string name in main)
                {
                    cartes.Add(new Carte(name));
                }
                OnMainChanged(this, new NouvelleMain(joueur, cartes));
            }
        }

        public void parisChanged(string joueur, int paris)
        {
            if (OnParisChanged != null)
            {
                OnParisChanged(this, new NouveauParis(joueur, paris));
            }
        }

        public void carteJoueeChanged(string joueur, string carteName)
        {
            if (OnCarteJoueeChanged != null)
            {
                Carte carte = new Core.Carte(carteName);
                OnCarteJoueeChanged(this, new NouvelleCarteJouee(joueur, carte));
            }
        }

        public void pointsMancheChanged(string joueur, int points)
        {
            if (OnPointsMancheChanged != null)
            {
                OnPointsMancheChanged(this, new NouveauxPoints(joueur, points));
            }
        }

        public void pointsGameChanged(string joueur, int points)
        {
            if (OnPointsGameChanged != null)
            {
                OnPointsGameChanged(this, new NouveauxPoints(joueur, points));
            }
        }

        public void gameOver()
        {
            if (OnGameOver != null)
            {
                OnGameOver(this, EventArgs.Empty);
            }
        }

        public void gagnantTour(string joueur)
        {
            if (OnGagnantTour != null)
            {
                OnGagnantTour(this, new NouveauGagnantTour(joueur));
            }
        }

        public void mancheEnd()
        {
            if (OnMancheEnd != null)
            {
                OnMancheEnd(this, EventArgs.Empty);
            }
        }

        public JoueurArg getCarteJouee(string joueur)
        {
            JoueurArg joueurArg = new JoueurArg(joueur);
            if (OnGetCarteJouee != null)
            {
                OnGetCarteJouee(this, joueurArg);
            }
            return joueurArg;
        }

        public JoueurArg getPari(string joueur)
        {
            JoueurArg joueurArg = new JoueurArg(joueur);
            if (OnGetPari != null)
            {
                OnGetPari(this, joueurArg);
            }
            return joueurArg;
        }

        public void Send()
        {
            // Essaye d'envoyer tous les signaux disponibles
            if (OnTourChanged != null)
            {
                OnTourChanged(this, new NouveauTour(42));
            }
            if (OnMancheChanged != null)
            {
                OnMancheChanged(this, new NouvelleManche(42));
            }
            if (OnMainChanged != null)
            {
                List<Carte> main1 = new List<Carte>();
                main1.Add(new Core.Carte("1"));
                main1.Add(new Core.Carte("2"));
                main1.Add(new Core.Carte("3"));
                main1.Add(new Core.Carte("4"));

                List<Carte> main2 = new List<Carte>();
                main2.Add(new Core.Carte("5"));
                main2.Add(new Core.Carte("6"));
                main2.Add(new Core.Carte("7"));
                main2.Add(new Core.Carte("8"));

                OnMainChanged(this, new NouvelleMain("player1", main1));
                OnMainChanged(this, new NouvelleMain("player2", main2));
            }
            if (OnParisChanged != null)
            {
                OnParisChanged(this, new NouveauParis("player1", 1));
                OnParisChanged(this, new NouveauParis("player2", 2));
            }
            if (OnCarteJoueeChanged != null)
            {
                Carte carte1 = new Core.Carte("1");
                Carte carte2 = new Core.Carte("5");
                OnCarteJoueeChanged(this, new NouvelleCarteJouee("player1", carte1));
                OnCarteJoueeChanged(this, new NouvelleCarteJouee("player2", carte2));
            }
            if (OnPointsGameChanged != null)
            {
                OnPointsGameChanged(this, new NouveauxPoints("player1", 5));
                OnPointsGameChanged(this, new NouveauxPoints("player2", 7));
            }
            if (OnPointsMancheChanged != null)
            {
                OnPointsMancheChanged(this, new NouveauxPoints("player1", 0));
                OnPointsMancheChanged(this, new NouveauxPoints("player2", 3));
            }
            if (OnGameOver != null)
            {
                //OnGameOver(this, EventArgs.Empty);
            }
            if (OnGagnantTour != null)
            {
                //OnGagnantTour(this, new NouveauGagnantTour("player1", 42));
            }
        }
    }
}
