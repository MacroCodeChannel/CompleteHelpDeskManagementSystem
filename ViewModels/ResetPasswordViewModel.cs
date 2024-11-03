using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required]
        [StringLength(100,ErrorMessage ="The {0} must be at least {2} charcters long",MinimumLength =6)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} charcters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password",ErrorMessage ="The Password and Confirmation Password do not match")]
        public string ConfirmPassword { get; set; }


        [Required]
        [EmailAddress]
        [DisplayName("Email Address")]
        public string Email { get; set; }   

        public string Id {  get; set; }


        [DisplayName("User FullName")]
        public string FullName { get; set; }

        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }

        [DisplayName("User Role")]
        public IdentityRole Role { get; set; }

        [DisplayName("User Role")]
        public string RoleId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }


        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }


        [DisplayName("Last Name")]
        public string LastName { get; set; }


        [DisplayName("Gender")]
        public int? GenderId { get; set; }
        public SystemCodeDetail Gender { get; set; }
    }
}
