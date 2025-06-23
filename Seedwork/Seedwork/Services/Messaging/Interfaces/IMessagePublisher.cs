using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;

namespace Seedwork.Services.Messaging.Interfaces
{
    public interface IMessagePublisher<T>
    {
        bool Publish(Message<T> message);
    }

    [ExcludeFromCodeCoverage]
    public class Message<T>
    {
        public Headers Headers { get; }
        public T Payload { get; }
    }

    [ExcludeFromCodeCoverage]
    public class Headers
    {
        IDictionary<string, object> _headers = new Dictionary<string, object>();

        public void Add(string key, object value) =>
            _headers.Add(key, value);
    }
}
