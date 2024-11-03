using HelpDeskSystem.Models;
using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class TicketSubCategoriesVM :UserActivity
    {

        [DisplayName("Category No")]
        public int Id { get; set; }

        [DisplayName("Category Name")]
        public int CategoryId { get; set; }
        public TicketCategory Category { get; set; }

        [DisplayName("Sub Category Code")]
        public string Code { get; set; }

        [DisplayName("Sub Category Name")]
        public string Name { get; set; }

        public List<TicketSubCategory> TicketSubCategories { get; set;}

    }
}
