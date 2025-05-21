using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Configuration;
using PatientRecovery.Shared.Messaging;
using System.Text.Json;

namespace PatientRecoverySystem.DoctorService.Messaging
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                UserName = configuration["RabbitMQ:Username"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = "patient_recovery";

            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic);
        }

        public void PublishMessage(string routingKey, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }

        public void PublishMessage<T>(string exchange, string routingKey, T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }

        public void BindQueue(string exchange, string queue, string routingKey)
        {
            _channel.QueueDeclare(queue, true, false, false, null);
            _channel.QueueBind(queue, exchange, routingKey);
        }

        public void SubscribeToQueue<T>(string queueName, Func<T, Task> handler)
        {
            _channel.QueueDeclare(queueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<T>(message);
                if (data != null)
                {
                    await handler(data);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: queueName,
                                autoAck: false,
                                consumer: consumer);
        }

        public void PublishDoctorCreated(string message)
        {
            PublishMessage("doctor.created", message);
        }

        public void PublishDoctorUpdated(string message)
        {
            PublishMessage("doctor.updated", message);
        }

        public void PublishConsultationScheduled(string message)
        {
            PublishMessage("consultation.scheduled", message);
        }

        public void PublishConsultationStatusChanged(string message)
        {
            PublishMessage("consultation.status.changed", message);
        }

        public void PublishVitalSignsAlert(string message)
        {
            PublishMessage("vitalsigns.alert", message);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}