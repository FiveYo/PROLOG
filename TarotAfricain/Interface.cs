using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarotAfricain.Core;

namespace TarotAfricain
{
    class Interface
    {
        public void creerPartie(List<Joueur> joueurs, int nbCartes)
        // demarre une partie avec les joueurs et le nombre de cartes passes en parametre
        {

        }
        public int getManche()
        // renvoie le numéro de la manche courante
        {
            return 1;
        }

        public int getTour()
        // renvoie le numéro du tour de la manche actuelle
        {
            return 1;
        }

        public List<Carte> getMain(string nomJoueur)
        // renvoie la main d'un joueur
        {
            List<Carte> main = new List<Carte>();
            main.Add(new Core.Carte("10 trefle"));
            main.Add(new Core.Carte("Q pique"));
            main.Add(new Core.Carte("K carreau"));
            main.Add(new Core.Carte("7 coeur"));
            main.Add(new Core.Carte("1 carreau"));
            return main;
        }

        public string carteJouee(string nomJoueur)
        // renvoie la carte jouee au Tour courant par nomJoueur
        {
            return "Q pique";
        }

        public bool isGameOver()
        // renvoie true si la partie est finie, false sinon
        {
            return false;
        }

        public string getJoueurGagnantTour()
        // renvoie le nom du joueur gagnant le tour courant
        {
            return "Michel";
        }

        public int getPoints(string nomJoueur)
        // renvoie les points du joueur *nomJoueur*
        {
            return 2;
        }

        public int getParis(string nomJoueur)
        // renvoie le paris du joueur "nomJoueur* pour le Tour courant
        {
            return 0;
        }
    }
}
