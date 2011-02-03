using Ellemy.CQRS.Config;
using NServiceBus;

namespace NServiceBusPublisher
{
    public static class ConfigurationExtensions
    {
        public static Configuration NServiceBusPublisher(this Configuration configuration,IBus bus)
        {
            configuration.PublishEventsWith(new NServiceBusPublisher(bus));
            return configuration;
        }  
    }
}