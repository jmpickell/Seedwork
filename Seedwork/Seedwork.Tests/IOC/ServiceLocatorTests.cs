using Autofac;
using Moq;
using Seedwork.IOC;
using Seedwork.IOC.Interfaces;

namespace Seedwork.Tests.IOC
{
    [TestClass]
    public sealed class ServiceLocatorTests
    {
        private Mock<IIocScope> _mockScope;
        private ServiceLocator _sut;

        [TestInitialize]
        public void Initialize() 
        {
            _mockScope = new Mock<IIocScope>();
            _sut = ServiceLocator.Instance;
        
        }

        [TestMethod]
        public void GetScope_NotNull_Success()
        {
            _sut.RegisterScope(_mockScope.Object);
            Assert.IsNotNull(_sut.GetScope());
        }
    }
}
