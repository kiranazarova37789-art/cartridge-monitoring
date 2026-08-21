namespace Project.DbModels
{
    public class Printer
    {
        public string Ip { get; set; }
        public int IdPrint { get; set; }
        public int? CartridgeId { get; set; }
        public int OffId { get; set; }
        public byte[]? QrCode { get; set; }
        public int ModelId { get; set; }
        public string Number { get; set; }
    }
}
