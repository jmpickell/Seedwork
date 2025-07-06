using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seedwork.Services.Messaging.Interfaces
{
    public interface IMessagePublisher<T>
    {
        Task<bool> Publish(Message<T> message, CancellationToken token = default);
    }

    [ExcludeFromCodeCoverage]
    public class Message<T>
    {
        public string Id { get; set; }
        public Headers Headers { get; set; }
        public T Payload { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Headers
    {
        public Headers() =>
            _headers = new Dictionary<string, object>();

        public Headers(IDictionary<string, object> headers) =>
            _headers = headers;

        IDictionary<string, object> _headers;

        public void Add(string key, object value) =>
            _headers.Add(key, value);

        public IDictionary<string, object> GetAll() => _headers;
    }
}
