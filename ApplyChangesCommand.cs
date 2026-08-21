using Folivora.Scaffold;
using Project.DbModels;

namespace Project
{

    public class ApplyChangesCommand
    {
        private readonly CartridgeDbContext _context;
        private readonly Changes _changes;

        public ApplyChangesCommand(CartridgeDbContext context, Changes changes)
        {
            _context = context;
            _changes = changes;

        }

        public void Execute()
        {
            // Получение данных
            var cartridge = _context.Cartridges.Find(_changes.IdCartridgeFk);
            var printer = _context.Printers.Find(_changes.IdPrinterFk);

            if (cartridge == null || printer == null)
                throw new Exception("Данные не найдены");

            //Проверка картриджа
            bool cartridgeIsBusy = _context.Printers.Any(p => p.CartridgeId == _changes.IdCartridgeFk && p.IdPrint != printer.IdPrint);
            if (cartridgeIsBusy)
            {
                throw new Exception("Этот картридж уже установлен в другом принтере.");
            }

            // Старый картридж
            if (printer.CartridgeId != null)
            {
                var oldcartridge = _context.Cartridges.Find(printer.CartridgeId);
                oldcartridge.StatusCr = CartridgeStatus.Broken;
                oldcartridge.LocationCr = "Склад";
            }

            Request? request = _context.Requests.FirstOrDefault(u => u.IdPrinterFk == printer.IdPrint);
            if (request == null)
            {
                throw new Exception("Такой заявки не существует");
            }
            request.StatusZv = RequestStatus.Closed;

            cartridge.StatusCr = CartridgeStatus.Working;
            cartridge.LocationCr = "Кабинет";

            printer.CartridgeId = _changes.IdCartridgeFk;

            _context.SaveChanges();
        }
    }
}
