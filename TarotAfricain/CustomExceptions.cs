using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain
{
    class LeJoueurQuiDoitPiocherEstUneIaException : Exception
    {
        string message;
        public LeJoueurQuiDoitPiocherEstUneIaException(string msg)
        {
            this.message = msg;
        }
    }
}
