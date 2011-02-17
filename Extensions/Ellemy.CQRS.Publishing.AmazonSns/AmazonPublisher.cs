using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Publishing.Amazon
{
    public class AmazonPublisher : IEventPublisher
    {
        private readonly AmazonConfig _config;
        private readonly AmazonSimpleNotificationServiceClient _client;
        private readonly string _topicArn;

        internal AmazonPublisher(AmazonConfig config)
        {
            _config = config;
            _client = new AmazonSimpleNotificationServiceClient(config.AccessKeyId, config.SecretKey);
            
            _topicArn = config.TopicAccessResourceName;
        }
       public void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
       {
           var payload = _config.EllemyConfiguration.Serializer.Serialize(@event);
            
            var request = new PublishRequest
                              {
                                  Message = payload,
                                  Subject = @event.GetType().FullName,
                                  TopicArn = _topicArn
                              };
            _client.Publish(request);
           
        }
    }
}