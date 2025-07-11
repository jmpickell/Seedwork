using IBM.XMS;
using Seedwork.Services.Messaging.IbmMq.Settings;
using Seedwork.Services.Messaging.IbmMq.Utilities;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Utilities;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Kafka
{
    public class IbmMessageConsumer<T> : IMessageConsumer<T>
    {
        CancellationTokenSource _source;
        private readonly IAdapter<byte[], T> _mAdapter;
        private readonly IbmMqSettings _settings;
        private readonly ISession _session;
        private readonly IMessageConsumer _consumer;
        Task _task;

        public IbmMessageConsumer(IbmMqSettings settings, IConnection connection, IAdapter<byte[], T> mAdapter)
        {
            _mAdapter = mAdapter;
            _settings = settings;
            _session = connection.CreateSession(_settings.Transacted, _settings.AcknowledgeMode);
            var destination = _session.Create(_settings.Destination.name, _settings.Destination.type);
            _consumer = _session.CreateConsumer(destination);

        }

        public Message<T> Consume(CancellationToken token)
        {
            var result = _consumer.ReceiveNoWait();
            if (result == null)
                return null;

            var msg = (IBytesMessage)result;
            var message = new Message<T>();
            message.Id = msg.JMSMessageID;
            byte[] buffer = new byte[msg.BodyLength];
            msg.ReadBytes(buffer);
            message.Payload = _mAdapter.Convert(buffer);
            return message;
        }

        public Task Start(Func<Message<T>, CancellationToken, Task> OnMessage) =>
            _task = _source.WhileNotCanceled(() => OnMessage(Consume(_source.Token), _source.Token));


        public async Task Stop()
        {
            _source.Cancel();
            if (_task != null)
                await _task;
            _consumer.Dispose();
        }
    }
}
