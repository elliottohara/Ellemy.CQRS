using Ellemy.CQRS.Event;
using NServiceBus;
using NServiceBus.ObjectBuilder.StructureMap262;
using StructureMap;
using IContainer = NServiceBus.ObjectBuilder.Common.IContainer;

namespace Ellemy.CQRS.NServiceBusSubscriber
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
            ObjectFactory.Initialize(r =>
                                         {
                                             r.Scan(scan =>
                                                        {
                                                            scan.AssemblyContainingType<EndpointConfig>();
                                                            scan.AddAllTypesOf(typeof (IDomainEventHandler<>));
                                                        });
                                             r.For<IContainer>().Use
                                                 <StructureMapObjectBuilder>();
                                         }
                );


            Configure.With().StructureMapBuilder().BinarySerializer().UnicastBus().LoadMessageHandlers();
        }
    }
}