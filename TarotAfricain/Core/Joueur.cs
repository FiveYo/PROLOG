using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain.Core
{
    class Joueur : GameObject
    {
        public ObjectWithText nameField;
        public Textbox pointField;
        public Textbox parisField;
        public GameObject mainField;
        public string _nom;
        public string nom
        {
            get { return this._nom; }
            set { _nom = this.nameField.text; }
        }
        public List<Carte> main;
        public int points;
        public string carteJouee;
        public int paris;
        public bool IsIA;

        public Joueur(string name)
        {
            this.nameField = new Core.ObjectWithText();
            _nom = name;
        }
    }
}
