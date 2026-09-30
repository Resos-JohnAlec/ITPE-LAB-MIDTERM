import assert from "node:assert/strict";
import { mkdir, writeFile } from "node:fs/promises";
import { chromium } from "playwright";
import AxeBuilder from "@axe-core/playwright";

const adminKey = process.env.CAMPUS_ADMIN_KEY;
if (!adminKey)
  throw new Error(
    "Set CAMPUS_ADMIN_KEY to the running verification server key.",
  );
const output = "docs/evidence";
await mkdir(output, { recursive: true });
const browser = await chromium.launch({
  executablePath:
    process.env.CHROME_PATH ||
    "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
  headless: true,
});
const context = await browser.newContext({
  viewport: { width: 1440, height: 1080 },
  reducedMotion: "reduce",
});
const page = await context.newPage();
const failures = [];
const checks = [];
page.on("pageerror", (error) => failures.push(error.message));
const check = (condition, message) => {
  assert.ok(condition, message);
  checks.push(message);
  console.log(`PASS: ${message}`);
};
async function accessibility(label) {
  const report = await new AxeBuilder({ page })
    .withTags(["wcag2a", "wcag2aa", "wcag21a", "wcag21aa"])
    .analyze();
  await writeFile(
    `${output}/axe-${label}.json`,
    JSON.stringify(
      {
        url: report.url,
        violations: report.violations,
        passes: report.passes.map((item) => item.id),
        incomplete: report.incomplete.map((item) => ({
          id: item.id,
          description: item.description,
        })),
      },
      null,
      2,
    ),
  );
  check(
    report.violations.length === 0,
    `No automated WCAG A/AA violations: ${label} (${report.violations.map((item) => item.id).join(", ")})`,
  );
}

try {
  await page.goto("http://127.0.0.1:5080/");
  await page.locator(".event-card").first().waitFor();
  check(
    (await page.locator(".event-card").count()) >= 6,
    "Catalog renders real API event data",
  );
  await accessibility("catalog");
  await page.screenshot({
    path: `${output}/catalog-desktop.png`,
    fullPage: true,
  });

  for (const width of [375, 768, 1024, 1440]) {
    await page.setViewportSize({ width, height: 900 });
    check(
      await page.evaluate(
        () => document.documentElement.scrollWidth <= innerWidth,
      ),
      `No page overflow at ${width}px`,
    );
  }
  await page.setViewportSize({ width: 375, height: 812 });
  await page.screenshot({
    path: `${output}/catalog-mobile.png`,
    fullPage: true,
  });
  await page.getByRole("searchbox").fill("zz-no-such-event");
  check(
    (await page.locator(".event-card").count()) === 0,
    "Search displays a genuine empty state",
  );
  await page.getByRole("searchbox").fill("");
  await page.setViewportSize({ width: 1440, height: 1080 });
  await page.keyboard.press("Control+Home");
  await page
    .getByRole("button", {
      name: "Register for Build Something Good",
      exact: true,
    })
    .focus();
  await page.keyboard.press("Enter");
  await page.getByRole("dialog").waitFor();
  check(
    await page
      .getByLabel("Full name", { exact: true })
      .evaluate((node) => node === document.activeElement),
    "Opening registration moves keyboard focus into the form",
  );
  await accessibility("registration");
  await page.getByRole("button", { name: "Confirm registration" }).click();
  check(
    (await page.locator("#name-error").textContent()) !== "",
    "Empty form produces a textual validation error",
  );
  await page
    .getByLabel("Full name", { exact: true })
    .fill("Browser Demo Student");
  await page
    .getByLabel("University email", { exact: true })
    .fill("student@dlsud.edu.ph.evil.test");
  await page.getByRole("button", { name: "Confirm registration" }).click();
  check(
    (await page.locator("#email-error").textContent()) !== "",
    "Form rejects a deceptive university-domain suffix",
  );
  const email = `browser.${Date.now()}@dlsud.edu.ph`;
  await page.getByLabel("University email", { exact: true }).fill(email);
  await page.screenshot({
    path: `${output}/registration-form.png`,
    fullPage: true,
  });
  await page.getByRole("button", { name: "Confirm registration" }).click();
  await page.getByRole("heading", { name: "You're on the list!" }).waitFor();
  check(
    (await page.locator("#confirmation-number").textContent()) !== "",
    "Registration confirms a real server registration ID",
  );
  await accessibility("success");
  await page.screenshot({
    path: `${output}/registration-success.png`,
    fullPage: true,
  });
  await page.getByRole("button", { name: "Back to exploring" }).click();
  await page
    .getByRole("button", {
      name: "Register for Build Something Good",
      exact: true,
    })
    .click();
  await page
    .getByLabel("Full name", { exact: true })
    .fill("Browser Demo Student");
  await page.getByLabel("University email", { exact: true }).fill(email);
  await page.getByRole("button", { name: "Confirm registration" }).click();
  await page
    .getByRole("alert")
    .filter({ hasText: "already registered" })
    .waitFor();
  check(true, "Duplicate registration is shown as an actionable server error");
  await page.keyboard.press("Escape");
  await page.getByRole("dialog").waitFor({ state: "hidden" });
  check(true, "Escape closes the registration dialog");

  await page.goto("http://127.0.0.1:5080/admin.html");
  await accessibility("admin-locked");
  await page.getByLabel("Administrator access key").fill("incorrect-test-key");
  await page.getByRole("button", { name: "View attendees" }).click();
  await page
    .locator("#admin-status")
    .filter({ hasText: "incorrect" })
    .waitFor();
  check(
    await page.locator("#attendees-panel").isHidden(),
    "Wrong administrator key does not expose attendees",
  );
  await page.getByLabel("Administrator access key").fill(adminKey);
  await page.getByRole("button", { name: "View attendees" }).click();
  await page.locator("#attendees-panel").waitFor();
  await page
    .getByLabel("Choose an event")
    .selectOption({ label: "Build Something Good" });
  await page.getByRole("cell", { name: email, exact: true }).waitFor();
  check(true, "Organizer table shows the student registered through the UI");
  await accessibility("admin-attendees");
  await page.screenshot({
    path: `${output}/admin-attendees.png`,
    fullPage: true,
  });
  await page.setViewportSize({ width: 375, height: 812 });
  check(
    await page.evaluate(
      () => document.documentElement.scrollWidth <= innerWidth,
    ),
    "Administrator page fits mobile width",
  );
  await page.getByRole("button", { name: "Lock access" }).click();
  check(
    (await page.locator("#attendee-rows tr").count()) === 0,
    "Locking access clears attendee data from the page",
  );
  check(
    await page.evaluate(
      () => localStorage.length === 0 && sessionStorage.length === 0,
    ),
    "No credentials or attendee data are persisted in browser storage",
  );
  check(failures.length === 0, "No browser JavaScript exceptions");
  await writeFile(
    `${output}/browser-results.json`,
    JSON.stringify(
      {
        checkedAt: new Date().toISOString(),
        checks,
        pageErrors: failures,
        browser: await browser.version(),
        note: "Automated checks do not establish complete WCAG conformance or replace human review.",
      },
      null,
      2,
    ),
  );
  console.log(`Browser verification passed: ${checks.length} checks.`);
} finally {
  await browser.close();
}
