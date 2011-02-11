using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
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
}