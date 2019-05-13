namespace wdsolver {
    public abstract class Cell {
        public static Cell FromString(string val) {
            if (char.IsDigit(val[0]))
                return new WayPoint { Value = int.Parse(val) };

            if (val[0] == 'X')
                return new Wall();

            var amount = int.Parse(val[1].ToString());

            if (val[0] == 'w')
                return new Tank { Type = Water.White, Amount = amount };
            if (val[0] == 'b')
                return new Tank { Type = Water.Blue, Amount = amount };
            if (val[0] == 'W')
                return new House { Type = Water.White, Amount = amount };
            if (val[0] == 'B')
                return new House { Type = Water.Blue, Amount = amount };

            throw new System.Exception();
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
