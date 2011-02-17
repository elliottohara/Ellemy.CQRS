using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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

        public int MessagesProcessed { get; private set; }
        public void Start()
        {
            ThreadPool.QueueUserWorkItem(delegate { BeginWork(); });
        }
        private void BeginWork()
        {
            while(!_stopped)
            {
                HandleMessasges();
            }
        }
        private void HandleMessasges()
        {
            var response = _client.ReceiveMessage(new ReceiveMessageRequest { QueueUrl = _config.SqsQueueUrl });
            if (!response.IsSetReceiveMessageResult())
            {
                return;
            }
            var messageResult = response.ReceiveMessageResult;
            foreach (var message in messageResult.Message)
            {
                var serializer = _config.EllemyConfiguration.Serializer;
                var messageBody = (Dictionary<String,Object>)serializer.DeserializeObject(message.Body);
                var eventType = GetEventType(messageBody);
                var handlerInterface = GetHandlerInterface(eventType);
                
                var @event = serializer.Deserialize((string)messageBody["Message"], eventType);
                foreach (var handler in _config.EllemyConfiguration.ObjectBuilder.BuildAll(handlerInterface))
                {
                    var handlerMethod = handler.GetType().GetMethod("Handle");
                    handlerMethod.Invoke(handler, new [] { @event });
                }
                Delete(message);
                MessagesProcessed++;
            }
        }

        private static Type GetHandlerInterface(Type eventType)
        {
            return typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        }

        private Type GetEventType(Dictionary<string, object> thing)
        {
            Type eventType = null;
            foreach (var eventAssembly in _config.EventAssemblies)
            {
                var t = eventAssembly.GetType((string)thing["Subject"]);
                if (t == null) continue;
                eventType = t;
                break;
            }
            if (eventType == null)
                throw new ConfigurationException(
                    String.Format(
                        "Could not load type {0}, please make sure you call AmazonConfig.EventsAreInAssemblyContainingType<TEvent>() during bootstrap.",
                        thing["Subject"]));
            return eventType;
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