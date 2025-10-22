using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Services;
using ContractMonthlyClaimSystem.Models;
// avoid collision with System.Security.Claims.Claim
using ClaimModel = ContractMonthlyClaimSystem.Models.Claim;

namespace ContractMonthlyClaimSystem.Controllers;

[Authorize(Roles = "Lecturer")]
public class ClaimsController : Controller
{
    private readonly IClaimStore _store;
    private readonly IWebHostEnvironment _env;
    private readonly IFileGuard _guard;

    public ClaimsController(IClaimStore store, IWebHostEnvironment env, IFileGuard guard)
    {
        _store = store;
        _env = env;
        _guard = guard;
    }

    [HttpGet]
    public IActionResult Create() => View(new ClaimModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClaimModel model, List<IFormFile>? files)
    {
        if (!ModelState.IsValid) return View(model);

        // who is submitting
        model.LecturerEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        // handle uploads
        if (files is { Count: > 0 })
        {
            var uploadRoot = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadRoot);

            foreach (var f in files)
            {
                if (!_guard.IsAllowed(f.FileName, f.Length, out var err))
                {
                    ModelState.AddModelError("", $"{f.FileName}: {err}");
                    return View(model);
                }

                var unique = $"{Guid.NewGuid():N}{Path.GetExtension(f.FileName)}";
                var savePath = Path.Combine(uploadRoot, unique);

                using (var fs = System.IO.File.Create(savePath))
                    await f.CopyToAsync(fs);

                model.Uploads.Add(new UploadMeta
                {
                    FileName = f.FileName,
                    SavedAs = unique,
                    Size = f.Length
                });
                model.Status = ClaimStatus.Pending;
                await _store.AddAsync(model);

            }
        }

        model.Status = ClaimStatus.Pending;
        await _store.AddAsync(model);

        TempData["ok"] = "Claim submitted.";
        return RedirectToAction(nameof(Thanks), new { id = model.Id });
    }

    public async Task<IActionResult> Thanks(Guid id)
    {
        var claim = await _store.GetAsync(id);
        return View(claim);
    }
}
