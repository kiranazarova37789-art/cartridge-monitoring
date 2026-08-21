using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DbModels;
using System.Net;

namespace Project.Controllers
{
    public class PrinterController : Controller
    {
        private CartridgeDbContext _context;
        public PrinterController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: PrinterController
        public IActionResult Index([FromQuery(Name = "location_id")] int? locationId)
        {
            List<Office> filteredOffices = new();
            if (locationId != null)
            {
                filteredOffices = FilterOffices(_context, locationId);
            }
            else
            {
                filteredOffices = _context.Offices.AsNoTracking().ToList();
            }

            var offices = filteredOffices;

            var printers = _context.Printers.Where(printer => offices.Select(of => of.Id).ToList().Contains(printer.OffId)).AsNoTracking().ToList();

            var models = _context.Models.AsNoTracking().ToList();
            var vendor = _context.Vendors.AsNoTracking().ToList();

            var result = printers.Select(p =>
            {
                var office = offices.FirstOrDefault(o => o.Id == p.OffId);
                var model = models.FirstOrDefault(m => m.Id == p.ModelId);

                Vendor firm = null;
                if (model != null)
                {
                    firm = vendor.FirstOrDefault(f => f.Id == model.FirmId);
                }

                string modelName = "Неизвестная модель";
                if (model != null)
                {
                    string vendorName = string.Empty;
                    if (firm != null)
                    {
                        vendorName = firm.Name;
                    }

                    modelName = $"{vendorName} {model.ModelNumber}".Trim();
                }

                string officePath = "Офис не назначен";
                if (office != null)
                {
                    officePath = new OfficeToStringCommand(_context, office).Execute();
                }

                return new
                {
                    IdPrint = p.IdPrint,
                    Ip = p.Ip,
                    ModelName = modelName,
                    SerialNumber = p.Number,
                    CartridgeId = p.CartridgeId,
                    HasCartridge = p.CartridgeId.HasValue,
                    FullOfficePath = officePath,
                    qr = p.QrCode
                };
            }).ToList();

            ViewOfficePrinter();
            return View(result);
        }

        private List<Office> FilterOffices(CartridgeDbContext context, int? locationId)
        {
            var location = _context.Offices.Find(locationId) ?? throw new ArgumentException($"Invalid location {locationId}");
            var offices = _context.Offices.Where(office => office.Parent == location.Id).ToList();

            var childrens = offices.SelectMany(loc => FilterOffices(context, loc.Id)).Where(child => child.Id != locationId).ToList();

            offices.AddRange(childrens);
            offices.Add(location);
            return offices;
        }

        public void ViewOfficePrinter()
        {
            var offices = _context.Offices
               .OrderBy(o => o.Parent)
               .ThenBy(o => o.Level)
               .ToList();

            var printers = _context.Printers.AsNoTracking().ToList();

            var firms = _context.Vendors.ToList();
            var models = _context.Models.ToList();

            var printerFullNames = (from m in models
                                    join f in firms on m.FirmId equals f.Id
                                    select new { m.Id, FullName = $"{f.Name} {m.ModelNumber}" })
                                    .ToDictionary(x => x.Id, x => x.FullName);

            var printersByOffice = printers
                .Where(p => p.OffId > 0)
                .GroupBy(p => p.OffId)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.Offices = offices;
            ViewBag.PrintersByOffice = printersByOffice;
            ViewBag.PrinterFullNames = printerFullNames;
        }



        [HttpGet]
        public IActionResult ShowQr(int id)
        {
            var printer = _context.Printers.Find(id);
            if (printer == null) return NotFound();

            string baseUrl = "https://cartridgesmonitoring.azurewebsites.net/api/requestrest/new?printer=";
            string fullUrl = baseUrl + id;

            var qrCommand = new GenerateQrCommand();
            byte[] qrCode = qrCommand.Execute(fullUrl);

            printer.QrCode = qrCode;
            _context.SaveChanges();

            string base64String = Convert.ToBase64String(qrCode);
            ViewBag.QrCode = $"data:image/png;base64,{base64String}";

            ViewBag.PrinterIp = printer.Ip;

            return View();
        }


        // GET: PrinterController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PrinterController/Create
        public IActionResult Create()
        {
            FillViewData();
            return View();
        }

        private void FillViewData()
        {
            var allOffices = _context.Offices.AsNoTracking().ToList();

            var officesForView = allOffices
                .Select(o =>
                {
                    string path = new OfficeToStringCommand(_context, o).Execute();

                    return new
                    {
                        Id = o.Id,
                        Path = path
                    };
                })
                .OrderBy(x => x.Path)
                .ToList();

            ViewBag.Offices = officesForView;

            var firms = _context.Vendors
                .AsNoTracking()
                .ToDictionary(f => f.Id, f => f.Name);

            var printerModels = _context.Models.AsNoTracking().ToList();

            var modelsForView = printerModels
                .Select(m =>
                {
                    string firmName = "Unknown";

                    if (firms.ContainsKey(m.FirmId))
                    {
                        firmName = firms[m.FirmId];
                    }

                    string modelName = firmName + " " + m.ModelNumber;

                    return new
                    {
                        Id = m.Id,
                        Name = modelName
                    };
                })
                .OrderBy(x => x.Name)
                .ToList();

            ViewBag.Models = modelsForView;

            var busyCartridgeIds = _context.Printers
                  .Where(p => p.CartridgeId != null)
                  .Select(p => (int)p.CartridgeId)
                  .ToList();

            var modelNames = printerModels
                .ToDictionary(m => m.Id, m => m.ModelNumber);

            var cartridgesForView = _context.Cartridges
                .AsNoTracking()
                .Where(c => !busyCartridgeIds.Contains(c.IdCr))
                .ToList()
                .Select(c =>
                {
                    string modelName = "Не указана";

                    if (modelNames.ContainsKey(c.ModelId))
                    {
                        modelName = modelNames[c.ModelId];
                    }

                    string name = $"ID: {c.IdCr} (Модель: {modelName})";

                    return new
                    {
                        Id = c.IdCr,
                        Name = name
                    };
                })
                .ToList();

            ViewBag.Cartridges = cartridgesForView;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Printer printer)
        {
            string? customerIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (customerIp == "::1")
            {
                customerIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            }

            printer.Ip = customerIp ?? "0.0.0.0";

            try
            {

                if (printer.CartridgeId != null && _context.Printers.Any(p => p.CartridgeId == printer.CartridgeId))
                {
                    ModelState.AddModelError("", "Этот картридж уже установлен в другом принтере");
                }
                else if (ModelState.IsValid)
                {
                    if (printer.CartridgeId != null)
                    {
                        var cartridge = _context.Cartridges.Find(printer.CartridgeId);
                        if (cartridge != null)
                        {
                            cartridge.StatusCr = CartridgeStatus.Working;
                            cartridge.LocationCr = "В принтере";
                        }
                    }

                    _context.Printers.Add(printer);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при сохранении: " + ex.Message);
            }

            FillViewData();
            return View(printer);
        }

        // GET: PrinterController/Edit/5
        public IActionResult Edit(int id)
        {
            var printer = _context.Printers.Find(id);
            if (printer == null)
            {
                return NotFound();
            }

            var allOffices = _context.Offices.AsNoTracking().ToList();

            var officesForView = allOffices
                .Select(o =>
                {
                    string path = new OfficeToStringCommand(_context, o).Execute();

                    return new
                    {
                        Id = o.Id,
                        Path = path
                    };
                })
                .OrderBy(x => x.Path)
                .ToList();

            ViewBag.Offices = officesForView;

            var models = _context.Models.AsNoTracking().ToList();
            var firms = _context.Vendors.AsNoTracking().ToDictionary(f => f.Id, f => f.Name);

            var modelNames = new Dictionary<int, string>();

            foreach (var model in models)
            {
                string firmName = "???";

                if (firms.ContainsKey(model.FirmId))
                {
                    firmName = firms[model.FirmId];
                }

                string fullModelName = firmName + " " + model.ModelNumber;
                modelNames.Add(model.Id, fullModelName);
            }

            var busyCartridgeIds = _context.Printers
                .Where(p => p.CartridgeId != null && p.IdPrint != id)
                .Select(p => (int)p.CartridgeId)
                .ToList();

            var cartridgesForView = _context.Cartridges
                .AsNoTracking()
                .Where(c => c.ModelId == printer.ModelId)
                .Where(c => !busyCartridgeIds.Contains(c.IdCr))
                .ToList()
                .Select(c =>
                {
                    string modelName = "Модель не указана";

                    if (modelNames.ContainsKey(c.ModelId))
                    {
                        modelName = modelNames[c.ModelId];
                    }

                    string name = "ID: " + c.IdCr + " | " + modelName;

                    return new
                    {
                        Id = c.IdCr,
                        Name = name
                    };
                })
                .ToList();

            ViewBag.Cartridges = cartridgesForView;

            string modelDisplayName = "Неизвестная модель";

            if (modelNames.ContainsKey(printer.ModelId))
            {
                modelDisplayName = modelNames[printer.ModelId];
            }

            ViewBag.ModelDisplayName = modelDisplayName;

            return View(printer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Printer printer)
        {
            try
            {
                var printInDb = _context.Printers.FirstOrDefault(u => u.IdPrint == id);
                if (printInDb == null) return NotFound();

                printInDb.OffId = printer.OffId;

                if (printInDb.CartridgeId != printer.CartridgeId)
                {
                    printInDb.CartridgeId = printer.CartridgeId;

                    if (printer.CartridgeId != null)
                    {
                        var cartridge = _context.Cartridges.Find(printer.CartridgeId);
                        if (cartridge != null)
                        {
                            cartridge.StatusCr = CartridgeStatus.Working;
                            cartridge.LocationCr = "В принтере";
                        }
                    }
                }



                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка: " + ex.Message);
                return View(printer);
            }
        }

        // GET: PrinterController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PrinterController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var printer = _context.Printers.Find(id);

            if (printer == null)
            {
                return NotFound();
            }

            try
            {
                var relatedZaiavki = _context.Requests.Where(z => z.IpPrinter == printer.Ip).ToList();

                if (relatedZaiavki.Any())
                {
                    _context.Requests.RemoveRange(relatedZaiavki);
                }

                var relatedChanges = _context.Change.Where(c => c.IdPrinterFk == id).ToList();

                if (relatedChanges.Any())
                {
                    _context.Change.RemoveRange(relatedChanges);
                }

                _context.SaveChanges();

                _context.Printers.Remove(printer);
                _context.SaveChanges();

                return RedirectToAction(nameof(Delete));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Не удалось удалить принтер: " + ex.Message);
                return View("Delete", printer);
            }
        }

    }
}
