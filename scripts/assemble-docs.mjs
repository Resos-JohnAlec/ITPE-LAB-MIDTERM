// Mechanical assembly preserves exact prompt/output text rather than retyping it.
import { readFile, writeFile } from "node:fs/promises";
const read = (path) => readFile(path, "utf8");
const pack = await read("MIDTERM_EXAM_PROMPT_PACK.md");
const blocks = [...pack.matchAll(/```text\r?\n([\s\S]*?)\r?\n```/g)].map(
  (match) => match[1],
);
const task1 = blocks.find(
  (block) => block.startsWith("ROLE\n") || block.startsWith("ROLE\r\n"),
);
if (!task1) throw new Error("RCTC prompt not found in original prompt pack.");
const task1Output = await read("docs/task-1-ai-output.md");
const erd = (await read("database/erd.mmd")).trimEnd();
const template = await read("docs/SUBMISSION.template.md");
const submission = template
  .replace("{{TASK1_PROMPT}}", () => task1)
  .replace("{{TASK1_OUTPUT}}", () => task1Output)
  .replace("{{ERD}}", () => erd);
if (/\{\{[A-Z0-9_]+\}\}/.test(submission))
  throw new Error("Unresolved report token.");
await writeFile("SUBMISSION.md", submission);

const adopted = [
  [
    "Master project context",
    blocks.find((block) => block.startsWith("PROJECT:")),
  ],
  ["Task 1 — Exact RCTC architecture prompt", task1],
  [
    "Task 2 — Exact frontend prompt",
    blocks.find((block) => block.startsWith("Act as a senior frontend")),
  ],
  [
    "Task 3 — Exact database prompt",
    blocks.find((block) => block.startsWith("Act as a senior database")),
  ],
  [
    "Task 4A — Exact unit-test prompt",
    blocks.find((block) => block.startsWith("Act as a senior QA")),
  ],
  [
    "Task 4B — Exact security diagnosis prompt",
    blocks.find((block) =>
      block.startsWith("Act as a senior application security"),
    ),
  ],
  [
    "Task 4C — Exact C# refactoring prompt",
    blocks.find((block) => block.startsWith("Act as a senior C#")),
  ],
  [
    "Pack quality-check prompt",
    blocks.find((block) => block.startsWith("Does this directly")),
  ],
];
let prompts = `# Prompt documentation\n\n## Provenance\n\nThe prompts below are copied exactly from MIDTERM_EXAM_PROMPT_PACK.md and were adopted as task specifications while implementing this repository in one Codex session. They are not claimed to be separate messages pasted into an AI tool by students. The Task 1 architecture output is preserved verbatim in docs/task-1-ai-output.md and embedded in SUBMISSION.md. Other outputs are the generated project files, with a final snapshot in docs/AI_OUTPUTS.md. The original prompt pack remains unchanged.\n\n## Actual user instructions in this session\n\n1. \"analyze the midterm_exam_propmt_pack.md and start with what's needed, after that, give me all prompts to document. make sure to accomplish deliverables.\"\n2. \"use @dlsud.edu.ph\"\n3. \"for the names, its John Alec Resos, Renz Mathieu Raquin, and Matthew Ryan Sabino\"\n\n## Project-specific implementation context\n\nThe adopted task prompts were interpreted with the actual repository context: initially only README and the pack; a three-member roster; dlsud.edu.ph domain; SQL Server LocalDB installed; no available .NET SDK on PATH. A project-local .NET 10 SDK was installed to build and verify a single ASP.NET Core application, plain semantic frontend, and SQL Server schema. xUnit/Moq were selected for isolated tests. A local organizer access key guards the attendee API. These are disclosed implementation decisions, not quotations from a separately submitted prompt.\n\n`;
for (const [title, body] of adopted) {
  if (!body) throw new Error(`Missing prompt: ${title}`);
  prompts += `## ${title}\n\n\`\`\`text\n${body}\n\`\`\`\n\n`;
}
prompts += `## Task 5 — Documentation instruction\n\nThe pack provides a report structure rather than a separate role/task prompt. The complete original structure is in its section \"TASK 5 - SUBMISSION.MD TEMPLATE\". It was applied to the actual implementation in root SUBMISSION.md. No student review, member correction, or GitHub publication is claimed without evidence.\n\n## Optional future prompts (not used as user messages in this session)\n\nThese are ready to paste if the team needs further help. Keep future responses and actual human corrections in your own log.\n\n### Human-grounding support\n\n\`\`\`text\nReview SUBMISSION.md against MIDTERM_EXAM_PROMPT_PACK.md and the current source. Map every required deliverable to a file and execution result. Identify missing or unsupported claims. Do not invent manual verification, team contributions, model versions, or GitHub publication. Give the team a short checklist of concrete manual reviews still needed. Do not change code unless asked.\n\`\`\`\n\n### Focused correction\n\n\`\`\`text\nReview the failing behavior and evidence I provide for Campus Gather. Preserve the plain HTML/CSS/JavaScript frontend, ASP.NET Core C# backend, SQL Server 3NF schema, and exact dlsud.edu.ph domain. Explain the cause, apply the smallest necessary correction, and run the relevant existing checks. Record the actual change without representing AI edits as student-made manual corrections.\n\`\`\`\n\n### Consolidation after team review\n\n\`\`\`text\nUpdate docs/SUBMISSION.template.md using the actual team role confirmations, manual verification notes, and corrections I supply. Keep Task 1's exact adopted prompt and original AI output unchanged. Preserve the Mermaid diagram from database/erd.mmd. Regenerate SUBMISSION.md, docs/PROMPTS.md, and docs/AI_OUTPUTS.md with node scripts/assemble-docs.mjs. Mark checklist items complete only when supplied evidence supports them.\n\`\`\`\n`;
await writeFile("docs/PROMPTS.md", prompts);

const files = [
  "docs/task-1-ai-output.md",
  "docs/frontend-notes.md",
  "database/DESIGN.md",
  "database/erd.mmd",
  "database/schema.sql",
  "database/seed.sql",
  "docs/test-notes.md",
  "docs/security-review.md",
  "frontend/index.html",
  "frontend/styles.css",
  "frontend/api.js",
  "frontend/app.js",
  "frontend/admin.html",
  "frontend/admin.js",
  "backend/RegistrationService.cs",
  "backend/EmailValidator.cs",
  "backend/Models.cs",
  "backend/Program.cs",
  "backend/DatabaseBootstrap.cs",
  "tests/CampusEvents.Tests/EmailValidatorTests.cs",
  "tests/DatabaseVerification.cs",
  "tests/browser.mjs",
];
let outputs =
  "# AI output evidence\n\nThis is a verbatim snapshot of the final AI-authored deliverable files, assembled mechanically for documentation. It is not an invented chat transcript or a claim that each adopted prompt was submitted separately. Task 1 preserves the original architecture output; other files include corrections made during implementation and verification. Use the source files as the executable artifacts and the raw reports in docs/evidence as execution evidence. Human changes made later must be recorded separately.\n\n";
for (const path of files) {
  const body = await read(path);
  const extension = path.split(".").pop();
  const language =
    {
      cs: "csharp",
      js: "javascript",
      mjs: "javascript",
      mmd: "mermaid",
      md: "markdown",
    }[extension] || extension;
  outputs += `## ${path}\n\n\`\`\`\`\`${language}\n${body.trimEnd()}\n\`\`\`\`\`\n\n`;
}
await writeFile("docs/AI_OUTPUTS.md", outputs.trimEnd() + "\n");
// Normalize only encoding/newlines in generated console evidence.
try {
  const evidencePath = "docs/evidence/integration-results.txt";
  const evidence = await read(evidencePath);
  await writeFile(evidencePath, evidence.replace(/^\uFEFF/, "").trimEnd() + "\n");
} catch (error) {
  if (error.code !== "ENOENT") throw error;
}
console.log(
  "Assembled SUBMISSION.md, docs/PROMPTS.md, and docs/AI_OUTPUTS.md from their exact sources.",
);
