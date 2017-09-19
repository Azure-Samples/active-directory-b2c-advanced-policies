using System;

namespace WingTipToysWebApplication.ViewModels.Report
{
    public class B2CAuthenticationCountSummaryReportEntryViewModel
    {
        public int AuthenticationCount { get; set; }

        public DateTime Date { get; set; }

        public int AuthorizationCodeAsConfidentialClientAuthenticationCount { get; set; }

        public int AuthorizationCodeAsPublicClientAuthenticationCount { get; set; }

        public int HybridAuthenticationCount { get; set; }

        public int ImplicitAuthenticationCount { get; set; }
    }
}
