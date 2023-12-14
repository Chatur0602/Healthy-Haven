using Microsoft.AspNetCore.Mvc.Rendering;

namespace Healthy_Haven.Models
{
    public class CategoryViewModel
    {
        public int id { get; set; }
        public string categoryName { get; set; }
        public IEnumerable<SelectListItem> listOfCategory { get; set; }
    }
}
