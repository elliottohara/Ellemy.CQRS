using System;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Ellemy.CQRS.Config;
using Ellemy.CQRS.Event;
using Ellemy.CQRS.Implementations.StructureMap;
using Ellemy.CQRS.Publishing.AmazonSns;
using NUnit.Framework;

namespace AmazonTests
{
    [TestFixture]
    public class when_using_the_amazon_subscriber
    {
        private AmazonConfig _config;
        private AmazonSQS _amazonClient;
        private const string awsKey = "AKIAIN2KJH4QJIUV7CGQ";
        private const string secret = "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";
        private const string topicArn = "arn:aws:sns:us-east-1:451419498740:EventMessage";
        private string tempQueueName;
        private AmazonPublisher _publisher;
        private AmazonSimpleNotificationService _snsClient;

        [SetUp]
        public void Arrange()
        {

            tempQueueName = "Test_QUEUE_1";
            _config = Configure.With()
                .StructureMapBuilder()
                .HandlersAreInAssemblyContainingType<TestEvent>()
                .AmazonPublisher();
            
            _config
                .AwsAccessKeyId(awsKey)
                .AwsSecretKey(secret)
                .TopicArn(topicArn)
                .QueueName(tempQueueName)
                .EventsAreInAssemblyContainingType<TestEvent>();

            _publisher = new AmazonPublisher(_config);

            _amazonClient = AWSClientFactory.CreateAmazonSQSClient(awsKey, secret);
            _snsClient = AWSClientFactory.CreateAmazonSNSClient(awsKey, secret);

        }
        [TearDown]
        //[Test]
        public void CleanUp()
        {
            var listQueuesRequest = new ListQueuesRequest().WithQueueNamePrefix(tempQueueName);
            var result = _amazonClient.ListQueues(listQueuesRequest);
            foreach (var queueUrl in result.ListQueuesResult.QueueUrl)
            {
                var subscriptions = _snsClient.ListSubscriptions(new ListSubscriptionsRequest());
                foreach (var subscription in subscriptions.ListSubscriptionsResult.Subscriptions)
                {
                    // This is really bad, it's killing all subscriptions. We need to figure out a better
                    // way to do this (or just create our own topic for the test.
                    if (subscription.Protocol != "sqs") continue;
                    _snsClient.Unsubscribe(new UnsubscribeRequest().WithSubscriptionArn(subscription.SubscriptionArn));
                }
                
                //_amazonClient.DeleteQueue(new DeleteQueueRequest().WithQueueUrl(queueUrl));
            }


        }
        [Test]
        [Ignore("Only use this to clean up queues")]
        public void delete_the_queue()
        {
            var listQueuesRequest = new ListQueuesRequest().WithQueueNamePrefix(tempQueueName);
            var result = _amazonClient.ListQueues(listQueuesRequest);
            foreach (var queueUrl in result.ListQueuesResult.QueueUrl)
            {
                _amazonClient.DeleteQueue(new DeleteQueueRequest().WithQueueUrl(queueUrl));
            }
        }
        [Test]
        public void it_will_create_the_queue()
        {
            //lets just leave the logic in the ctor, it'll be singleton for consumers
            

            var listQueuesResponse = _amazonClient.ListQueues(new ListQueuesRequest());
            var foundExpectedQueue = false;
            foreach (var queueUrl in listQueuesResponse.ListQueuesResult.QueueUrl)
            {
                if (queueUrl.Contains(tempQueueName))
                    foundExpectedQueue = true;
            }
            Assert.IsTrue(foundExpectedQueue,"Queue was not found");

        }
        
        [Test]
        public void the_message_is_proccessed()
        {
            var subscriber = new AmazonSqsSubscriber(_config);

            var @event = new TestEvent {SomeGuid = Guid.NewGuid(), SomeInt = 1, SomeString = "Some String"};
            _publisher.Publish(@event);
           
            subscriber.Start();
           //Ugly, but I wanna make sure the message arrives
            while(subscriber.MessagesProcessed == 0){}
            
            subscriber.Stop();
            Assert.AreEqual(@event.SomeGuid, ConsoleWriter.LastSomeGuid);

        }
        
    }
    public class TestEvent : IDomainEvent
    {
        public Guid SomeGuid { get; set; }
        public String SomeString { get; set; }
        public int SomeInt { get; set; }
    }
    public class ConsoleWriter : IDomainEventHandler<TestEvent>{
        public static Guid LastSomeGuid { get; private set; }
        public void Handle(TestEvent @event)
        {
            Console.WriteLine("SomeGuid: \t{0}",@event.SomeGuid);
            Console.WriteLine("SomeInt: \t {0}",@event.SomeInt);
            Console.WriteLine("SomeString: \t{0}",@event.SomeString);
            LastSomeGuid = @event.SomeGuid;

        }
    }
}