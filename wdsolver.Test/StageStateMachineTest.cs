using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace wdsolver.Test {
    [TestClass]
    public class StageStateMachineTest {
        [TestMethod]
        public void TestBasicMove() {
            var map = @"
XX XX XX XX
01 00 00 99
XX XX XX XX";

            var stage = new Stage(map);

            Assert.IsFalse(stage.CanGo(Vector2.UP));
            Assert.IsFalse(stage.CanGo(Vector2.DOWN));
            Assert.IsFalse(stage.CanGo(Vector2.LEFT));
            Assert.IsTrue(stage.CanGo(Vector2.RIGHT));

            var actions = stage.Goto(Vector2.RIGHT);

            CollectionAssert.AreEqual(new[] { new Goto { xy = (0, 1) } }, actions);
        }
    }
}
