using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class ApplicationUserViewModel :IdentityUser
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }


        [DisplayName("Last Name")]
        public string LastName { get; set; }


        [DisplayName("Gender")]
        public int? GenderId { get; set; }
        public SystemCodeDetail Gender { get; set; }


        [DisplayName("Country Name")]
        public string Country { get; set; }

        public string City { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}";


        [DisplayName("Role Name")]
        public string? RoleId { get; set; }

        public IdentityRole Role { get; set; }

        [DisplayName("Is Locked")]
        public bool? IsLocked { get; set; }

        public List<ApplicationUser> ApplicationUsers { get; set; }
    }
}
