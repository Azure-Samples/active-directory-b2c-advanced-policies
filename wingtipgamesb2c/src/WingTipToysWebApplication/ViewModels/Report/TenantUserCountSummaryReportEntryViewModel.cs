using System;

namespace WingTipToysWebApplication.ViewModels.Report
{
    public class TenantUserCountSummaryReportEntryViewModel
    {
        public int AmazonExternalUserCount { get; set; }

        public DateTime Date { get; set; }

        public int ExternalUserCount { get; set; }

        public int FacebookExternalUserCount { get; set; }

        public int InternalUserCount { get; set; }

        public int LocalUserCount { get; set; }

        public int TotalUserCount { get; set; }
    }
}
