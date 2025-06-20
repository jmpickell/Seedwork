using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Utilities.Interfaces
{
    public interface IMapper<in TInput, TOutput>
    {
        TOutput Map(TInput input, TOutput output = default);
    }
}
