using Seedwork.IOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC
{
    public static class Extensions
    {
        public static void Bind<TFrom, TTo>(this IIocBinder binder) =>
            binder.Bind(typeof(TFrom), typeof(TTo));
        public static void Bind<TFrom, TTo, TTo2>(this IIocBinder binder) =>
            binder.Bind(typeof(TFrom), typeof(TTo), typeof(TTo2));
        public static void Bind<TFrom, TTo, TTo2, TTo3>(this IIocBinder binder) =>
            binder.Bind(typeof(TFrom), typeof(TTo), typeof(TTo2), typeof(TTo3));

        public static void BindSelf<TSelf>(this IIocBinder binder) =>
            binder.Bind(typeof(TSelf), typeof(TSelf));

        public static void BindMethod<TTo>(this IIocBinder binder, Func<IIocScope, object> method) =>
            binder.BindMethod(method, typeof(TTo));
        public static void BindMethod<TTo, TTo2>(this IIocBinder binder, Func<IIocScope, object> method) =>
            binder.BindMethod(method, typeof(TTo), typeof(TTo2));
        public static void BindMethod<TTo, TTo2, TTo3>(this IIocBinder binder, Func<IIocScope, object> method) =>
            binder.BindMethod(method, typeof(TTo), typeof(TTo2), typeof(TTo3));
    }
}
