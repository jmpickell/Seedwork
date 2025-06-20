using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.IOC.Ninject.Tests.Mock
{
    public interface ITestClass
    {
        public string Name { get; }
    }

    public interface ITestClass<T>
    {
        public string Name { get; }
    }

    public interface ITestClass2
    {
        public string Name2 { get; }
    }

    public class TestClass1 : ITestClass
    {
        public string Name => "Cutie!";
    }

    public class TestClass2 : ITestClass, ITestClass2
    {
        private readonly Lazy<ITestClass<string>> _str;
        public TestClass2(Lazy<ITestClass<string>> str)
        {
            _str = str;
        }

        public string Name => _str.Value.Name + "desu~ ";
        public string Name2 => "Moshi Moshi!";
    }

    public class TestClass3<T> : ITestClass<T>
    {
        public string Name => "Kawaii! ";
    }
}
