
using Vec = System.ValueTuple<int, int>;

namespace wdsolver
{
    class InteractAction
    {
        public Vec xy;
    }
    class Goto : InteractAction
    {

    }

    class Pull : InteractAction
    {
        public Water[] oils;
    }

    class Pour : InteractAction
    {

    }
}
