using System.Linq;
using Vec = System.ValueTuple<int, int>;

namespace wdsolver {
    public class InteractAction {
        public Vec xy;

        public override bool Equals(object obj) {
            return obj is InteractAction i && i.xy == xy;
        }
    }

    public class Goto : InteractAction {
        public override bool Equals(object obj) {
            return obj is Goto && base.Equals(obj);
        }
    }

    public class Pull : InteractAction {
        public Water oil;
        public int idx;

        public override bool Equals(object obj) {
            return obj is Pull p && p.oil == oil && p.idx == idx && base.Equals(obj);
        }
    }

    public class Pour : InteractAction {
        public override bool Equals(object obj) {
            return obj is Pour && base.Equals(obj);
        }
    }
}
