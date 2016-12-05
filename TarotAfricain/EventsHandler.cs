using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TarotAfricain.Core;
using TarotAfricain;

namespace TarotAfricain
{
    public class EventsHandler
    {
        TarotAfricain jeu;
        public EventsHandler(TarotAfricain jeu)
        {
            this.jeu = jeu;
        }
        public void Subscribe(GenerateEvents g)
        {
            g.OnTourChanged += new GenerateEvents.ChangedTourEventHandler(OnTourChangedHandler);
            g.OnMancheChanged += new GenerateEvents.ChangedMancheEventHandler(OnMancheChangedHandler);
            g.OnCarteJoueeChanged += new GenerateEvents.ChangedCarteJoueeEventHandler(OnCarteJoueeChangedHandler);
            g.OnGameOver += new GenerateEvents.GameOverEventHandler(OnGameOverHandler);
            g.OnMainChanged += new GenerateEvents.ChangedMainEventHandler(OnMainChangedHandler);
            g.OnParisChanged += new GenerateEvents.ChangedParisEventHandler(OnParisChangedHandler);
            g.OnPointsGameChanged += new GenerateEvents.ChangedPointsEventHandler(OnPointGameChangedHandler);
            g.OnPointsMancheChanged += new GenerateEvents.ChangedPointsEventHandler(OnPointMancheChangedHandler);
            g.OnGagnantTour += new GenerateEvents.ChangedGagnantTourEventHandler(OnGagnantTourHandler);
            g.OnGetCarteJouee += new GenerateEvents.GetCarteJoueeJoueurEventHandler(OnGetCarteJoueeHandler);
        }
        public void OnTourChangedHandler(object sender, NouveauTour e)
        {
            jeu.tour = e.tour;
        }

        public void OnMancheChangedHandler(object sender, NouvelleManche e)
        {
            jeu.manche = e.manche;
        }

        public void OnPointGameChangedHandler(object sender, NouveauxPoints e)
        {
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.joueur));
            j.pointsGame = e.points;
        }
        public void OnPointMancheChangedHandler(object sender, NouveauxPoints e)
        {
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.joueur));
            j.pointsManche = e.points;
        }
        public void OnParisChangedHandler(object sender, NouveauParis e)
        {
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.joueur));
            j.paris = e.paris;
        }
        public void OnCarteJoueeChangedHandler(object sender, NouvelleCarteJouee e)
        {
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.joueur));
            j.carteJouee = e.carteJouee;            
        }
        public void OnMainChangedHandler(object sender, NouvelleMain e)
        {
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.joueur));
            j.main = e.main;
        }
        public void OnGameOverHandler(object sender, EventArgs e)
        {
            jeu.IsGameOver = true;
        }
        public void OnGagnantTourHandler(object sender, NouveauGagnantTour e)
        {
            // Fin du tour, on jette les cartes du pli et on affiche le gagnant dans une boite de dialogue
            foreach (Joueur jr in jeu.joueurs)
            {
                jr.carteJouee = null;
            }
            jeu.dialogueBox.text = String.Format("{0} gagne le tour {1} !", e.joueur, jeu.tour.ToString());
            jeu.gameState = TarotAfricain.GameState.DialogueBox;
        }
        public void OnGetCarteJoueeHandler(object sender, JoueurArg e)
        {
            jeu.dialogueBox.text = String.Format("{0} choisissez une carte a jouer:", e.nomJoueur);
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.nomJoueur));
            if (j.IsIA == true)
            {
                throw new LeJoueurQuiDoitPiocherEstUneIaException(String.Format("{0} est une IA", j.nom));
            }
            j.carteJouee = null;
            e.joueur = j;
            // On selectionne le joueur et on deselectionne tous les autres
            foreach (Joueur jr in jeu.joueurs)
            {
                jr.selectionne = false;
            }
            j.selectionne = true;
            jeu.gameState = TarotAfricain.GameState.ChooseCard;
        }
    }
}
