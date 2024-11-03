using HelpDeskSystem.Models;
using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class TicketCategoryViewModel : UserActivity
    {
        [DisplayName("Category No")]
        public int Id { get; set; }

        [DisplayName("Category Code")]
        public string Code { get; set; }

        [DisplayName("Category Name")]

        public string Name { get; set; }

        public List<TicketCategory> TicketCategories { get; set; }
    }
}
