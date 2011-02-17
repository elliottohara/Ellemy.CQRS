using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web.Script.Serialization;
using Amazon.SQS;
using Amazon.SQS.Model;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class AmazonSqsSubscriber
    {
        private readonly AmazonConfig _config;
        private readonly AmazonSQS _client;
      

        public AmazonSqsSubscriber(AmazonConfig config)
        {
            _config = config;
            _client = Amazon.AWSClientFactory.CreateAmazonSQSClient(config.AccessKeyId, config.SecretKey);
            SetupQueue();
        }

        private void SetupQueue()
        {
            new QueueManager(_client, _config).SetupQueue();
            new TopicSubscriber(_client,_config).Subscribe();
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