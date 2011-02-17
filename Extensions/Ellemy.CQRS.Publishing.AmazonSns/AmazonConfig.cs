using System.Collections.Generic;
using System.Reflection;
using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Publishing.Amazon
{
    public class AmazonConfig
    {
        internal readonly Configuration EllemyConfiguration;

        public AmazonConfig(Configuration ellemyConfiguration)
        {
            EllemyConfiguration = ellemyConfiguration;
        }


        internal string AccessKeyId { get; private set; }
        internal string SecretKey { get; private set; }
        internal string TopicAccessResourceName { get; private set; }
        internal string SqsQueueName { get; private set; }
        internal ICollection<Assembly> EventAssemblies { get; private set; }
        
        public string SqsQueueUrl { get; internal set; }
        
        
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
            EllemyConfiguration.PublishEventsWith(new AmazonPublisher(this));
            return EllemyConfiguration;
        }
        public AmazonConfig EventsAreInAssemblyContainingType<TEvent>()
        {
            if (EventAssemblies == null)
                EventAssemblies = new List<Assembly>();
            EventAssemblies.Add(typeof(TEvent).Assembly);
            return this;
        }
    }
}