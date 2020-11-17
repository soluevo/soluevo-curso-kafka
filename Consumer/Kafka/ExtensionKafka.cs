using System;
using System.Net;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consumer.Kafka
{
    public static class ExtensionKafka
    {
        public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(c => AddConsumerAnima(configuration));
        }

        private static IConsumer<Ignore, string> AddConsumerAnima(IConfiguration configuration)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:Servers"],
                GroupId = configuration["Kafka:ConsumerGroupId"],
                ClientId = configuration["Kafka:ClientId"] + "-" + Dns.GetHostName(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = Convert.ToBoolean(configuration["Kafka:EnableAutoCommit"])
            };
            
            return new ConsumerBuilder<Ignore, string>(config)
                .SetValueDeserializer(new AnimaJsonSerializer<string>())
                .Build();
        }
    }
}