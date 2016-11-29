using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SbsSW.SwiPlCs;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!PlEngine.IsInitialized)
            {
                //string filename = @"C:\Users\Mathieu\Documents\Visual Studio 2015\Projects\TarotAfricain\TarotAfricain\Prolog\prolog.pro";
                string filename = @"prolog.pro";
                // Console only :
                // FileStream fs = File.Open(filename, FileMode.Open);



                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);
                //string query = "playGame([\"Quentin\",\"Mathieu\"],4)";

                //PlQuery.PlCall(query);

                PlQuery q = new PlQuery("playGame([\"Quentin\",\"Mathieu\"],4),pointGame(X,Y)");

                foreach (PlQueryVariables v in q.SolutionVariables)
                {
                    Console.Write(v["X"].ToString());
                    Console.WriteLine(" : " + v["Y"].ToString());
                }
                PlEngine.PlCleanup();
            }
        }
    }
}
