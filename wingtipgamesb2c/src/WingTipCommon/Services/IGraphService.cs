using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WingTipCommon.Services
{
    public interface IGraphService
    {
        Task<JObject> CreateUserAsync(
            string userName,
            string password,
            string displayName,
            string activationStatus,
            string playerTag,
            string termsOfServiceConsented);

        Task DeleteUserAsync(string userName);

        Task<JObject> GetAuditAsync(string activityType, int top, string userId);

        Task<JObject> GetReportAsync(string reportType, string userId);

        Task ResetAsync(string userName);

        Task SetUserActivationStatusAsync(string userName, string activationStatus);

        Task SetUserPasswordAsync(string userName, string password);
    }
}
