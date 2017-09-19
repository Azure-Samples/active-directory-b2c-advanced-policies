using System;

namespace WingTipToysWebApplication.ViewModels.Report
{
    public class B2CUserJourneySummaryEventsReportEntryViewModel
    {
        public string BasePolicyUniqueId { get; set; }

        public int CallerErrorCount { get; set; }

        public int FailureCount { get; set; }

        public int Id { get; set; }

        public string ResourceId { get; set; }

        public int SuccessCount { get; set; }

        public string UserJourneyId { get; set; }
    }
}
