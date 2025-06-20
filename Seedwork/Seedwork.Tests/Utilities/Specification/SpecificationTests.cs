using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.Tests.Utilities.Specification
{
    public class MockSpecification : Specification<object>
    {
        private readonly bool _isSatisfied;

        public MockSpecification(bool isSatisfied) => 
            _isSatisfied = isSatisfied;

        public override bool IsSatisfied(object item) =>
            _isSatisfied;
    }

    [TestClass]
    public class SpecificationTests
    {
        [TestMethod]
        public void IsSatisifed_And_Success()
        {
            var @true = new MockSpecification(true);
            var @false = new MockSpecification(false);

            var sut1 = @false & @false;
            var sut2 = @true  & @false;
            var sut3 = @false & @true;
            var sut4 = @true  & @true;

            Assert.IsFalse(sut1.IsSatisfied(new object()));
            Assert.IsFalse(sut2.IsSatisfied(new object()));
            Assert.IsFalse(sut3.IsSatisfied(new object()));
            Assert.IsTrue(sut4.IsSatisfied(new object()));
        }

        [TestMethod]
        public void IsSatisifed_Or_Success()
        {
            var @true = new MockSpecification(true);
            var @false = new MockSpecification(false);

            var sut1 = @false | @false;
            var sut2 = @true  | @false;
            var sut3 = @false | @true;
            var sut4 = @true  | @true;

            Assert.IsFalse(sut1.IsSatisfied(new object()));
            Assert.IsTrue(sut2.IsSatisfied(new object()));
            Assert.IsTrue(sut3.IsSatisfied(new object()));
            Assert.IsTrue(sut4.IsSatisfied(new object()));
        }

        [TestMethod]
        public void IsSatisifed_Not_Success()
        {
            var @true = new MockSpecification(true);

            var sut1 = @true;
            var sut2 = !@true;

            Assert.IsTrue(sut1.IsSatisfied(new object()));
            Assert.IsFalse(sut2.IsSatisfied(new object()));
        }
    }
}
