using System.ComponentModel.DataAnnotations;

namespace WingTipToysWebApplication.ViewModels.User
{
    public class CreateViewModel
    {
        [Display(Name = "User name")]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Display name")]
        [Required]
        public string DisplayName { get; set; }

        [Display(Name = "Player tag")]
        public string PlayerTag { get; set; }
    }
}
