using System.Collections.Generic;
using System.Linq;

namespace NoSQL_project.Models.ViewModels
{
    public class StatusBreakdownVM
    {
        public string Title { get; set; } = "";
        public int Open { get; set; }          // status 0 (no resuelto aún)
        public int InProgress { get; set; }    // status 1 (si lo usas)
        public int Resolved { get; set; }      // status 2 (resuelto y cerrado)
        public int Closed { get; set; }        // status 3 (cerrado sin resolver)
        public int Total => Open + InProgress + Resolved + Closed;

        public double PctOpen => Total == 0 ? 0 : (double)Open / Total * 100;
        public double PctInProgress => Total == 0 ? 0 : (double)InProgress / Total * 100;
        public double PctResolved => Total == 0 ? 0 : (double)Resolved / Total * 100;
        public double PctClosed => Total == 0 ? 0 : (double)Closed / Total * 100;

        public static StatusBreakdownVM FromTickets(IEnumerable<NoSQL_project.Models.Tickets> tickets)
        {
            var vm = new StatusBreakdownVM();
            foreach (var g in tickets.GroupBy(t => t.Status))
            {
                switch (g.Key)
                {
                    case 0: vm.Open = g.Count(); break;
                    case 1: vm.InProgress = g.Count(); break;
                    case 2: vm.Resolved = g.Count(); break;
                    case 3: vm.Closed = g.Count(); break;
                }
            }
            return vm;
        }
    }
}
