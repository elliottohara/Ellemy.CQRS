using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Ellemy.CQRS.Command;

namespace Ellemy.Mvc
{
    public class MvcController : Controller
    {
        public ActionResult Command<TCommand>(TCommand command, Func<ActionResult> success,
                                              Func<ActionResult> fail = null)
            where TCommand : ICommand
        {
            if (fail == null)
                fail = () => View();

            if (!ModelState.IsValid)
                return fail();

            CommandDispatcher.Dispatch(command);

            return success();
        }
    }

   
}