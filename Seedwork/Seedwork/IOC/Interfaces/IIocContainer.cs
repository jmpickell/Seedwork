using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC.Interfaces
{
    public interface IIocContainer : IDisposable
    {
        IIocBuilder AsDependency();
        IIocBuilder AsRequest();
        IIocBuilder AsSingleton();
        IIocScope Build();
    }

    public interface IIocBuilder : IIocBinder
    {
        IIocBinder Named(string name);
    }

    public interface IIocBinder
    {
        void Bind(Type from, params Type[] to);
        void BindGeneric(Type from, params Type[] to);
        void BindMethod(Func<IIocScope, object> method, params Type[] to);
    }
}
