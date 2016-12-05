using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Callback;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TarotAfricain
{
    class InterfacePl
    {
        Thread tarot;
        List<string> names;
        List<int> isIa;
        int nbCarte;
        GenerateEvents events;
        
        public void StartGame(GenerateEvents generateEvents, List<string> names, List<int> isIa, int nbCarte)
        {
            events = generateEvents;
            this.names = names;
            this.isIa = isIa;
            this.nbCarte = nbCarte;
            tarot = new Thread(startGame);
            tarot.Start();
        }

        public void StopGame()
        {
            if(tarot.IsAlive)
            {
                tarot.Abort();
            }
        }

        private void startGame()
        {
            if (!PlEngine.IsInitialized)
            {
                //string filename = @"C:\Users\Mathieu\Documents\Visual Studio 2015\Projects\TarotAfricain\TarotAfricain\Prolog\prolog.pro";
                string filename = "TarotAfrikMulti.pl";

                string text = System.IO.File.ReadAllText(filename);

                string serialNames = SerializeList(names);
                string serialIsIa = SerializeList(isIa);

                String[] param = { "-q", "-f", filename };

                string query = "playGame(" + serialNames + ", " + serialIsIa + ", " + nbCarte.ToString() + ").";

                PlEngine.Initialize(param);

                InitializeCallBack();
                PlQuery.PlCall(query);
            }
        }

        private void InitializeCallBack()
        {
            List<string> rulesToRemove = new List<string>
            {
                "callPariJoueur(_,_)",
                "callJouerCarte(_,_)",
                "callPlayManche",
                "callPlayManche2",
                "callPlayManche3",
                "callJoueurPioche",
                "callJoueurPioche2",
                "callPlayerPari",
                "callPlayerPari2",
                "callPlayTour(_)",
                "callPlayTour2(_)",
                "callPlayerJoue(_)"
            };

            foreach (var item in rulesToRemove)
            {
                PlQuery.PlCall("retractall(" + item + ").");
            }

            List<Delegate> collbacks = new List<Delegate>
            {
                //new DelegateParameter2(callPariJoueur),
                //new DelegateParameter2(callJouerCarte),
                //new DelegateParameter0(callPlayManche),
                //new DelegateParameter0(callPlayManche2),
                //new DelegateParameter0(callPlayManche3),
                //new DelegateParameter0(callJoueurPioche),
                //new DelegateParameter0(callJoueurPioche2),
                //new DelegateParameter0(callPlayerPari),
                //new DelegateParameter0(callPlayerPari2),
                //new DelegateParameter1(callPlayTour),
                //new DelegateParameter1(callPlayTour2),
                //new DelegateParameter1(callPlayerJoue),
            };

            foreach (var item in collbacks)
            {
                PlEngine.RegisterForeign(item);
            }

        }

        private bool callPlayerJoue(PlTerm carte)
        {
            Debug.WriteLine("hello");
            Debug.WriteLine(carte.ToString());
            Thread.Sleep(10000);
            return true;
        }

        private bool callPlayTour2(PlTerm term)
        {
            return true;
        }

        private bool callPlayTour(PlTerm term)
        {
            return true;
        }

        private bool callPlayerPari2()
        {
            return true;
        }

        private bool callPlayerPari()
        {
            return true;
        }

        private bool callJoueurPioche2()
        {
            return true;
        }

        private bool callJoueurPioche()
        {
            return true;
        }

        private bool callPlayManche3()
        {
            return true;
        }

        private bool callPlayManche2()
        {
            return true;
        }

        private bool callPlayManche()
        {
            return true;
        }

        private bool callJouerCarte(PlTerm term1, PlTerm term2)
        {
            return true;
        }

        private bool callPariJoueur(PlTerm term1, PlTerm term2)
        {
            return true;
        }

        private string SerializeList(IEnumerable<string> list)
        {
            string result = "[";
            foreach (var item in list.Take(list.Count() - 1))
            {
                result += "\"" + item.ToString() + "\", ";
            }
            result += "\"" + list.Last() + "\"]";
            return result;
        }

        private string SerializeList(IEnumerable<int> list)
        {
            string result = "[";
            foreach (var item in list.Take(list.Count() - 1))
            {
                result += item.ToString() + ", ";
            }
            result += list.Last() + "]";
            return result;
        }
    }
}
