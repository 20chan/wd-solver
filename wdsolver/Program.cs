using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vec = System.ValueTuple<int, int>;

namespace wdsolver {
    class Program {
        static string MAP = @"
W1 00 00 00 00 00 00 00
W1 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00
01 00 w1 b1 w1 b1 00 99
00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00
B1 00 00 00 00 00 00 00
B1 00 00 00 00 00 00 00";

        static void Main(string[] args) {
            var solver = new Solver(MAP, 2);
            if (!solver.TrySolveOneDefaultDirs(out var stage, out var step)) {
                Console.WriteLine("failed");
                return;
            }

            Console.WriteLine($"solved at step {step}");
            stage.PrintMap();
        }
    }
}
