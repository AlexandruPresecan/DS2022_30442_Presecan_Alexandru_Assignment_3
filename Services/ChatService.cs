using Grpc.Core;

namespace DS2022_30442_Presecan_Alexandru_Assignment_3.Services
{
    public class ChatService : Chat.ChatBase
    {
        readonly ILogger<ChatService> _logger;
        readonly Connections<Message> _messageConnections;
        readonly Connections<IsTyping> _isTypingConnections;
        readonly Connections<Seen> _seenConnections;

        public ChatService(ILogger<ChatService> logger, Connections<Message> messageConnections, Connections<IsTyping> isTypingConnections, Connections<Seen> seenConnections)
        {
            _logger = logger;
            _messageConnections = messageConnections;
            _isTypingConnections = isTypingConnections;
            _seenConnections = seenConnections;
        }

        public override async Task MessageStream(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            string source = context.RequestHeaders.Get("username").Value;

            _messageConnections.Add(source, responseStream);

            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested && context.Status.StatusCode != StatusCode.Cancelled)
            {
                Message message = new Message
                {
                    UserName = source,
                    Text = requestStream.Current.Text
                };

                if (_messageConnections.Contains(requestStream.Current.UserName))
                    try
                    {
                        await _messageConnections.Get(requestStream.Current.UserName).WriteAsync(message);
                    }
                    catch
                    {

                    }
            }

            await Task.Delay(Timeout.Infinite);

            _messageConnections.Remove(source);
        }

        public async override Task IsTypingStream(IAsyncStreamReader<IsTyping> requestStream, IServerStreamWriter<IsTyping> responseStream, ServerCallContext context)
        {
            string source = context.RequestHeaders.Get("username").Value;

            _isTypingConnections.Add(source, responseStream);

            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested && context.Status.StatusCode != StatusCode.Cancelled)
            {
                IsTyping isTyping = new IsTyping
                {
                    UserName = source
                };

                if (_isTypingConnections.Contains(requestStream.Current.UserName))
                    try
                    {
                        await _isTypingConnections.Get(requestStream.Current.UserName).WriteAsync(isTyping);
                    }
                    catch
                    {

                    }
            }

            await Task.Delay(Timeout.Infinite);

            _isTypingConnections.Remove(source);
        }

        public async override Task SeenStream(IAsyncStreamReader<Seen> requestStream, IServerStreamWriter<Seen> responseStream, ServerCallContext context)
        {
            string source = context.RequestHeaders.Get("username").Value;

            _seenConnections.Add(source, responseStream);

            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested && context.Status.StatusCode != StatusCode.Cancelled)
            {
                Seen seen = new Seen
                {
                    UserName = source
                };

                if (_seenConnections.Contains(requestStream.Current.UserName)) try
                    {
                        await _seenConnections.Get(requestStream.Current.UserName).WriteAsync(seen);
                    }
                    catch
                    {

                    }
            }

            await Task.Delay(Timeout.Infinite);

            _seenConnections.Remove(source);
        }
    }
}