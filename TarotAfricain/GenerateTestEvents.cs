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

    public class GenerateTestEvents
    {
        public delegate void ChangedMancheEventHandler(object sender, NouvelleManche e);
        public delegate void ChangedTourEventHandler(object sender, NouveauTour e);
        public delegate void ChangedMainEventHandler(object sender, NouvelleMain e);
        public delegate void ChangedParisEventHandler(object sender, NouveauParis e);
        public delegate void ChangedCarteJoueeEventHandler(object sender, NouvelleCarteJouee e);
        public delegate void ChangedPointsEventHandler(object sender, NouveauxPoints e);
        public delegate void GameOverEventHandler(object sender, EventArgs e);
        public event ChangedMancheEventHandler OnMancheChanged;
        public event ChangedTourEventHandler OnTourChanged;
        public event ChangedMainEventHandler OnMainChanged;
        public event ChangedParisEventHandler OnParisChanged;
        public event ChangedCarteJoueeEventHandler OnCarteJoueeChanged;
        public event ChangedPointsEventHandler OnPointsChanged;
        public event GameOverEventHandler OnGameOver;

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
                main1.Add(new Core.Carte("1 trefle"));
                main1.Add(new Core.Carte("1 pique"));
                main1.Add(new Core.Carte("1 carreau"));
                main1.Add(new Core.Carte("1 coeur"));

                List<Carte> main2 = new List<Carte>();
                main2.Add(new Core.Carte("7 trefle"));
                main2.Add(new Core.Carte("7 pique"));
                main2.Add(new Core.Carte("7 carreau"));
                main2.Add(new Core.Carte("7 coeur"));

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
                Carte carte1 = new Core.Carte("1 coeur");
                Carte carte2 = new Core.Carte("7 coeur");
                OnCarteJoueeChanged(this, new NouvelleCarteJouee("player1", carte1));
                OnCarteJoueeChanged(this, new NouvelleCarteJouee("player2", carte2));
            }
            if (OnPointsChanged != null)
            {
                OnPointsChanged(this, new NouveauxPoints("player1", 5));
                OnPointsChanged(this, new NouveauxPoints("player2", 7));
            }
            if (OnGameOver != null)
            {
                //OnGameOver(this, EventArgs.Empty);
            }
        }
    }
}
