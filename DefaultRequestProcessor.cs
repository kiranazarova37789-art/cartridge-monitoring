using Folivora.Scaffold;
using Project.DbModels;

namespace Project
{
    public class DefaultRequestProcessor : RequestProcessor
    {
        private readonly CartridgeDbContext _context;
        public DefaultRequestProcessor(CartridgeDbContext context)
        {
            _context = context;
        }
        protected override string GetCartridge(Request request)
        {
            var printer = _context.Printers.FirstOrDefault(p => p.IdPrint == request.IdPrinterFk);

            if (printer?.CartridgeId == null) return request.CartridgeModel;


            var data = (from c in _context.Cartridges
                        join m in _context.Models on c.ModelId equals m.Id
                        join f in _context.Vendors on m.FirmId equals f.Id
                        where c.IdCr == printer.CartridgeId
                        select new { f.Name, m.ModelNumber }
                        ).FirstOrDefault();

            return data != null ? $"{data.Name} {data.ModelNumber}".Trim() : request.CartridgeModel;
        }

        protected override string GetLocation(Request request)
        {
            var printer = _context.Printers.FirstOrDefault(p => p.IdPrint == request.IdPrinterFk);
            var office = _context.Offices.FirstOrDefault(o => o.Id == printer.OffId);

            if (office == null) return "Местоположение не указано";

            return new OfficeToStringCommand(_context, office).Execute();
        }
    }
}
