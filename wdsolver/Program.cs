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
        static void Main(string[] args) {
        }
    }
}
