using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Services.Http
{
    public interface IHttpService<in TRequest, out TResponse>
    {
        TResponse Invoke(TRequest request, Endpoint endpoint);
    }
}
