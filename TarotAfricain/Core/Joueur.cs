using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain.Core
{
    public class Joueur : ObjectWithText
    {
        public ObjectWithText nameField;
        public ObjectWithText iaField;
        public Textbox pointField;
        public Textbox parisField;
        public string _nom;
        public string nom
        {
            get { return this._nom; }
            set { _nom = this.nameField.text; }
        }
        public List<Carte> main;
        public int pointsManche;
        public int pointsGame;
        public Carte carteJouee;
        public int paris;
        public bool IsIA;
        public bool selectionne;

        public Joueur(string name)
        {
            this.nameField = new Core.ObjectWithText();
            this.iaField = new Core.ObjectWithText();
            this.pointField = new Core.Textbox();
            this.parisField = new Core.Textbox();
            _nom = name;
            selectionne = false;
            IsIA = true;
        }
    }
}
