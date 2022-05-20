using System;
using System.Threading.Tasks;

namespace Janacek
{
    public interface IMessageConsumer
    {
        IMessageConsumer Client(IChannelSender sender);

        IMessageConsumer Client(IChannelSender sender, string pattern);

        IMessageConsumer Add(string pattern, Func<Message, Task<Message>> action);

        IMessageConsumer Listen(IChannelReceiver receiver);

        Task<Message> ActAsync(Message message);

        Task Run();
    }
}
