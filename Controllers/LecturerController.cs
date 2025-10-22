using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContractMonthlyClaimSystem.Services;

namespace ContractMonthlyClaimSystem.Controllers;

[Authorize(Roles = "Lecturer")]
public class LecturerController : Controller
{
    private readonly IClaimStore _claims;

    public LecturerController(IClaimStore claims)
    {
        _claims = claims;
    }

    public async Task<IActionResult> MyClaims()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var all = await _claims.GetAllAsync();
        var mine = all.Where(c => c.LecturerEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                      .OrderByDescending(c => c.CreatedUtc)
                      .ToList();
        return View(mine);
    }
}