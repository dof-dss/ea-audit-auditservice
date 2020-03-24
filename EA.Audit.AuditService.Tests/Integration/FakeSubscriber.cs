using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Tests.Integration
{
    internal class FakeSubscriber : ISubscriber
    {
        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public bool TryWait(Task task)
        {
            throw new NotImplementedException();
        }

        public void Wait(Task task)
        {
            throw new NotImplementedException();
        }

        public T Wait<T>(Task<T> task)
        {
            throw new NotImplementedException();
        }

        public void WaitAll(params Task[] tasks)
        {
            throw new NotImplementedException();
        }

        public IConnectionMultiplexer Multiplexer { get; }
        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public EndPoint IdentifyEndpoint(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected(RedisChannel channel = new RedisChannel())
        {
            throw new NotImplementedException();
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public async Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            return 1;
        }

        public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public ChannelMessageQueue Subscribe(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<ChannelMessageQueue> SubscribeAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public EndPoint SubscribedEndpoint(RedisChannel channel)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }
    }
}
