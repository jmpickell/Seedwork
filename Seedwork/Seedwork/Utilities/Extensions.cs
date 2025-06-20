using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Seedwork.Utilities
{
    public static class Extensions
    {
        public static T To<T>(this object o) => (T)o;

        public static T Convert<T>(this object o) => 
            (T)System.Convert.ChangeType(o, typeof(T));

        public static U Then<T, U>(this T t, Func<T, U> func) =>
            func.Invoke(t);

        public static U Then<T, P, U>(this T t, P parameter, Func<T, P, U> func) =>
            func.Invoke(t, parameter);

        public static void Lastly<T>(this T t, Action<T> func) =>
            func.Invoke(t);
    }
}
