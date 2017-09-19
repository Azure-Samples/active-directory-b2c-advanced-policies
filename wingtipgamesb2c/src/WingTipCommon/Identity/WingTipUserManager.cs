using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WingTipCommon.Identity
{
    public class WingTipUserManager : UserManager<WingTipUser>
    {
        public WingTipUserManager(
            IUserStore<WingTipUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<WingTipUser> passwordHasher,
            IEnumerable<IUserValidator<WingTipUser>> userValidators,
            IEnumerable<IPasswordValidator<WingTipUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<WingTipUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<IdentityResult> SetPasswordAsync(WingTipUser user, string newPassword)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var passwordStore = Store as IUserPasswordStore<WingTipUser>;

            if (passwordStore == null)
            {
                throw new InvalidOperationException();
            }

            await passwordStore.SetPasswordHashAsync(user, newPassword != null ? PasswordHasher.HashPassword(user, newPassword) : null, CancellationToken);

            if (SupportsUserSecurityStamp)
            {
                var securityStampStore = Store as IUserSecurityStampStore<WingTipUser>;

                if (securityStampStore == null)
                {
                    throw new InvalidOperationException();
                }

                await securityStampStore.SetSecurityStampAsync(user, Guid.NewGuid().ToString(), CancellationToken);
            }

            return await UpdateAsync(user);
        }
    }
}
