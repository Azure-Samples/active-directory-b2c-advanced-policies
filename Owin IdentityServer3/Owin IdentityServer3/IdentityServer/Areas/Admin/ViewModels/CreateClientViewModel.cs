using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Areas.Admin.ViewModels
{
    public class CreateClientViewModel
    {
        [Display(Name = "Tenant name")]
        [RegularExpression("[0-9A-Za-z]*.onmicrosoft.com", ErrorMessage = "The tenant name must be formatted as '<tenant>.onmicrosoft.com'.")]
        [Required(ErrorMessage = "The tenant name is required.")]
        public string TenantName { get; set; }
    }
}
