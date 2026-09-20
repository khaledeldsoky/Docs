# Ansible Inventory (`hosts`)

## Source
- **Folder**: `Docs/Ansible`
- **File**: `hosts`
- **What it is**: the inventory describing a bastion host, private instances reached through it, and a Kubernetes master.

## The original file
```ini
[bastion]
3.219.233.141
3.87.225.170
[private]
privateinstance1 ansible_host=10.0.2.160 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
privateinstance2 ansible_host=10.0.1.91 ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
[Master]
master
```

## What it means
- **`[bastion]`** — publicly reachable jump hosts (two public IPs). Ansible connects to these directly.
- **`[private]`** — machines with private IPs only; Ansible reaches them *through* the bastion:
  ```bash
  ssh -W %h:%p bastion   # 'ssh -W' = netcat-style port forwarding
  ```
  So `privateinstance1` is reached by SSH to bastion, which tunnels to `10.0.2.160`.
- **`[Master]`** — Kubernetes master. **Broken in the original**: the host is named `master` but there is no `master` host definition anywhere, and it doesn't specify `ansible_host`, `ansible_user`, or the proxy. As-is, hitting this group fails with `skipping: no hosts matched`.

## Completed & corrected version (placeholders)
```ini
[bastion]
<BASTION_IP>                      # public IP of the jump host

[private]
privateinstance1 ansible_host=<PRIVATE1_IP> ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
privateinstance2 ansible_host=<PRIVATE2_IP> ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'

[master]                          # single master, reachable through bastion
master ansible_host=<MASTER_PRIVATE_IP> ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
```

## Wait — where is the master actually?
In this deployment the **master is one of the private instances** (it lives in the private subnet). Use that instance's private IP in `[master]` and pick the other one as the worker (or add a `[worker]` group with the same proxy pattern):
```ini
[workers]
worker1 ansible_host=<WORKER_IP> ansible_user=ubuntu ansible_ssh_common_args='-o ProxyCommand="ssh -W %h:%p bastion"'
```
With this setup, the role files' `hostvars['master']` references (join/kubeconfig) resolve correctly because the host in inventory is actually named `master`.

## Verify
```bash
ansible-inventory --list --yaml
ansible bastion -m ping          # reachable
ansible private -m ping          # through the proxy
ansible all --list-hosts
```

## Gotchas
- Keep the **group name lowercase** in the playbooks (`hostsvars['master']` is the *host*, not the group — matching `master` in the `[master]` group).
- The `bastion` group historically had **two** IPs; only one was needed. Keep one.
- These were real AWS public IPs — replace with placeholders before reuse.