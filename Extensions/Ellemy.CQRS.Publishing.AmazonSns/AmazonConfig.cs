using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public class AmazonConfig
    {
        private readonly Configuration _configuration;

        public AmazonConfig(Configuration configuration)
        {
            _configuration = configuration;
        }


        internal string AccessKeyId { get; private set; }
        internal string SecretKey { get; private set; }
        internal string TopicAccessResourceName { get; private set; }
        internal string SqsQueueName { get; private set; }
        internal string SqsQueueUrl { get; set; }
        
        public AmazonConfig AwsAccessKeyId(string accessKeyId)
        {
            AccessKeyId = accessKeyId;
            return this;
        }

        public AmazonConfig AwsSecretKey(string awsSecretKey)
        {
            SecretKey = awsSecretKey;
            return this;
        }

        public AmazonConfig TopicArn(string value)
        {
            TopicAccessResourceName = value;
            return this;
        }
        public AmazonConfig QueueName(string value)
        {
            SqsQueueName = value;
            return this;
        }
        public Configuration CreatePublisher()
        {
            _configuration.PublishEventsWith(new AmazonPublisher(this));
            return _configuration;
        }
    }
}