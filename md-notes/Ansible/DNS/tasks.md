# Role: DNS — fix systemd-resolved nameservers (`DNS/tasks/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/DNS`
- **File**: `tasks/main.yaml`
- **What it is**: writes Google/Cloudflare DNS into `/etc/systemd/resolved.conf` and restarts resolution. Created because the private lab machines couldn't resolve names (e.g. Docker/ArgoCD downloads).

## Content
```yaml
- name: Set DNS servers
  community.general.ini_file:
    path: /etc/systemd/resolved.conf
    section: Resolve
    option: DNS
    value: "8.8.8.8 1.1.1.1"

- name: Set Fallback DNS servers
  community.general.ini_file:
    path: /etc/systemd/resolved.conf
    section: Resolve
    option: FallbackDNS
    value: "8.8.4.4 1.0.0.1"

- name: Restart systemd-resolved
  ansible.builtin.systemd_service:
    name: systemd-resolved
    state: restarted

- name: Ensure /etc/resolv.conf points to systemd-resolved
  ansible.builtin.file:
    src: /run/systemd/resolve/resolv.conf
    dest: /etc/resolv.conf
    state: link
    force: true
```

## What it fixes
Private instances without internet-facing DNS were failing to resolve `download.docker.com`, `raw.githubusercontent.com`, etc. — this points them at public resolvers.

## Prerequisite
The `community.general` collection must be installed on the **control node**:
```bash
ansible-galaxy collection install community.general
```

## Completion notes / alternatives
- On air-gapped networks this *breaks* things — only use with internet access. For offline, set an internal resolver instead.
- If the box uses `NetworkManager`, also set DNS there; `resolved.conf` alone may be overridden.
- Add a `notify: restart systemd-resolved` handler instead of inline restart for idempotency (optional).

## Verify
```bash
resolvectl status
resolvectl query github.com
```

## Gotchas
- `/etc/resolv.conf` becomes a symlink to systemd-resolved (by design) — tools like docker's embedded DNS keep working through it.
- Same fix appears manually in `Docs/Argocd/Argocd.ipynb` — this role is the idempotent, replayable version.