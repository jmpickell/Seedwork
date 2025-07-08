using IBM.WMQ;
using IBM.XMS;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace Seedwork.Services.Messaging.IbmMq.Utilities
{
    public static class Extensions
    {
        public static IDestination Create(this ISession session, string name, DestinationType type)
        {
            switch (type)
            {
                case DestinationType.Topic: return session.CreateTopic(name);
                case DestinationType.Queue: return session.CreateQueue(name);
                default: throw new ArgumentOutOfRangeException("Invalid Destination Type");
            }
        }
    }
}
