using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.IOC.Ninject.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void Convert_Parameter_Converted() =>
            Assert.IsInstanceOfType<IParameter>(new IocParameter { Name = "A", Value = "B" }.Convert());
    }
}
