# Ansible Config (`ansible.cfg`)

## Source
- **Folder**: `Docs/Ansible`
- **File**: `ansible.cfg`
- **What it is**: the inventory + SSH settings for a bastion/private layout.

## The original file
```ini
[defaults]
inventory = hosts
remote_user = ubuntu
private_key_file = /mnt/linux/data/project/final-project/terraform/khaled-key.pem
host_key_checking = False

{#
remote_user = root
inventory = hosts
host_key_checking = False
fact_caching = jsonfile
fact_caching_connection = /tmp/ansible_cache
fact_caching_timeout = 3600
#}

[privilege_escalation]
become = False
become_ask_pass = False
```

## What each section does
| Setting | Meaning |
|---|---|
| `inventory = hosts` | Use the `hosts` file in this directory |
| `remote_user = ubuntu` | Default SSH user on all hosts |
| `private_key_file = ...` | SSH key used for login |
| `host_key_checking = False` | Don't prompt on unknown host keys (less secure, practical for labs) |
| `become = False` | Do not escalate to root unless a task says `become: true` |

## Problems in the original
1. `{# ... #}` is **Jinja comment syntax, not valid in an ini file** — the fact-caching block is useless as written. It was clearly meant as a plain `#` comment.
2. The key file points at an unrelated mount path.
3. `host_key_checking = False` + no known_hosts is fine for a lab but should be tightened in production.

## Corrected and completed version
```ini
[defaults]
inventory = hosts
remote_user = ubuntu
private_key_file = <PATH_TO_PRIVATE_KEY>.pem
host_key_checking = False

# Optional: enable fact caching so 'cacheable' set_facts survive across plays
fact_caching = jsonfile
fact_caching_connection = /tmp/ansible_cache
fact_caching_timeout = 3600

[privilege_escalation]
become = False
become_ask_pass = False
```

## Why fact_caching matters here
The `master-config` and `join` roles share data with `set_fact(..., cacheable: true)` (join command + kubeconfig). Those values only persist between play runs when a **fact cache backend is configured** — `jsonfile` above is the lightweight choice. Without it, the `join` role cannot read `hostvars['master'].join_cmd`.

Verify it works:
```bash
ansible --version              # config loads from ./ansible.cfg
ansible all --list-hosts       # reads inventory
```

## Gotchas
- `ansible.cfg` is only loaded from current dir (or `~/.ansible.cfg` / `/etc/ansible/ansible.cfg`) — run ansible from this directory.
- Set `private_key_file` to the key host that can reach the bastion; the bastion routes SSH to private nodes (see `hosts.md`).