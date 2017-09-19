using System.Collections.Generic;

namespace WingTipToysWebApplication.ViewModels.Audit
{
    public class IndexViewModel
    {
        public List<AuditEntryViewModel> AuditEntries { get; set; }

        public int Top { get; set; }
    }
}
