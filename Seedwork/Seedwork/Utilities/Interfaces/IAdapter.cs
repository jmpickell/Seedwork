using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Utilities.Interfaces
{
    public interface IAdapter<in T, out O>
    {
        O Convert(T t);
    }
}
