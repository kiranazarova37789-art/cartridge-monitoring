using Project.DbModels;

namespace Project.Models
{
    public class RequestViewModel
    {
        public int? Id { get; set; }
        public RequestStatus Status { get; set; }
        public string Ip { get; set; }
        public string Cartridge { get; set; }
        public string FullLocation { get; set; }
        public string? AssignedTo { get; set; }
    }
}
