using System.Web.Mvc;
using System.Web.Routing;
using Ellemy.CQRS.Config;
using Ellemy.CQRS.Example.Commands;
using Ellemy.CQRS.Example.Query;
using Ellemy.CQRS.Example.Web.Infrastructure;
using Ellemy.CQRS.Implementations.StructureMap;
using Ellemy.CQRS.Publishing.NServiceBus;
using StructureMap;
using NServiceBus;
using Configure = Ellemy.CQRS.Config.Configure;

namespace Ellemy.CQRS.Example.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Message", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ObjectFactory.Configure(c =>
                                        {
                                            c.For(typeof (IRepository<>)).Use(typeof(InMemoryCacheRepository<>));
                                        });
            
            NServiceBus.Configure.WithWeb() //for web apps this should be WithWeb()
                .StructureMapBuilder()
                .BinarySerializer()
                .MsmqTransport()
                .MsmqSubscriptionStorage()
                .UnicastBus()
                .LoadMessageHandlers()
                .CreateBus()
                .Start();
           
            Configure.With()
                .StructureMapBuilder(ObjectFactory.Container)
                .CommandExecutorsAreInAssemblyContainingType<CreateMessage>()
                .HandlersAreInAssemblyContainingType<MessageReadModel>()
                .NServiceBusPublisher(ObjectFactory.Container.GetInstance<IBus>());


        }
    }
}