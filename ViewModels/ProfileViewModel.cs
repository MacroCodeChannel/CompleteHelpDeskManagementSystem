using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class ProfileViewModel
    {

        public ICollection<SystemTask> SystemTasks { get; set; }

        public ICollection<IdentityRole> SystemRoles { get; set; }

        public ICollection<int> RightsIdsAssigned { get; set; }

        public int[] Ids { get; set; }

        [DisplayName("Role Name")]
        public string RoleId { get;set; }


        [DisplayName("System Task")]
        public int TaskId { get; set; }
    }
}
