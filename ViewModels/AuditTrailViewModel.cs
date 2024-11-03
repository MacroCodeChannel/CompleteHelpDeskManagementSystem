using HelpDeskSystem.Models;
using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class AuditTrailViewModel
    {
        [DisplayName("No")]
        public int Id { get; set; }

        [DisplayName("Action Type")]
        public string Action { get; set; }


        [DisplayName("Module")]
        public string Module { get; set; }

        public string AffectedTable { get; set; }



        [DisplayName("Created On")]
        public DateTime TimeStamp { get; set; } = DateTime.Now;



        [DisplayName("Created By")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        [DisplayName("Old Values")]
        public string? OldValues { get; set; }


        [DisplayName("New Values")]
        public string? NewValues { get; set; }


        [DisplayName("Affected Columns")]
        public string? AffectedColumns { get; set; }


        [DisplayName("Primary Key")]
        public string? PrimaryKey { get; set; }

        public List<AuditTrail> AuditTrails { get; set; }
    }
}
