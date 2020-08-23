using System.ComponentModel.DataAnnotations;
namespace WebAdvert.Web.Models.Accounts
{
  public class ForgotPasswordModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}