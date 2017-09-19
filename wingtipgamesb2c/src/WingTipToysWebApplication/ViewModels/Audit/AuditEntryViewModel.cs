using System;

namespace WingTipToysWebApplication.ViewModels.Audit
{
    public class AuditEntryViewModel
    {
        public string Activity { get; set; }

        public DateTime ActivityDate { get; set; }

        public string ActorName { get; set; }

        public string ActorType { get; set; }

        public string CorrelationId { get; set; }

        public string TargetResourceName { get; set; }

        public string TargetResourceType { get; set; }
    }
}
