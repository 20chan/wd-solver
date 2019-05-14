using System;

namespace wdsolver {
    public abstract class Cell {
        public static Cell FromString(string val) {
            if (char.IsDigit(val[0])) {
                return new WayPoint { Value = int.Parse(val) };
            }

            if (val[0] == 'X') {
                return new Wall();
            }

            if (val[0] == '+') {
                return new Cross();
            }

            var type = WaterFromChar(val[0]);
            var amount = int.Parse(val[1].ToString());

            if (char.IsUpper(val[0])) {
                return new House { Type = type, Amount = amount };
            }
            else {
                return new Tank { Type = type, Amount = amount };
            }
        }
        
        private static Water WaterFromChar(char c) {
            switch (c) {
                case 'g':
                case 'G':
                    return Water.Green;
                case 'y':
                case 'Y':
                    return Water.Yellow;
                case 'r':
                case 'R':
                    return Water.Red;
                case 'w':
                case 'W':
                    return Water.White;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public class Wall : Cell {
        public override string ToString() {
            return "XX";
        }
        public override bool Equals(object obj) {
            return obj is Wall;
        }
    }

    public class Cross : Cell {
        public override string ToString() {
            return "++";
        }
        public override bool Equals(object obj) {
            return obj is Cross;
        }
    }

    public class WayPoint : Cell {
        public int Value;

        public override string ToString() {
            return $"{Value:D2}";
        }

        public override bool Equals(object obj) {
            return obj is WayPoint w && w.Value == Value;
        }
    }

    public abstract class ColoredCell : Cell {
        public Water Type;
        public int Amount;

        public override bool Equals(object obj) {
            return obj is ColoredCell c && c.Type == Type && c.Amount == Amount;
        }
    }

    public class Tank : ColoredCell {
        public override string ToString() {
            return $"{char.ToLower(Type.ToString()[0])}{Amount}";
        }

        public override bool Equals(object obj) {
            return obj is Tank && base.Equals(obj);
        }
    }

    public class House : ColoredCell {
        public override string ToString() {
            return $"{char.ToUpper(Type.ToString()[0])}{Amount}";
        }
        public override bool Equals(object obj) {
            return obj is House && base.Equals(obj);
        }
    }
}
