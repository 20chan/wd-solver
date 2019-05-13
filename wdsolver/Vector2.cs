namespace wdsolver {
    [System.Diagnostics.DebuggerDisplay("({X}, {Y})")]
    public class Vector2 {
        public int X, Y;

        public Vector2(int x, int y) {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2((int x, int y) vec) {
            return new Vector2(vec.x, vec.y);
        }

        public static implicit operator (int x, int y)(Vector2 vec) {
            return (vec.X, vec.Y);
        }

        public void Deconstruct(out int x, out int y) {
            x = X;
            y = Y;
        }

        public override bool Equals(object obj) {
            return obj is Vector2 v && v.X == X && v.Y == Y;
        }

        public static bool operator ==(Vector2 a, Vector2 b) {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b) {
            return !(a == b);
        }

        public static readonly Vector2 UP = (0, -1);
        public static readonly Vector2 DOWN = (0, 1);
        public static readonly Vector2 LEFT = (-1, 0);
        public static readonly Vector2 RIGHT = (1, 0);

        public static readonly Vector2[] DIRS = { UP, DOWN, LEFT, RIGHT };
    }
}
