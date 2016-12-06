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

using TarotAfricain.Core;

namespace TarotAfricain
{
    class InterfacePl
    {
        Thread tarot;
        List<string> names;
        List<int> isIa;
        int nbCarte;
        GenerateEvents events;
        const int timeIaThink = 1000;

        List<Delegate> mesDelegate;
        
        public void StartGame(GenerateEvents generateEvents, List<string> names, List<int> isIa, int nbCarte)
        {
            mesDelegate = new List<Delegate>();
            events = generateEvents;
            this.names = names;
            this.isIa = isIa;
            this.nbCarte = nbCarte;
            tarot = new Thread(startGame);
            tarot.Start();
        }

        public void StopGame()
        {
            Debug.WriteLine("On es ici");
            if(tarot != null && tarot.IsAlive)
            {
                PlEngine.PlHalt();
                tarot.Abort();
            }
            else if(tarot.ThreadState == System.Threading.ThreadState.Stopped)
            {
                PlEngine.PlCleanup();
            }
        }

        private void startGame()
        {
            if (!PlEngine.IsInitialized)
            {
                // Thread clean = new Thread();
                //string filename = @"C:\Users\Mathieu\Documents\Visual Studio 2015\Projects\TarotAfricain\TarotAfricain\Prolog\prolog.pro";
                string filename = "TarotAfrikMulti.pl";

                string text = System.IO.File.ReadAllText(filename);

                string serialNames = SerializeList(names);
                string serialIsIa = SerializeList(isIa);

                String[] param = { "-q", "-f", filename };

                string query = "playGame(" + serialNames + ", " + serialIsIa + ", " + nbCarte.ToString() + "), write('end').";


                

                
                PlEngine.Initialize(param);
                Debug.WriteLine(PlEngine.IsInitialized);
                Debug.WriteLine(PlEngine.PlThreadSelf());

                //Debug.WriteLine("Attachement : " + PlEngine.PlThreadAttachEngine());
                PlEngine.SetStreamFunctionWrite(SbsSW.SwiPlCs.Streams.PlStreamType.Output, stdout);

                InitializeCallBack();
                PlQuery.PlCall(query);

                Debug.WriteLine("end");
            }
        }

        private void clean()
        {
            PlEngine.PlHalt();
        }

        private long stdout(IntPtr handle, string buffer, long bufferSize)
        {
            string s = buffer.Substring(0, (int)bufferSize);
            Debug.WriteLine(s);
            return bufferSize;
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
                Debug.WriteLine(
                PlQuery.PlCall("retractall(" + item + ")."));
            }

            List<Delegate> collbacks = new List<Delegate>
            {
                new DelegateParameter2(callPariJoueur),
                new DelegateParameter2(callJouerCarte),
                new DelegateParameter0(callPlayManche),
                new DelegateParameter0(callPlayManche2),
                new DelegateParameter0(callPlayManche3),
                new DelegateParameter0(callJoueurPioche),
                new DelegateParameter0(callJoueurPioche2),
                new DelegateParameter0(callPlayerPari),
                new DelegateParameter0(callPlayerPari2),
                new DelegateParameter1(callPlayTour),
                new DelegateParameter1(callPlayTour2),
                new DelegateParameter1(callPlayerJoue),
            };

            foreach (var item in collbacks)
            {
                PlEngine.RegisterForeign(item);
                // stock les delegates pour pas qu'ils soient bouffé par le garbage
                mesDelegate.Add(item);
            }

        }

        /// <summary>
        /// Player joue la carte carte
        /// </summary>
        /// <param name="carte"></param>
        /// <returns></returns>
        private bool callPlayerJoue(PlTerm carte)
        {
            string player = getNameCurrentPlayer();
            string carteJouee = getIdCarte(carte);
            List<string> main = getMainCurrentPlayer();

            events.carteJoueeChanged(player, carteJouee);
            events.mainChanged(player, main);

            Thread.Sleep(timeIaThink);
            return true;
        }

        /// <summary>
        /// Fin du tour le gagnant est winner
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        private bool callPlayTour2(PlTerm winner)
        {
            string player = getNamePlayer(winner);
            int nbPoint = getPointsManchePlayer(winner);
            events.pointsMancheChanged(player, nbPoint);
            events.gagnantTour(winner.ToString());

            //update les points game
            Thread.Sleep(timeIaThink/10);
            return true;
        }

        /// <summary>
        /// Début du tour nbTour
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private bool callPlayTour(PlTerm term)
        {
            int nbTour = int.Parse(term.ToString());
            events.tourChanged(nbTour+1);
            return true;
        }

        private bool callPlayerPari2()
        {
            string player = getNameCurrentPlayer();
            int nbPari = getNbPariCurrentPlayer();
            events.parisChanged(player, nbPari);
            Thread.Sleep(timeIaThink/10);
            return true;
        }

        private bool callPlayerPari()
        {
            Thread.Sleep(timeIaThink);
            return true;
        }

        private bool callJoueurPioche2()
        {
            string player = getNameCurrentPlayer();
            List<string> main = getMainCurrentPlayer();

            Debug.WriteLine(player);
            foreach (var item in main)
            {
                Debug.Write(item + ", ");
            }
            Debug.WriteLine("");

            events.mainChanged(player, main);
            //Thread.Sleep(timeIaThink);
            return true;
        }

        private bool callJoueurPioche()
        {
            return true;
        }

        private bool callPlayManche3()
        {
            callPlayManche2();
            events.gameOver();
            return true;
        }

        private bool callPlayManche2()
        {
            Thread.Sleep(timeIaThink/10);
            List<Tuple<string, int>> points = getPointsGame();
            foreach (var player in points)
            {
                events.pointsGameChanged(player.Item1, player.Item2);
                events.pointsMancheChanged(player.Item1, 0);
            }
            return true;
        }

        private bool callPlayManche()
        {
            Thread.Sleep(timeIaThink);
            events.mancheChanged();
            return true;
        }

        private bool callJouerCarte(PlTerm joueur, PlTerm carte)
        {
            Thread.Sleep(2*timeIaThink);
            JoueurArg joueurArg = events.getCarteJouee(getNamePlayer(joueur));
            while (joueurArg == null || joueurArg.joueur.carteJouee == null)
            {
                // on attend
            }

            carte.Unify(new PlTerm(joueurArg.joueur.carteJouee.nom));
            return true;
        }

        private bool callPariJoueur(PlTerm joueur, PlTerm nbPli)
        {
            Thread.Sleep(timeIaThink * 2);
            JoueurArg joueurArg = events.getPari(getNamePlayer(joueur));
            while (joueurArg == null || joueurArg.joueur.paris == -1)
            {
                // on attend
            }
            nbPli.Unify(new PlTerm(joueurArg.joueur.paris));
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
            result +=  list.Last() + "]";
            return result;
        }

        private string getNameCurrentPlayer()
        {
            string name;
            using (PlFrame fr = new PlFrame())
            {
                PlTerm playerName = new PlTerm("NomPlayer");
                using (PlQuery query = new PlQuery("currentPlayer(Player)."))
                {
                    PlTerm currentPlayer = query.Solutions.First()[0];
                    currentPlayer.Unify(PlTerm.PlCompound("player", playerName));
                    name = playerName.ToString();
                }
            }
            return name;
        }

        private string getIdCarte(PlTerm carte)
        {
            string idCarte;
            using (PlFrame fr = new PlFrame())
            {
                PlTerm id = new PlTerm("Id");
                var carte2 = PlTerm.PlCompound("carte", id, PlTerm.PlVar(), PlTerm.PlVar());
                carte.Unify(carte2);
                idCarte = id.ToString();
            }
            return idCarte;
        }

        private string getNamePlayer(PlTerm player)
        {
            string name;
            using (PlFrame fr = new PlFrame())
            {
                PlTerm playerName = new PlTerm("NomPlayer");
                player.Unify(PlTerm.PlCompound("player", playerName));
                name = playerName.ToString();
            }
            return name;
        }

        private List<string> getMainCurrentPlayer()
        {
            List<string> main = new List<string>();

            using (PlFrame fr = new PlFrame())
            {
                using (PlQuery query = new PlQuery("currentPlayer(Player),jeuPlayer(Player, Jeu)."))
                {
                    var jeu = query.SolutionVariables.First()["Jeu"];
                    if (jeu.IsList)
                    {
                        foreach (var item in jeu.ToList())
                        {
                            main.Add(getIdCarte(item));
                        }
                    }
                }
            }

            return main;
        }

        private int getNbPariCurrentPlayer()
        {
            int result;
            using (PlFrame fr = new PlFrame())
            {
                using (PlQuery query = new PlQuery("currentPlayer(Player),pari(Player, Pari)."))
                {
                    string nbPari = query.SolutionVariables.First()["Pari"].ToString();
                    result = int.Parse(nbPari);
                }
            }
            return result;
        }

        private int getPointsManchePlayer(PlTerm player)
        {
            int result;
            using (PlFrame fr = new PlFrame())
            {
                PlTerm nbPoint = new PlTerm("NbPoint");
                PlTermV terms = new PlTermV(player, nbPoint);
                using (PlQuery query = new PlQuery("pointManchePlayer",terms))
                {
                    string nbPari = query.Solutions.First()[1].ToString();
                    result = int.Parse(nbPari);
                }
            }
            return result;
        }

        private List<Tuple<string, int>> getPointsGame()
        {
            List<Tuple<string, int>> result = new List<Tuple<string, int>>();
            using (PlFrame fr = new PlFrame())
            {
                PlTerm player = new PlTerm("Player");
                PlTerm points = new PlTerm("Points");
                PlTermV terms = new PlTermV(player, points);
                using (PlQuery query = new PlQuery("pointGame", terms))
                {
                    foreach (var item in query.Solutions)
                    {
                        player = item[0];
                        string nomPlayer = getNamePlayer(player);
                        int nbPointsGame = int.Parse(item[1].ToString());
                        result.Add(new Tuple<string, int>(nomPlayer, nbPointsGame));
                        
                    }
                }
            }
            return result;
        }
    }
}
