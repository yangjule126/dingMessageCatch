using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dingMessageCatch
{
    class Kafka
    {
        /// <summary>
        /// KAFKA produce
        /// </summary>
        public static void Produce(String msg)
        {
            var conf = new ProducerConfig { BootstrapServers = "10.10.1.177:9092" };
            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");
            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                p.Produce("dingding", new Message<Null, string> { Value = msg }, handler);
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
