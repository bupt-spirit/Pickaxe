using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PickaxeCore.Relation.Tests
{
    [TestClass()]
    public class AttributeTests
    {
        [TestMethod()]
        public void AttributeTest()
        {
            AttributeType a1 = new AttributeType.Numeric();
            AttributeType a2 = new AttributeType.Nominal(new string[] { "type1", "type2" });
            AttributeType a3 = new AttributeType.Binary();
            Assert.IsTrue(a1.ValidateValue(Value.ToValue(1)));
            Assert.IsFalse(a1.ValidateValue(Value.MISSING));
            Assert.IsTrue(a1.ValidateValueWithMissing(Value.MISSING));

            Assert.IsTrue(a2.ValidateValue(Value.ToValue(1)));
            Assert.IsFalse(a2.ValidateValue(Value.ToValue(2)));
            Assert.IsFalse(a2.ValidateValue(Value.MISSING));
            Assert.IsTrue(a2.ValidateValueWithMissing(Value.MISSING));

            Assert.IsTrue(a3.ValidateValue(Value.ToValue(0)));
            Assert.IsTrue(a3.ValidateValue(Value.ToValue(1)));
            Assert.IsFalse(a3.ValidateValue(Value.ToValue(2)));
            Assert.IsFalse(a3.ValidateValue(Value.MISSING));
            Assert.IsTrue(a3.ValidateValueWithMissing(Value.MISSING));

            Assert.AreEqual(a1.GetType(), typeof(AttributeType.Numeric));
            Assert.AreEqual(a2.GetType(), typeof(AttributeType.Nominal));
            Assert.AreEqual(a3.GetType(), typeof(AttributeType.Binary));
        }
    }

    [TestClass()]
    public class NominalTests
    {
        [TestMethod()]
        public void AddTest()
        {
            AttributeType.Nominal n = new AttributeType.Nominal();
            n.Add("nominal1");
            Assert.AreEqual(n.Count, 1);
            n.Add("nominal2");
            Assert.AreEqual(n.Count, 2);
            n.Add("nominal3");
            Assert.AreEqual(n.Count, 3);
            Assert.AreEqual(n[0], "nominal1");
            Assert.AreEqual(n["nominal1"], 0);
            Assert.AreEqual(n[1], "nominal2");
            Assert.AreEqual(n["nominal2"], 1);
            Assert.AreEqual(n[2], "nominal3");
            Assert.AreEqual(n["nominal3"], 2);
            Assert.ThrowsException<System.ArgumentException>(() =>
            {
                n.Add("nominal1");
            });
        }

        [TestMethod()]
        public void RemoveTest()
        {
            AttributeType.Nominal n = new AttributeType.Nominal(
                new string[] { "nominal1", "nominal2" }
                );
            Assert.AreEqual(n.Count, 2);
            var data = new List<Value> { 0, 1, 0, 1 };
            n.Remove("nominal1", data);
            Assert.AreEqual(n.Count, 1);
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                n.Remove("nominal1", data);
            });
            n.Remove(0, data);
            Assert.AreEqual(n.Count, 0);
            CollectionAssert.AreEqual(
                data,
                new Value[] { Value.MISSING, Value.MISSING, Value.MISSING, Value.MISSING });
        }
    }
}