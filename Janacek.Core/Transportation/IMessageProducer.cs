using System.Threading.Tasks;

namespace Janacek
{
    public interface IMessageProducer
    {
        IMessageProducer Client(IChannelSender sender);

        IMessageProducer Client(IChannelSender sender, string pattern);

        Task<Message> ActAsync(Message message);
    }
}
