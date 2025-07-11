using Confluent.Kafka;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Utilities;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Kafka
{
    public class KafkaMessageConsumer<T> : IMessageConsumer<T>
    {
        CancellationTokenSource _source;
        private readonly IConsumer<string, T> _consumer;
        private readonly IAdapter<byte[], object> _adapter;
        Task _task;

        public KafkaMessageConsumer(IConsumer<string, T> consumer, IAdapter<byte[], object> adapter)
        {
            _source = new CancellationTokenSource();
            _consumer = consumer;
            _adapter = adapter;
        }

        public Message<T> Consume(CancellationToken token)
        {
            var result = _consumer.Consume(token);
            var message = new Message<T>();
            message.Payload = result.Message.Value;

            foreach (var header in result.Message.Headers)
                message.Headers.Add(header.Key, _adapter.Convert(header.GetValueBytes()));

            return message;
        }

        public Task Start(Func<Message<T>, CancellationToken, Task> OnMessage) =>
            _task = _source.WhileNotCanceled(() => OnMessage(Consume(_source.Token), _source.Token));


        public async Task Stop()
        {
            _source.Cancel();
            if(_task != null)
                await _task;
            _consumer.Dispose();
        }
    }
}
