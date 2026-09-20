# OpenCode CLI — Install & Session Management

## Source
- **Folder**: `Docs/AI`
- **File**: `opencode.ipynb`
- **What it is**: quick notes for installing the opencode terminal agent and working with its sessions.

## What opencode is
A terminal-first AI coding agent (like aider but from the opencode project). It edits files, searches the codebase, and runs commands from a CLI session.

## Install
```bash
sudo snap install opencode --classic
```
`--classic` is needed so the snap can act on files outside its sandbox.

## Start
```bash
opencode
```

## Working with sessions
Sessions keep your chat history between runs so you can pause and resume work.

List all previous session IDs:
```bash
opencode session list
```

Resume / reopen a specific session:
```bash
opencode -s <SESSION_ID>
```

## Verify
```bash
opencode --version
opencode session list
```

## Gotchas / fixes applied
- **Model/API**: opencode needs a model provider configured (e.g. Ollama, OpenAI, Anthropic) — set the API key or local endpoint in `opencode.json` or env vars before the first run.
- **snap classic**: if `sudo snap install` fails with a not-supported error, install via the curl installer instead:
  ```bash
  curl -fsSL https://opencode.ai/install | bash
  ```
- **Stale sessions**: `opencode session list` shows all sessions; use `-s` with the ID to jump back into context instead of starting fresh.