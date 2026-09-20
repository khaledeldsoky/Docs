# Install Ansible — Plain-Text Standalone Version

## Source
- **Folder**: `Docs/Ansible`
- **File**: `intallation` (no extension)
- **What it is**: the same install recipe as `install.ipynb`, written as a flat text command list.

## The original content
```bash
sudo apt update
sudo apt install software-properties-common
sudo add-apt-repository --yes --update ppa:ansible/ansible
sudo apt install ansible
```

## What changes after completion

### 1. That PPA is deprecated
`ppa:ansible/ansible` ships Ansible 2.9 and is no longer maintained. The `--yes --update` flag combines adding the repo and running `apt update` in one step (this part is fine).

### 2. Corrected one-liner install script
```bash
sudo apt update
sudo apt install -y software-properties-common gnupg curl
sudo install -d /etc/apt/keyrings
curl -fsSL https://packages.ansible.com/keys/ansible-pubkey.asc | sudo gpg --dearmor -o /etc/apt/keyrings/ansible.gpg
echo "deb [signed-by=/etc/apt/keyrings/ansible.gpg] https://deb.ansible.com/ansible-bookworm bookworm main" | sudo tee /etc/apt/sources.list.d/ansible.sources
sudo apt update
sudo apt install -y ansible
```
(Use `ansible-jammy` / `jammy` for Ubuntu 22.04.)

### 3. Or the venv alternative
```bash
sudo apt install -y python3-venv python3-pip
python3 -m venv ~/ansible-env
source ~/ansible-env/bin/activate
pip install ansible
```

## Verify
```bash
ansible --version
```

## Gotchas
- Must start with `sudo apt update` so `software-properties-common` resolves.
- If you previously installed from the PPA, remove it first or apt will mix two Ansible installs.