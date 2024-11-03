using HelpDeskSystem.Models;
using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class SystemCodeDetailViewModel : UserActivity
    {
        public int Id { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; }

        [DisplayName("Description")]    
        public string Description { get; set; }


        [DisplayName("Order No")]
        public int? OrderNo { get; set; }


        [DisplayName("System Code")]    
        public int SystemCodeId { get; set; }
        public SystemCode SystemCode { get; set; }

        public List<SystemCodeDetail>  SystemCodeDetails { get; set; }  
    }
}
