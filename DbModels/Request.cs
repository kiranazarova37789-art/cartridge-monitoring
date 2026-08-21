using System.ComponentModel.DataAnnotations;

namespace Project.DbModels
{
    public enum RequestStatus
    {
        [Display(Name = "Open")]
        Open,

        [Display(Name = "Closed")]
        Closed,

        [Display(Name = "At work")]
        AtWork
    }
    public class Request
    {
        internal readonly object Printer;

        public int IdZv { get; set; }
        public RequestStatus StatusZv { get; set; }
        public string? IpPrinter { get; set; }
        public string? CartridgeModel { get; set; }
        public string? LastName { get; set; }
        public int? IdPrinterFk { get; set; }
    }
}
