using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace PickaxeCore.Relation.Tests
{
    [TestClass()]
    public class RelationTests
    {
        [TestMethod()]
        public void AttributeTest()
        {
            var relation = new Relation();
            Assert.AreEqual(relation.AttributeCount, 0);
            Assert.AreEqual(relation.TupleCount, 0);
            relation.AddAttribute("a1", new AttributeType.Numeric());
            Assert.AreEqual(relation.AttributeCount, 1);
            relation.AddAttribute("a2", new AttributeType.Nominal(new string[] { "n1", "n2" }));
            Assert.ThrowsException<ArgumentException>(() =>
            {
                relation.AddTuple(new Value[] { 1.11f, 2f });
            });
            Assert.AreEqual(relation.TupleCount, 0);
            relation.AddTuple(new Value[] { 1.11f, 1f });
            Assert.AreEqual(relation.TupleCount, 1);
            relation.AddTuple(new Value[] { Value.MISSING, 0f });
            Assert.AreEqual(relation.TupleCount, 2);
        }

        [TestMethod()]
        public void ConstructTest()
        {
            var relation = new Relation(
                new List<RelationAttribute>() {
                    new RelationAttribute("a1", new AttributeType.Numeric(), new List<Value>()),
                    new RelationAttribute("a2", new AttributeType.Binary(), new List<Value>()),
                    new RelationAttribute("a3", new AttributeType.Nominal(), new List<Value>()),
                });
            Assert.AreEqual(relation.AttributeCount, 3);
            Assert.AreEqual(relation.TupleCount, 0);

            // tuple count not same
            Assert.ThrowsException<ArgumentException>(() =>
            {
                new Relation(
                    new List<RelationAttribute>() {
                    new RelationAttribute("a1", new AttributeType.Numeric(), new List<Value> { 1f }),
                    new RelationAttribute("a2", new AttributeType.Binary(), new List<Value>()),
                    new RelationAttribute("a3", new AttributeType.Nominal(), new List<Value>()),
                    });
            });
        }

        [TestMethod()]
        public void IndexerTest()
        {
            var r = new Relation(
                new List<RelationAttribute>() {
                    new RelationAttribute("a1",
                        new AttributeType.Numeric(),
                        new List<Value>{ 1.11f, 2f, Value.MISSING }),
                    new RelationAttribute("a2",
                        new AttributeType.Nominal(),
                        new List<Value>{ Value.MISSING, Value.MISSING, Value.MISSING }),
                    new RelationAttribute("a3",
                        new AttributeType.Nominal(new []{ "a", "b", "c" }),
                        new List<Value>{ 0f, 1f, 2f }),
                });
            r[0, 0] = Value.ToValue(1.12);
            Assert.AreEqual(r[0, 0], Value.ToValue(1.12));
            Assert.AreEqual(r[1, 0], Value.MISSING);
            Assert.AreEqual(r[2, 0], Value.ToValue(0));
            Assert.AreEqual(r[0, 1], Value.ToValue(2));
            Assert.AreEqual(r[1, 1], Value.MISSING);
            Assert.AreEqual(r[2, 1], Value.ToValue(1));
            Assert.AreEqual(r[0, 2], Value.MISSING);
            Assert.AreEqual(r[1, 2], Value.MISSING);
            Assert.AreEqual(r[2, 2], Value.ToValue(2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                r[3, 0] = Value.ToValue(1);
            });
            Assert.ThrowsException<ArgumentException>(() =>
            {
                r[2, 2] = Value.ToValue(3);
            });
        }
    }
}