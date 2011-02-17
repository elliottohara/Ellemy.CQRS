using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Publishing.Amazon
{
    public static class AmazonConfigurationExtensions
    {
        public static AmazonConfig AmazonPublisher(this Configuration configuration)
        {
            return new AmazonConfig(configuration);
        }
    }
}