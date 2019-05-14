using System;
using System.Linq;
namespace wdsolver {
    public class Car {
        public Water[] oils;
        public int idx;

        public Car(int length) {
            oils = new Water[length];
            idx = 0;
        }

        public void SetOil(Water oil) {
            for (int i = 0; i < oils.Length; i++)
                oils[i] = oil;
            idx--;
        }

        public bool CanPull(Water type) {
            if (idx >= oils.Length) {
                return false;
            }

            if (idx == 0) {
                return true;
            }

            if (type == Water.White) {
                return true;
            }

            if (oils.Take(idx).Contains(Water.White)) {
                return true;
            }

            if (oils.Take(idx).Contains(type)) {
                return true;
            }

            return false;
        }

        public bool CanPour(Water type)
            => idx > 0 && oils.Take(idx).Contains(type);

        public int HowMuchCanPour(Water type)
            => oils.Take(idx).Count(w => w == type);

        public void Pull(Water type) {
            if (type == Water.White && idx >= 1)
                type = oils[0];
            oils[idx] = type;
            if (type != Water.White)
                for (int i = 0; i < idx; i++)
                    oils[i] = type;
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
