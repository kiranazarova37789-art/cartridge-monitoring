using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Project.DbModels;

namespace Project.Controllers
{
    public class OfficeController : Controller
    {
        private CartridgeDbContext _context;
        public OfficeController(CartridgeDbContext context)
        {
            _context = context;
        }

        // GET: HomeController
        public ActionResult Index()
        {
            var offices = _context.Offices
                .OrderBy(o => o.Parent)
                .ThenBy(o => o.Level)
                .ToList();

            return View(offices);
        }


        // GET: HomeController/Create
        public IActionResult Create(int? parentId)
        {
            var office = new Office();

            if (parentId.HasValue)
            {
                var parentOffice = _context.Offices.FirstOrDefault(o => o.Id == parentId.Value);

                if (parentOffice != null)
                {
                    office.Parent = parentId;
                    office.Level = parentOffice.Level + 1;

                    ViewBag.ParentName = parentOffice.Name;
                }
                else
                {
                    office.Level = 1;
                }
            }
            else
            {
                office.Level = 1;
            }

            return View(office);
        }


        // POST: Office/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Office office)
        {
            if (ModelState.IsValid)
            {
                _context.Add(office);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            if (office.Parent.HasValue)
            {
                var parent = _context.Offices.Find(office.Parent);
                ViewBag.ParentId = office.Parent.Value;
                ViewBag.ParentName = parent?.Name;
            }

            return View(office);
        }

        // POST: HomeController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var office = await _context.Offices.FindAsync(id);

            if (office != null)
            {
                _context.Offices.Remove(office);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
