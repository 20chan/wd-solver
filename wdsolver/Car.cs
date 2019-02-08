using System;
using System.Linq;
namespace wdsolver {
    public class Car {
        Water[] oils;
        int idx;

        public Car(int length) {
            oils = new Water[length];
            idx = 0;
        }

        public Water[] GetOilCopy() {
            var w = new Water[oils.Length];
            for (int i = 0; i < w.Length; i++)
                w[i] = oils[i];
            return w;
        }

        public void SetOil(Water[] oil) {
            for (int i = 0; i < oil.Length; i++)
                oils[i] = oil[i];
            idx--;
        }

        public bool CanPull()
            => idx < oils.Length;

        public bool CanPour(Water type)
            => idx > 0 && oils.Take(idx).Contains(type);

        public int HowMuchCanPour(Water type)
            => oils.Take(idx).Count(w => w == type);

        public void Pull(Water type) {
            if (type == Water.White && idx >= 1)
                type = oils[0];
            oils[idx] = type;
            if (type == Water.Blue)
                for (int i = 0; i < idx; i++)
                    oils[i] = Water.Blue;
            idx++;
        }

        public Water Pour() {
            return oils[--idx];
        }

        public void DebugOil() {
            Console.WriteLine($"oils: {string.Join(' ', oils.Take(idx))}");
        }
    }
}
