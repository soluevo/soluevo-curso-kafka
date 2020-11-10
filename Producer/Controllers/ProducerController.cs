using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers
{
    //http://localhost:porta/producer
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {

        [HttpGet]
        public ActionResult<string> Home()
        {
            return Ok("You're ready to send messages to kafka");
        }

        [HttpGet("send/{value}")]
        public async Task<ActionResult<string>> Send(string value)
        {
            using (var producer = new ProducerBuilder<Null, string>(GetProducerConfig()).Build())
            {
                await producer.ProduceAsync("anima", new Message<Null, string>()
                {
                    Value = value
                });
            }

            return Ok($"Message Sent to Kafka --> {value}");
        }

        [HttpGet("send-ordered/{key}/{value}")]
        public async Task<ActionResult<string>> SendOrdered(string key, string value)
        {
            using (var producer = new ProducerBuilder<string, string>(GetProducerConfig()).Build())
            {
                await producer.ProduceAsync("anima-p2", new Message<string, string>()
                {
                    Key =  key,
                    Value = value
                });
            }

            return Ok($"Message Sent to Kafka with Order (key/value) --> {key}/{value}");
        }

        private ProducerConfig GetProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                ClientId = "producer-anima",
                Acks = Acks.All
            };
        }
    }
}