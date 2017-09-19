using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WingTipCommon.Identity
{
    public class WingTipUser : IdentityUser
    {
        public int MigrationStatus { get; set; }
    }
}
