using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers;

public class HomeController : Controller
{
    private readonly IClaimStore _store;

    public HomeController(IClaimStore store)
    {
        _store = store;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            // Redirect based on role
            if (User.IsInRole("Lecturer"))
                return RedirectToAction("MyClaims", "Lecturer");
            if (User.IsInRole("Coordinator"))
                return RedirectToAction("Index", "Coordinator");
            if (User.IsInRole("Manager"))
                return RedirectToAction("Index", "Manager");
        }

        var all = await _store.GetAllAsync();
        ViewBag.Pending = all.Count(c => c.Status == ClaimStatus.Pending);
        ViewBag.Verified = all.Count(c => c.Status == ClaimStatus.Verified);
        ViewBag.Approved = all.Count(c => c.Status == ClaimStatus.Approved);
        ViewBag.Rejected = all.Count(c => c.Status == ClaimStatus.Rejected);
        return View();
    }
}