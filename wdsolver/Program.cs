using System;
using System.Collections.Generic;
using System.Linq;

using Vec = System.ValueTuple<int, int>;

namespace wdsolver
{
    class Program
    {
        /*
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

        static string SOLVE = @"
W0 16 17 18 19 20 21 00
W0 15 14 13 12 11 22 00
00 04 05 06 07 10 23 00
00 03 00 00 08 09 24 00
01 02 w0 b0 w0 b0 25 44
00 31 30 29 28 27 26 43
00 32 00 00 00 00 00 42
B0 33 00 00 00 00 00 41
B0 34 35 36 37 38 39 40";
        */

        /*
        static string MAP = @"
00 00 00 00 00 00 00
00 00 00 00 00 00 99
00 B1 W1 00 w2 00 00
00 B1 B1 00 b2 00 00
00 00 00 00 00 00 01
00 00 00 00 00 00 00";

        static string SOLVE = @"
21 22 23 24 25 26 27
20 00 00 07 06 05 28
19 B1 W1 08 w2 04 03
18 B1 B1 09 b2 00 02
17 16 15 10 11 00 01
00 00 14 13 12 00 00";
        */
        static string MAP = @"
01 00 00 00 00 00 00 00 99
00 B1 00 00 W1 00 00 B1 00
00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00
w2 00 00 b2 00 00 w2 00 00
00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00
00 B1 00 00 W1 00 00 B1 00
00 00 00 00 00 00 00 00 00";

        static string SOLVE = @"";

        static Vec UP = (0, -1);
        static Vec DOWN = (0, 1);
        static Vec LEFT = (-1, 0);
        static Vec RIGHT = (1, 0);

        static Vec[] DIRS = { LEFT, RIGHT, UP, DOWN };

        static int width, height;
        static List<Vec> houses;
        static List<Vec> tanks;

        static Cell[][] parsed_map;
        static Cell[][] parsed_solve;

        static int counter = 1;
        static Vec xy;
        static Vec endpoint;

        static Car car = new Car(2);

        static void Main(string[] args)
        {
            parsed_map = (from s in MAP.Replace("\r", "").Split('\n')
                          where s.Length > 0
                          select s.Split(' ').Select(Cell.FromString).ToArray()
            ).ToArray();

            /*
            parsed_solve = (from s in SOLVE.Replace("\r", "").Split('\n')
                            where s.Length > 0
                            select s.Split(' ').Select(Cell.FromString).ToArray()
            ).ToArray();
            */

            height = parsed_map.Length;
            width = parsed_map[0].Length;

            houses = new List<Vec>();
            tanks = new List<Vec>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var p = parsed_map[y][x];

                    switch (p) {
                        case WayPoint w:
                            if (w.Value == 1)
                                xy = (x, y);
                            else if (w.Value == 99)
                                endpoint = (x, y);
                            break;
                        case Tank t:
                            tanks.Add((x, y));
                            break;
                        case House h:
                            houses.Add((x, y));
                            break;
                    }
                }
            }

            Solve();
            // Debug();
        }

        static void PrintMap()
        {
            for (int y = 0; y < height; y++)
                Console.WriteLine(string.Join<Cell>(' ', parsed_map[y]));
            car.DebugOil();
        }

        static bool InRange(Vec coord)
        {
            var (x, y) = coord;
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        static Cell at(Vec coord)
        {
            var (x, y) = coord;
            return parsed_map[y][x];
        }

        static bool CanGo(Vec direct)
        {
            var (x, y) = xy;
            var (dx, dy) = direct;
            var coord = (x + dx, y + dy);
            if (!InRange(coord))
                return false;
            if (at(coord) is WayPoint w)
            {
                return w.Value == 0 || w.Value == 99;
            }
            else return false;
        }

        static List<InteractAction> Interact()
        {
            var (x, y) = xy;
            var actions = new List<InteractAction>();

            foreach (var d in DIRS)
            {
                var newxy = (x + d.Item1, y + d.Item2);

                if (InRange(newxy))
                {
                    if (at(newxy) is Tank t)
                    {
                        if (t.Amount > 0 && car.CanPull())
                        {
                            var oils = car.GetOilCopy();
                            while (t.Amount > 0 && car.CanPull())
                            {
                                t.Amount -= 1;
                                actions.Add(new Pull { xy = newxy, oils = oils });
                                car.Pull(t.Type);
                            }
                        }

                    }
                }
            }
            foreach (var d in DIRS)
            {
                var newxy = (x + d.Item1, y + d.Item2);

                if (InRange(newxy))
                {
                    if (at(newxy) is House h)
                    {
                        if (h.Amount > 0 && car.CanPour(h.Type))
                        {
                            h.Amount -= 1;
                            car.Pour();
                            actions.Add(new Pour { xy = newxy });
                        }
                    }
                }
            }

            return actions;
        }

        static List<InteractAction> Goto(Vec direct)
        {
            counter++;
            var (x, y) = xy;
            var (dx, dy) = direct;
            xy = (x + dx, y + dy);
            (at(xy) as WayPoint).Value = counter;
            var actions = Interact();
            actions.Insert(0, new Goto { xy = (x, y) });
            return actions;
        }

        static void GoBack(List<InteractAction> actions)
        {
            foreach (Goto act in actions.Where(a => a is Goto))
            {
                (at(xy) as WayPoint).Value = 0;
                xy = act.xy;
                (at(endpoint) as WayPoint).Value = 99;
            }
            foreach (Pour act in actions.Where(a => a is Pour))
            {
                (at(act.xy) as ColoredCell).Amount += 1;
                car.Pull((at(act.xy) as ColoredCell).Type);
            }
            foreach (Pull act in actions.Where(a => a is Pull))
            {
                (at(act.xy) as ColoredCell).Amount += 1;
                car.SetOil(act.oils);
            }

            counter -= 1;
        }

        static bool IsOver()
        {
            if ((at(xy) as WayPoint).Value == 99)
                return true;
            if (xy == endpoint)
                return true;

            bool stuck = true;
            foreach (var d in DIRS)
            {
                if (CanGo(d))
                    stuck = false;
            }

            if (stuck) return true;
            return false;
        }

        static bool IsWin()
        {
            if (xy != endpoint)
                return false;

            if ((at(endpoint) as WayPoint).Value == 99)
                return false;

            foreach (var h in houses)
            {
                if ((at(h) as House).Amount > 0)
                    return false;
            }

            return true;
        }

        static bool IsNoHope()
        {
            if (!IsReachable(xy.Item1, xy.Item2, endpoint.Item1, endpoint.Item2))
                return true;
            foreach (var house in houses.Where(h => (at(h) as House).Amount > 0))
            {
                var h = at(house) as House;
                if (!IsReachable(xy.Item1, xy.Item2, house.Item1, house.Item2, nearD: true))
                    return true;

                // 만약 탱크로부터 물을 가져와야 한다면 적어도 하나 이상의 탱크에서 부족한 물을 가져올 수 있어야 한다
                var needed = h.Amount - car.HowMuchCanPour(h.Type);
                if (needed <= 0)
                    continue;
                var notank = true;
                foreach (var tank in tanks.Where(t => (at(t) as Tank).Amount > 0 && (at(t) as Tank).Type == h.Type))
                {
                    if (IsReachable(house.Item1, house.Item2, tank.Item1, tank.Item2, nearS: true, nearD: true)
                        && IsReachable(xy.Item1, xy.Item2, tank.Item1, tank.Item2, nearD: true))
                    {
                        notank = false;
                        break;
                    }
                }

                if (notank)
                    return true;
            }

            return false;
        }

        static bool IsReachable(int sx, int sy, int dx, int dy, bool nearS = false, bool nearD = false)
        {
            int s = -1, d = -1;
            var g = new Graph(width, height);

            int k = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (InRange((x, y)) && at((x, y)) is WayPoint w && (w.Value == 0 || w.Value == counter))
                    {
                        if (reachableBlock(x + 1, y))
                            g.Add(k, k + 1);
                        if (reachableBlock(x - 1, y))
                            g.Add(k, k - 1);
                        if (reachableBlock(x, y + 1))
                            g.Add(k, k + width);
                        if (reachableBlock(x, y - 1))
                            g.Add(k, k - width);
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

            bool reachableBlock(int x, int y)
            {
                return InRange((x, y)) && at((x, y)) is WayPoint w && (w.Value == 0 || w.Value == counter || w.Value == 99);
            }
        }

        static int step = 0;
        static bool Solve(bool right = true)
        {
            step++;
            if (IsOver())
            {
                if (IsWin())
                {
                    Console.WriteLine($"Solved at step {step}!");
                    PrintMap();
                    return true;
                }
                else return false;
            }

            if (counter % 2 == 0)
                if (IsNoHope())
                    return false;

            foreach (var d in DIRS)
            {
                if (CanGo(d))
                {
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
                    */

                    // if (Solve(nextright))
                    if (Solve())
                        return true;

                    GoBack(actions);
                }
            }
            return false;
        }

        static void Debug()
        {
            while (true)
            {
                var curd = (-1, -1);
                foreach (var d in DIRS)
                {
                    var newxy = (xy.Item1 + d.Item1, xy.Item2 + d.Item2);
                    if (InRange(newxy))
                        if (parsed_solve[newxy.Item2][newxy.Item1] is WayPoint w)
                            if (w.Value == counter + 1)
                            {
                                curd = d;
                                // counter++;
                                break;
                            }
                }

                if (curd == (-1, -1))
                {
                    Console.WriteLine("end");
                    return;
                }

                var actions = Goto(curd);
                PrintMap();
                Console.WriteLine(string.Join(' ', actions));

                Console.Read();
            }

        }
    }

}
