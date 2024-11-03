using AutoMapper;
using HelpDeskSystem.Models;
using HelpDeskSystem.ViewModels;

namespace HelpDeskSystem.Services
{
    public class AutomapperProfileService : Profile
    {
        public AutomapperProfileService()
        {
            CreateMap<TicketViewModel, Ticket>().ReverseMap();
            CreateMap<SystemCodeViewModel, SystemCode>().ReverseMap();
        }
    }
}
