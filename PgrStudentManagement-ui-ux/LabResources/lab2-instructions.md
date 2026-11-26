# Lab 2: Usability & Accessibility Testing Across Branches (`main` vs. `ui-ux-improvements`)

## Learning Objectives

In [lab1-instructions.md](./lab1-instructions.md), you created a new Git branch (`ui-ux-improvements`) and redesigned the **PhD Applications** views (`Create.cshtml`, `Details.cshtml`, `Index.cshtml`, and optionally `EditStatus.cshtml`) to improve user experience, semantic HTML structure, validation recovery, and WCAG 2.2 AA accessibility.

In this second lab, you will **measure and verify** the impact of your improvements by testing the untouched **`main` branch** against your **`ui-ux-improvements` branch** using four complementary testing methods:

1. **5-Minute Peer Guerrilla Usability Test** — observing real human friction, completion time, and error rates.
2. **Browser DevTools Inspection** — inspecting the **Accessibility Tree**, running **Lighthouse** audits, and emulating visual impairments and 400% zoom reflow.
3. **Visual Keyboard & Tab-Stop Auditing** — using **Microsoft Accessibility Insights for Web** to map keyboard navigation paths.
4. **Automated Usability & Accessibility Testing with Playwright** — writing repeatable end-to-end tests that interact with your pages using human labels and ARIA roles (`getByLabel`, `getByRole`) and run automated **axe-core** WCAG 2.2 AA scans.

---

## Before You Start: Testing Against `main` and `ui-ux-improvements`

Make sure all of your Lab 1 changes are committed on your `ui-ux-improvements` branch (`git status` should report a clean working tree).

You can test the two branches in either of two ways:

### Workflow A: Switch Branches in Place (Single Server on Port `5277`)

Run your tests first on `main`, then switch to `ui-ux-improvements` and re-run them on the same port (`http://localhost:5277`):

```bash
# 1. Test the untouched baseline on main
git switch main
dotnet run --project PgrStudentManagement.Web --urls "http://localhost:5277"

# 2. Stop the server (Ctrl+C), switch to your improved branch, and run again
git switch ui-ux-improvements
dotnet run --project PgrStudentManagement.Web --urls "http://localhost:5277"
```

### Workflow B (Optional): Run Both Branches Simultaneously with `git worktree`

If you want both `main` (`:5277`) and `ui-ux-improvements` (`:5278`) running at the same time in side-by-side browser tabs without cloning the repository twice, you can check out `main` into a temporary worktree folder:

```bash
# Terminal 1: Start main on port 5277 via a Git worktree
git worktree add ../pgr-main main
dotnet run --project ../pgr-main/PgrStudentManagement.Web --urls "http://localhost:5277"

# Terminal 2: Start ui-ux-improvements on port 5278 in your main working directory
git switch ui-ux-improvements
dotnet run --project PgrStudentManagement.Web --urls "http://localhost:5278"
```
*(When finished with the lab, remove the temporary worktree with `git worktree remove ../pgr-main`.)*

---

## Part 1: 5-Minute Peer "Guerrilla Usability Test"

Automated tools can verify technical accessibility rules, but they cannot tell you whether a human understands what to type or feels overwhelmed by a wall of 12 ungrouped text inputs. Before running any tools, conduct a quick 5-minute observational test with a classmate.

### How to Run the Test

1. Pair up with another student.
2. Ask your partner to complete the **User Scenario** below on the **`main` branch** (`/PhdApplications/Create`) first, thinking aloud as they go.
3. **Do not give hints** (for example, do not tell them what values `FundingType` expects or where to find a Supervisor's staff number). Only step in if they are completely stuck for more than 60 seconds.
4. Repeat the exact same scenario on your **`ui-ux-improvements` branch** (`/PhdApplications/Create`).

### User Scenario Script

> *"Imagine you are **Alex Morgan** (`alex.morgan@example.ac.uk`). You want to apply for a **Part-time** PhD in **Computing (`PHD-COMP`)** starting on **1 October 2027**, funded by a **University studentship**, supervised by **Robin Carter**, with the working thesis title **'Automated Accessibility Testing in Higher Education Portals'** and a short research summary.*
>
> *1. Complete and submit the application, then note your application reference number.*
> *2. Return to the PhD Applications list (`/PhdApplications`) and filter the list to show only **Submitted** applications.*
> *3. (Optional — if you completed Part 10) Act as an Admissions Tutor and update your application status to **Under Review**."*

### Record Your Observations

| Metric | `main` Branch | `ui-ux-improvements` Branch |
|---|---|---|
| **Time to complete task (seconds)** | | |
| **Number of times user asked "What do I put here?"** | | |
| **Number of failed form submissions** | | |
| **Did the user have to leave the page to look up an ID or code?** | | |
| **Did the user receive a confirmation banner and filter the applications table easily?** | | |
| **Subjective Cognitive Load (1 = effortless, 5 = frustrating)** | | |

---

## Part 2: Chrome DevTools — Accessibility Tree, Lighthouse & Vision Emulation

Modern browsers compile the HTML DOM into an **Accessibility Tree** that screen readers (NVDA, VoiceOver, JAWS) actually read. Inspecting this tree directly shows you how assistive technology perceives your form controls.

### 2.1 Inspect the Accessibility Tree

1. Open Chrome or Edge DevTools (`F12` or `Cmd+Opt+I` / `Ctrl+Shift+I`).
2. In the **Elements** panel, open the secondary sidebar (`Computed` / `Layout` / **Accessibility**) and select **Accessibility**, then check **Enable full-page accessibility tree** (or inspect individual inputs).
3. Inspect the **`ModeOfStudy` (Mode of study)** and **`ProposedSupervisorStaffNumber` (Proposed main supervisor)** controls on both `main` and `ui-ux-improvements` and compare what the browser reports:

| Property in Accessibility Tree | `main` Branch | `ui-ux-improvements` Branch |
|---|---|---|
| **`FirstName` Computed Name** | `""` *(empty — `<span>` is not a `<label>`)* | `"First name"` (via `<label for="FirstName">`) |
| **`ModeOfStudy` Role** | `textbox` | `radio` inside `group` (`"Mode of study"`) |
| **`ProposedSupervisorStaffNumber` Role** | `textbox` | `combobox` with named `<option>` items |
| **`ApplicationReference` Computed Name** | `""` *(unlabelled system field exposed to user)* | *(auto-generated on server; removed from form)* |
| **`Description` (`aria-describedby`)** | *(empty)* | Format hint + error message when invalid |
| **`Invalid` state on empty submit** | `false` (silent failure) | `true` (native HTML5 `:invalid` / `aria-invalid="true"`) |

### 2.2 Run a Lighthouse Accessibility Audit

1. In Chrome DevTools, open the **Lighthouse** tab.
2. Select **Mode: Navigation**, **Device: Desktop**, and check **Categories: Accessibility**.
3. Run the audit on `/PhdApplications/Create` on the **`main` branch** and on the **`ui-ux-improvements` branch**.
4. Also trigger a failed form submission on **`ui-ux-improvements`** and run a **Snapshot** audit in Lighthouse to verify that error states maintain sufficient colour contrast and valid ARIA attributes. *(All portal theme, button, and alert colours are defined as CSS custom properties in `:root` at the top of `PgrStudentManagement.Web/wwwroot/css/site.css`—you can adjust `--theme-*`, `--btn-*`, and `--status-*` variables there if you want to experiment with colour contrast ratios).*

### 2.3 Vision Deficiency Emulation & 400% Reflow (WCAG 1.4.1 & 1.4.10)

1. With a validation error visible on your form, open the DevTools Command Menu (`Cmd+Shift+P` on macOS or `Ctrl+Shift+P` on Windows/Linux) and type **`Show Rendering`**.
2. Scroll down to **Emulate vision deficiencies**:
   - Select **Achromatopsia (no colour)**: Can you still tell which fields are invalid without seeing red? *(Hint: Look for borders, error text, and icons rather than colour alone — WCAG 1.4.1).*
   - Select **Blurred vision**: Are field labels, hint text, and error messages clearly grouped next to their inputs?
3. Reset vision emulation to **No emulation**, set the browser window width to `1280px`, and zoom the page to **400%** (`Cmd/+` or `Ctrl/+`).
   - Verify that all form inputs stack cleanly into a single column without horizontal scrolling or overlapping labels (**WCAG 1.4.10 Reflow**).

---

## Part 3: Visual Keyboard & Tab-Stop Auditing (Accessibility Insights for Web)

One of the clearest ways to see the difference between the **`main`** and **`ui-ux-improvements`** branches is to trace the keyboard `Tab` path visually.

### 3.1 Install Accessibility Insights for Web

1. Install the free **Accessibility Insights for Web** extension for Chrome or Edge (`https://accessibilityinsights.io/downloads/`).
2. Click the extension icon in your browser toolbar and select **FastPass**.

### 3.2 Run Automated Checks

1. Run **FastPass → Automated Checks** on `/PhdApplications/Create` on both **`main`** and **`ui-ux-improvements`**.
2. Note any violations flagged directly on the page.

### 3.3 Record Visual Tab Stops

1. In the FastPass window, select **Tab Stops** and toggle **Show tab stops** to **On**.
2. Click the address bar, then press `Tab` repeatedly through the entire `Create` form on **`main`** and then on **`ui-ux-improvements`**.
3. The tool draws numbered circles (`1`, `2`, `3`…) connected by lines showing the exact focus trajectory. Compare both branches:
   - **Focus visibility**: Is the `:focus-visible` outline clearly visible on every control?
   - **Radio button behaviour**: On **`ui-ux-improvements`**, pressing `Tab` moves into the `<fieldset>` radio group once, and arrow keys switch between choices.
   - **Error recovery**: Submit an empty form on **`ui-ux-improvements`** and press `Tab` once from the Error Summary — does clicking an error link jump keyboard focus directly into the offending input?
4. Take a screenshot of the **Tab Stops** overlay on both **`main`** and **`ui-ux-improvements`** to include in your lab reflection.

---

## Part 4: Automated Usability & Accessibility Testing with Playwright

Why use **Playwright** for UI/UX and accessibility testing?

Playwright encourages writing tests that locate elements the exact same way a human or screen reader user does: by **visible label** (`page.getByLabel('First name')`) and by **semantic role** (`page.getByRole('radio', { name: 'Full-time (typically 3 years)' })`) rather than brittle CSS selectors (`#FirstName`).

Because of this:
- A test written from a **human user's perspective** will naturally **fail** on the poorly designed **`main` branch** form (where labels are CamelCase property names and every control is a plain text box) and **pass** on your **`ui-ux-improvements` branch**!
- Combining Playwright with **`@axe-core/playwright`** gives you automated WCAG 2.2 AA auditing across multi-step states and validation error states.

Below you can choose **Option A (Node.js / TypeScript — recommended for Playwright's interactive `--ui` runner)** or **Option B (.NET / C# xUnit)**.

---

### Option A: Playwright with Node.js (Recommended)

#### Step 1: Initialise a Playwright Test Folder

Create a dedicated `ui-ux-tests` folder outside your repository (so switching branches in `PgrStudentManagement-ui-ux` never overwrites your test files):

```bash
cd ..
mkdir ui-ux-tests
cd ui-ux-tests
npm init -y
npm install -D @playwright/test @axe-core/playwright
npx playwright install chromium
```

#### Step 2: Create `playwright.config.ts`

Create a file named `playwright.config.ts` in `ui-ux-tests`:

```typescript
import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  timeout: 30000,
  use: {
    // Defaults to http://localhost:5277, or override with BASE_URL=http://localhost:5278
    baseURL: process.env.BASE_URL || 'http://localhost:5277',
    headless: true,
    viewport: { width: 1280, height: 800 },
  },
});
```

#### Step 3: Create the Usability & Accessibility Test Suite

Create `tests/phd-applications-ux.spec.ts` inside `ui-ux-tests`:

```typescript
import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test.describe('PhD Application Form — UI/UX & Accessibility Suite', () => {

  test('1. Human-Readable Labels & HTML Constraints: Controls use plain-English labels and HTML5 validation attributes', async ({ page }) => {
    await page.goto('/PhdApplications/Create');

    // A user (or screen reader) looks for "First name", "Last name", "Email address"
    // MAIN FAILS: Field prompts use <span> instead of <label> and raw C# property names ("FirstName", "LastName", "EmailAddress")
    // UI-UX-IMPROVEMENTS PASSES: Controls use <label for="..."> with human-readable text and native HTML5 validation attributes
    const firstName = page.getByLabel('First name');
    const lastName = page.getByLabel('Last name');
    const email = page.getByLabel('Email address');

    await expect(firstName).toBeVisible();
    await expect(firstName).toHaveAttribute('required', '');
    await expect(lastName).toBeVisible();
    await expect(lastName).toHaveAttribute('required', '');
    await expect(email).toBeVisible();
    await expect(email).toHaveAttribute('type', 'email');
    await expect(email).toHaveAttribute('required', '');
  });

  test('2. Semantic Controls: Constrained choices use radios and dropdowns instead of textboxes', async ({ page }) => {
    await page.goto('/PhdApplications/Create');

    // MAIN FAILS: ModeOfStudy and ProposedSupervisorStaffNumber are generic role="textbox" inputs
    // UI-UX-IMPROVEMENTS PASSES: Mode of study is a fieldset radio group and Supervisor is a combobox
    const studyModeGroup = page.getByRole('group', { name: /Mode of study/i });
    await expect(studyModeGroup).toBeVisible();
    await expect(studyModeGroup.getByRole('radio')).toHaveCount(2);

    const supervisorSelect = page.getByRole('combobox', { name: /Proposed main supervisor/i });
    await expect(supervisorSelect).toBeVisible();
    // Should contain real supervisor names rather than requiring a raw staff number
    const optionCount = await supervisorSelect.locator('option').count();
    expect(optionCount).toBeGreaterThan(1);
  });

  test('3. Error Recovery & Keyboard Focus: Server validation focuses Error Summary and links to invalid fields', async ({ page }) => {
    await page.goto('/PhdApplications/Create');

    // Bypass browser HTML5 constraint blocking so we can verify server-side accessible error recovery
    await page.locator('form').evaluate((form: HTMLFormElement) => {
      form.noValidate = true;
    });

    // Submit the form empty
    const submitBtn = page.getByRole('button', { name: /Submit application|Create/i }).first();
    await submitBtn.click();

    // MAIN FAILS: Page reloads silently with no alert, no title change, and no aria-invalid
    // UI-UX-IMPROVEMENTS PASSES:
    // a) Page <title> is prefixed with "Error:" for screen reader announcement (WCAG 3.3.1)
    await expect(page).toHaveTitle(/^Error:/);

    // b) Error summary banner with role="alert" is visible and receives keyboard focus
    const errorSummary = page.locator('#error-summary[role="alert"]');
    await expect(errorSummary).toBeVisible();
    await expect(errorSummary).toBeFocused();

    // c) Clicking the first error summary link moves keyboard focus directly to the invalid input
    const firstErrorLink = errorSummary.locator('a').first();
    const targetHref = await firstErrorLink.getAttribute('href');
    await firstErrorLink.click();

    if (targetHref) {
      const targetInput = page.locator(targetHref);
      await expect(targetInput).toBeFocused();
      await expect(targetInput).toHaveAttribute('aria-invalid', 'true');
    }
  });

  test('4. Live Character Counter & Accessible Applications Table (Part 5 & Part 9)', async ({ page }) => {
    await page.goto('/PhdApplications/Create');

    // Check live character counter on Research Summary (Part 5)
    const summaryField = page.getByLabel(/Research summary/i);
    await summaryField.fill('Exploring automated accessibility testing in academic portals.');
    const counter = page.locator('#ResearchSummary-count');
    await expect(counter).toContainText(/remaining/i);

    // Check accessible table semantics, status filter, and contextual action links on Index (Part 9)
    await page.goto('/PhdApplications');
    await expect(page.getByLabel('Filter by status')).toBeVisible();
    await expect(page.getByRole('table', { name: /List of PhD applications/i })).toBeVisible();
    await expect(page.getByRole('link', { name: /Details for PHD-/i }).first()).toBeVisible();
  });

  test('5. Automated WCAG 2.2 AA Audit (axe-core)', async ({ page }) => {
    await page.goto('/PhdApplications/Create');

    const accessibilityScanResults = await new AxeBuilder({ page })
      .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'wcag22aa'])
      .analyze();

    expect(accessibilityScanResults.violations).toEqual([]);
  });
});
```

#### Step 4: Run the Tests Against `main` vs. `ui-ux-improvements`

1. **Test the `main` branch**:
   With the web server running on `main` (`git switch main && dotnet run --project PgrStudentManagement.Web --urls "http://localhost:5277"`):

   ```bash
   npx playwright test
   ```

   Observe how tests 1–5 fail on `main` because the starter form uses `<span>` tags instead of `<label>` elements and has low colour contrast on the `Create` button (`#7cb8eb` on `#ffffff`, failing both `label` and `color-contrast` in Test 5) and lacks human-readable labels, semantic radio/select roles, an error summary, and `aria-invalid` attributes (Tests 1–4).

2. **Test the `ui-ux-improvements` branch**:
   Switch the web server to your `ui-ux-improvements` branch (`git switch ui-ux-improvements && dotnet run --project PgrStudentManagement.Web --urls "http://localhost:5277"`):

   ```bash
   npx playwright test
   ```

   Verify that all 5 tests pass!

3. **Explore interactively with Playwright UI Mode**:

   ```bash
   npx playwright test --ui
   ```

   In Playwright UI Mode, click through each step on the timeline and inspect the DOM snapshot and accessibility locators at every millisecond of the test.

---

### Option B: Playwright for .NET (C# / xUnit)

If you prefer to write your automated tests in C#:

#### Step 1: Create an xUnit Test Project

```bash
cd ..
mkdir PgrStudentManagement.UiTests
cd PgrStudentManagement.UiTests
dotnet new xunit
dotnet add package Microsoft.Playwright
dotnet add package Deque.AxeCore.Playwright
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

*(Note: If you do not have PowerShell `pwsh` installed, you can install the Chromium binary via `npx playwright install chromium`.)*

#### Step 2: Add `PhdApplicationUxTests.cs`

Replace `UnitTest1.cs` with `PhdApplicationUxTests.cs`:

```csharp
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Xunit;

namespace PgrStudentManagement.UiTests;

public class PhdApplicationUxTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private readonly string _baseUrl =
        Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5277";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task CreateForm_UsesHumanReadableLabelsAndSemanticRoles()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync($"{_baseUrl}/PhdApplications/Create");

        // Verify human-readable labels exist
        Assert.True(await page.GetByLabel("First name").IsVisibleAsync());
        Assert.True(await page.GetByLabel("Last name").IsVisibleAsync());
        Assert.True(await page.GetByLabel("Email address").IsVisibleAsync());

        // Verify WCAG 2.2 AA compliance via AxeCore
        var axeResults = await page.RunAxe();
        Assert.Empty(axeResults.Violations);
    }

    [Fact]
    public async Task EmptySubmit_FocusesErrorSummaryAndMarksFieldsInvalid()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync($"{_baseUrl}/PhdApplications/Create");

        // Bypass native HTML5 constraint blocking to test server-side error summary recovery
        await page.Locator("form").EvaluateAsync("form => form.noValidate = true");

        await page.GetByRole(AriaRole.Button, new() { NameRegex = new("Submit application|Create") })
            .First.ClickAsync();

        // Title starts with "Error:"
        var title = await page.TitleAsync();
        Assert.StartsWith("Error:", title);

        // Error summary is visible
        var errorSummary = page.Locator("#error-summary[role='alert']");
        Assert.True(await errorSummary.IsVisibleAsync());
    }
}
```

#### Step 3: Run Against `main` and `ui-ux-improvements`

```bash
# 1. With the server running on the main branch:
dotnet test

# 2. Switch the server to ui-ux-improvements (or pass BASE_URL if using git worktree):
dotnet test
```

---

## Part 5: Branch Comparison Scorecard & Reflection

Complete the summary table below comparing your findings across all four testing approaches between `main` and `ui-ux-improvements`:

| Evaluation Dimension | Tool / Method Used | `main` Branch Result | `ui-ux-improvements` Branch Result | Key WCAG 2.2 Criterion |
|---|---|---|---|---|
| **Human Task Completion Time** | Part 1: Peer Guerrilla Test | | | — |
| **User Confusion / Lookup Errors** | Part 1: Peer Guerrilla Test | | | **3.3.2** Labels or Instructions |
| **Semantic Control Roles** | Part 2: DevTools Accessibility Tree | | | **1.3.1** Info & Relationships / **4.1.2** Name, Role, Value |
| **400% Zoom Reflow** | Part 2: Browser Zoom (`1280px` @ 400%) | | | **1.4.10** Reflow |
| **Error Identification Without Colour** | Part 2: DevTools Achromatopsia Emulation | | | **1.4.1** Use of Color |
| **Keyboard Tab Path & Radio Grouping** | Part 3: Accessibility Insights Tab Stops | | | **2.1.1** Keyboard / **2.4.7** Focus Visible |
| **Playwright Semantic Locator Tests** | Part 4: Playwright (`getByLabel` / `getByRole`) | | | **2.4.6** Headings & Labels / **3.3.1** Error Identification |
| **Automated WCAG 2.2 AA Violations** | Part 4: `@axe-core/playwright` | | | WCAG 2.2 Level A & AA |

### Reflection Questions

1. **Automated vs. Manual Detection**: Why did Lighthouse and `axe-core` immediately flag the missing `<label>` tags and the low-contrast `Create` button (`#7cb8eb` vs `#ffffff`, `2.12:1`) on the **`main` branch**, yet after you completed **Part 1** (`Snippet 1.A` & `1.B`), `axe-core` would pass the form even before you fixed the deeper UX problems (requiring raw staff numbers like `AS1001` and exact enum strings)? What does this tell you about relying solely on automated accessibility scanners?
2. **Playwright Locators as UX Tests**: How did writing Playwright tests with `getByLabel` and `getByRole` expose usability flaws on `main` that a CSS selector test (`page.locator('#ProposedSupervisorStaffNumber')`) would have missed?
3. **Error Recovery**: How did combining native HTML5 validation constraints (`required`, `maxlength`, `type="email"`) with the server-side focusable `#error-summary[role="alert"]`, the `Error:` page `<title>` prefix, and `aria-invalid="true"` on the `ui-ux-improvements` branch change the experience for a keyboard-only or screen-reader user when submitting an incomplete form?
