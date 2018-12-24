using Microsoft.VisualStudio.TestTools.UnitTesting;
using PickaxeCore.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeCore.Utility.ListExtension.Tests
{
    [TestClass()]
    public class ListExtensionTests
    {
        [TestMethod()]
        public void ResizeTest()
        {
            var list = new List<int> { 0, 1, 2, 3 };
            list.Resize(6);
            CollectionAssert.AreEqual(list, new int[] { 0, 1, 2, 3, 0, 0 });
            list.Resize(8, -1);
            CollectionAssert.AreEqual(list, new int[] { 0, 1, 2, 3, 0, 0, -1, -1 });
            list.Resize(3);
            CollectionAssert.AreEqual(list, new int[] { 0, 1, 2 });
        }
    }
}