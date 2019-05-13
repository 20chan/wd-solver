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
            Assert.AreEqual(new WayPoint { Value = 1 }, stage.atDEBUG(new Vector2(0, 1)));
            Assert.AreEqual(new WayPoint { Value = 2 }, stage.atDEBUG(new Vector2(1, 1)));
            Assert.AreEqual(new WayPoint { Value = 3 }, stage.atDEBUG(new Vector2(2, 1)));
            Assert.AreEqual(new WayPoint { Value = 4 }, stage.atDEBUG(new Vector2(3, 1)));
        }
    }
}
