# Role: nginx_proxy — handlers (`nginx_proxy/handlers/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/nginx_proxy`
- **File**: `handlers/main.yaml`
- **What it is**: cache refresh + nginx restart used by the nginx_proxy role.

## Content
```yaml
- name: update
  ansible.builtin.apt:
    update_cache: yes

- name: Restart Nginx
  ansible.builtin.service:
    name: nginx
    state: restarted
```

## How it fits
- `update` fires after the nginx package install (`notify: update` in tasks) so the cache is fresh.
- `Restart Nginx` is also fired by `notify: Restart Nginx` in tasks so a new vhost config takes effect.

## Completion notes
- Handler names matching tasks is the whole contract — present ones match (`update`, `Restart Nginx`).
- On modern nginx from apt, `systemctl reload nginx` is gentler than `restarted` (no dropped in-flight connections):
  ```yaml
  - name: Reload Nginx
    ansible.builtin.service:
      name: nginx
      state: reloaded
  ```
  Keep `restarted` here for the "first install" case where reload has nothing to signal; either works.

## Verify
In a play run, you should see `RUNNING HANDLER [Restart Nginx]` after the config copy/notify steps.

## Gotchas
- The tasks use `ignore_errors: yes` around the restart (config may be mid-copy) — the handler restart is the authoritative one.