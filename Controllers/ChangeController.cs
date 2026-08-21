using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Project.DbModels;

namespace Project.Controllers
{
    public class ChangeController : Controller
    {
        private readonly CartridgeDbContext _context;
        public ChangeController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: ChangeController
        public ActionResult Index()
        {
            var error = _context.Change.Where(x => x.CommentStatus == "Ошибка").ToList();
            var received = _context.Change.Where(x => x.CommentStatus == "Поступили").ToList();
            var completed = _context.Change.Where(x => x.CommentStatus == "Выполнено").ToList();

            ViewBag.ErrorCount = error.Count();
            ViewBag.ReceivedCount = received.Count();
            ViewBag.CompletedCount = completed.Count();

            ViewBag.Error = error;
            ViewBag.Received = received;
            ViewBag.Completed = completed;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Changes changes)
        {
            try
            {
                changes.CommentStatus = "Поступили";
                _context.Change.Add(changes);
                _context.SaveChanges();

                var command = new ApplyChangesCommand(_context, changes);
                command.Execute();

                changes.CommentStatus = "Выполнено";
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            catch
            {
                changes.CommentStatus = "Ошибка";
                _context.SaveChanges();
                return StatusCode(500);
            }
        }
    }
}
