using System;
using NServiceBus;

namespace Ellemy.CQRS.NServiceBusSubscriber
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server,IWantCustomInitialization
    {
       public void Init()
       {
           Configure.With().StructureMapBuilder().XmlSerializer().UnicastBus();
       }
    }
}