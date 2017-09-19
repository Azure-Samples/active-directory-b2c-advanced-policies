using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WingTipCommon.Identity
{
    public class WingTipDbContext : IdentityDbContext<WingTipUser>
    {
        public WingTipDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
