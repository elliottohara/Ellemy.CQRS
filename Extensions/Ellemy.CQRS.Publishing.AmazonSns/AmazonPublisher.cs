using System;
using System.Web.Script.Serialization;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class AmazonPublisher : IEventPublisher
    {
        private readonly AmazonSimpleNotificationServiceClient _client;
        private readonly string _topicArn;

        internal AmazonPublisher(AmazonPublisherConfig config)
        {
            _client = new AmazonSimpleNotificationServiceClient(config.AccessKeyId, config.SecretKey);
            _topicArn = config.TopicAccessResourceName;
        }
       public void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
            var serializer = new JavaScriptSerializer();
            var payload = serializer.Serialize(@event);
            
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