using System;
using System.Net;
using Confluent.Kafka;
using Consumer.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer.Kafka;

namespace Consumer.Kafka
{
    public static class ExtensionKafka
    {
        public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(c => AddConsumerPayment(configuration));
            services.AddSingleton(c => AddConsumerRollback(configuration));
        }

        private static IConsumer<string, Payment> AddConsumerPayment(IConfiguration configuration)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:Servers"],
                GroupId = configuration["Kafka:ConsumerGroupId"],
                ClientId = configuration["Kafka:ClientId"] + "-" + Dns.GetHostName(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = Convert.ToBoolean(configuration["Kafka:EnableAutoCommit"]),
                EnableAutoOffsetStore = Convert.ToBoolean(configuration["Kafka:EnableAutoOffsetStore"])
            };
            
            return new ConsumerBuilder<string, Payment>(config)
                .SetValueDeserializer(new AnimaJsonSerializer<Payment>())
                .Build();
        }
        
        private static IConsumer<string, RollbackPayment> AddConsumerRollback(IConfiguration configuration)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:Servers"],
                GroupId = configuration["Kafka:ConsumerGroupId"],
                ClientId = configuration["Kafka:ClientId"] + "-" + Dns.GetHostName(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = Convert.ToBoolean(configuration["Kafka:EnableAutoCommit"])
            };
            
            return new ConsumerBuilder<string, RollbackPayment>(config)
                .SetValueDeserializer(new AnimaJsonSerializer<RollbackPayment>())
                .Build();
        }
    }
}