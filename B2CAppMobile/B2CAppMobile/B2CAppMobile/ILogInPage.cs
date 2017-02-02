using System.Threading.Tasks;

namespace B2CAppMobile
{
    public interface ILogInPage
    {
        Task RunPolicyAsync(
            string authority,
            string[] scope,
            string policy);
    }
}
