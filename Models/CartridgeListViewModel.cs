using Microsoft.AspNetCore.Mvc.Rendering;
using Project.DbModels;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class CartridgeListViewModel
    {
        public int IdView { get; set; }
        public string? firmNameView { get; set; }
        public string? modelNumberView { get; set; }
        public CartridgeStatus status { get; set; }
        public string location { get; set; }
        public IEnumerable<SelectListItem>? ModelOptions { get; set; }
        public IEnumerable<SelectListItem>? StatusOptions { get; set; }
        [Required(ErrorMessage = "Пожалуйста, выберите модель")]
        public int modelId { get; set; }
    }
}
