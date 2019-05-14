using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Vec = wdsolver.Vector2;

namespace wdsolver {
    public class Stage {
        public readonly int Width, Height;
        private Cell[][] _map;
        private Graph g;
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

            g = new Graph(Width, Height);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Cell at(in int x, in int y) {
            return _map[y][x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Cell at(in Vec coord) {
            return at(in coord.X, in coord.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T at<T>(in int x, in int y) where T : Cell {
            return (T)_map[y][x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T at<T>(in Vec coord) where T : Cell {
            return at<T>(in coord.X, in coord.Y);
        }

        private bool InRange(in int x, in int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private bool InRange(in Vec coord) {
            return InRange(in coord.X, in coord.Y);
        }

        public bool CanGo(in Vec direct) {
            var coord = xy + direct;
            if (!InRange(in coord))
                return false;
            if (at(in coord) is WayPoint w) {
                return w.Value == 0 || w.Value == 99;
            }

            return false;
        }

        private List<InteractAction> Interact(in Vec prevXY) {
            var actions = new List<InteractAction>();

            actions.Add(new Goto { xy = prevXY });

            foreach (var d in Vec.DIRS) {
                Vec newxy = xy + d;

                if (InRange(in newxy)) {
                    if (at(in newxy) is Tank t) {
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

                if (InRange(in newxy)) {
                    if (at(in newxy) is House h) {
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

        public List<InteractAction> Goto(in Vec direct) {
            counter++;
            var prevXY = xy;
            xy += direct;
            at<WayPoint>(in xy).Value = counter;
            var actions = Interact(in prevXY);
            return actions;
        }

        public void GoBack(List<InteractAction> actions) {
            foreach (var act in actions.OfType<Goto>()) {
                at<WayPoint>(in xy).Value = 0;
                xy = act.xy;
                at<WayPoint>(in endpoint).Value = 99;
            }

            foreach (var act in actions.OfType<Pour>()) {
                var cell = at<ColoredCell>(in act.xy);
                cell.Amount += 1;
                car.Pull(cell.Type);
            }

            foreach (var act in actions.OfType<Pull>()) {
                at<ColoredCell>(in act.xy).Amount += 1;
                car.SetOil(act.oil);
            }

            counter -= 1;
        }

        public bool IsOver() {
            if (at<WayPoint>(xy).Value == 99)
                return true;
            if (xy == endpoint)
                return true;

            bool stuck = true;
            foreach (var d in Vec.DIRS) {
                if (CanGo(in d))
                    stuck = false;
            }

            if (stuck) return true;
            return false;
        }

        public bool IsWin() {
            if (xy != endpoint)
                return false;

            if (at<WayPoint>(in endpoint).Value == 99)
                return false;

            foreach (var h in houses) {
                if (at<House>(in h).Amount > 0)
                    return false;
            }

            return true;
        }

        private Graph InitGraph() {
            g.Clear();

            int k = 0;
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (InRange(in x, in y) && at(in x, in y) is WayPoint w && (w.Value == 0 || w.Value == counter)) {
                        if (reachableBlock(x + 1, in y))
                            g.Add(k, k + 1);
                        if (reachableBlock(x - 1, in y))
                            g.Add(k, k - 1);
                        if (reachableBlock(in x, y + 1))
                            g.Add(k, k + Width);
                        if (reachableBlock(in x, y - 1))
                            g.Add(k, k - Width);
                    }
                    k++;
                }
            }

            return g;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool reachableBlock(in int x, in int y) {
            return InRange(in x, in y) && at(in x, in y) is WayPoint w &&
                   (w.Value == 0 || w.Value == counter || w.Value == 99);
        }

        public bool IsNoHope() {
            InitGraph();

            if (!IsReachable(xy.X, xy.Y, endpoint.X, endpoint.Y))
                return true;
            foreach (var house in houses) {
                var h = at<House>(in house);

                if (h.Amount <= 0) {
                    continue;
                }

                if (!IsReachable(xy.X, xy.Y, house.X, house.Y, nearD: true))
                    return true;

                // 만약 탱크로부터 물을 가져와야 한다면 적어도 하나 이상의 탱크에서 부족한 물을 가져올 수 있어야 한다
                var needed = h.Amount - car.HowMuchCanPour(h.Type);
                if (needed <= 0)
                    continue;
                var notank = true;
                foreach (var tank in tanks) {
                    var cell = at<Tank>(in tank);
                    if (cell.Amount <= 0 || cell.Type != h.Type) {
                        continue;
                    }
                    if (IsReachableFromNear(house.X, house.Y, tank.X, tank.Y, nearD: true)
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

        bool IsReachable(int sx, int sy, int dx, int dy, bool nearD = false) {
            return g.BFS(sx, sy, dx, dy, nearD);
        }

        bool IsReachableFromNear(int sx, int sy, int dx, int dy, bool nearD = false) {
            return g.BFSFromNearSources(sx, sy, dx, dy, nearD);
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