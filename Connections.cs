using Grpc.Core;
using System.Collections.Concurrent;

namespace DS2022_30442_Presecan_Alexandru_Assignment_3
{
    public class Connections<T>
    {
        readonly ConcurrentDictionary<string, IServerStreamWriter<T>> _connections = new ConcurrentDictionary<string, IServerStreamWriter<T>>();

        public bool Contains(string userName) => _connections.ContainsKey(userName);
        public IServerStreamWriter<T> Get(string userName) => _connections[userName];
        public void Add(string userName, IServerStreamWriter<T> stream) => _connections[userName] = stream;
        public void Remove(string userName) => _connections.Remove(userName, out var removed);
    }
}
