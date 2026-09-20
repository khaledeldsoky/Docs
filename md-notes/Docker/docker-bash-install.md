# Docker engine install script (`docker-bash-install.sh`)
## Source
- **Folder**: `Docs/Docker`
- **File**: `docker-bash-install.sh`
- **What it is**: a **local copy of Docker's official installer** (the `get.docker.com` script, commit `53a22f61`), saved so it can be reused without re-downloading.

> Not written by you — it's the upstream script. Nothing inside is user-specific; treat it as a pinned vendor artifact.

## What the script does (overview)
1. Detects the distro from `/etc/os-release` (Ubuntu, Debian/Raspbian, CentOS/RHEL, Fedora, SLES).
2. Adds Docker's package repo and GPG key for that distro (apt or dnf/yum).
3. Installs `docker-ce`, `docker-ce-cli`, `containerd.io`, plus buildx, compose plugin, and rootless extras on newer versions.
4. Sets up the daemon to run as root via systemd (unless you choose rootless).
5. Detects WSL and EOL distros, printing warnings / sleeping 10s before continuing.

## Usage (proven by the script header)
```bash
# 1. get it (or use this local copy)
curl -fsSL https://get.docker.com -o install-docker.sh

# 2. inspect it before running (always a good habit)
cat install-docker.sh

# 3. dry-run to preview the exact commands
sh install-docker.sh --dry-run

# 4. real install as root/sudo
sudo sh install-docker.sh
```

## Command-line options
| Flag | Effect | Example |
|---|---|---|
| `--version <VER>` | install a specific engine version | `sudo sh install-docker.sh --version 23.0` |
| `--channel <stable\|test>` | pick release channel (test = pre-releases) | `--channel test` |
| `--mirror <Aliyun\|AzureChinaCloud>` | install from a CN mirror | `--mirror Aliyun` |
| `--dry-run` | print what would run, do nothing | `sh install-docker.sh --dry-run` |

Env-var equivalents: `VERSION`, `CHANNEL`, `DOWNLOAD_URL`, `REPO_FILE`, `DRY_RUN`.

## Completion notes (things to know beyond the script)
- **Not for production upgrades**: the header explicitly says dependency versions may not update consistently; use it for fresh installs, then manage via `apt`.
- **Rootless**: for `VERSION >= 20.10` it suggests `dockerd-rootless-setuptool.sh install` — running Docker without root.
- **Post-install**: to use docker as a non-root user (the script tells you too):
  ```bash
  sudo usermod -aG docker $USER
  newgrp docker
  ```

## Verify install
```bash
docker version
docker run --rm hello-world
docker compose version
sudo systemctl status docker
```

## Gotchas / fixes applied
- **Do NOT hand-edit the script's version logic** — it's generated upstream; keep the pinned copy byte-identical for easy diffing against a fresh `curl`.
- Pinned at commit `53a22f61c0628e58e1d6680b49e82993d304b449`; when you want newer logic, re-pin: `curl -fsSL https://get.docker.com -o docker-bash-install.sh`.
- On **air-gapped** hosts this can't fetch the repo — mirror the `.deb`s or ship an offline bundle instead.
- For the Ansible-managed nodes in this repo, prefer the `docker` role (`Docs/Ansible/docker/tasks.md`) — it's idempotent, unlike this script.