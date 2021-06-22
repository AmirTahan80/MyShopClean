using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class RoleModel:IdentityRole
    {
        [Required]
        [MaxLength(250)]
        public string RoleNamePersian { get; set; }
    }
}