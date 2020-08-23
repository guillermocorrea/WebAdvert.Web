using System;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAdvert.Web.Configuration;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
  public class AccountsController : Controller
  {
    private readonly SignInManager<CognitoUser> _signIngManager;
    private readonly UserManager<CognitoUser> _userManager;
    private readonly CognitoUserPool _pool;
    private readonly IOptions<AWSConfiguration> _awsOptions;

    public AccountsController(SignInManager<CognitoUser> signIngManager,
      UserManager<CognitoUser> userManager,
      CognitoUserPool pool,
      IOptions<AWSConfiguration> options)
    {
      _pool = pool;
      _awsOptions = options;
      _userManager = userManager;
      _signIngManager = signIngManager;
    }
    public async Task<IActionResult> Signup()
    {
      var model = new SignupModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupModel model)
    {
      if (ModelState.IsValid)
      {
        var user = _pool.GetUser(model.Email);
        if (user.Status != null)
        {
          ModelState.AddModelError("UserExists", "User already exists");
          return View(model);
        }

        user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
        var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

        if (createdUser.Succeeded)
        {
          return RedirectToAction("Confirm");
        }
        else
        {
          foreach (var error in createdUser.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
        }
      }
      return View();
    }

    public async Task<IActionResult> Confirm()
    {
      var model = new ConfirmModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(ConfirmModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
          ModelState.AddModelError("NotFound", "Wrong login");
          return View(model);
        }

        var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true);
        if (result.Succeeded)
        {
          return RedirectToAction("Index", "Home");
        }
        else
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
          return View(model);
        }
      }

      return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
      return View(new LoginModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await _signIngManager.PasswordSignInAsync(model.Email, model.Password,
          model.RememberMe, false).ConfigureAwait(false);
        if (result.Succeeded)
        {
          return RedirectToAction("Index", "Home");
        }
        else
        {
          ModelState.AddModelError("LoginError", "Wrong login");
        }
      }

      return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ForgotPassword()
    {
      return View(new ForgotPasswordModel());
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
          ModelState.AddModelError("Email", "Email sent");
          return View(model);
        }
        var response = await (_userManager as CognitoUserManager<CognitoUser>).ResetPasswordAsync(user);
        if (response.Succeeded)
        {
          return RedirectToAction("ResetPassword");
        }
        else
        {
          foreach (var error in response.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
        }
      }

      return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword()
    {
      return View(new ResetPasswordModel());
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
          ModelState.AddModelError("Email", "Invalid code or email");
          return View(model);
        }
        var response = await (_userManager as CognitoUserManager<CognitoUser>).ResetPasswordAsync(user, model.Code, model.Password);
        if (response.Succeeded)
        {
          return RedirectToAction("Index", "Home");
        }
        else
        {
          foreach (var error in response.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
        }
      }

      return View(model);
    }
  }
}