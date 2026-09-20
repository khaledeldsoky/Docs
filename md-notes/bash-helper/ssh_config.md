# bash-helper: `ssh_config.sh` — bastion jump-host SSH config generator
## Source
- **Folder**: `Docs/bash-helper`
- **File**: `ssh_config.sh`
- **What it is**: writes `~/.ssh/config` so you can `ssh bastion` directly, and reach private hosts transparently through the bastion.

## Original content
```bash
cat <<EOF > ~/.ssh/config
host bastion
   HostName $1
   User <REMOTE_USER>
   IdentityFile <PATH_TO_PRIVATE_FILE>
   StrictHostKeyChecking=no

host <ANY_NAME_YOU_NEED>
   HostName  $2
   user  <REMOTE_USER>
   IdentityFile <PATH_TO_PRIVATE_FILE>
   ProxyCommand ssh -q -W %h:%p  bastion
   StrictHostKeyChecking=no
EOF
```
- `$1` = bastion public IP, `$2` = private host IP. `<REMOTE_USER>` and `<PATH_TO_PRIVATE_FILE>` are placeholders to fill.

## Completed version
```bash
#!/bin/bash
# Usage: ./ssh_config.sh <BASTION_IP> <PRIVATE_IP> [REMOTE_USER] [KEY_FILE] [ALIAS]
BASTION="${1:?bastion IP required}"
PRIVATE="${2:?private IP required}"
REMOTE_USER="${3:-ubuntu}"
KEY="${4:-<PATH_TO_PRIVATE_KEY>.pem}"
ALIAS="${5:-private1}"

cat <<EOF > "$HOME/.ssh/config"
Host bastion
   HostName $BASTION
   User $REMOTE_USER
   IdentityFile $KEY
   StrictHostKeyChecking=no
   ServerAliveInterval 60

Host $ALIAS
   HostName $PRIVATE
   User $REMOTE_USER
   IdentityFile $KEY
   ProxyCommand ssh -q -W %h:%p bastion
   StrictHostKeyChecking=no
EOF

chmod 600 "$HOME/.ssh/config"
echo "ssh config written for bastion + $ALIAS"
```
Additions: arg defaults + validation, `chmod 600` (ssh ignores world-readable configs), keep-alive on the jump host.

## Test it
```bash
ssh bastion            # jumps straight to the bastion
ssh private1            # tunnels 'bastion → private1' in one command
```

## How the pieces fit this repo
- This is how `Ansible` reaches private hosts: the inventory's
  `ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'`
  relies on a `bastion` entry existing in `~/.ssh/config` (`Docs/Ansible/hosts.md`).
- Run this generator **before** any `ansible-playbook` against `private`.

## Gotchas
- Host keys: `StrictHostKeyChecking=no` disables host verification — fine for labs; drop it in production and populate `known_hosts` instead.
- Add one `Host <alias>` block **per private machine** (run the script multiple times with different `$2`).
- The private host's `HostName` must be reachable *from the bastion* — the proxy only forwards your TCP, DNS still resolves from the bastion.