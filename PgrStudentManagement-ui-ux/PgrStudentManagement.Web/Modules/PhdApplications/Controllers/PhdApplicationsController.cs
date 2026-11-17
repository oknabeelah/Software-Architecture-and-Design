using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Modules.PhdApplications.Models;
using PgrStudentManagement.Web.Modules.PhdApplications.Services;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Controllers;

public sealed class PhdApplicationsController : Controller
{
    private readonly PhdApplicationService _phdApplicationService;

    public PhdApplicationsController(
        PhdApplicationService phdApplicationService)
    {
        _phdApplicationService = phdApplicationService;
    }

    public async Task<IActionResult> Index(
        ApplicationStatus? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        var applications = await _phdApplicationService.GetApplicationsAsync(
            statusFilter,
            cancellationToken);

        ViewData["SelectedStatus"] = statusFilter;

        return View(applications);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        string applicationReference,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(applicationReference))
        {
            return View("NotFound");
        }

        var application =
            await _phdApplicationService.GetApplicationByReferenceAsync(
                applicationReference,
                cancellationToken);

        if (application is null)
        {
            return View("NotFound");
        }

        return View(application);
    }

    // TODO (Lab Part 2 & Part 3):
    // 1. Make Create() async and load eligible supervisors into ViewBag.Supervisors
    //    using await _phdApplicationService.GetEligibleMainSupervisorsAsync(cancellationToken).
    // 2. Return a CreatePhdApplicationViewModel instead of the PhdApplication entity.
    // See LabResources/code-snippets.md (Snippet 2.A and Snippet 2.B) for the complete code.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new PhdApplication
        {
            SubmittedDate = DateTime.UtcNow.Date,
            ProposedStartDate = new DateTime(DateTime.UtcNow.Year + 1, 10, 1)
        });
    }

    // TODO (Lab Part 2 & Part 6):
    // 1. Add [ValidateAntiForgeryToken].
    // 2. Bind to CreatePhdApplicationViewModel instead of PhdApplication.
    // 3. Auto-generate ApplicationReference using
    //    await _phdApplicationService.GenerateNextApplicationReferenceAsync(cancellationToken).
    // 4. Set TempData["SuccessMessage"] before RedirectToAction(nameof(Details), ...) so the user
    //    sees confirmation feedback on the Details page (Post-Redirect-Get pattern).
    // See LabResources/code-snippets.md (Snippet 2.B and Snippet 2.C) for the complete code.
    [HttpPost]
    public async Task<IActionResult> Create(
        PhdApplication application,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(application);
        }

        var outcome = await _phdApplicationService.CreateApplicationAsync(
            application,
            cancellationToken);

        switch (outcome)
        {
            case ApplicationOutcome.Created:
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        applicationReference = application.ApplicationReference
                    });

            case ApplicationOutcome.DuplicateReference:
                ModelState.AddModelError(
                    nameof(PhdApplication.ApplicationReference),
                    "An application with this reference already exists.");
                return View(application);

            case ApplicationOutcome.SupervisorNotFound:
                ModelState.AddModelError(
                    nameof(PhdApplication.ProposedSupervisorStaffNumber),
                    "The proposed academic supervisor was not found.");
                return View(application);

            case ApplicationOutcome.SupervisorNotEligible:
                ModelState.AddModelError(
                    nameof(PhdApplication.ProposedSupervisorStaffNumber),
                    "The proposed academic supervisor is not eligible to act as a main supervisor.");
                return View(application);

            default:
                ModelState.AddModelError(
                    string.Empty,
                    "The PhD application could not be created.");
                return View(application);
        }
    }

    // TODO (Lab Part 10 - Optional):
    // Pass allowed transitions to the view using:
    // ViewBag.AllowedTransitions = _phdApplicationService.GetAllowedTransitions(application.Status);
    // See LabResources/code-snippets.md (Snippet 10.A).
    [HttpGet]
    public async Task<IActionResult> EditStatus(
        string applicationReference,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(applicationReference))
        {
            return View("NotFound");
        }

        var application =
            await _phdApplicationService.GetApplicationByReferenceAsync(
                applicationReference,
                cancellationToken);

        if (application is null)
        {
            return View("NotFound");
        }

        return View(application);
    }

    [HttpPost]
    public async Task<IActionResult> EditStatus(
        string applicationReference,
        ApplicationStatus status,
        DateTime? decisionDate = null,
        string? enrolledStudentNumber = null,
        CancellationToken cancellationToken = default)
    {
        // Server-side safety net: if the free-text status could
        // not be bound (e.g. "Offer made" instead of "OfferMade"), the
        // parameter falls back to its default. Do not persist that.
        // No error message is shown yet - see lab-instructions.md.
        if (!ModelState.IsValid)
        {
            var unchanged =
                await _phdApplicationService.GetApplicationByReferenceAsync(
                    applicationReference,
                    cancellationToken);

            return unchanged is null
                ? View("NotFound")
                : View(unchanged);
        }

        var outcome = await _phdApplicationService.UpdateApplicationStatusAsync(
            applicationReference,
            status,
            decisionDate,
            enrolledStudentNumber,
            cancellationToken);

        if (outcome == ApplicationOutcome.ApplicationNotFound)
        {
            return View("NotFound");
        }

        if (outcome == ApplicationOutcome.Updated)
        {
            // TODO (Lab Part 10 - Optional): Set TempData["SuccessMessage"] before redirecting so the Details
            // page confirms that the status was updated (see Snippet 10.A in LabResources/code-snippets.md).
            return RedirectToAction(
                nameof(Details),
                new { applicationReference });
        }

        var application =
            await _phdApplicationService.GetApplicationByReferenceAsync(
                applicationReference,
                cancellationToken);

        if (application is null)
        {
            return View("NotFound");
        }

        var errorMessage = outcome switch
        {
            ApplicationOutcome.InvalidStatusTransition =>
                "This application status cannot be changed once finalized.",
            ApplicationOutcome.StudentNotFound =>
                "A valid enrolled student number is required when marking an application as Enrolled.",
            _ => "The application status could not be updated."
        };

        ModelState.AddModelError(string.Empty, errorMessage);

        return View(application);
    }
}
