# Local AI Stack — Ollama + Qwen2.5-Coder + Aider + Code-Server

## Source
- **Folder**: `Docs/AI`
- **File**: `local-ai.ipynb`
- **What it is**: a step-by-step recipe for running a fully local coding AI on an Ubuntu server (Ollama as the model server, Qwen2.5-Coder 14B as the model, aider as the coding agent, code-server as a browser IDE).

## What this gives you
| Component | Role |
|---|---|
| Ollama | Local model runner (`localhost:11434`) |
| qwen2.5-coder:14b | Code-specialised LLM run entirely on your machine |
| aider-chat | CLI pair-programmer that edits files, drives by the model |
| code-server | VS Code in the browser |
| ddgs | DuckDuckGo Search Python lib, lets the agent search the web |

## Prerequisites
- Ubuntu server (tested on Ubuntu 22.04/24.04)
- At least 16 GB RAM (recommended 32 GB) — a 14B model needs room
- Root or sudo
- Internet access (one-time, for download/install)

## Steps

### 1. Base system update
```bash
sudo apt update && sudo apt upgrade -y
```

### 2. Build tools and Python
```bash
sudo apt install -y git curl build-essential python3 python3-venv
```

### 3. Install Ollama
```bash
curl -fsSL https://ollama.com/install.sh | sh
```

### 4. Start Ollama as a service (survives reboots)
```bash
systemctl enable ollama --now
```

### 5. Pull the model
```bash
ollama pull qwen2.5-coder:14b
```
Other useful code models: `qwen2.5-coder:7b` (lighter), `deepseek-coder-v2`, `codellama`.
Run `ollama list` to confirm; `ollama ps` shows loaded models.

### 6. Install aider in its own virtualenv
```bash
python3 -m venv ~/aider-env
source ~/aider-env/bin/activate
pip install aider-chat
```
Keeping aider in a venv avoids polluting the system Python.

### 7. Install code-server (browser VS Code)
```bash
curl -fsSL https://code-server.dev/install.sh | sh
```

### 8. Start code-server
```bash
code-server
```
It serves on `http://<SERVER_IP>:8080` by default (password in `~/.config/code-server/config.yaml`).

### 9. Web search capability (optional but useful)
```bash
pip install ddgs
```

### 10. Launch aider against the local model
```bash
aider --model ollama/qwen2.5-coder:14b
```

## Verify everything works
```bash
ollama list                          # model present
systemctl status ollama              # service running
curl -s http://localhost:11434/api/tags   # API responding
aider --version
```

## Gotchas / fixes applied
- **Model choice**: 14b needs ~10–14 GB VRAM/RAM. If the box is small, use `qwen2.5-coder:7b`.
- **aider + Ollama**: model name must be prefixed `ollama/`; ensure Ollama is running *before* aider starts.
- **Venv**: always `source ~/aider-env/bin/activate` before `aider` in new shells.
- **code-server password**: it's random and stored in `~/.config/code-server/config.yaml` — read it there, or set `PASSWORD=` when launching.
- **Access from another machine**: open port 8080 (and 8081 if used) in the security group / firewall.