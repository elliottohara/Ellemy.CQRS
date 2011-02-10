using System.Web.Script.Serialization;
using Amazon.SimpleNotificationService.Model;
using Ellemy.CQRS.Config;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class IAmazonSnsSettings
    {
        public string TopicArn { get { return "arn:aws:sns:us-east-1:451419498740:EventMessage"; } }
        public string AWSAccessKeyId { get { return "AKIAIN2KJH4QJIUV7CGQ"; } }
        public string AmazonUrl { get { return "https://sns.us-east-1.amazonaws.com/"; } }
        public string Secret { get { return "YeahRIGHT"; } }
    }
    public class AmazonPublisher : IEventPublisher 
    {
        private readonly IAmazonSnsSettings _settings;
        
        public AmazonPublisher(IAmazonSnsSettings settings)
        {
            _settings = settings;
        }

        public void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
              //WebRequest.Create()
            var serializer = new JavaScriptSerializer();
            var payload = serializer.Serialize(@event);
            var x = new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(_settings.AWSAccessKeyId,_settings.Secret);
            x.Publish(new PublishRequest
                          {Message = payload, Subject = @event.GetType().Name, TopicArn = _settings.TopicArn});
        }
    }
    public static class ConfigurationExtensions
    {
        public static Configuration AmazonSns(this Configuration config)
        {
            config.PublishEventsWith(new AmazonPublisher(new IAmazonSnsSettings()));
            return config;
        }
    }
}