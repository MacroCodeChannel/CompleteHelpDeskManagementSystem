namespace HelpDeskSystem.Models
{
    public class TicketsSummaryView
    {
        public int TotalTickets { get; set; }

        public int PendingTickets { get; set; }
        public int AssignedTickets { get; set; }

        public int ClosedTickets { get; set; }

        public int ResolvedTickets { get; set; }

        public int ReOpenedTickets { get; set; }

        public int EscalatedTickets { get; set; }
    }
}
