using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Ellemy.CQRS.Command;
using StructureMap;
using AutoMapper;
using Ellemy.CQRS.Query;

namespace Ellemy.Mvc
{
    public static class ControllerExtensions
    {
        private static IContainer _container;

        public static IContainer Container
        {
            get { return _container ?? ObjectFactory.Container; }
            set { _container = value; }
        }

        private static IMappingEngine Mapper { get { return Container.GetInstance<IMappingEngine>(); } }
        public static ActionResult Command<TCommand>(this Controller controller, TCommand command,
                                                     Func<ActionResult> success,
                                                     Func<ActionResult> fail = null,
                                                     Func<ActionResult> ajaxSuccess = null,
                                                     Func<ActionResult> ajaxFail = null)
            where TCommand : ICommand

        {
            if (fail == null)
                fail = View(controller);
            if (!controller.ModelState.IsValid)
            {
                return Contextual(controller, fail, ajaxFail);
            }
            CommandDispatcher.Dispatch(command);
            return Contextual(controller, success, ajaxSuccess);
        }

        private static Func<ActionResult> View(Controller c)
        {
            var method = typeof (Controller)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .First(m =>
                       m.Name == "View" &&
                       m.GetParameters().Count() == 0
                );
            return () => (ActionResult) method.Invoke(c, null);
        }
        private static Func<ActionResult> View(Controller c,string viewName,object viewModel)
        {
            var method = typeof(Controller)
              .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
              .First(m =>
                     m.Name == "View" &&
                     m.GetParameters().Count() == 2 &&
                     m.GetParameters()[0].ParameterType ==typeof(string) &&
                     m.GetParameters()[1].ParameterType == typeof(object)
              );
            return () => (ActionResult)method.Invoke(c, new[]{viewName,viewModel});
        }
        private static Func<ActionResult> View(Controller c, object viewModel)
        {
            var method = typeof(Controller)
              .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
              .First(m =>
                     m.Name == "View" &&
                     m.GetParameters().Count() == 1 &&
                     m.GetParameters()[0].ParameterType == typeof(object)
              );
            return () => (ActionResult)method.Invoke(c, new[] { viewModel });
        }
        private static ActionResult Contextual(Controller controller, Func<ActionResult> standard,
                                               Func<ActionResult> ajax)
        {
            if (ajax == null) ajax = standard;
            return controller.Request.IsAjaxRequest() ? ajax() : standard();
        }

        public static ActionResult Query<TQuery>(this Controller controller, Action<TQuery> buildUpQuery = null,string viewName = null) where TQuery:IQuery
        {
            var query = Container.GetInstance<TQuery>();
            if (buildUpQuery != null) buildUpQuery(query);
            var model = query.Results();
            var standard = viewName == null ? View(controller, model) : View(controller, viewName, model);
            Func<ActionResult> ajax = () => new JsonResult {Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
            return Contextual(controller, standard, ajax);
        }
        public static ActionResult Query<TQuery,TViewModel>(this Controller controller, Action<TQuery> buildUpQuery = null, string viewName = null) where TQuery : IQuery
        {
            var query = Container.GetInstance<TQuery>();
            if (buildUpQuery != null) buildUpQuery(query);
            var unMappedModel = query.Results();
            var model = Mapper.Map(unMappedModel, unMappedModel.GetType(), typeof (TViewModel));
            var standard = viewName == null ? View(controller, model) : View(controller, viewName, model);
            Func<ActionResult> ajax = () => new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return Contextual(controller, standard, ajax);
        }
    }
}