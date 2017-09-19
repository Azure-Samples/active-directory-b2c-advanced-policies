using System;

namespace WingTipToysWebApplication.ViewModels.Report
{
    public class B2CMfaRequestCountSummaryReportEntryViewModel
    {
        public DateTime Date { get; set; }

        public int MfaCount { get; set; }

        public int SmsMfaCount { get; set; }
    }
}
