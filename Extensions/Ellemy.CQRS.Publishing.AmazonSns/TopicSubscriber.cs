using System;
using System.Linq;
using Amazon;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Attribute = Amazon.SQS.Model.Attribute;

namespace Ellemy.CQRS.Publishing.Amazon
{
    public class TopicSubscriber
    {
        private readonly AmazonSQS _client;
        private readonly AmazonConfig _config;

        public TopicSubscriber(AmazonSQS client, AmazonConfig config)
        {
            _client = client;
            _config = config;
        }
        public void Subscribe()
        {
            SetPermissions();
            var getArnRequest = new GetQueueAttributesRequest().WithQueueUrl(_config.SqsQueueUrl).WithAttributeName("QueueArn");
            var clientArn = _client.GetQueueAttributes(getArnRequest).GetQueueAttributesResult.Attribute[0].Value;

            var sns = AWSClientFactory.CreateAmazonSNSClient(_config.AccessKeyId,
                                                                    _config.SecretKey);

            var subscriptionRequest = new SubscribeRequest()
                .WithEndpoint(clientArn)
                .WithProtocol("sqs")
                .WithTopicArn(_config.TopicAccessResourceName);

            sns.Subscribe(subscriptionRequest);
        }

        private void SetPermissions()
        {
            var setQueueAttributeRequest = new SetQueueAttributesRequest()
               .WithQueueUrl(_config.SqsQueueUrl)
               .WithAttribute(new Attribute { Name = "Policy", Value = AllowSnsAttribute() });
            _client.SetQueueAttributes(setQueueAttributeRequest);
        }
        private string AllowSnsAttribute()
        {
            var attribute = _client.GetQueueAttributes(
             new GetQueueAttributesRequest().WithQueueUrl(_config.SqsQueueUrl).WithAttributeName("QueueArn"));
            var queueArn = attribute.GetQueueAttributesResult.Attribute.First().Value;
            const string format = @"{{""Version"": ""2008-10-17"",""Statement"": [{{""Resource"": ""{0}"", ""Effect"": ""Allow"", ""Sid"": ""1"", ""Action"": ""sqs:*"", ""Condition"": {{""StringEquals"": {{""aws:SourceArn"": ""{1}""}}}}, ""Principal"": {{""AWS"": ""*""}}}}]}}";
            return String.Format(format, queueArn, _config.TopicAccessResourceName);
        }
        
    }
}