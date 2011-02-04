using System;
using Ellemy.CQRS.Event;
using NServiceBus;
using StructureMap;

namespace Ellemy.CQRS.NServiceBusSubscriber
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server,IWantCustomInitialization
    {
       public void Init()
       {
           ObjectFactory.Initialize(r => r.Scan(scan =>
                                                    {
                                                        scan.AssemblyContainingType<EndpointConfig>();
                                                        scan.AddAllTypesOf(typeof (IDomainEventHandler<>));
                                                    }));

       

        Configure.With().StructureMapBuilder().BinarySerializer().UnicastBus().LoadMessageHandlers();
       }
    }
}