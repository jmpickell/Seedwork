using RabbitMQ.Client;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.RabbitMq
{
    public class RabbitMessageConsumer<T> : IMessageConsumer<T>
    {
        private CancellationTokenSource _source;
        private readonly IChannel _channel;
        private readonly IAdapter<byte[], T> _adapter;
        private readonly string _queue;
        private Task _task;

        public RabbitMessageConsumer(IConnection connection, IAdapter<byte[], T> adapter, string queue)
        {
            _channel = connection.CreateChannelAsync().Result;
            _source = new CancellationTokenSource();
            _adapter = adapter;
            _queue = queue;
        }

        public async Task<Message<T>> Consume(CancellationToken token = default)
        {
            var result = await _channel.BasicGetAsync(_queue, true, token);
            var message = new Message<T>();
            message.Payload = _adapter.Convert(result.Body.ToArray());
            message.Headers = new Interfaces.Headers(result.BasicProperties.Headers);

            return message;
        }

        public async Task Start(Func<Message<T>, CancellationToken, Task> onMessage) =>
            _task = Task.Run(async () => { while (!_source.IsCancellationRequested) await onMessage(await Consume(_source.Token), _source.Token); });

        public async Task Stop()
        {
            _source.Cancel();
            if(_task != null)
                await _task;
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
    }
}
