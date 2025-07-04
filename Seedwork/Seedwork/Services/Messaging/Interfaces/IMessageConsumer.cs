using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Interfaces
{
    public interface IMessageConsumer<T>
    {
        Task<Message<T>> Consume(CancellationToken token);
        Task Start(Action<Message<T>, CancellationToken> action);
        Task Stop();
    }
}
