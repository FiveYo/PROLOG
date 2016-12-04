﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarotAfricain.Core;

namespace TarotAfricain
{
    public class EventsHandler
    {
        TarotAfricain jeu;
        public EventsHandler(TarotAfricain jeu)
        {
            this.jeu = jeu;
        }
        public void Subscribe(GenerateTestEvents g)
        {
            g.OnTourChanged += new GenerateTestEvents.ChangedTourEventHandler(OnTourChangedHandler);
            g.OnMancheChanged += new GenerateTestEvents.ChangedMancheEventHandler(OnMancheChangedHandler);
            g.OnCarteJoueeChanged += new GenerateTestEvents.ChangedCarteJoueeEventHandler(OnCarteJoueeChangedHandler);
            g.OnGameOver += new GenerateTestEvents.GameOverEventHandler(OnGameOverHandler);
            g.OnMainChanged += new GenerateTestEvents.ChangedMainEventHandler(OnMainChangedHandler);
            g.OnParisChanged += new GenerateTestEvents.ChangedParisEventHandler(OnParisChangedHandler);
            g.OnPointsChanged += new GenerateTestEvents.ChangedPointsEventHandler(OnPointChangedHandler);
        }
        public void OnTourChangedHandler(object sender, NouveauTour e)
        {
            jeu.tour += e.tour;
        }

        public void OnMancheChangedHandler(object sender, NouvelleManche e)
        {
            jeu.manche += e.manche;
        }

        public void OnPointChangedHandler(object sender, NouveauxPoints e)
        {
            Joueur j = jeu.joueurs.Find(v => v.nom.Equals(e.joueur));
            j.points = e.points;
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
    }
}
