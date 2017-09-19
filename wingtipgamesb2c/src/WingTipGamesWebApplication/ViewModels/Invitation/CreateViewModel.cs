using System.ComponentModel.DataAnnotations;

namespace WingTipGamesWebApplication.ViewModels.Invitation
{
    public class CreateViewModel
    {
        [Display(Name = "Email address")]
        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }

        [Display(Name = "Redemption method")]
        [Required]
        public string RedemptionMethod { get; set; }
    }
}
