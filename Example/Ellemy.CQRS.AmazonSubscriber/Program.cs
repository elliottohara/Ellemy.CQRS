using Ellemy.CQRS.Config;
using Ellemy.CQRS.Publishing.AmazonSns;

namespace Ellemy.CQRS.AmazonSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = 
            Configure.With()
                .AmazonPublisher()
                .AwsAccessKeyId("AKIAIN2KJH4QJIUV7CGQ")
                .AwsSecretKey("18ypN0y7SGA+L0XDVMHm9lBVmQ2oF2bdm7CGIijA")
                .QueueName("Ellemy_CQRS_Example");

            config.CreatePublisher();

            var subscriber = new AmazonSqsSubscriber(config);
            subscriber.Start();


        }
    }
}
