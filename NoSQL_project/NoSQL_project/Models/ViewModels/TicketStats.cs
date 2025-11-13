namespace NoSQL_project.Models.ViewModels
{
    // Deze klass is om te samen maken de Tickets met de dingentjes dat zijn openen of niet
    public class TicketStats
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public double PercentOpen => TotalTickets > 0 ? OpenTickets * 100.0 / TotalTickets : 0;
        public double PercentResolved => TotalTickets > 0 ? ResolvedTickets * 100.0 / TotalTickets : 0;
        public double PercentClosed => TotalTickets > 0 ? ClosedTickets * 100.0 / TotalTickets : 0;

    }
}
