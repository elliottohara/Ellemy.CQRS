using System;
using System.Web.Script.Serialization;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Ellemy.CQRS.Config;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public static class AmazonConfigurationExtensions
    {
        public static AmazonPublisherConfig AmazonPublisher(this Configuration configuration)
        {
            return new AmazonPublisherConfig(configuration);
        }
    }

    public class AmazonPublisherConfig
    {
        private readonly Configuration _configuration;

        public AmazonPublisherConfig(Configuration configuration)
        {
            _configuration = configuration;
        }


        internal string AccessKeyId { get; private set; }
        internal string SecretKey { get; private set; }
        internal string TopicAccessResourceName { get; private set; }

        public AmazonPublisherConfig AwsAccessKeyId(string accessKeyId)
        {
            AccessKeyId = accessKeyId;
            return this;
        }

        public AmazonPublisherConfig AwsSecretKey(string awsSecretKey)
        {
            SecretKey = awsSecretKey;
            return this;
        }

        public AmazonPublisherConfig TopicArn(string value)
        {
            TopicAccessResourceName = value;
            return this;
        }
        public Configuration CreatePublisher()
        {
            _configuration.PublishEventsWith(new AmazonPublisher(this));
            return _configuration;
        }
    }

    public class AmazonPublisher : IEventPublisher
    {
        private AmazonSimpleNotificationServiceClient _client;
        private string _topicArn;

        internal AmazonPublisher(AmazonPublisherConfig config)
        {
            _client = new AmazonSimpleNotificationServiceClient(config.AccessKeyId, config.SecretKey);
            _topicArn = config.TopicAccessResourceName;
        }
       public void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
            var serializer = new JavaScriptSerializer();
            string payload = serializer.Serialize(@event);
            
            var request = new PublishRequest
                              {
                                  Message = payload,
                                  Subject = @event.GetType().Name,
                                  TopicArn = _topicArn
                              };
            _client.Publish(request);
        }
    }
}