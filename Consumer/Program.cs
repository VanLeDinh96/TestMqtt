using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    static async Task Main(string[] args)
    {
        // Cấu hình MQTT client
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883) // RabbitMQ MQTT broker
            .WithClientId("SubscriberClient")
            .Build();

        // Đăng ký sự kiện nhận tin nhắn
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            Console.WriteLine($"Received message: Topic = {topic}, Payload = {payload}");
            await Task.CompletedTask;
        };

        // Kết nối đến MQTT broker
        await mqttClient.ConnectAsync(options, CancellationToken.None);
        Console.WriteLine("Subscriber connected to MQTT broker!");

        // Đăng ký nhận tin nhắn (Subscribe)
        var topicFilter = new MqttTopicFilter
        {
            Topic = "sensor/temperature",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
        };

        await mqttClient.SubscribeAsync(new MqttClientSubscribeOptions
        {
            TopicFilters = { topicFilter }
        });

        Console.WriteLine("Subscribed to topic: sensor/temperature");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        // Ngắt kết nối
        await mqttClient.DisconnectAsync();
        Console.WriteLine("Subscriber disconnected.");
    }
}
