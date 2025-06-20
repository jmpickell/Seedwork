using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Interfaces
{
    public interface IMessageConsumer<T>
    {
        Message<T> Consume();
        Task Start(Action<Message<T>> action);
        void Stop();
    }
}
