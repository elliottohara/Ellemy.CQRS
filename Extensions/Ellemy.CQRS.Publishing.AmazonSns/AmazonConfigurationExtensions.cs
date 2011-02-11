using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Publishing.AmazonSns
{
    public static class AmazonConfigurationExtensions
    {
        public static AmazonPublisherConfig AmazonPublisher(this Configuration configuration)
        {
            return new AmazonPublisherConfig(configuration);
        }
    }
}