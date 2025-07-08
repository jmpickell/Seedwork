using Confluent.Kafka;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Services.Messaging.Kafka.Settings;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Kafka
{
    public class KafkaMessagePublisher<T> : IMessagePublisher<T>
    {
        private readonly KafkaPublisherSettings _settings;
        private readonly IProducer<string, T> _producer;
        private readonly IAdapter<object, byte[]> _hAdapter;

        public KafkaMessagePublisher(KafkaPublisherSettings settings, IProducer<string, T> producer, IAdapter<object, byte[]> hAdapter)
        {
            _settings = settings;
            _producer = producer;
            _hAdapter = hAdapter;
        }

        public async Task<bool> Publish(Message<T> message, CancellationToken token = default)
        {
            var kMessage = new Message<string, T>
            {
                Key = message.Id,
                Value = message.Payload,
                Timestamp = new Timestamp(DateTime.Now)
            };
            
            kMessage.Headers = new Confluent.Kafka.Headers();
            foreach (var header in message.Headers.GetAll())
                kMessage.Headers.Add(new Header(header.Key, _hAdapter.Convert(header.Value)));

            var result = await _producer.ProduceAsync(_settings.Topic, kMessage, token);

            return result.Status == PersistenceStatus.Persisted;
        }
    }
}
