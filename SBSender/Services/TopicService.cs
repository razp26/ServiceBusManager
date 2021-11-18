using Azure.Messaging.ServiceBus;

using System.Text;
using System.Text.Json;

using static SBShared.Utils.Enums;

namespace SBSender.Services
{
    public class TopicService : ITopicService
    {
        public IConfiguration Config { get; }

        public TopicService(IConfiguration config)
        {
            Config = config;
        }

        /// <summary>
        /// Sends async message into the service bus
        /// </summary>
        /// <typeparam name="T">Any type of object</typeparam>
        /// <param name="serviceBusMessage">Any type of object containing message to be sent</param>
        /// <param name="topic">Topic Id</param>
        /// <returns></returns>
        public async Task SendMessageAsync<T>(T serviceBusMessage, TopicEnum topic)
        {
            //Getting topic name
            string topicName = GetTopicName(topic);

            //Creating client and sender
            var endpoint = Config.GetValue<string>("AzureServiceBus:Endpoint");
            var client = new ServiceBusClient(Config.GetValue<string>("AzureServiceBus:Endpoint"));
            var sender = client.CreateSender(topicName);

            //Creating message to be sent
            string messageBody = JsonSerializer.Serialize(serviceBusMessage);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));

            try
            {
                //Sending message
                await sender.SendMessageAsync(message);
            }
            finally
            {
                //Cleaning up resources
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
        /// <summary>
        /// Returns topic name according to its id
        /// </summary>
        /// <param name="topic">Topic id</param>
        /// <returns>Topic name</returns>
        private string GetTopicName(TopicEnum topic)
        {
            //Getting key name in settings from topic enum
            var topicKey = Enum.GetName(typeof(TopicEnum), topic);

            // Getting topic name from settings
            return Config.GetValue<string>($"AzureServiceBus:TopicNames:{topicKey}");
        }
    }
}
