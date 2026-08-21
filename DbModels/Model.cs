using System.ComponentModel.DataAnnotations;

namespace Project.DbModels
{
    public enum EnumModelType
    {
        [Display(Name = "Printer")]
        Printer = 1,

        [Display(Name = "Cartridge")]
        Cartridge = 2
    }
    public class Model
    {
        public int Id { get; set; }
        public int FirmId { get; set; }
        public string ModelNumber { get; set; }
        public EnumModelType Type { get; set; }
    }
}
