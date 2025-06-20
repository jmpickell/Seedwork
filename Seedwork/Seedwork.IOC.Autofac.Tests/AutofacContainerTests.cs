using Seedwork.IOC.Autofac.Tests.Mock;
using Seedwork.Utilities;

namespace Seedwork.IOC.Autofac.Tests
{
    [TestClass]
    public sealed class AutofacContainerTests
    {
        private AutofacContainer _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new AutofacContainer();
        }

        [TestMethod]
        public void Bind_AsDependency_TwoInstancesNotSame()
        {
            _sut.AsDependency().Bind<TestClass1, ITestClass>();
            var scope = _sut.Build();

            var r1 = scope.Resolve<ITestClass>();
            var r2 = scope.Resolve<ITestClass>();

            Assert.AreNotSame(r1, r2);
        }

        [TestMethod]
        public void Bind_AsSingleton_TwoInstancesNotSame()
        {
            _sut.AsSingleton().Bind<TestClass1, ITestClass>();
            var scope = _sut.Build();

            var r1 = scope.Resolve<ITestClass>();
            var r2 = scope.Resolve<ITestClass>();

            Assert.AreSame(r1, r2);
        }

        //[TestMethod]
        public void Bind_AsRequest_TwoInstancesSameDifferentThreadNot()
        {
            _sut.AsRequest().Bind<TestClass1, ITestClass>();
            var scope = _sut.Build();

            Task<ITestClass> task1 = new Task<ITestClass>(() =>
            {
                var r1 = scope.Resolve<ITestClass>();
                var r2 = scope.Resolve<ITestClass>();
                Assert.AreSame(r1, r2);
                return r1;
            });
            Task<ITestClass> task2 = new Task<ITestClass>(() =>
            {
                var r1 = scope.Resolve<ITestClass>();
                var r2 = scope.Resolve<ITestClass>();
                Assert.AreSame(r1, r2);
                return r1;
            });

            task2.Start();
            task1.Start();
            Task.WaitAll(task1, task2);
            Assert.AreNotSame(task1.Result, task2.Result);
        }

        [TestMethod]
        public void Bind_Named_NotNull()
        {
            _sut.AsDependency().Named("applepie").Bind<TestClass1, ITestClass>();
            var result = _sut.Build();

            Assert.IsNotNull(result.Resolve<ITestClass>("applepie"));
        }

        [TestMethod]
        public void BindMethod_NoIssue_NotNull()
        {
            _sut.AsDependency().BindMethod(s => new TestClass1(), typeof(ITestClass));
            var result = _sut.Build();

            Assert.IsNotNull(result.Resolve<ITestClass>());
        }

        [TestMethod]
        public void BindGeneric_NoIssue_NotNull()
        {
            _sut.AsDependency().BindGeneric(typeof(TestClass3<>), typeof(ITestClass<>));
            var result = _sut.Build();

            Assert.IsNotNull(result.Resolve<ITestClass<int>>());
            Assert.IsNotNull(result.Resolve<ITestClass<string>>());
        }

        [TestMethod]
        public void BindSingleton_Multiple_SameObject()
        {
            _sut.AsDependency().BindGeneric(typeof(TestClass3<>), typeof(ITestClass<>));
            _sut.AsSingleton().Bind(typeof(TestClass2), typeof(ITestClass), typeof(ITestClass2));
            var result = _sut.Build();

            var r1 = result.Resolve<ITestClass>();
            var r2 = result.Resolve<ITestClass2>();

            Assert.AreSame(r1.To<TestClass2>(), r2.To<TestClass2>());
        }

        [TestCleanup]
        public void Cleanup() 
        {
            _sut.Dispose();
            _sut = null;
            GC.SuppressFinalize(this);
        }
    }
}
