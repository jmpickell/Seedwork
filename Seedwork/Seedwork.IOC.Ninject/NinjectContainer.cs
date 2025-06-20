using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Syntax;
using Seedwork.IOC.Interfaces;
using Seedwork.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Seedwork.IOC.Ninject
{
    public class NinjectContainer : NinjectModule, IIocContainer, IIocBuilder
    {
        static readonly List<Action<IKernel>> _actions = new List<Action<IKernel>>();
        Func<IBindingWhenInNamedWithOrOnSyntax<object>, IBindingNamedWithOrOnSyntax<object>> _scope;
        Action<IBindingNamedWithOrOnSyntax<object>> _named;
        private string _name;

        public NinjectContainer()
        {
            _scope = b => b.To<IBindingNamedWithOrOnSyntax<object>>();
            _named = b => { };
        }

        public override void Load()
        {
            Bind<IIocScope>().To<NinjectScope>();
            foreach(var action in _actions)
                action.Invoke(this.Kernel);
        }

        public IIocBuilder AsDependency()
        {
            _name = null;
            _named = b => { };
            _scope = b => b.To<IBindingNamedWithOrOnSyntax<object>>();
            return this;
        }

        public IIocBuilder AsRequest()
        {
            _name = null;
            _named = b => { };
            _scope = b => b.InThreadScope();
            return this;
        }

        public IIocBuilder AsSingleton()
        {
            _name = null;
            _named = b => { };
            _scope = b => b.InSingletonScope();
            return this;
        }

        public IIocBinder Named(string name)
        {
            _name = name;
            _named = b => b.Named(name);
            return this;
        }

        public void Bind(Type from, params Type[] to) =>
            Bind(b => b.To(from), to);

        public void BindGeneric(Type from, params Type[] to) =>
            Bind(b => b.To(from), to);

        public void BindMethod(Func<IIocScope, object> method, params Type[] to) =>
            Bind(b => b.ToMethod(k => k.Kernel.Get<IIocScope>().Then(method)), to);

        private void Bind(
            Func<IBindingToSyntax<object>, IBindingWhenInNamedWithOrOnSyntax<object>> binding, 
            params Type[] to)
        {
            if (to.Length == 0)
                return;

            var scope = _scope;
            var name = _name;
            var named = _named;
            var interfaces = to;
            var bindings = binding;

            _actions.Add(k =>
            {
                k.Bind(interfaces[0]).Then(bindings).Then(scope).Lastly(named);
                for (int i = 1; i < interfaces.Length; i++)
                    k.Bind(interfaces[i]).ToMethod(c => name != null ? 
                        c.Kernel.Get(interfaces[0], name) : 
                        c.Kernel.Get(interfaces[0]))
                    .Then(scope).Lastly(named);
            });

            _name = null;
            _scope = b => b.To<IBindingNamedWithOrOnSyntax<object>>();
            _named = b => { };
        }

        public IIocScope Build()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _actions.Clear();

            var parameter = new ConstructorArgument("kernel", kernel);
            return kernel.Get<IIocScope>(parameter);
        }
    }
}
