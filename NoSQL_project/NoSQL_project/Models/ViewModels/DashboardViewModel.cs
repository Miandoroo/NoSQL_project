namespace NoSQL_project.Models.ViewModels
{
    // ViewModel for the Dashboard page
    public class DashboardViewModel
    {
       public TicketStats Stats { get; set; }
       public List<Ticket> RecentTickets { get; set; }
       public bool IsServiceDesk { get; set; }

        public DashboardViewModel(TicketStats stats, List<Ticket> recentTickets, bool isServiceDesk)
        {
            Stats = stats;
            RecentTickets = recentTickets;
            IsServiceDesk = isServiceDesk;
        }
    }
}

