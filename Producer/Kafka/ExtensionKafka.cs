using System.Net;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer.Domain;

namespace Producer.Kafka
{
    public static class ExtensionKafka
    {
        public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(c => AddProducerPayment(configuration));
            services.AddSingleton(c => AddProducerRollback(configuration));
        }

        private static IProducer<string, RollbackPayment> AddProducerRollback(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:Servers"],
                ClientId = configuration["Kafka:ClientId"] + "-" + Dns.GetHostName(),
                Acks = Acks.All,
                SecurityProtocol =  SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = configuration["Kafka:Username"],
                SaslPassword = configuration["Kafka:Password"]
            };

            return new ProducerBuilder<string, RollbackPayment>(config)
                .SetValueSerializer(new AnimaJsonSerializer<RollbackPayment>())
                .Build();
        }

        private static IProducer<string, Payment> AddProducerPayment(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:Servers"],
                ClientId = configuration["Kafka:ClientId"] + "-" + Dns.GetHostName(),
                Acks = Acks.All,
                SecurityProtocol =  SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = configuration["Kafka:Username"],
                SaslPassword = configuration["Kafka:Password"]
            };

            var schemaConfig = new SchemaRegistryConfig
            {
                Url = configuration["SchemaRegistry:Url"],
                BasicAuthUserInfo = configuration["SchemaRegistry:UsernamePassword"]
            };

            var schemaRegistry = new CachedSchemaRegistryClient(schemaConfig);
            
            return new ProducerBuilder<string, Payment>(config)
                //.SetValueSerializer(new AnimaJsonSerializer<Payment>())
                .SetValueSerializer(new AvroSerializer<Payment>(schemaRegistry).AsSyncOverAsync())
                .Build();
        }
    }
}