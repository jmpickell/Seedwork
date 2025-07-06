using RabbitMQ.Client;
using Seedwork.IOC;
using Seedwork.IOC.Interfaces;
using Seedwork.Services.Messaging.Interfaces;
using Seedwork.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Services.Messaging.RabbitMq.IOC
{
    public static class Bootstrapper
    {
        public static void RegisterRabbit(this IIocContainer container)
        {
            container.AsDependency().BindGeneric(typeof(RabbitMessagePublisher<>), typeof(RabbitMessagePublisher<>));
            container.AsSingleton().BindMethod<IConnection>(c => c.Resolve<IConnectionFactory>().CreateConnectionAsync().Result);

        }

        public static void RegisterRabbitMqPublisher<T, TSerializer>(this IIocContainer container, string name)
            where TSerializer : IAdapter<T, byte[]>
        {
            container.AsDependency().BindSelf<TSerializer>();
            container.AsDependency().Named(name).BindMethod<IMessagePublisher<T>>(c => c.Resolve<RabbitMessagePublisher<T>>(("queue", name), ("adapter", c.Resolve<TSerializer>(name))));
        }

        public static void RegisterRabbitMqConsumer<T, TDeserializer>(this IIocContainer container, string name)
            where TDeserializer : IAdapter<byte[], T>
        {
            container.AsDependency().BindSelf<TDeserializer>();
            container.AsDependency().Named(name).BindMethod<IMessageConsumer<T>>(c => c.Resolve<RabbitMessageConsumer<T>>(("queue", name), ("adapter", c.Resolve<TDeserializer>(name))));
        }
    }
}
