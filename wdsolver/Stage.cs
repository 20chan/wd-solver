using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vec = wdsolver.Vector2;

namespace wdsolver {
    public class Stage {
        public readonly int Width, Height;
        private Cell[][] _map;
        private readonly Vec[] houses;
        private readonly Vec[] tanks;

        private Car car;

        private Vec xy;
        private Vec endpoint;

        private int counter = 1;

        public StageDebugger Debug;

        public Stage(Cell[][] map, int truckAmount) {
            _map = map;
            car = new Car(truckAmount);
            Debug = new StageDebugger(this);

            Width = map[0].Length;
            Height = map.Length;

            var houses = new List<Vec>();
            var tanks = new List<Vec>();

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    var p = map[y][x];

                    switch (p) {
                        case WayPoint w:
                            if (w.Value == 1)
                                xy = new Vec(x, y);
                            else if (w.Value == 99)
                                endpoint = new Vec(x, y);
                            break;
                        case Tank t:
                            tanks.Add(new Vec(x, y));
                            break;
                        case House h:
                            houses.Add(new Vec(x, y));
                            break;
                    }
                }
            }

            this.houses = houses.ToArray();
            this.tanks = tanks.ToArray();
        }

        public Stage(string rawMap, int truckAmount) : this(ParseMapFromString(rawMap), truckAmount) {
        }

        internal static Cell[][] ParseMapFromString(string data) {
            return (from s in data.Replace("\r", "").Split('\n')
                    where s.Length > 0
                    select s.Split(' ').Select(Cell.FromString).ToArray()).ToArray();
        }

        public void PrintMap() {
            for (int y = 0; y < Height; y++)
                Console.WriteLine(string.Join<Cell>(' ', _map[y]));
        }

        private Cell at(in int x, in int y) {
            return _map[y][x];
        }

        private Cell at(in Vec coord) {
            return at(in coord.X, in coord.Y);
        }

        private bool InRange(in int x, in int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private bool InRange(in Vec coord) {
            return InRange(in coord.X, in coord.Y);
        }

        public bool CanGo(in Vec direct) {
            var coord = xy + direct;
            if (!InRange(coord))
                return false;
            if (at(coord) is WayPoint w) {
                return w.Value == 0 || w.Value == 99;
            }

            return false;
        }

        private List<InteractAction> Interact() {
            var actions = new List<InteractAction>();

            foreach (var d in Vec.DIRS) {
                Vec newxy = xy + d;

                if (InRange(newxy)) {
                    if (at(newxy) is Tank t) {
                        if (t.Amount > 0 && car.CanPull(t.Type)) {
                            while (t.Amount > 0 && car.CanPull(t.Type)) {
                                t.Amount -= 1;
                                var oil = car.oils[0];
                                actions.Add(new Pull { xy = newxy, oil = oil, idx = car.idx });
                                car.Pull(t.Type);
                            }
                        }
                    }
                }
            }

            foreach (var d in Vec.DIRS) {
                var newxy = xy + d;

                if (InRange(newxy)) {
                    if (at(newxy) is House h) {
                        while (h.Amount > 0 && car.CanPour(h.Type)) {
                            h.Amount -= 1;
                            car.Pour();
                            actions.Add(new Pour { xy = newxy });
                        }
                    }
                }
            }

            return actions;
        }

        public List<InteractAction> Goto(Vec direct) {
            counter++;
            var gotoAction = new Goto { xy = xy };
            xy += direct;
            (at(xy) as WayPoint).Value = counter;
            var actions = Interact();
            actions.Insert(0, gotoAction);
            return actions;
        }

        public void GoBack(List<InteractAction> actions) {
            foreach (var act in actions.OfType<Goto>()) {
                (at(xy) as WayPoint).Value = 0;
                xy = act.xy;
                (at(endpoint) as WayPoint).Value = 99;
            }

            foreach (var act in actions.OfType<Pour>()) {
                (at(act.xy) as ColoredCell).Amount += 1;
                car.Pull((at(act.xy) as ColoredCell).Type);
            }

            foreach (var act in actions.OfType<Pull>()) {
                (at(act.xy) as ColoredCell).Amount += 1;
                car.SetOil(act.oil);
            }

            counter -= 1;
        }

        public bool IsOver() {
            if ((at(xy) as WayPoint).Value == 99)
                return true;
            if (xy == endpoint)
                return true;

            bool stuck = true;
            foreach (var d in Vec.DIRS) {
                if (CanGo(d))
                    stuck = false;
            }

            if (stuck) return true;
            return false;
        }

        public bool IsWin() {
            if (xy != endpoint)
                return false;

            if ((at(endpoint) as WayPoint).Value == 99)
                return false;

            foreach (var h in houses) {
                if ((at(h) as House).Amount > 0)
                    return false;
            }

            return true;
        }

        public bool IsNoHope() {
            if (!IsReachable(xy.X, xy.Y, endpoint.X, endpoint.Y))
                return true;
            foreach (var house in houses.Where(h => (at(h) as House).Amount > 0)) {
                var h = at(house) as House;
                if (!IsReachable(xy.X, xy.Y, house.X, house.Y, nearD: true))
                    return true;

                // 만약 탱크로부터 물을 가져와야 한다면 적어도 하나 이상의 탱크에서 부족한 물을 가져올 수 있어야 한다
                var needed = h.Amount - car.HowMuchCanPour(h.Type);
                if (needed <= 0)
                    continue;
                var notank = true;
                foreach (var tank in tanks.Where(t => (at(t) as Tank).Amount > 0 && (at(t) as Tank).Type == h.Type)) {
                    if (IsReachable(house.X, house.Y, tank.X, tank.Y, nearS: true, nearD: true)
                        && IsReachable(xy.X, xy.Y, tank.X, tank.Y, nearD: true)) {
                        notank = false;
                        break;
                    }
                }

                if (notank)
                    return true;
            }

            return false;
        }

        bool IsReachable(int sx, int sy, int dx, int dy, bool nearS = false, bool nearD = false) {
            int s = -1, d = -1;
            var g = new Graph(Width, Height);

            int k = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++) {
                    if (InRange(x, y) && at(x, y) is WayPoint w && (w.Value == 0 || w.Value == counter)) {
                        if (reachableBlock(x + 1, y))
                            g.Add(k, k + 1);
                        if (reachableBlock(x - 1, y))
                            g.Add(k, k - 1);
                        if (reachableBlock(x, y + 1))
                            g.Add(k, k + Width);
                        if (reachableBlock(x, y - 1))
                            g.Add(k, k - Width);
                    }

                    if (x == sx && y == sy)
                        s = k;
                    if (x == dx && y == dy)
                        d = k;

                    k++;
                }

            if (nearS)
                return g.BFSFromNearSources(s, d, nearD);
            else
                return g.BFS(s, d, nearD);

            bool reachableBlock(int x, int y) {
                return InRange(x, y) && at(x, y) is WayPoint w &&
                       (w.Value == 0 || w.Value == counter || w.Value == 99);
            }
        }

        public class StageDebugger {
            private Stage _stage;

            public StageDebugger(Stage stage) {
                _stage = stage;
            }

            public Cell At(Vector2 coord)
                => _stage.at(coord);

            public Water TruckType()
                => _stage.car.oils[0];

            public int TruckAmount()
                => _stage.car.idx;
        }
    }
}