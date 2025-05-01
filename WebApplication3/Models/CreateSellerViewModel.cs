using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication3.Models
{
    public class CreateSellerViewModel
    {
        public int? UserId { get; set; }
        public IEnumerable<SelectListItem> UsersList { get; set; }
    }

}
