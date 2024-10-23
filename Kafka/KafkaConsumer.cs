using ClientApi.Data;
using ClientApi.Models;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Kafka;

public class KafkaConsumer : IHostedService
{
    private readonly IConfiguration _config;

    public KafkaConsumer()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddIniFile("kafkaclient.properties", false)
            .Build();
        _config["group.id"] = "client-api";
        _config["auto.offset.reset"] = "earliest";
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ConsumeAsync(cancellationToken));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {   
        return Task.CompletedTask;
    }

    private async Task ConsumeBanNumberMessageAsync(Message<string, string> banNumberMessage)
    {
        using (var db = new ClientApiContext())
        {
            SavedNumber numberToBan = await db.SavedNumbers.FirstAsync(n => n.Value == int.Parse(banNumberMessage.Value));
            numberToBan.IsBanned = true;
            await db.SaveChangesAsync();
        }
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        string topic = "numbers"; 
        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        // creates a new consumer instance
        using (var consumer = new ConsumerBuilder<string, string>(_config.AsEnumerable()).Build()) {
            consumer.Subscribe(topic);
            try
            {
                while (!cancellationToken.IsCancellationRequested) {
                    ConsumeResult<string, string> consumeResult = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consuming event from topic {topic}: key = {consumeResult.Message.Key,-10} value = {consumeResult.Message.Value}");
                    switch (consumeResult.Message.Key)
                    {
                        case "banNumber":
                            await ConsumeBanNumberMessageAsync(consumeResult.Message);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ctrl-C was pressed
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}