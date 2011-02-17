using System;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Ellemy.CQRS.Publishing.Amazon
{
    public class QueueManager
    {
        private readonly AmazonSQS _client;
        private readonly AmazonConfig _config;

        public QueueManager(AmazonSQS client,AmazonConfig config)
        {
            _client = client;
            _config = config;
        }
        public void SetupQueue()
        {
            _config.SqsQueueUrl = GetOrCreateQueueUrl();
        }
        private string GetOrCreateQueueUrl()
        {
            var listQueuesResponse = _client.ListQueues(new ListQueuesRequest { QueueNamePrefix = _config.SqsQueueName });
            var queueUrl = TheQueueUrlFrom(listQueuesResponse);
            return queueUrl;
        }

        private string TheQueueUrlFrom(ListQueuesResponse listQueuesResponse)
        {
            string queueUrl;
            if (listQueuesResponse.ListQueuesResult.QueueUrl.Count == 0)
            {
                var response = _client.CreateQueue(new CreateQueueRequest { QueueName = _config.SqsQueueName });
                queueUrl = response.CreateQueueResult.QueueUrl;
            }
            else
            {
                queueUrl = listQueuesResponse.ListQueuesResult.QueueUrl[0];
            }
            return queueUrl;
        }

        private bool TheQueueIsAlreadySet()
        {
            return !String.IsNullOrEmpty(_config.SqsQueueUrl);
        }
    }
}