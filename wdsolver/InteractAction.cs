
using Vec = System.ValueTuple<int, int>;

namespace wdsolver {
    public class InteractAction {
        public Vec xy;
    }

    public class Goto : InteractAction {

    }

    public class Pull : InteractAction {
        public Water[] oils;
    }

    public class Pour : InteractAction {

    }
}
