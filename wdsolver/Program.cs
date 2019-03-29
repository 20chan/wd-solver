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

        /*

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


        
        static string MAP = @"
00 00 00 00 00 00 00
00 00 00 00 00 00 99
00 B1 W1 00 w2 00 00
00 B1 B1 00 b2 00 00
00 00 00 00 00 00 01
00 00 00 00 00 00 00";

        /*

        static string SOLVE = @"
21 22 23 24 25 26 27
20 00 00 07 06 05 28
19 B1 W1 08 w2 04 03
18 B1 B1 09 b2 00 02
17 16 15 10 11 00 01
00 00 14 13 12 00 00";

        static string MAP = @"
01 00 00 00 00 00
00 00 W1 00 00 W1
00 00 00 00 00 00
00 00 00 XX 00 00
00 w2 W1 w1 00 00
00 00 00 00 00 99";
*/


        static string SOLVE = @"";

        static Vec UP = (0, -1);
        static Vec DOWN = (0, 1);
        static Vec LEFT = (-1, 0);
        static Vec RIGHT = (1, 0);

        static Vec[] DIRS = { LEFT, RIGHT, UP, DOWN };
        static Vec[] DIRS2 = { DOWN, UP, RIGHT, LEFT };

        static int width, height;
        static List<Vec> houses;
        static List<Vec> tanks;

        static Cell[][] parsed_map;
        static Cell[][] parsed_solve;

        static int counter = 1;
        static Vec xy;
        static Vec endpoint;

        static Car car = new Car(2);

        static void Main(string[] args) {
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

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
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

            Task.WaitAny(Solve(DIRS), Solve(DIRS2));
            // Debug();
        }

    }

}
