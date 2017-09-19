using System.ComponentModel.DataAnnotations;

namespace WingTipToysWebApplication.ViewModels.User
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Id { get; set; }

        [Display(Name = "New password")]
        [Required]
        public string NewPassword { get; set; }
    }
}
