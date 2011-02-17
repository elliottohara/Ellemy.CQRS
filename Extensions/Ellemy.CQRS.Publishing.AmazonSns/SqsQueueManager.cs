using System;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class SqsQueueManager
    {
        private readonly AmazonConfig _config;
        private AmazonSQS _client;

        public SqsQueueManager(AmazonConfig config)
        {
            _config = config;
        }

        public void SetupQueue()
        {
            if (TheQueueIsAlreadySet()) return;
            var queueUrl = GetOrCreateQueueUrl();
            _config.SqsQueueUrl = queueUrl;
            _config.TopicSubscriber.SubscribeToTopic();
        }



        private string GetOrCreateQueueUrl()
        {
            //TODO: we should probably not create this here...
            _client = Amazon.AWSClientFactory.CreateAmazonSQSClient(_config.AccessKeyId, _config.SecretKey);
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