using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    class Program
    {        
        static void Main(string[] args)
        {
            // Predict eleciton via monte carlo 

            int nTrials = 1000;
            
            // Compute 
            Console.WriteLine("** Versus population");
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                const double pRed = .499;

                for(int popSize = 500*1000; popSize < 1500*1000; popSize += 100*1000)
                {
                    var s = new Simulation(pRed, popSize);
                    Run(s, nTrials);
                }
                sw.Stop();
                Console.WriteLine("Elapsed:{0}s", sw.ElapsedMilliseconds / 1000);
            }

            Console.WriteLine("** Versus P");
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (var pRed = 0.46; pRed <= .54; pRed += .005)
                {
                    var s = new Simulation(pRed, 5000);
                    Run(s, nTrials);
                }
                sw.Stop();
                Console.WriteLine("Elapsed:{0}s", sw.ElapsedMilliseconds / 1000);
            }
        }


        // Run an experiment 
        // Return % of times that Red wins. 
        static double Run(Simulation s, int nTrials)
        {
            int cRed = 0;
            int cBlue = 0;

            for (int i = 0; i < nTrials; i++)
            {
                var result = s.Run();
                if (result.DidRedWin)
                {
                    cRed++;
                }
                else
                {
                    cBlue++;
                }
            }

            var percentRedWins = cRed * 100.0 / (cBlue + cRed);
            Console.WriteLine("{3}: {0}, {1}, red% = {2:0.000}", cRed, cBlue, cRed * 100.0 / (cBlue + cRed), s);
            return percentRedWins;            
        }

    }

    // Run 1 election experiment under the given parameters. 
    public class Simulation
    {
        static Random _random = new Random();

        private readonly double _percentRed; // 0..1 . % change of voting for Red. 
        private readonly int _popSize; // # of voters

        public Simulation(double percentRed, int N)
        {
            _percentRed = percentRed;
            _popSize = N;
        }

        public Result Run()
        {
            int cRed = 0;
            int cBlue = 0;
            
            for (int i = 0; i < _popSize; i++)
            {
                var b = DidVoteRed();
                if (b)
                {
                    cRed++;
                }
                else
                {
                    cBlue++;
                }
            }

            return new Result
            {
                 Red = cRed,
                 Blue = cBlue
            };
        }

        public override string ToString()
        {
            return string.Format("P(Red)={0:0.000}%, PopSize={1}", _percentRed, _popSize);
        }

        // Result of an election. Red + Blue should be the population total. 
        public class Result
        {
            public int Red; // Number of people that voted Red
            public int Blue; // Number of people that voted Blue

            public bool DidRedWin { get { return this.Red > this.Blue; } }
        }

        bool DidVoteRed()
        {
            bool x = _random.NextDouble() < _percentRed;
            return x;
        }
    }
}
