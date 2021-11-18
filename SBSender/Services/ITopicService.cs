
using static SBShared.Utils.Enums;

namespace SBSender.Services
{
    public interface ITopicService
    {
        IConfiguration Config { get; }

        Task SendMessageAsync<T>(T serviceBusMessage, TopicEnum topic);
    }
}