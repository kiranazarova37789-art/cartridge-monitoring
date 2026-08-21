using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.DbModels;


namespace Project.Controllers
{
    public class ModelCompatibilityController : Controller
    {
        private CartridgeDbContext _context;
        public ModelCompatibilityController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: ModelCompatibilityController1
        public async Task<IActionResult> Index(int? id)
        {
            ViewBag.SelectedPrinterId = id;

            await LoadCommonData();

            if (!id.HasValue)
            {
                ViewBag.PrinterTitle = "Выберите модель принтера";
                return View(new List<Model>());
            }

            var printers = (IEnumerable<dynamic>)ViewBag.Printers;
            var currentPrinter = printers.FirstOrDefault(x => x.Id == id.Value);

            if (currentPrinter == null)
                return NotFound();

            ViewBag.PrinterTitle = $"{currentPrinter.FirmName} {currentPrinter.ModelNumber}";

            await LoadCartridgesData(id);
            return View();
        }


        private async Task LoadCommonData()
        {
            var vendors = await _context.Vendors.OrderBy(x => x.Name).ToListAsync();
            ViewBag.VendorsList = new SelectList(vendors, "Id", "Name");

            var printers = await (
                from m in _context.Models
                join v in _context.Vendors on m.FirmId equals v.Id
                where m.Type == EnumModelType.Printer
                orderby v.Name
                select new
                {
                    m.Id,
                    m.ModelNumber,
                    FirmName = v.Name
                })
                .ToListAsync();

            ViewBag.Printers = printers;
        }

        private async Task LoadCartridgesData(int? printerId)
        {
            var cartridges = await (
        from mc in _context.ModelCompatibilities
        join m in _context.Models on mc.CartridgeModelId equals m.Id
        join v in _context.Vendors on m.FirmId equals v.Id
        where mc.PrinterModelId == printerId
        orderby v.Name, m.ModelNumber
        select new
        {
            m.Id,
            m.ModelNumber,
            FirmName = v.Name
        })
        .ToListAsync();

            ViewBag.Cartridges = cartridges;

            var linkedIds = await _context.ModelCompatibilities.Where(x => x.PrinterModelId == printerId).Select(x => x.CartridgeModelId).ToListAsync();

            var availableCartridges = await (
                from m in _context.Models
                join v in _context.Vendors on m.FirmId equals v.Id
                where m.Type == EnumModelType.Cartridge && !linkedIds.Contains(m.Id)
                orderby v.Name, m.ModelNumber
                select new
                {
                    m.Id,
                    DisplayText = $"[{v.Name}] {m.ModelNumber}"
                })
                .ToListAsync();

            ViewBag.AvailableCartridges = new SelectList(
                availableCartridges,
                "Id",
                "DisplayText"
            );
        }

        // POST: ModelCompatibility/AddCompatibility
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCompatibility(int printerModelId, int cartridgeModelId)
        {
            var exists = await _context.ModelCompatibilities
                .AnyAsync(x => x.PrinterModelId == printerModelId && x.CartridgeModelId == cartridgeModelId);

            if (!exists)
            {
                var newLink = new ModelCompatibility
                {
                    PrinterModelId = printerModelId,
                    CartridgeModelId = cartridgeModelId
                };

                _context.ModelCompatibilities.Add(newLink);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { id = printerModelId });
        }

        // POST: ModelCompatibility/RemoveCompatibility
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCompatibility(int printerModelId, int cartridgeModelId)
        {
            var record = await _context.ModelCompatibilities.FirstOrDefaultAsync(x => x.PrinterModelId == printerModelId && x.CartridgeModelId == cartridgeModelId);

            if (record != null)
            {
                _context.ModelCompatibilities.Remove(record);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { id = printerModelId });
        }

        // POST: ModelCompatibility/CreateSpecification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSpecification(int? firmId, string newFirmName, string modelNumber, EnumModelType type)
        {
            if (string.IsNullOrWhiteSpace(modelNumber))
            {
                return RedirectToAction(nameof(Index));
            }

            int targetFirmId = await GetOrCreateFirm(firmId, newFirmName);

            if (targetFirmId == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            string cleanModelNumber = modelNumber.Trim();

            var isDuplicate = await _context.Models.AnyAsync(x =>
                x.FirmId == targetFirmId &&
                x.ModelNumber.ToLower() == cleanModelNumber.ToLower() &&
                x.Type == type);

            if (isDuplicate)
            {
                return RedirectToAction(nameof(Index));
            }

            var newModel = new Model
            {
                FirmId = targetFirmId,
                ModelNumber = modelNumber.Trim(),
                Type = type
            };

            _context.Models.Add(newModel);
            await _context.SaveChangesAsync();

            if (type == EnumModelType.Printer)
            {
                return RedirectToAction(nameof(Index), new { id = newModel.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<int> GetOrCreateFirm(int? firmId, string newFirmName)
        {
            if (firmId is > 0) return firmId.Value;

            if (string.IsNullOrWhiteSpace(newFirmName)) return 0;

            newFirmName = newFirmName.Trim();

            var vendor = await _context.Vendors.FirstOrDefaultAsync(x => x.Name.ToLower() == newFirmName.ToLower());
            if (vendor == null)
            {
                vendor = new Vendor { Name = newFirmName };
                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();
            }
            return vendor.Id;
        }


    }
}
