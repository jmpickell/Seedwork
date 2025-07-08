using IBM.XMS;
using Seedwork.Services.Messaging.IbmMq.Settings;
using Seedwork.Services.Messaging.IbmMq.Utilities;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Kafka
{
    public class IbmMessagePublisher<T> : IMessagePublisher<T>
    {
        private readonly IAdapter<T, byte[]> _mAdapter;
        private readonly IbmMqSettings _settings;
        private readonly IMessageProducer _producer;
        private readonly ISession _session;

        public IbmMessagePublisher(IbmMqSettings settings, IConnection connection, IAdapter<T, byte[]> mAdapter)
        {
            _mAdapter = mAdapter;
            _settings = settings;
            _session = connection.CreateSession(_settings.Transacted, _settings.AcknowledgeMode);
            _producer = _session.CreateProducer(null);
        }

        public Task<bool> Publish(Message<T> message, CancellationToken token = default)
        {
            var ibmMessage = _session.CreateBytesMessage();
            ibmMessage.JMSMessageID = message.Id;
            ibmMessage.JMSDestination = _session.Create(_settings.Destination.name, _settings.Destination.type);
            foreach(var header in message.Headers.GetAll())
                ibmMessage.SetObjectProperty(header.Key, header.Value);
            ibmMessage.WriteBytes(_mAdapter.Convert(message.Payload));
            
            _producer.Send(ibmMessage);

            return Task.FromResult(true);
        }
    }
}
