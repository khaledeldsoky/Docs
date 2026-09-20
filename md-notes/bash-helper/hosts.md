# bash-helper: `hosts.sh` — Ansible inventory generator
## Source
- **Folder**: `Docs/bash-helper`
- **File**: `hosts.sh`
- **What it is**: generates an Ansible inventory (`hosts` file) for a bastion + two private instances with ProxyCommand.

## Original content
```bash
cat  <<-OEF  > <PATH_TO_HOSTS_ANSIBLE_FILE>
[bastion]
$1
$2
[private]
privateinstance1 ansible_host=$3 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
privateinstance2 ansible_host=$4 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
OEF
```
- Args: `$1` = bastion host1 IP, `$2` = bastion host2 IP (second bastion), `$3` = private1 IP, `$4` = private2 IP.
- Writes to `<PATH_TO_HOSTS_ANSIBLE_FILE>` — a placeholder you must replace.

> In the original, the bastion group takes **two** IPs (both jumphosts). Only one is typically needed.

## Completed version
```bash
#!/bin/bash
# Usage: ./hosts.sh <BASTION_IP> <PRIVATE1_IP> <PRIVATE2_IP> [OUT_FILE]
BASTION="${1:?bastion IP required}"
P1="${2:?private1 IP required}"
P2="${3:?private2 IP required}"
OUT="${4:-./hosts}"

cat <<EOF > "$OUT"
[bastion]
$BASTION

[private]
privateinstance1 ansible_host=$P1 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
privateinstance2 ansible_host=$P2 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'

[master]
master ansible_host=$P1 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'

[workers]
worker1 ansible_host=$P2 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
EOF

echo "Inventory written to $OUT"
```
Additions: arg validation, output path param, and **master/workers groups** wired to the same hosts (so the k8s roles resolve `hostvars['master']` — see `Docs/Ansible/hosts.md`).

## Verify
```bash
./hosts.sh 1.2.3.4 10.0.2.160 10.0.1.91 ./hosts
ansible-inventory --list --yaml -i hosts
ansible bastion -m ping -i hosts
```

## Gotchas
- Group name `bastion` must exist for the ProxyCommand reference `%h:%p bastion` to resolve — it does (it's the SSH alias from `ssh_config.sh`, not an IP).
- The original `<<-OEF` strips leading tabs but **not** spaces — if your editor re-tabs the heredoc, the quotes around the ProxyCommand arg can get mangled. The fixed version uses `<<EOF` + space-indented body to stay consistent.
- Only three hosts max in the original; extend by editing the block for longer farms.