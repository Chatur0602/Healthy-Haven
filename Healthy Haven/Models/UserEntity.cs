using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class UserEntity
    {
        [Key] 
        public int UserId { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public char Gender { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

    }
}
