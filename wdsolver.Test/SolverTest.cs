using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace wdsolver.Test {
    [TestClass]
    public class SolverTest {
        [TestMethod]
        public void TestSolve() {
            var map = @"
XX XX XX XX
01 00 00 99
XX XX XX XX";

            var solver = new Solver(map, 1);
            Assert.IsTrue(solver.TrySolveOneDefaultDirs(out var stage, out int step));
            stage.PrintMap();
            Assert.AreEqual(new WayPoint { Value = 1 }, stage.Debug.At(new Vector2(0, 1)));
            Assert.AreEqual(new WayPoint { Value = 2 }, stage.Debug.At(new Vector2(1, 1)));
            Assert.AreEqual(new WayPoint { Value = 3 }, stage.Debug.At(new Vector2(2, 1)));
            Assert.AreEqual(new WayPoint { Value = 4 }, stage.Debug.At(new Vector2(3, 1)));
        }

        [TestMethod]
        public void TestB4() {
            var map = MAPS.B4;
            var solver = new Solver(map, 2);
            Assert.IsTrue(solver.TrySolveOneDefaultDirs(out var stage, out int step));
            stage.PrintMap();
        }

        [TestMethod]
        public void TestB10() {
            var map = MAPS.B10;
            var solver = new Solver(map, 2);
            Assert.IsTrue(solver.TrySolveOneDefaultDirs(out var stage, out int step));
            stage.PrintMap();
        }

        [TestMethod]
        public void TestZentrollHard() {
            var map = MAPS.B_7;
            var solver = new Solver(map, 2);
            Assert.IsTrue(solver.TrySolveOneDefaultDirs(out var stage, out int step));
            stage.PrintMap();
        }
    }
}
