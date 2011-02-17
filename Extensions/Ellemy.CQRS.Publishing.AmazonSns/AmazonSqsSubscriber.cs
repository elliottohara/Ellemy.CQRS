using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Ellemy.CQRS.Event;
using Attribute = Amazon.SQS.Model.Attribute;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class AmazonSqsSubscriber
    {
        private readonly AmazonConfig _config;
        private AmazonSQS _client;
      

        public AmazonSqsSubscriber(AmazonConfig config)
        {
            _config = config;
            _client = Amazon.AWSClientFactory.CreateAmazonSQSClient(config.AccessKeyId, config.SecretKey);
            SetupQueue();
        }

        private void SetupQueue()
        {
            new QueueManager(_client, _config).SetupQueue();
            SubscribeToTopic();
        }

        private void SubscribeToTopic()
        {
            SetPermissions();
            var getArnRequest = new GetQueueAttributesRequest().WithQueueUrl(_config.SqsQueueUrl).WithAttributeName("QueueArn");
            var clientArn = _client.GetQueueAttributes(getArnRequest).GetQueueAttributesResult.Attribute[0].Value;

            var sns = Amazon.AWSClientFactory.CreateAmazonSNSClient(_config.AccessKeyId,
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
        private bool _stopped;
        public void Stop()
        {
            Console.WriteLine("Stopping....");
            _stopped = true;
        }
        public void Start()
        {
            ThreadPool.QueueUserWorkItem(delegate { BeginWork(); });
        }
        private void BeginWork()
        {
            while(!_stopped)
            {
                ThreadPool.QueueUserWorkItem(delegate { DoWork(); });
            }
        }
        private void DoWork()
        {
            var response = _client.ReceiveMessage(new ReceiveMessageRequest { QueueUrl = _config.SqsQueueUrl });
            if (!response.IsSetReceiveMessageResult())
            {
                return;
            }
            var messageResult = response.ReceiveMessageResult;
            foreach (var message in messageResult.Message)
            {
                var serializer = new JavaScriptSerializer();
                var thing = (Dictionary<String,Object>)serializer.DeserializeObject(message.Body);
                Type eventType = null;
                foreach (var eventAssembly in _config.EventAssemblies)
                {
                    var t = eventAssembly.GetType((string)thing["Subject"]);
                    if(t!=null)
                    {
                        eventType = t;
                        break;
                    }
                }
                if (eventType == null)
                    throw new ConfigurationException(
                        String.Format(
                            "Could not load type {0}, please make sure you call AmazonConfig.EventsAreInAssemblyContainingType<TEvent>() during bootstrap.",
                            thing["Subject"]));

                var @event = serializer.Deserialize((string)thing["Message"], eventType);
                var handlerInterface = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                foreach (var handler in _config.EllemyConfiguration.ObjectBuilder.BuildAll(handlerInterface))
                {
                    var handlerMethod = handler.GetType().GetMethod("Handle");
                    handlerMethod.Invoke(handler, new [] { @event });
                }
                Delete(message);
            }
        }

        private void Delete(Message message)
        {
            var deleteRequest = new DeleteMessageRequest()
                .WithReceiptHandle(message.ReceiptHandle)
                .WithQueueUrl(_config.SqsQueueUrl);
            _client.DeleteMessage(deleteRequest);
        }
    }
}