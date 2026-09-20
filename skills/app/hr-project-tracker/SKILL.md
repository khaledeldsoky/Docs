---
name: hr-project-tracker
description: >
  Use this skill for an HR system built with React, .NET, and PostgreSQL (containerized).
  Trigger this skill when the user wants to:
  - Get ideas or suggestions for new features to build in their HR project
  - Log, record, or write a summary for a task they just completed
  - Mark a task as done and save progress
  - Browse or review what has been completed so far
  - Plan what to work on next
  - Ask "what should I build next?" or "give me feature ideas"
  - Say "I finished X, write a summary" or "task done, document it"
  Always use this skill whenever the user mentions their HR project and wants to track progress or get enhancement ideas.
---

# HR Project Tracker Skill

This skill helps Khaled manage and grow his HR system. It does two things:

1. **Suggest Features** — Generate relevant, well-scoped ideas for enhancing the HR system
2. **Document Completed Work** — Write clean summaries of finished tasks and save them to a file

The project stack is: **React (Vite)** (frontend) · **.NET 8 ASP.NET Core** (backend API) · **PostgreSQL via EF Core** (database)

> For project structure, commands, and conventions see the `hr-system` skill.

---

## File Structure for Saved Data

All project files live in the working directory under `hr-project-docs/`:

```
hr-project-docs/
├── completed-tasks/
│   ├── tasks.md     ← all tasks in one growing file
│   └── ...
├── feature-ideas/
│   └── ideas.md                    ← all feature ideas in one growing file
└── PROJECT-SUMMARY.md              ← high-level overview, auto-updated
```

Always check if these files exist before writing. Create them if they don't.

### Rules for managing ideas

- **Ideas are numbered** (1, 2, 3) — user picks by number to implement next
- **Always keep exactly 3 ideas** in `ideas.md`. When one is completed, remove it immediately and generate a fresh idea to replace it so the list stays at 3
- When removing a completed idea, write its name under the "Completed" section at the bottom of `ideas.md` (no details needed, links to `completed-tasks/tasks.md`)
- Check existing ideas before generating new ones — never duplicate

---

## Mode 1 — Suggest Feature Ideas

### When to use
User asks for ideas, says "what should I build next?", "give me feature ideas", or "how can I enhance my HR system".

### How to respond

1. **Ask context questions first** (if not already known):
   - What HR modules already exist? (e.g., employees, payroll, leave, attendance)
   - Any pain points the user mentioned?
   - What's the complexity preference — quick wins or big features?

2. **Generate exactly 3 ideas** numbered 1–3, structured like this:

```
### 1. [Feature Name]
**Why it's useful:** One sentence on the business value.
**What it involves:**
- React: [what changes on the frontend]
- .NET: [what API endpoints or services are needed]
- PostgreSQL: [new tables or columns needed, if any]
**Effort estimate:** Small / Medium / Large
**Good to build after:** [prerequisite feature, or "can start anytime"]
```

3. **Write ideas to `hr-project-docs/feature-ideas/ideas.md`**
   - Replace the entire list (don't append — the list is always exactly 3 items)
   - Add a date header: `## Ideas added on YYYY-MM-DD`
   - Don't duplicate ideas previously suggested
   - Keep the file readable — no clutter

4. **Tell the user** the ideas were saved as numbered items and ask which number they want to start on.

### HR-specific idea categories to draw from

Pull ideas from these domains relevant to HR systems:

- **Employee Management**: profiles, org charts, document storage, onboarding checklists
- **Leave & Attendance**: leave requests, approval workflows, calendars, attendance tracking
- **Payroll**: salary slips, deductions, tax calculations, export to PDF
- **Performance**: goals, reviews, KPIs, 360 feedback
- **Recruitment**: job postings, applicant tracking, interview scheduling
- **Reports & Analytics**: dashboards, headcount trends, turnover rates, export to Excel
- **Notifications**: email alerts, in-app notifications for approvals/rejections
- **Auth & Roles**: role-based access control (HR manager, employee, admin)
- **Self-Service Portal**: employees update their own info, view payslips, request leave
- **Audit Logs**: track who changed what and when

Always suggest ideas that make sense for the current stage of the project. Don't suggest something that obviously requires a missing foundation.

---

## Mode 2 — Document a Completed Task

### When to use
User says "I finished X", "task done", "write a summary for what I did", or "mark this as complete".

### How to respond

1. **Gather task details** — ask if not provided:
   - What was built or changed?
   - Which layer was touched? (React / .NET / PostgreSQL / all)
   - Any important decisions made? (e.g., "used JWT for auth", "added index on employee_id")
   - Did anything not work as expected and get solved?

2. **Write the summary file** at `hr-project-docs/completed-tasks/tasks.md`

Use this structure:

```markdown
# [Task Name]
**Date completed:** YYYY-MM-DD
**Stack touched:** React / .NET / PostgreSQL (list what was changed)

## What Was Built
Plain English description of the feature or fix. 2–4 sentences max.

## How It Works
Short explanation of the logic. Use simple language.
Only include a code snippet if it genuinely helps clarity — keep it short.

## Key Decisions
- [Decision 1]: Why it was made
- [Decision 2]: Why it was made

## What's Next (optional)
Any follow-up tasks or known limitations.
```

3. **Remove the completed idea from `ideas.md`**
   - Delete the completed item (numbering shifts automatically)
   - Add it to the "Completed" section at the bottom of `ideas.md`
   - If the list drops below 3 ideas, generate a new idea to refill it back to 3

4. **Update `hr-project-docs/PROJECT-SUMMARY.md`**
   - Add the task to a "Completed Tasks" list with its date and one-line description
   - Keep this file as a quick overview — not a repeat of every detail

5. **Confirm to the user** that the summary was saved, show them the file path, and present the current 3 ideas so they can pick the next one by number.

---

## Writing Rules (always follow)

- **Clear, simple English** — write like you're explaining to a smart colleague, not writing a textbook
- **No jargon without explanation** — if you use a technical term, explain it in one short phrase
- **Short paragraphs** — 2–4 sentences max per block
- **Code only when necessary** — a snippet is useful when it shows *how* something works; skip it if plain words are enough
- **Organized structure** — use headings and bullet points, but don't over-format
- **Honest estimates** — don't oversell effort or complexity

---

## Creating Files

When creating or updating any file, always:
1. Check if the file already exists (`bash_tool` with `ls` or `cat`)
2. If it exists, **append** or **update** the right section — don't overwrite the whole file
3. If it doesn't exist, create it with proper headers
4. After saving, tell the user the file path and a one-line confirmation

---

## Example Interaction Flows

### Example A: User wants ideas
> "Give me some ideas to improve my HR system — I already have employee profiles and basic leave requests."

→ Read `ideas.md` to avoid repeating → Generate 3 fresh numbered ideas → Write (overwrite) to `ideas.md` → Report back with the numbered list.

### Example B: User finished a task
> "I just finished building the leave approval workflow. Write a summary."

→ Ask any missing details → Write `completed-tasks/tasks.md` → Remove the idea from `ideas.md`, add to Completed section → Refill to 3 ideas if needed → Update `PROJECT-SUMMARY.md` → Confirm and present the current 3 ideas.
