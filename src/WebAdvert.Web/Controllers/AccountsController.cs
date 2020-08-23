using System;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
  public class AccountsController : Controller
  {
    private readonly SignInManager<CognitoUser> _signIngManager;
    private readonly UserManager<CognitoUser> _userManager;
    private readonly CognitoUserPool _pool;
    public AccountsController(SignInManager<CognitoUser> signIngManager,
      UserManager<CognitoUser> userManager,
      CognitoUserPool pool)
    {
      _pool = pool;
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
          await user.ResendConfirmationCodeAsync();
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
  }
}