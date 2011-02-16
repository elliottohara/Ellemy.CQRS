using System;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class AmazonSqsSubscriber
    {
        private readonly AmazonConfig _config;
        private AmazonSQSClient _client;

        public AmazonSqsSubscriber(AmazonConfig config)
        {
            _config = config;
            SetupQueue();
        }

        private void SetupQueue()
        {
            if (!String.IsNullOrEmpty(_config.SqsQueueUrl)) return;
            _client = new AmazonSQSClient(_config.AccessKeyId, _config.SecretKey);
            var listQueuesResponse = _client.ListQueues(new ListQueuesRequest {QueueNamePrefix = _config.SqsQueueName});
            string queueUrl;
            if(listQueuesResponse.ListQueuesResult.QueueUrl.Count == 0)
            {
                var response = _client.CreateQueue(new CreateQueueRequest{QueueName = _config.SqsQueueName});
                queueUrl = response.CreateQueueResult.QueueUrl;
            }
            else
            {
                queueUrl = listQueuesResponse.ListQueuesResult.QueueUrl[0];
            }
            _config.SqsQueueUrl = queueUrl;
        }
        public void Run()
        {
            while(true)
            {
               
                var response = _client.ReceiveMessage(new ReceiveMessageRequest {QueueUrl = _config.SqsQueueUrl});
                if(!response.IsSetReceiveMessageResult())
                {
                    continue;
                }
                var messageResult = response.ReceiveMessageResult;
                foreach (var message in messageResult.Message)
                {
                    Console.WriteLine(message.Body);
                }
            }
        }
    }
}