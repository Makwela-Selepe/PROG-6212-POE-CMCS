using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ContractMonthlyClaimSystem.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerController : Controller
{
    private readonly IClaimStore _store;

    public ManagerController(IClaimStore store)
    {
        _store = store;
    }

    public async Task<IActionResult> Index()
    {
        var items = (await _store.GetAllAsync())
            .Where(c => c.Status == ClaimStatus.Verified)
            .OrderByDescending(c => c.CreatedUtc)
            .ToList();
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(Guid id)
    {
        var claim = await _store.GetAsync(id);
        if (claim is null) return NotFound();

        claim.Status = ClaimStatus.Approved;
        await _store.UpdateAsync(claim);

        TempData["Success"] = "Claim approved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(Guid id)
    {
        var claim = await _store.GetAsync(id);
        if (claim is null) return NotFound();

        claim.Status = ClaimStatus.Rejected;
        await _store.UpdateAsync(claim);

        TempData["Success"] = "Claim rejected.";
        return RedirectToAction(nameof(Index));
    }
}