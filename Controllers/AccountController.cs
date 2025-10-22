using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;

// alias to avoid collision with your model Claim
using SecClaim = System.Security.Claims.Claim;

namespace ContractMonthlyClaimSystem.Controllers;

public class AccountController(IUserStore users) : Controller
{
    // -------- Login --------
    [HttpGet, AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await users.FindByEmailAndRoleAsync(vm.Email, vm.Role);
        if (user is null)
        {
            ModelState.AddModelError("", "Invalid email or role.");
            return View(vm);
        }

        // ✅ Pass email (string), not the AppUser object
        if (!await users.ValidatePasswordAsync(user.Email, vm.Password))
        {
            ModelState.AddModelError("", "Invalid password.");
            return View(vm);
        }

        await SignInAsync(user);

        return user.Role switch
        {
            UserRole.Manager => RedirectToAction("Index", "Manager"),
            UserRole.Coordinator => RedirectToAction("Index", "Coordinator"),
            _ => RedirectToAction("Index", "Home"),
        };

    }

    // -------- Register (Lecturer only) --------
    [HttpGet, AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

   
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        // Make sure the email isn't already used for a lecturer
        var existing = await users.FindByEmailAndRoleAsync(vm.Email, UserRole.Lecturer);
        if (existing != null)
        {
            ModelState.AddModelError("", "This email is already registered.");
            return View(vm);
        }

        // Try create the lecturer
        var newUser = await users.CreateLecturerAsync(vm.Name, vm.Email, vm.Password); // returns AppUser?
        if (newUser is null)
        {
            ModelState.AddModelError("", "Something went wrong while creating the account.");
            return View(vm);
        }

        // Sign in and go home
        await SignInAsync(newUser);
        return RedirectToAction("Index", "Home");
    }


    // -------- Logout / AccessDenied --------
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult Denied() => View();

    // -------- Helper (required to avoid CS1503) --------
    private async Task SignInAsync(AppUser user)
    {
        var claims = new List<SecClaim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,  user.Name ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role,  user.Role.ToString())
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
