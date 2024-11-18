using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace Producer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Cấu hình MQTT client
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) // RabbitMQ MQTT broker
                .WithClientId("PublisherClient")
                .Build();

            // Kết nối đến MQTT broker
            await mqttClient.ConnectAsync(options);
            Console.WriteLine("Publisher connected to MQTT broker!");

            // Gửi tin nhắn (Publish)
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("sensor/temperature")
                .WithPayload("Temperature: 25°C")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine("Message published: Temperature: 25°C");

            // Ngắt kết nối
            await mqttClient.DisconnectAsync();
            Console.WriteLine("Publisher disconnected.");
        }
    }
}
