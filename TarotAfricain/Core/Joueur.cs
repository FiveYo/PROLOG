using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain.Core
{
    class Joueur : GameObject
    {
        public string nom;
        public List<Carte> main;
        public int paris;

        public Joueur(string nom)
        {
            this.nom = nom;
        }
    }
}
