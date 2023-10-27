using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class ApplicationUser : IdentityUser
    {
        public String FirstName { get; set; }
 
        public string LastName { get; set; }
        
        public char Gender { get; set; }

        public int Age { get; set; }

    }
}
