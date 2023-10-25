using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Please Enter Your First Name")]
        public String FirstName { get; set; }
        [Required(ErrorMessage = "Please Enter Your Last Name")]
        public string LastName { get; set; }
        
        public char Gender { get; set; }

        [Required(ErrorMessage = "Please Insert your Age")]
        public int Age { get; set; }

    }
}
