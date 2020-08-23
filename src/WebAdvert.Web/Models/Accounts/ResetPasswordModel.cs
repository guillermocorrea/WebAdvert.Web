using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
  public class ResetPasswordModel
  {
    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least six characters long")]
    [Display(Name = "New Password")]
    public string Password { get; set; }
  }
}