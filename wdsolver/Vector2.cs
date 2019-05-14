namespace wdsolver {
    [System.Diagnostics.DebuggerDisplay("({X}, {Y})")]
    public struct Vector2 {
        public int X, Y;

        public Vector2(int x, int y) {
            X = x;
            Y = y;
        }

        public static Vector2 operator+(Vector2 a, Vector2 b) {
            return new Vector2(a.X + b.X, a.Y + b.Y);
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

        public static readonly Vector2 UP = new Vector2(0, -1);
        public static readonly Vector2 DOWN = new Vector2(0, 1);
        public static readonly Vector2 LEFT = new Vector2(-1, 0);
        public static readonly Vector2 RIGHT = new Vector2(1, 0);

        public static readonly Vector2[] DIRS = { UP, DOWN, LEFT, RIGHT };
    }
}
