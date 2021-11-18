using Azure.Messaging.ServiceBus;

using SBShared.Models;

using System.Text;
using System.Text.Json;

namespace SBReceiver
{
    class Program
    {
        static string endpoint = "";
        static string topicName = "";
        static string subscriptionName = "";

        //Client that owns the connection to the service bus
        static ServiceBusClient client;
        // Processor that reads and processes messages from the subscription
        static ServiceBusProcessor processor;
        static async Task Main()
        {
            // Create the clients that we'll use for sending and processing messages.
            client = new ServiceBusClient(endpoint);

            // create a processor that we can use to process the messages
            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var jsonString = Encoding.UTF8.GetString(args.Message.Body);
            SBModel<ExampleModel>? message = JsonSerializer.Deserialize<SBModel<ExampleModel>>(jsonString);
            Console.WriteLine($"Received Id: {message?.Message.Id} - Value: {message?.Message.Value} - Publisher:{message?.Publisher} from subscription: {subscriptionName}");

            // complete the message. messages is deleted from the subscription. 
            await args.CompleteMessageAsync(args.Message);
        }

        // handle any errors when receiving messages
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}