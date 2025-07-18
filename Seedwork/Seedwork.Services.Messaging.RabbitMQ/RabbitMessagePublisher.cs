﻿using RabbitMQ.Client;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.RabbitMq
{
    public class RabbitMessagePublisher<T> : IMessagePublisher<T>
    {
        private readonly IChannel _channel;
        private readonly IAdapter<T, byte[]> _mAdapter;
        private readonly string _queue;

        public RabbitMessagePublisher(IConnection connection, IAdapter<T, byte[]> mAdapter, string queue)
        {
            _channel = connection.CreateChannelAsync().Result;        
            _mAdapter = mAdapter;
            _queue = queue;
        }

        public async Task<bool> Publish(Message<T> message, CancellationToken token = default)
        {
            BasicProperties properties = new BasicProperties();
            properties.Headers = message.Headers.GetAll();

            await _channel.BasicPublishAsync(string.Empty, _queue, false, properties, _mAdapter.Convert(message.Payload), token);
            return true;
        }
    }


}
