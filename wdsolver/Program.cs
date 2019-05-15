using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vec = System.ValueTuple<int, int>;

namespace wdsolver {
    class Program {
        static string MAP = MAPS.B_7;

        static void Main(string[] args) {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var solver = new Solver(MAP, 2);
            if (!solver.TrySolveOneDefaultDirs(out var stage, out var step)) {
                Console.WriteLine("failed");
                return;
            }
            stopwatch.Stop();

            Console.WriteLine($"solved at step {step}");
            Console.WriteLine($"elapsed: {stopwatch.ElapsedMilliseconds}ms");
            stage.PrintMap();
        }
    }
}
