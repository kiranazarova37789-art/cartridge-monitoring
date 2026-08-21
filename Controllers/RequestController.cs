using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Project.DbModels;

namespace Project.Controllers
{
    public class RequestController : Controller
    {
        private CartridgeDbContext _context;
        public RequestController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: ZaiavkaController
        public IActionResult Index()
        {
            var request = _context.Requests.ToList();
            var processor = new DefaultRequestProcessor(_context);

            var viewModels = processor.Process(request);

            return View(viewModels);
        }

        // GET: ZaiavkaController/Details/5
        public ActionResult Details(int id)
        {
            Request? request = new Request();
            request = _context.Requests.FirstOrDefault(x => x.IdZv == id);

            return View(request);
        }

        // GET: ZaiavkaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ZaiavkaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Request request)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ZaiavkaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ZaiavkaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ZaiavkaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ZaiavkaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


    }
}