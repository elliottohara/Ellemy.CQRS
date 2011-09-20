using System;
using System.Linq;
using Ellemy.CQRS.Command;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Ellemy.CQRS.Implementations.StructureMap
{
    public class CloseCommandsConvention:IRegistrationConvention    
    {
        public void Process(Type type, Registry registry)
        {
            if(!typeof(ICommand).IsAssignableFrom(type))
            {
                return;
            }
            
            var commandType = type.GetInterfaces().FirstOrDefault(iface => iface == typeof (ICommand));
            //there isn't one, don't know what to do, they'll have to solve the problem some other way.
            if(commandType == null) return;
                
            //find the ICommandHander<T> or any deriving class
            var openGenericCommandHandlerType = typeof (ICommandHandler<>);
            var closedGenericCommandHandlerType = openGenericCommandHandlerType.MakeGenericType(commandType);

            registry.For(openGenericCommandHandlerType.MakeGenericType(commandType)).Use(
                c => c.GetInstance(closedGenericCommandHandlerType));

        }
    }
}