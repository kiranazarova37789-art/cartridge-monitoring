using Folivora.Scaffold;
using Project.DbModels;

namespace Project
{
    public class OfficeToStringCommand
    {
        private readonly CartridgeDbContext _context;
        private readonly Office _office;

        public OfficeToStringCommand(CartridgeDbContext context, Office office)
        {
            _context = context;
            _office = office;
        }

        public string Execute()
        {
            List<string> hierarchy = new List<string> { _office.Name };
            HashSet<int> visitedOffices = new HashSet<int>();
            int? currentParent = _office.Parent;

            while (currentParent.HasValue)
            {
                if (visitedOffices.Contains(currentParent.Value))
                {
                    throw new Exception("Циклическая зависимость в иерархии офисов.");
                }

                visitedOffices.Add(currentParent.Value);

                var parentOffice = _context.Offices.FirstOrDefault(o => o.Id == currentParent.Value);
                if (parentOffice != null)
                {
                    hierarchy.Insert(0, parentOffice.Name);
                    currentParent = parentOffice.Parent;
                }
                else
                {
                    break;
                }
            }

            string fullLocation = string.Join(" > ", hierarchy);

            switch (_office.Level)
            {
                case 1:
                    return $"{fullLocation} (Корпус)";
                case 2:
                    return $"{fullLocation} (Этаж)";
                case 3:
                    return $"{fullLocation} (Кабинет)";
                default:
                    return $"{fullLocation} ";
            }
        }

    }
}
