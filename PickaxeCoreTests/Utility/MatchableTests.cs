using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PickaxeCore.Utility.Matchable.Tests
{
    [TestClass()]
    public class MatchableTests
    {
        [TestMethod()]
        public void MatchTest()
        {
            var array = new Matchable<int, string>[]
            {
                new Matchable<int, string>.TypeA(1),
                new Matchable<int, string>.TypeB("hello"),
                new Matchable<int, string>.TypeA(2),
                new Matchable<int, string>.TypeA(3),
            };
            var results = new string[] {
                "2",
                "hello, world!",
                "4",
                "6",
            };
            Assert.AreEqual(array.Length, results.Length);
            for (int i = 0; i < array.Length; ++i)
            {
                var item = array[i];
                item.Match(
                    (ref int num) => num *= 2,
                    (ref string str) => str += ", world!"
                    );
                string output = item.Match(
                    (ref int num) => num.ToString(),
                    (ref string str) => str.ToString()
                    );
                Assert.AreEqual(results[i], output);
            }
        }
    }
}