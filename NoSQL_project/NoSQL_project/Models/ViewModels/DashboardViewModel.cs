using NoSQL_project.Models;

namespace NoSQL_project.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public double PercentOpen { get; set; }
        public double PercentResolved { get; set; }
        public double PercentClosed { get; set; }
        public List<Ticket> RecentTickets { get; set; }
        public bool IsServiceDesk { get; set; }
    }
}

