# Lab Companion: Step-by-Step Code Snippets & Heavy Hints

This companion file contains ready-to-adapt code snippets for every part of [`lab1-instructions.md`](./lab1-instructions.md) (and supports the improvements you will test in [`lab2-instructions.md`](./lab2-instructions.md)). Each section matches the corresponding **Part** in the Lab 1 instructions and tells you **which file to edit** and **what to replace**.

---

## Table of Contents

- [Part 1 – Quick Wins: Colour Contrast & Semantic `<label>` Elements](#part-1--quick-wins-colour-contrast--semantic-label-elements)
  - [Snippet 1.A: Fix Primary Button Colour Contrast in `wwwroot/css/site.css`](#snippet-1a-fix-primary-button-colour-contrast-in-wwwrootcsssitecss)
  - [Snippet 1.B: Replace `<span>` Prompts with Semantic `<label asp-for="...">` Elements in `Create.cshtml`](#snippet-1b-replace-span-prompts-with-semantic-label-asp-for-elements-in-createcshtml)
- [Part 2 – View Model, Controller Binding & `TempData` User Feedback](#part-2--view-model-controller-binding--tempdata-user-feedback)
  - [Snippet 2.A: Create `CreatePhdApplicationViewModel.cs`](#snippet-2a-create-createphdapplicationviewmodelcs)
  - [Snippet 2.B: Update `PhdApplicationsController.Create` (GET & POST)](#snippet-2b-update-phdapplicationscontrollercreate-get--post)
  - [Snippet 2.C: Display `TempData["SuccessMessage"]` Banner in `Details.cshtml`](#snippet-2c-display-tempdatasuccessmessage-banner-in-detailscshtml)
- [Part 3 – Appropriate HTML Input Types & Controls](#part-3--appropriate-html-input-types--controls)
  - [Snippet 3.A: Personal Details & Email (`type="email"`, `autocomplete`)](#snippet-3a-personal-details--email-typeemail-autocomplete)
  - [Snippet 3.B: Programme `<select>` & Mode of Study Radio Buttons (`<fieldset>`)](#snippet-3b-programme-select--mode-of-study-radio-buttons-fieldset)
  - [Snippet 3.C: Funding Source Radio Buttons from Enum](#snippet-3c-funding-source-radio-buttons-from-enum)
  - [Snippet 3.D: Proposed Supervisor `<select>` Dropdown](#snippet-3d-proposed-supervisor-select-dropdown)
- [Part 4 – Form Layout, Visual Section Grouping & Proportionate Input Sizing](#part-4--form-layout-visual-section-grouping--proportionate-input-sizing)
  - [Snippet 4.A: Form Layout, Section Card & Input Width Classes in `wwwroot/css/site.css`](#snippet-4a-form-layout-section-card--input-width-classes-in-wwwrootcsssitecss)
  - [Snippet 4.B: Structured Form Layout with Section Cards & Sized Inputs in `Create.cshtml`](#snippet-4b-structured-form-layout-with-section-cards--sized-inputs-in-createcshtml)
- [Part 5 – Labels, Hints (`aria-describedby`) & Live Character Counter](#part-5--labels-hints-aria-describedby--live-character-counter)
  - [Snippet 5.A: Textarea with Hint Text Linked via `aria-describedby`](#snippet-5a-textarea-with-hint-text-linked-via-aria-describedby)
  - [Snippet 5.B: Accessible Live Character Counter (`aria-live="polite"`)](#snippet-5b-accessible-live-character-counter-aria-livepolite)
- [Part 6 – Server-Side Validation Attributes](#part-6--server-side-validation-attributes)
  - [Snippet 6.A: `CreatePhdApplicationViewModel.cs` with Validation & Display Attributes](#snippet-6a-createphdapplicationviewmodelcs-with-validation--display-attributes)
  - [Snippet 6.B: Adding Display Names to `FundingType.cs`](#snippet-6b-adding-display-names-to-fundingtypecs)
- [Part 7 – Accessible Error Summary & Inline Error Messages](#part-7--accessible-error-summary--inline-error-messages)
  - [Snippet 7.A: Focusable Error Summary with Anchor Links to Fields](#snippet-7a-focusable-error-summary-with-anchor-links-to-fields)
  - [Snippet 7.B: Inline Error Message with `aria-invalid` and `aria-describedby`](#snippet-7b-inline-error-message-with-aria-invalid-and-aria-describedby)
- [Part 8 – Client-Side Validation (Native HTML5 Constraints)](#part-8--client-side-validation-native-html5-constraints)
  - [Snippet 8.A: Adding HTML5 Validation Attributes in `Create.cshtml`](#snippet-8a-adding-html5-validation-attributes-in-createcshtml)
- [Part 9 – Improving `Index.cshtml` (Table Accessibility & Filter)](#part-9--improving-indexcshtml-table-accessibility--filter)
  - [Snippet 9.A: Accessible Table with `<caption>`, `scope="col"`, Filter, and Context Links](#snippet-9a-accessible-table-with-caption-scopecol-filter-and-context-links)
- [Part 10 (Optional) – Improving `EditStatus.cshtml` (Admissions Decision Form)](#part-10-optional--improving-editstatuscshtml-admissions-decision-form)
  - [Snippet 10.A: Controller & View for Allowed Status Transitions](#snippet-10a-controller--view-for-allowed-status-transitions)

---

## Part 1 – Quick Wins: Colour Contrast & Semantic `<label>` Elements

### Snippet 1.A: Fix Primary Button Colour Contrast in `wwwroot/css/site.css`

**File to edit:** `PgrStudentManagement.Web/wwwroot/css/site.css`

In the starter stylesheet, `--btn-primary-bg` is set to `#7cb8eb` (a light sky blue) paired with `--btn-primary-text: #ffffff` (white). That combination yields a contrast ratio of only **`2.12:1`**, failing **WCAG 2.2 Level AA Criterion 1.4.3 (Contrast Minimum)**, which requires at least **`4.5:1`** for normal text.

Update the primary button CSS custom properties inside `:root` (around line 36 of `wwwroot/css/site.css`) to use `#0f4c81` (**`8.86:1`** contrast ratio against `#ffffff`, passing both WCAG AA and AAA):

```css
  /* Primary Button: Main form submissions & primary workflows */
  --btn-primary-bg: #0f4c81;
  --btn-primary-hover: #09355c;
  --btn-primary-border: #0f4c81;
  --btn-primary-text: #ffffff;
```

### Snippet 1.B: Replace `<span>` Prompts with Semantic `<label asp-for="...">` Elements in `Create.cshtml`

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

In the starter `Create.cshtml`, each field prompt is rendered inside a plain `<span class="form-label">` element:

```cshtml
<!-- Starter code (inaccessible: no programmatic association with the input) -->
<div class="mb-3">
    <span class="form-label">FirstName</span>
    <input type="text"
           id="FirstName"
           name="FirstName"
           value="@Model.FirstName"
           class="form-control" />
</div>
```

Replace every `<span class="form-label">...</span>` with an ASP.NET Core `<label asp-for="..." class="form-label"></label>` tag helper so the rendered HTML includes `for="<FieldId>"` matching the input's `id`:

```cshtml
<div class="mb-3">
    <label asp-for="ApplicationReference" class="form-label"></label>
    <input type="text"
           id="ApplicationReference"
           name="ApplicationReference"
           value="@Model.ApplicationReference"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="FirstName" class="form-label"></label>
    <input type="text"
           id="FirstName"
           name="FirstName"
           value="@Model.FirstName"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="LastName" class="form-label"></label>
    <input type="text"
           id="LastName"
           name="LastName"
           value="@Model.LastName"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="EmailAddress" class="form-label"></label>
    <input type="text"
           id="EmailAddress"
           name="EmailAddress"
           value="@Model.EmailAddress"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="ProgrammeCode" class="form-label"></label>
    <input type="text"
           id="ProgrammeCode"
           name="ProgrammeCode"
           value="@Model.ProgrammeCode"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="ModeOfStudy" class="form-label"></label>
    <input type="text"
           id="ModeOfStudy"
           name="ModeOfStudy"
           value="@Model.ModeOfStudy"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="ProposedStartDate" class="form-label"></label>
    <input type="text"
           id="ProposedStartDate"
           name="ProposedStartDate"
           value="@Model.ProposedStartDate.ToString("yyyy-MM-dd")"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="ProposedThesisTitle" class="form-label"></label>
    <input type="text"
           id="ProposedThesisTitle"
           name="ProposedThesisTitle"
           value="@Model.ProposedThesisTitle"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="ResearchSummary" class="form-label"></label>
    <input type="text"
           id="ResearchSummary"
           name="ResearchSummary"
           value="@Model.ResearchSummary"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="FundingSource" class="form-label"></label>
    <input type="text"
           id="FundingSource"
           name="FundingSource"
           value="@Model.FundingSource"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="ProposedSupervisorStaffNumber" class="form-label"></label>
    <input type="text"
           id="ProposedSupervisorStaffNumber"
           name="ProposedSupervisorStaffNumber"
           value="@Model.ProposedSupervisorStaffNumber"
           class="form-control" />
</div>

<div class="mb-3">
    <label asp-for="SubmittedDate" class="form-label"></label>
    <input type="text"
           id="SubmittedDate"
           name="SubmittedDate"
           value="@Model.SubmittedDate.ToString("yyyy-MM-dd")"
           class="form-control" />
</div>
```

---

## Part 2 – View Model, Controller Binding & `TempData` User Feedback

### Snippet 2.A: Create `CreatePhdApplicationViewModel.cs`

**File to create:** `PgrStudentManagement.Web/Modules/PhdApplications/Models/CreatePhdApplicationViewModel.cs`

Notice that system-managed fields (`ApplicationReference`, `Status`, `SubmittedDate`, `DecisionDate`, `EnrolledStudentNumber`) are **omitted** so users cannot over-post them and do not have to invent them:

```csharp
using System;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Models;

public class CreatePhdApplicationViewModel
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string ProgrammeCode { get; set; } = string.Empty;

    public string ModeOfStudy { get; set; } = string.Empty;

    public DateTime ProposedStartDate { get; set; } =
        new DateTime(DateTime.UtcNow.Year + 1, 10, 1);

    public string ProposedThesisTitle { get; set; } = string.Empty;

    public string? ResearchSummary { get; set; }

    // Nullable so we can detect when the user hasn't picked a radio option yet
    public FundingType? FundingSource { get; set; }

    public string? ProposedSupervisorStaffNumber { get; set; }
}
```

### Snippet 2.B: Update `PhdApplicationsController.Create` (GET & POST)

**File to edit:** `PgrStudentManagement.Web/Modules/PhdApplications/Controllers/PhdApplicationsController.cs`

> **Hint:** `PhdApplicationService` already includes two helper methods for you:
> - `_phdApplicationService.GetEligibleMainSupervisorsAsync(cancellationToken)`
> - `_phdApplicationService.GenerateNextApplicationReferenceAsync(cancellationToken)`

Replace the `[HttpGet] Create` and `[HttpPost] Create` methods with:

```csharp
[HttpGet]
public async Task<IActionResult> Create(
    CancellationToken cancellationToken = default)
{
    ViewBag.Supervisors =
        await _phdApplicationService.GetEligibleMainSupervisorsAsync(
            cancellationToken);

    return View(new CreatePhdApplicationViewModel());
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(
    CreatePhdApplicationViewModel model,
    CancellationToken cancellationToken = default)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Supervisors =
            await _phdApplicationService.GetEligibleMainSupervisorsAsync(
                cancellationToken);
        return View(model);
    }

    var reference =
        await _phdApplicationService.GenerateNextApplicationReferenceAsync(
            cancellationToken);

    var application = new PhdApplication
    {
        ApplicationReference = reference,
        FirstName = model.FirstName.Trim(),
        LastName = model.LastName.Trim(),
        EmailAddress = model.EmailAddress.Trim(),
        ProgrammeCode = model.ProgrammeCode.Trim(),
        ModeOfStudy = model.ModeOfStudy.Trim(),
        ProposedStartDate = model.ProposedStartDate,
        ProposedThesisTitle = model.ProposedThesisTitle.Trim(),
        ResearchSummary = model.ResearchSummary?.Trim(),
        FundingSource = model.FundingSource!.Value,
        ProposedSupervisorStaffNumber =
            string.IsNullOrWhiteSpace(model.ProposedSupervisorStaffNumber)
                ? null
                : model.ProposedSupervisorStaffNumber.Trim(),
        Status = ApplicationStatus.Submitted,
        SubmittedDate = DateTime.UtcNow.Date
    };

    var outcome = await _phdApplicationService.CreateApplicationAsync(
        application,
        cancellationToken);

    switch (outcome)
    {
        case ApplicationOutcome.Created:
            TempData["SuccessMessage"] =
                $"Application {application.ApplicationReference} submitted successfully.";
            return RedirectToAction(
                nameof(Details),
                new { applicationReference = application.ApplicationReference });

        case ApplicationOutcome.SupervisorNotFound:
            ModelState.AddModelError(
                nameof(model.ProposedSupervisorStaffNumber),
                "The selected academic supervisor was not found.");
            break;

        case ApplicationOutcome.SupervisorNotEligible:
            ModelState.AddModelError(
                nameof(model.ProposedSupervisorStaffNumber),
                "The selected supervisor is not eligible to act as a main supervisor.");
            break;

        default:
            ModelState.AddModelError(
                string.Empty,
                "The PhD application could not be submitted.");
            break;
    }

    ViewBag.Supervisors =
        await _phdApplicationService.GetEligibleMainSupervisorsAsync(
            cancellationToken);
    return View(model);
}
```

Remember to also change line 1 of `Views/PhdApplications/Create.cshtml` to:
```cshtml
@model PgrStudentManagement.Web.Modules.PhdApplications.Models.CreatePhdApplicationViewModel
```
And delete the `ApplicationReference` and `SubmittedDate` `<div class="mb-3">` blocks from `Create.cshtml`!

### Snippet 2.C: Display `TempData["SuccessMessage"]` Banner in `Details.cshtml`

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Details.cshtml`

When `Create (POST)` or `EditStatus (POST)` succeeds, the controller calls `RedirectToAction(nameof(Details), ...)` (the **Post-Redirect-Get** pattern). Because a redirect triggers a second HTTP request, `ViewData` and `ViewBag` would be cleared before `Details.cshtml` renders. Storing the confirmation text in `TempData["SuccessMessage"]` preserves it across the single redirect and automatically removes it once read so refreshing the page does not repeat the alert.

Place this block right below `<h1>PhD Application Details</h1>` in `Views/PhdApplications/Details.cshtml`:

```cshtml
@if (TempData["SuccessMessage"] is string successMessage)
{
    <div class="alert alert-success" role="status">
        @successMessage
    </div>
}
```

> **Accessibility tip (WCAG 4.1.3 Status Messages):** Using `role="status"` creates an implicit `aria-live="polite"` region so screen readers announce the confirmation message (including the newly generated application reference number) without moving keyboard focus away from the page content.

---

## Part 3 – Appropriate HTML Input Types & Controls

### Snippet 3.A: Personal Details & Email (`type="email"`, `autocomplete`)

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

```cshtml
<div class="mb-3">
    <label asp-for="FirstName" class="form-label"></label>
    <input asp-for="FirstName"
           class="form-control"
           autocomplete="given-name"
           spellcheck="false" />
    <span asp-validation-for="FirstName" class="text-danger"></span>
</div>

<div class="mb-3">
    <label asp-for="LastName" class="form-label"></label>
    <input asp-for="LastName"
           class="form-control"
           autocomplete="family-name"
           spellcheck="false" />
    <span asp-validation-for="LastName" class="text-danger"></span>
</div>

<div class="mb-3">
    <label asp-for="EmailAddress" class="form-label"></label>
    <div id="EmailAddress-hint" class="form-text">
        We will use this email address to send your application reference and updates.
    </div>
    <input asp-for="EmailAddress"
           type="email"
           class="form-control"
           autocomplete="email"
           spellcheck="false"
           aria-describedby="EmailAddress-hint" />
    <span asp-validation-for="EmailAddress" class="text-danger"></span>
</div>
```

### Snippet 3.B: Programme `<select>` & Mode of Study Radio Buttons (`<fieldset>`)

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

```cshtml
<div class="mb-3">
    <label asp-for="ProgrammeCode" class="form-label"></label>
    <select asp-for="ProgrammeCode" class="form-select">
        <option value="">Select a programme…</option>
        <option value="PHD-COMP">PhD Computing (PHD-COMP)</option>
        <option value="PHD-DESIGN">PhD Design (PHD-DESIGN)</option>
        <option value="MPHIL-COMP">MPhil Computing (MPHIL-COMP)</option>
    </select>
    <span asp-validation-for="ProgrammeCode" class="text-danger"></span>
</div>

<fieldset class="mb-3">
    <legend class="form-label">Mode of study</legend>

    <div class="form-check">
        <input class="form-check-input"
               type="radio"
               asp-for="ModeOfStudy"
               id="ModeOfStudy-FullTime"
               value="Full-time" />
        <label class="form-check-label" for="ModeOfStudy-FullTime">
            Full-time (typically 3 years)
        </label>
    </div>

    <div class="form-check">
        <input class="form-check-input"
               type="radio"
               asp-for="ModeOfStudy"
               id="ModeOfStudy-PartTime"
               value="Part-time" />
        <label class="form-check-label" for="ModeOfStudy-PartTime">
            Part-time (typically 6 years)
        </label>
    </div>

    <span asp-validation-for="ModeOfStudy" class="text-danger"></span>
</fieldset>

<div class="mb-3">
    <label asp-for="ProposedStartDate" class="form-label"></label>
    <div id="ProposedStartDate-hint" class="form-text">
        For example, 01/10/2027. Most cohorts begin in October or January.
    </div>
    <input asp-for="ProposedStartDate"
           type="date"
           class="form-control"
           aria-describedby="ProposedStartDate-hint" />
    <span asp-validation-for="ProposedStartDate" class="text-danger"></span>
</div>
```

### Snippet 3.C: Funding Source Radio Buttons from Enum

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

```cshtml
<fieldset class="mb-3">
    <legend class="form-label">How will your PhD studies be funded?</legend>

    @foreach (var item in Html.GetEnumSelectList<FundingType>())
    {
        var radioId = $"FundingSource-{item.Value}";
        <div class="form-check">
            <input class="form-check-input"
                   type="radio"
                   name="FundingSource"
                   id="@radioId"
                   value="@item.Value"
                   checked="@(Model.FundingSource?.ToString() == item.Text || ((int?)Model.FundingSource)?.ToString() == item.Value)" />
            <label class="form-check-label" for="@radioId">
                @item.Text
            </label>
        </div>
    }

    <span asp-validation-for="FundingSource" class="text-danger"></span>
</fieldset>
```

### Snippet 3.D: Proposed Supervisor `<select>` Dropdown

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

```cshtml
@{
    var supervisors = ViewBag.Supervisors
        as IReadOnlyList<PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts.SupervisorSummary>
        ?? Array.Empty<PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts.SupervisorSummary>();
}

<div class="mb-3">
    <label asp-for="ProposedSupervisorStaffNumber" class="form-label">
        Proposed main supervisor (optional)
    </label>
    <div id="ProposedSupervisor-hint" class="form-text">
        Only academic staff eligible to act as a Main Supervisor are listed. Leave blank if you have not yet identified a supervisor.
    </div>
    <select asp-for="ProposedSupervisorStaffNumber"
            class="form-select"
            aria-describedby="ProposedSupervisor-hint">
        <option value="">I have not identified a supervisor yet</option>
        @foreach (var supervisor in supervisors)
        {
            <option value="@supervisor.StaffNumber">
                @supervisor.DisplayName (@supervisor.StaffNumber)
            </option>
        }
    </select>
    <span asp-validation-for="ProposedSupervisorStaffNumber" class="text-danger"></span>
</div>
```

---

## Part 4 – Form Layout, Visual Section Grouping & Proportionate Input Sizing

### Snippet 4.A: Form Layout, Section Card & Input Width Classes in `wwwroot/css/site.css`

**File to inspect / customise:** `PgrStudentManagement.Web/wwwroot/css/site.css` (inside `@layer components`)

These classes constrain the overall form container to a comfortable reading measure (`48rem` / `768px`), visually separate the 4 logical sections into `.form-section` cards, pair related fields side-by-side in `.form-grid-2` (collapsing automatically on narrow screens or at 400% zoom for **WCAG 1.4.10 Reflow**), and size inputs proportionately (`.input-width-sm`, `.input-width-md`):

```css
  /* ------------------------------------------------------------------------
     Form Layout, Visual Grouping & Input Sizing (Lab Part 4 - Snippet 4.A)
     ------------------------------------------------------------------------ */
  .form-container {
    max-width: 48rem;
  }

  .form-section {
    padding: 1.25rem 1.5rem;
    margin-block-end: 1.5rem;
    background-color: var(--theme-surface-subtle);
    border: 1px solid var(--theme-border);
    border-radius: var(--radius-md);
  }

  .form-section > h2 {
    margin-block-start: 0;
    margin-block-end: 1rem;
    padding-block-end: 0.5rem;
    font-size: 1.15rem;
    border-block-end: 1px solid var(--theme-border);
  }

  .form-grid-2 {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(15rem, 1fr));
    gap: 0 1.25rem;
  }

  .input-width-sm {
    max-width: 14rem;
  }

  .input-width-md {
    max-width: 28rem;
  }

  .form-actions {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: 0.75rem;
    margin-block-start: 1.5rem;
    padding-block-start: 1.25rem;
    border-block-start: 1px solid var(--theme-border);
  }
```

### Snippet 4.B: Structured Form Layout with Section Cards & Sized Inputs in `Create.cshtml`

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

Wrap the form in `<div class="form-container">`, group the controls into four `<section class="form-section">` cards, pair `FirstName` and `LastName` inside `<div class="form-grid-2">`, apply `input-width-sm` / `input-width-md`, and wrap the buttons in `<div class="form-actions">`:

```cshtml
<div class="form-container">
    <form asp-action="Create" method="post">

        <section class="form-section">
            <h2>1. About you</h2>

            <div class="form-grid-2">
                <div class="mb-3">
                    <label asp-for="FirstName" class="form-label"></label>
                    <input asp-for="FirstName"
                           class="form-control"
                           autocomplete="given-name"
                           spellcheck="false" />
                    <span asp-validation-for="FirstName" class="text-danger"></span>
                </div>

                <div class="mb-3">
                    <label asp-for="LastName" class="form-label"></label>
                    <input asp-for="LastName"
                           class="form-control"
                           autocomplete="family-name"
                           spellcheck="false" />
                    <span asp-validation-for="LastName" class="text-danger"></span>
                </div>
            </div>

            <div class="mb-3">
                <label asp-for="EmailAddress" class="form-label"></label>
                <div id="EmailAddress-hint" class="form-text">
                    We will use this email address to send your application reference and updates.
                </div>
                <input asp-for="EmailAddress"
                       type="email"
                       class="form-control input-width-md"
                       autocomplete="email"
                       spellcheck="false"
                       aria-describedby="EmailAddress-hint" />
                <span asp-validation-for="EmailAddress" class="text-danger"></span>
            </div>
        </section>

        <section class="form-section">
            <h2>2. Programme and mode of study</h2>

            <div class="mb-3">
                <label asp-for="ProgrammeCode" class="form-label"></label>
                <select asp-for="ProgrammeCode" class="form-select input-width-md">
                    <option value="">Select a programme…</option>
                    <option value="PHD-COMP">PhD Computing (PHD-COMP)</option>
                    <option value="PHD-DESIGN">PhD Design (PHD-DESIGN)</option>
                    <option value="MPHIL-COMP">MPhil Computing (MPHIL-COMP)</option>
                </select>
                <span asp-validation-for="ProgrammeCode" class="text-danger"></span>
            </div>

            <fieldset class="mb-3">
                <legend class="form-label">Mode of study</legend>

                <div class="form-check">
                    <input class="form-check-input"
                           type="radio"
                           asp-for="ModeOfStudy"
                           id="ModeOfStudy-FullTime"
                           value="Full-time" />
                    <label class="form-check-label" for="ModeOfStudy-FullTime">
                        Full-time (typically 3 years)
                    </label>
                </div>

                <div class="form-check">
                    <input class="form-check-input"
                           type="radio"
                           asp-for="ModeOfStudy"
                           id="ModeOfStudy-PartTime"
                           value="Part-time" />
                    <label class="form-check-label" for="ModeOfStudy-PartTime">
                        Part-time (typically 6 years)
                    </label>
                </div>

                <span asp-validation-for="ModeOfStudy" class="text-danger"></span>
            </fieldset>

            <div class="mb-3">
                <label asp-for="ProposedStartDate" class="form-label"></label>
                <div id="ProposedStartDate-hint" class="form-text">
                    For example, 01/10/2027. Most cohorts begin in October or January.
                </div>
                <input asp-for="ProposedStartDate"
                       type="date"
                       class="form-control input-width-sm"
                       aria-describedby="ProposedStartDate-hint" />
                <span asp-validation-for="ProposedStartDate" class="text-danger"></span>
            </div>
        </section>

        <section class="form-section">
            <h2>3. Research proposal</h2>

            <div class="mb-3">
                <label asp-for="ProposedThesisTitle" class="form-label"></label>
                <div id="ProposedThesisTitle-hint" class="form-text">
                    Provide a working title for your proposed research project (up to 300 characters).
                </div>
                <input asp-for="ProposedThesisTitle"
                       class="form-control"
                       aria-describedby="ProposedThesisTitle-hint" />
                <span asp-validation-for="ProposedThesisTitle" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="ResearchSummary" class="form-label">
                    Research summary (optional)
                </label>
                <div id="ResearchSummary-hint" class="form-text">
                    Outline your research question, background context, and proposed methodology in up to 2,000 characters.
                </div>
                <textarea asp-for="ResearchSummary"
                          rows="6"
                          class="form-control"
                          data-maxlength="2000"
                          aria-describedby="ResearchSummary-hint ResearchSummary-count"></textarea>
                <div id="ResearchSummary-count"
                     class="form-text"
                     aria-live="polite">
                    You have 2,000 characters remaining
                </div>
                <span asp-validation-for="ResearchSummary" class="text-danger"></span>
            </div>
        </section>

        <section class="form-section">
            <h2>4. Funding and supervision</h2>

            <fieldset class="mb-3">
                <legend class="form-label">How will your PhD studies be funded?</legend>

                @foreach (var item in Html.GetEnumSelectList<FundingType>())
                {
                    var radioId = $"FundingSource-{item.Value}";
                    <div class="form-check">
                        <input class="form-check-input"
                               type="radio"
                               name="FundingSource"
                               id="@radioId"
                               value="@item.Value"
                               checked="@(Model.FundingSource?.ToString() == item.Text || ((int?)Model.FundingSource)?.ToString() == item.Value)" />
                        <label class="form-check-label" for="@radioId">
                            @item.Text
                        </label>
                    </div>
                }

                <span asp-validation-for="FundingSource" class="text-danger"></span>
            </fieldset>

            <div class="mb-3">
                <label asp-for="ProposedSupervisorStaffNumber" class="form-label">
                    Proposed main supervisor (optional)
                </label>
                <div id="ProposedSupervisor-hint" class="form-text">
                    Only academic staff eligible to act as a Main Supervisor are listed. Leave blank if you have not yet identified a supervisor.
                </div>
                <select asp-for="ProposedSupervisorStaffNumber"
                        class="form-select input-width-md"
                        aria-describedby="ProposedSupervisor-hint">
                    <option value="">I have not identified a supervisor yet</option>
                    @foreach (var supervisor in supervisors)
                    {
                        <option value="@supervisor.StaffNumber">
                            @supervisor.DisplayName (@supervisor.StaffNumber)
                        </option>
                    }
                </select>
                <span asp-validation-for="ProposedSupervisorStaffNumber" class="text-danger"></span>
            </div>
        </section>

        <div class="form-actions">
            <button type="submit" class="btn btn-primary">
                Submit application
            </button>
            <a asp-action="Index" class="btn btn-secondary">
                Cancel
            </a>
        </div>
    </form>
</div>
```

---

## Part 5 – Labels, Hints (`aria-describedby`) & Live Character Counter

### Snippet 5.A: Textarea with Hint Text Linked via `aria-describedby`

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml`

```cshtml
<div class="mb-3">
    <label asp-for="ProposedThesisTitle" class="form-label"></label>
    <div id="ProposedThesisTitle-hint" class="form-text">
        Provide a working title for your proposed research project (up to 300 characters).
    </div>
    <input asp-for="ProposedThesisTitle"
           class="form-control"
           aria-describedby="ProposedThesisTitle-hint" />
    <span asp-validation-for="ProposedThesisTitle" class="text-danger"></span>
</div>

<div class="mb-3">
    <label asp-for="ResearchSummary" class="form-label">
        Research summary (optional)
    </label>
    <div id="ResearchSummary-hint" class="form-text">
        Outline your research question, background context, and proposed methodology in up to 2,000 characters.
    </div>
    <textarea asp-for="ResearchSummary"
              rows="6"
              class="form-control"
              data-maxlength="2000"
              aria-describedby="ResearchSummary-hint ResearchSummary-count"></textarea>
    <div id="ResearchSummary-count"
         class="form-text"
         aria-live="polite">
        You have 2,000 characters remaining
    </div>
    <span asp-validation-for="ResearchSummary" class="text-danger"></span>
</div>
```

### Snippet 5.B: Accessible Live Character Counter (`aria-live="polite"`)

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml` (inside `@section Scripts`, or handled automatically by `wwwroot/js/site.js` when `data-maxlength="2000"` is present on `#ResearchSummary`)

```cshtml
@section Scripts {
    <script>
        (function () {
            const textarea = document.getElementById('ResearchSummary');
            const counter = document.getElementById('ResearchSummary-count');
            if (!textarea || !counter) return;

            const max = parseInt(textarea.getAttribute('data-maxlength') || '2000', 10);

            function updateCount() {
                const remaining = max - textarea.value.length;
                if (remaining >= 0) {
                    counter.textContent = `You have ${remaining.toLocaleString()} characters remaining`;
                    counter.classList.remove('text-danger');
                } else {
                    counter.textContent = `You have ${Math.abs(remaining).toLocaleString()} characters too many`;
                    counter.classList.add('text-danger');
                }
            }

            textarea.addEventListener('input', updateCount);
            updateCount();
        })();
    </script>
}
```

---

## Part 6 – Server-Side Validation Attributes

### Snippet 6.A: `CreatePhdApplicationViewModel.cs` with Validation & Display Attributes

**File to edit:** `PgrStudentManagement.Web/Modules/PhdApplications/Models/CreatePhdApplicationViewModel.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Models;

public class CreatePhdApplicationViewModel
{
    [Display(Name = "First name")]
    [Required(ErrorMessage = "Enter your first name")]
    [StringLength(100, ErrorMessage = "First name must be 100 characters or fewer")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last name")]
    [Required(ErrorMessage = "Enter your last name")]
    [StringLength(100, ErrorMessage = "Last name must be 100 characters or fewer")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Email address")]
    [Required(ErrorMessage = "Enter your email address")]
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [StringLength(255, ErrorMessage = "Email address must be 255 characters or fewer")]
    public string EmailAddress { get; set; } = string.Empty;

    [Display(Name = "Programme")]
    [Required(ErrorMessage = "Select the programme you are applying for")]
    public string ProgrammeCode { get; set; } = string.Empty;

    [Display(Name = "Mode of study")]
    [Required(ErrorMessage = "Select whether you will study full-time or part-time")]
    public string ModeOfStudy { get; set; } = string.Empty;

    [Display(Name = "Proposed start date")]
    [Required(ErrorMessage = "Enter your proposed start date")]
    [DataType(DataType.Date)]
    public DateTime ProposedStartDate { get; set; } =
        new DateTime(DateTime.UtcNow.Year + 1, 10, 1);

    [Display(Name = "Proposed thesis title")]
    [Required(ErrorMessage = "Enter a working title for your proposed thesis")]
    [StringLength(300, ErrorMessage = "Proposed thesis title must be 300 characters or fewer")]
    public string ProposedThesisTitle { get; set; } = string.Empty;

    [Display(Name = "Research summary (optional)")]
    [StringLength(2000, ErrorMessage = "Research summary must be 2,000 characters or fewer")]
    public string? ResearchSummary { get; set; }

    [Display(Name = "Funding source")]
    [Required(ErrorMessage = "Select how your PhD studies will be funded")]
    public FundingType? FundingSource { get; set; }

    [Display(Name = "Proposed main supervisor (optional)")]
    public string? ProposedSupervisorStaffNumber { get; set; }
}
```

### Snippet 6.B: Adding Display Names to `FundingType.cs`

**File to edit:** `PgrStudentManagement.Web/Modules/PhdApplications/Models/FundingType.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Models;

public enum FundingType
{
    [Display(Name = "Self-funded")]
    SelfFunded,

    [Display(Name = "University studentship")]
    Studentship,

    [Display(Name = "UK Research Council (UKRI)")]
    ResearchCouncil,

    [Display(Name = "External or industry sponsor")]
    ExternalSponsor
}
```

---

## Part 7 – Accessible Error Summary & Inline Error Messages

### Snippet 7.A: Focusable Error Summary with Anchor Links to Fields

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Create.cshtml` (place right above `<form>`)

```cshtml
@{
    var hasErrors = !ViewData.ModelState.IsValid;
    ViewData["Title"] = hasErrors
        ? "Error: Create PhD Application"
        : "Create PhD Application";
}

@if (hasErrors)
{
    <div id="error-summary"
         class="alert alert-danger"
         role="alert"
         aria-labelledby="error-summary-title"
         tabindex="-1">
        <h2 id="error-summary-title" class="h5">There is a problem</h2>
        <ul class="mb-0">
            @foreach (var entry in ViewData.ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    <li>
                        @if (!string.IsNullOrEmpty(entry.Key))
                        {
                            <a href="#@entry.Key" class="alert-link">@error.ErrorMessage</a>
                        }
                        else
                        {
                            @error.ErrorMessage
                        }
                    </li>
                }
            }
        </ul>
    </div>
    <script>
        document.getElementById('error-summary')?.focus();
    </script>
}
```

### Snippet 7.B: Inline Error Message with `aria-invalid` and `aria-describedby`

When a specific field has a validation error, linking its error `<span>` in `aria-describedby` and setting `aria-invalid="true"` ensures screen readers announce the error immediately when the user focuses the input:

```cshtml
@{
    var emailInvalid = ViewData.ModelState.ContainsKey(nameof(Model.EmailAddress))
        && ViewData.ModelState[nameof(Model.EmailAddress)]!.Errors.Count > 0;
}
<div class="mb-3">
    <label asp-for="EmailAddress" class="form-label"></label>
    <div id="EmailAddress-hint" class="form-text">
        We will use this email address to send your application reference and updates.
    </div>
    <div>
        <span asp-validation-for="EmailAddress"
              id="EmailAddress-error"
              class="text-danger fw-semibold"></span>
    </div>
    <input asp-for="EmailAddress"
           type="email"
           class="form-control input-width-md @(emailInvalid ? "is-invalid" : "")"
           autocomplete="email"
           spellcheck="false"
           aria-invalid="@(emailInvalid ? "true" : "false")"
           aria-describedby="EmailAddress-hint @(emailInvalid ? "EmailAddress-error" : "")" />
</div>
```

---

## Part 8 – Client-Side Validation (Native HTML5 Constraints)

### Snippet 8.A: Adding HTML5 Validation Attributes in `Create.cshtml`

Client-side validation in this application uses native HTML5 constraint attributes only (no jQuery or JavaScript validation libraries):

1. Add `required` and `maxlength` attributes directly to the required controls in `Create.cshtml`:
   - `FirstName`: `required maxlength="100"`
   - `LastName`: `required maxlength="100"`
   - `EmailAddress`: `type="email" required maxlength="255"`
   - `ProgrammeCode` (`<select>`): `required`
   - `ModeOfStudy` (radio buttons): `required`
   - `ProposedStartDate`: `type="date" required`
   - `ProposedThesisTitle`: `required maxlength="300"`
   - `ResearchSummary` (`<textarea>`): `maxlength="2000"` (leave `required` off because this field is optional)
   - `FundingSource` (radio buttons): `required`

```cshtml
<input asp-for="FirstName"
       class="form-control"
       autocomplete="given-name"
       spellcheck="false"
       required
       maxlength="100" />
```

---

## Part 9 – Improving `Index.cshtml` (Table Accessibility & Filter)

### Snippet 9.A: Accessible Table with `<caption>`, `scope="col"`, Filter, and Context Links

**File to edit:** `PgrStudentManagement.Web/Views/PhdApplications/Index.cshtml`

```cshtml
@model IReadOnlyList<PgrStudentManagement.Web.Modules.PhdApplications.Models.PhdApplication>

@{
    ViewData["Title"] = "PhD Applications";
    var selectedStatus = ViewData["SelectedStatus"] as ApplicationStatus?;
}

<div class="d-flex justify-content-between align-items-center mb-3">
    <h1>PhD Applications</h1>
    <a asp-action="Create" class="btn btn-primary">Create Application</a>
</div>

<form method="get" asp-action="Index" class="row g-2 align-items-end mb-4">
    <div class="col-auto">
        <label for="statusFilter" class="form-label">Filter by status</label>
        <select id="statusFilter" name="statusFilter" class="form-select">
            <option value="">All statuses</option>
            @foreach (var status in Enum.GetValues<ApplicationStatus>())
            {
                <option value="@status" selected="@(selectedStatus == status)">
                    @status
                </option>
            }
        </select>
    </div>
    <div class="col-auto">
        <button type="submit" class="btn btn-outline-primary">Apply filter</button>
        @if (selectedStatus.HasValue)
        {
            <a asp-action="Index" class="btn btn-link">Clear</a>
        }
    </div>
</form>

@if (Model.Count == 0)
{
    <p class="alert alert-info" role="status">No PhD applications found.</p>
}
else
{
    <table class="table table-striped align-middle">
        <caption class="visually-hidden">List of PhD applications</caption>
        <thead>
            <tr>
                <th scope="col">Reference</th>
                <th scope="col">Applicant</th>
                <th scope="col">Programme</th>
                <th scope="col">Mode of study</th>
                <th scope="col">Funding</th>
                <th scope="col">Status</th>
                <th scope="col">Submitted</th>
                <th scope="col">Actions</th>
            </tr>
        </thead>
        <tbody>
        @foreach (var application in Model)
        {
            <tr>
                <td>@application.ApplicationReference</td>
                <td>@application.FullName</td>
                <td>@application.ProgrammeCode</td>
                <td>@application.ModeOfStudy</td>
                <td>@application.FundingSource</td>
                <td>@application.Status</td>
                <td>@application.SubmittedDate.ToString("yyyy-MM-dd")</td>
                <td>
                    <a asp-action="Details"
                       asp-route-applicationReference="@application.ApplicationReference">
                        Details<span class="visually-hidden"> for @application.ApplicationReference (@application.FullName)</span>
                    </a>
                    |
                    <a asp-action="EditStatus"
                       asp-route-applicationReference="@application.ApplicationReference">
                        Edit status<span class="visually-hidden"> for @application.ApplicationReference (@application.FullName)</span>
                    </a>
                </td>
            </tr>
        }
        </tbody>
    </table>
}
```

---

## Part 10 (Optional) – Improving `EditStatus.cshtml` (Admissions Decision Form)

### Snippet 10.A: Controller & View for Allowed Status Transitions

**1. In `PhdApplicationsController.cs` (`EditStatus` GET & POST):**
Populate `ViewBag.AllowedTransitions` using the helper already provided on `_phdApplicationService`:

```csharp
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

    ViewBag.AllowedTransitions =
        _phdApplicationService.GetAllowedTransitions(application.Status);

    return View(application);
}
```
*(Remember to also set `ViewBag.AllowedTransitions = _phdApplicationService.GetAllowedTransitions(application.Status);` before any `return View(application);` in the `[HttpPost] EditStatus` action, and set `TempData["SuccessMessage"]` before redirecting on success!)*

```csharp
if (outcome == ApplicationOutcome.Updated)
{
    TempData["SuccessMessage"] =
        $"Application {applicationReference} status updated to {status}.";

    return RedirectToAction(
        nameof(Details),
        new { applicationReference });
}
```

**2. In `Views/PhdApplications/EditStatus.cshtml`:**

```cshtml
@model PgrStudentManagement.Web.Modules.PhdApplications.Models.PhdApplication

@{
    ViewData["Title"] = "Update Application Status";
    var allowed = ViewBag.AllowedTransitions as IReadOnlyList<ApplicationStatus>
        ?? Array.Empty<ApplicationStatus>();
}

<h1>Update Application Status</h1>

<p>
    <strong>@Model.ApplicationReference:</strong> @Model.FullName
</p>
<p>
    <strong>Current status:</strong>
    <span class="badge bg-secondary">@Model.Status</span>
</p>

@if (!ViewData.ModelState.IsValid)
{
    <div asp-validation-summary="All"
         class="alert alert-danger"
         role="alert"></div>
}

@if (allowed.Count == 0)
{
    <div class="alert alert-info" role="status">
        This application is in a final state (<strong>@Model.Status</strong>) and its status can no longer be changed.
    </div>
    <a asp-action="Details"
       asp-route-applicationReference="@Model.ApplicationReference"
       class="btn btn-secondary">
        Return to details
    </a>
}
else
{
    <form asp-action="EditStatus" method="post">
        <input type="hidden"
               name="applicationReference"
               value="@Model.ApplicationReference" />

        <fieldset class="mb-3">
            <legend class="form-label">Select new status</legend>

            @foreach (var nextStatus in allowed)
            {
                var id = $"status-{nextStatus}";
                <div class="form-check">
                    <input class="form-check-input"
                           type="radio"
                           name="status"
                           id="@id"
                           value="@nextStatus"
                           checked="@(Model.Status == nextStatus)" />
                    <label class="form-check-label" for="@id">
                        @nextStatus
                    </label>
                </div>
            }
        </fieldset>

        <button type="submit" class="btn btn-primary">
            Save decision
        </button>

        <a asp-action="Details"
           asp-route-applicationReference="@Model.ApplicationReference"
           class="btn btn-secondary">
            Cancel
        </a>
    </form>
}
```
