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

            var stage = new Stage(map, 1);

            Assert.IsFalse(stage.CanGo(Vector2.UP));
            Assert.IsFalse(stage.CanGo(Vector2.DOWN));
            Assert.IsFalse(stage.CanGo(Vector2.LEFT));
            Assert.IsTrue(stage.CanGo(Vector2.RIGHT));

            var actions = stage.Goto(Vector2.RIGHT);

            CollectionAssert.AreEqual(new[] { new Goto { xy = (0, 1) } }, actions);
        }

        [TestMethod]
        public void TestMix() {
            var map = @"
XX w1 g1 y1 XX
01 00 00 00 99
XX XX XX XX XX";

            var stage = new Stage(map, 3);
            var goto0 = stage.Goto(Vector2.RIGHT);
            Assert.AreEqual(Water.White, stage.Debug.TruckType());
            Assert.AreEqual(1, stage.Debug.TruckAmount());
            CollectionAssert.AreEqual(new InteractAction[] {
                new Goto { xy = (0, 1) },
                new Pull { xy = (1, 0), oil = 0, idx = 0 }, // oil 은 기본값이어가지고 상관없다
            }, goto0);

            var goto1 = stage.Goto(Vector2.RIGHT);
            Assert.AreEqual(Water.Green, stage.Debug.TruckType());
            Assert.AreEqual(2, stage.Debug.TruckAmount());
            CollectionAssert.AreEqual(new InteractAction[] {
                new Goto { xy = (1, 1) },
                new Pull { xy = (2, 0), oil = Water.White, idx = 1 },
            }, goto1);

            var goto2 = stage.Goto(Vector2.RIGHT);
            Assert.AreEqual(Water.Green, stage.Debug.TruckType());
            Assert.AreEqual(2, stage.Debug.TruckAmount());
        }
    }
}
