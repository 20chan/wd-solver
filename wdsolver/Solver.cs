using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vec = wdsolver.Vector2;

namespace wdsolver
{
    public class Solver
    {
        private Stage _stage;
        
        public Solver(Stage stage)
        {
            _stage = stage;
        }

        public void Solve() {

        }

        /*
        async Task<bool> Solve(Vec[] dirs, bool right = true) {
            step++;
            if (IsOver()) {
                if (IsWin()) {
                    if (solved)
                        return true;
                    solved = true;
                    Console.WriteLine($"Solved at step {step}!");
                    PrintMap();
                    return true;
                } else return false;
            }

            if (counter % 2 == 0)
                if (IsNoHope())
                    return false;

            foreach (var d in dirs) {
                if (CanGo(d)) {
                    // var newxy = (xy.Item1 + d.Item1, xy.Item2 + d.Item2);
                    // var nextright = right && (parsed_solve[newxy.Item2][newxy.Item1] as WayPoint).Value == counter + 1;

                    var actions = Goto(d);
                    /*
                    if (right && nextright)
                    {
                        Console.WriteLine($"step {step} {counter} is right");
                        PrintMap();
                    }
                    if (right && !nextright)
                    {
                        Console.WriteLine($"step {step} {counter} goes wrong");
                        PrintMap();
                    }
                    

                    // if (Solve(nextright))
                    if (await Solve(dirs))
                        return true;

                    GoBack(actions);
                }
            }
            return false;
        }

        void Debug() {
            while (true) {
                var curd = (-1, -1);
                foreach (var d in DIRS) {
                    var newxy = (xy.Item1 + d.Item1, xy.Item2 + d.Item2);
                    if (InRange(newxy))
                        if (parsed_solve[newxy.Item2][newxy.Item1] is WayPoint w)
                            if (w.Value == counter + 1) {
                                curd = d;
                                // counter++;
                                break;
                            }
                }

                if (curd == (-1, -1)) {
                    Console.WriteLine("end");
                    return;
                }

                var actions = Goto(curd);
                PrintMap();
                Console.WriteLine(string.Join(' ', actions));

                Console.Read();
            }

        }
        */
    }
}