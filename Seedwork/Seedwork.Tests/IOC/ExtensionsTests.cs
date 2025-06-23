using Moq;
using Seedwork.IOC.Interfaces;
using Seedwork.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.Tests.IOC
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void Bind_NoIssues_BindCalled()
        {
            var sut = new Mock<IIocBuilder>();
            sut.Object.Bind<object, object>();
            sut.Object.Bind<object, object, object>();
            sut.Object.Bind<object, object, object, object>();
            sut.Object.BindSelf<object>();

            sut.Verify(x => x.Bind(It.IsAny<Type>(), It.IsAny<Type[]>()), Times.Exactly(4));
        }

        [TestMethod]
        public void BindMethod_NoIssues_BindMethodCalled()
        {
            var sut = new Mock<IIocBuilder>();
            sut.Object.BindMethod<object>(s => new object());
            sut.Object.BindMethod<object, object>(s => new object());
            sut.Object.BindMethod<object, object, object>(s => new object());

            sut.Verify(x => x.BindMethod(It.IsAny<Func<IIocScope, object>>(), It.IsAny<Type[]>()), Times.Exactly(3));
        }
    }
}
