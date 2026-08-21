using Project.DbModels;
using Project.Models;

namespace Project
{
    public abstract class RequestProcessor
    {
        public List<RequestViewModel> Process(IEnumerable<Request> requests)
        {
            var result = new List<RequestViewModel>();

            foreach (var request in requests)
            {
                var viewModel = Map(request);

                result.Add(viewModel);
            }
            return result;
        }

        protected virtual RequestViewModel Map(Request request)
        {
            return new RequestViewModel
            {
                Id = request.IdZv,
                Status = request.StatusZv,
                Ip = request.IpPrinter,
                AssignedTo = request.LastName,
                Cartridge = GetCartridge(request),
                FullLocation = GetLocation(request)
            };
        }
        protected abstract string GetCartridge(Request request);
        protected abstract string GetLocation(Request request);
    }
}
