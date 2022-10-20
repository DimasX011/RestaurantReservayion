﻿using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class Consumer : IDisposable
    {
        private readonly string _queueName; 
        private readonly string _hostName; 

        private readonly IConnection _connection;
        private readonly IModel _channel;
        
        public Consumer(string queueName, string hostName)
        {
            _queueName = queueName;
            _hostName = "codfish.rmq.cloudamqp.com"; 
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                Port = 5672,
                UserName = "jklmfzyn",
                Password = "G5k0mG5Z4YA1p5KWQ9bF-LVitLs6Soqz",
                VirtualHost = "jklmfzyn"

            };
            _connection = factory.CreateConnection(); //создаем подключение
            _channel = _connection.CreateModel();
        }

        public void Receive (EventHandler<BasicDeliverEventArgs> receiveCallback)
        {
            _channel.ExchangeDeclare(exchange: "direct_exchange",
                type: "direct"); // объявляем обменник
            
            _channel.QueueDeclare(queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null); //объявляем очередь
            
            _channel.QueueBind(queue: _queueName,
                exchange: "direct_exchange",
                routingKey: _queueName); //биндим

            var consumer = new EventingBasicConsumer(_channel); // создаем consumer для канала
            consumer.Received += receiveCallback; // добавляем обработчик события приема сообщения

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer); //стартуем!
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}