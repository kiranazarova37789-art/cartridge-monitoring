using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DbModels;

namespace Project.Controllers
{
    public class CartridgeController : Controller
    {
        private CartridgeDbContext _context;
        public CartridgeController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: CartridgController
        public IActionResult Index()
        {
            var query = from cartridg in _context.Cartridges
                        join model in _context.Models on cartridg.ModelId equals model.Id
                        join firm in _context.Vendors on model.FirmId equals firm.Id
                        select new
                        {
                            Cartridg = cartridg,
                            Model = model,
                            Firm = firm
                        };
            return View(query.ToList());
        }


        // GET: CartridgController/Details/5
        public ActionResult Details(int id)
        {
            var cartridge = _context.Cartridges.FirstOrDefault(x => x.IdCr == id);

            if (cartridge == null)
            {
                return NotFound();
            }

            return View(cartridge);
        }

        [HttpGet]
        public IActionResult ShowQr(int id)
        {
            var cartridge = _context.Cartridges.Find(id);
            if (cartridge == null) return NotFound();

            string baseUrl = "https://cartridgesmonitoring.azurewebsites.net/api/cartridgrest/";
            string fullUrl = baseUrl + id;

            var qrCommand = new GenerateQrCommand();
            byte[] qrCode = qrCommand.Execute(fullUrl);

            cartridge.QrCode = qrCode;
            _context.SaveChanges();

            string base64 = Convert.ToBase64String(qrCode);
            ViewBag.QrCode = $"data:image/png;base64, {base64}";

            ViewBag.CartridgeID = cartridge.IdCr;

            return View();

        }

        // GET: CartridgController/Create
        public ActionResult Create()
        {
            ViewBag.Statuses = Enum.GetValues(typeof(CartridgeStatus));

            ViewBag.Models = _context.Models
                .Join(_context.Vendors,
                      m => m.FirmId,
                      f => f.Id,
                      (m, f) => new
                      {
                          Id = m.Id,
                          FullName = f.Name + " - " + m.ModelNumber
                      })
                .ToList();

            return View();
        }

        // POST: Cartridg/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cartridge cartridg)
        {
            try
            {
                if (cartridg.ModelId != 0)
                {
                    _context.Cartridges.Add(cartridg);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Statuses = Enum.GetValues(typeof(CartridgeStatus));
                ViewBag.Models = _context.Models
                    .Join(_context.Vendors, m => m.FirmId, f => f.Id,
                          (m, f) => new { Id = m.Id, FullName = f.Name + " - " + m.ModelNumber })
                    .ToList();

                return View(cartridg);
            }
            catch
            {
                return View(cartridg);
            }
        }


        // GET: CartridgController/Edit/5
        public ActionResult Edit(int id)
        {
            var cr = _context.Cartridges
                .FirstOrDefault(u => u.IdCr == id);

            if (cr == null)
                return NotFound();

            ViewBag.Statuses = Enum.GetValues(typeof(CartridgeStatus));

            ViewBag.Models = _context.Models
                .Select(m => new
                {
                    Id = m.Id,
                    FullName = _context.Vendors.FirstOrDefault(f => f.Id == m.FirmId).Name + " " + m.ModelNumber
                }).ToList();

            return View(cr);
        }


        // POST: CartridgController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cartridge cartridg)
        {
            try
            {
                if (id != cartridg.IdCr)
                {
                    return NotFound();
                }
                var existingCr = _context.Cartridges.AsNoTracking().FirstOrDefault(c => c.IdCr == id);
                if (existingCr == null) return NotFound();

                if (cartridg.QrCode == null)
                {
                    cartridg.QrCode = existingCr.QrCode;
                }

                _context.Update(cartridg);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cartridges.Any(e => e.IdCr == id))
                {
                    return NotFound();
                }
                throw;
            }

        }

        // GET: CartridgController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CartridgController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var cartridg = _context.Cartridges.Find(id);
                if (cartridg != null)
                {
                    _context.Cartridges.Remove(cartridg);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Delete));
                }
                return BadRequest();
            }
            catch
            {
                return View();
            }
        }
    }
}
