namespace NoSQL_project.Models.ViewModels
{
    public class DashboardVm
    {
        public int Days { get; set; }
        public bool IsAll { get; set; }          // true = All, false = My
        public int Total { get; set; }
        public int Open { get; set; }            // 0
        public int InProgress { get; set; }      // 1
        public int Resolved { get; set; }        // 2
        public int Closed { get; set; }          // 3

        public double PctOpen => Total == 0 ? 0 : (Open * 100.0 / Total);
        public double PctInProgress => Total == 0 ? 0 : (InProgress * 100.0 / Total);
        public double PctResolved => Total == 0 ? 0 : (Resolved * 100.0 / Total);
        public double PctClosed => Total == 0 ? 0 : (Closed * 100.0 / Total);
    }
}
