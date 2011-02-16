using Ellemy.CQRS.Command;
using Ellemy.CQRS.Config;
using Ellemy.CQRS.Event;
using StructureMap;

namespace Ellemy.CQRS.Implementations.StructureMap
{
    public static class ConfigurationExtensions
    {
        private static IContainer _container;
        public static Configuration StructureMapBuilder(this Configuration config)
        {
            return config.StructureMapBuilder(ObjectFactory.Container);
        }
        public static Configuration StructureMapBuilder(this Configuration config, IContainer container)
        {
            _container = container;
            var builder = new StructureMapBuilder(_container);
            return config
                .CommandExecutorFactoryOf(builder)
                .HandlerFactoryOf(builder)
                .WithObjectBuilder(builder);
        }
        public static Configuration HandlersAreInAssemblyContainingType<THandler>(this Configuration config)
        {
            _container.Configure(c=> c.Scan(scanner=>
                                                {
                                                    scanner.AssemblyContainingType<THandler>();
                                                    scanner.AddAllTypesOf(typeof(IDomainEventHandler<>));
                                                }));
            return config;
        }
        public static Configuration CommandExecutorsAreInAssemblyContainingType<TCommandExecutor>(this Configuration config)
        {
            _container.Configure(c => c.Scan(scanner =>
            {
                scanner.AssemblyContainingType<TCommandExecutor>();
                scanner.ConnectImplementationsToTypesClosing(
                    typeof(ICommandHandler<>));
            }));
            return config;
        }
    }
}