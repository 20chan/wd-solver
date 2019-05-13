using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vec = wdsolver.Vector2;

namespace wdsolver
{
    public class Solver {
        private Cell[][] _map;
        private bool solved = false;

        public Solver(Cell[][] map) {
            _map = map;
        }

        public Solver(string map) {
            _map = Stage.ParseMapFromString(map);
        }

        public bool TrySolveOneDefaultDirs(out Stage stage, out int step) {
            var dirs0 = new Vec[] { Vec.LEFT, Vec.RIGHT, Vec.UP, Vec.DOWN };
            var dirs1 = new Vec[] { Vec.DOWN, Vec.UP, Vec.RIGHT, Vec.LEFT };
            var tasks = new Task<(bool, int, Stage)>[] { TrySolveOne(dirs0), TrySolveOne(dirs1) };

            var fastest = tasks[Task.WaitAny(tasks)].Result;

            stage = fastest.Item3;
            step = fastest.Item2;
            return fastest.Item1;
        }

        public async Task<(bool, int, Stage)> TrySolveOne(Vec[] dirs) {
            var stage = new Stage(_map);
            var (b, i) = await TrySolveOne(dirs, stage, 0);
            return (b, i, stage);
        }

        private async Task<(bool, int)> TrySolveOne(Vec[] dirs, Stage stage, int step) {
            step++;
            if (stage.IsOver()) {
                if (stage.IsWin()) {
                    if (solved)
                        return (true, step);
                    solved = true;
                    return (true, step);
                }
                else return (false, step);
            }

            if (step % 2 == 0)
                if (stage.IsNoHope())
                    return (false, step);

            foreach (var d in dirs) {
                if (stage.CanGo(d)) {
                    var actions = stage.Goto(d);
                    var res = await TrySolveOne(dirs, stage, step);
                    if (res.Item1)
                        return (true, res.Item2);

                    stage.GoBack(actions);
                }
            }
            return (false, step);
        }
    }
}