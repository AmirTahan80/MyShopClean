using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class UserDetail
    {
        [Key]
        public int UserDetailId { get; set; }
        [MaxLength(250)]
        public string FirstName { get; set; }
        [MaxLength(250)]
        public string LastName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        [MaxLength(550)]
        public string Address { get; set; }

    }
}
