using Seedwork.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.Tests.Utilities
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void To_SameType_NoIssue()
        {
            object o = 1;
            Assert.AreEqual(1, o.To<int>());
        }

        [TestMethod]
        public void Convert_TypeChanged_NoIssue()
        {
            object o = 1;
            Assert.AreEqual(1L, o.Convert<long>());
        }

        [TestMethod]
        public void Then_OneParameter_NoIssue()
        {
            var i = 1;
            Assert.AreEqual(2, i.Then(i => i + 1));
        }

        [TestMethod]
        public void Then_TwoParameters_NoIssue()
        {
            var a = 1;
            var b = 2;
            Assert.AreEqual(3, a.Then(b, (i, j) => i + j));
        }

        [TestMethod]
        public void Lastly_Executed_NoIssue()
        {
            var i = 1;
            i.Lastly(j => i++);
            Assert.AreEqual(2, i);
        }
    }
}
