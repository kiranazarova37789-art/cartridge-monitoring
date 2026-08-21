using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbModels
{
    public enum CartridgeStatus
    {
        [Display(Name = "Working")]
        Working,

        [Display(Name = "Broken")]
        Broken,

        [Display(Name = "Refilling")]
        Refilling
    }

    [Table("cartridg")]
    public class Cartridge
    {
        [Key]
        public int IdCr { get; set; }

        public int ModelId { get; set; }

        public CartridgeStatus StatusCr { get; set; }

        public string LocationCr { get; set; }

        public byte[]? QrCode { get; set; }
    }
}
