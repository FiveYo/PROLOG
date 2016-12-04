using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain.Core
{
    public class Carte : GameObject
    {
        // public string valeur;
        // public string couleur;
        public string nom;

        public Carte(string nom)
        {
            this.nom = nom;
        }
    }
}
