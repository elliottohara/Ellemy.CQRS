using System;
using System.Web.Mvc;
using Ellemy.CQRS.Command;
using Ellemy.CQRS.Example.Commands;
using Ellemy.CQRS.Example.Query;
using Ellemy.CQRS.Example.Web.Infrastructure;

namespace Ellemy.CQRS.Example.Web.Controllers
{
    public class MessageController : Controller
    {
        private readonly IRepository<MessageReadModel> _repository;

        public MessageController():this(new InMemoryCacheRepository<MessageReadModel>()){}
        public MessageController(IRepository<MessageReadModel> repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View(_repository.GetAll());
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(string text)
        {
            CommandDispatcher.Dispatch(new CreateMessage(Guid.NewGuid(), text));
            return RedirectToAction("Index");
        }


    }
}
