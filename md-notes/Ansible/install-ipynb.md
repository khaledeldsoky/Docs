# Install Ansible — Interactive Notebook Version

## Source
- **Folder**: `Docs/Ansible`
- **File**: `install.ipynb`
- **What it is**: the original install steps (run cell-by-cell in VS Code).

## The original commands
```bash
sudo apt install software-properties-common
sudo apt-add-repository ppa:ansible/ansible
sudo apt update
sudo apt install ansible
```

## Why this was replaced
The `ppa:ansible/ansible` PPA is **deprecated/unmaintained** — it stopped receiving new releases (stuck around Ansible 2.9). On modern Ubuntu you should use the official Ansible apt repository instead.

## Recommended install (completes this file)
```bash
sudo apt update
sudo apt install -y software-properties-common gnupg
# Add the official Ansible signing key
sudo install -d /etc/apt/keyrings
curl -fsSL https://packages.ansible.com/keys/ansible-pubkey.asc | sudo gpg --dearmor \
    -o /etc/apt/keyrings/ansible.gpg

# Add the repo
echo "deb [signed-by=/etc/apt/keyrings/ansible.gpg] https://deb.ansible.com/ansible-bookworm bookworm main" \
    | sudo tee /etc/apt/sources.list.d/ansible.sources
# (Or the ubuntu equivalent: https://deb.ansible.com/ansible-jammy jammy main)

sudo apt update
sudo apt install -y ansible
```

Or the simplest modern path (Python-based, always current):
```bash
python3 -m venv ~/ansible-env
source ~/ansible-env/bin/activate
pip install ansible-core    # or: pip install ansible
```

## Verify
```bash
ansible --version
```
You should see a version ≥ 2.15 (with the PPA you were stuck on 2.9).

## Gotchas
- Old PPA installs fight with the new repo package — remove first: `sudo apt remove ansible` then clean `sudo add-apt-repository --remove ppa:ansible/ansible`.
- Running via venv: always `source ~/ansible-env/bin/activate` before using `ansible`.