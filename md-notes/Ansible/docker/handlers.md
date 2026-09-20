# Role: Docker — handlers (`docker/handlers/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/docker`
- **File**: `handlers/main.yaml`
- **What it is**: the notify-handler run after package repos change.

## Content
```yaml
- name: Upgrade and update
  ansible.builtin.apt:
    upgrade: true
    update_cache: false
```

## How it fits
Tasks use `notify: upgrade and update` (a **name**, case-insensitive match) so Ansible runs this handler **once at the end of the play** — right after the Docker apt repo was added, ensuring cache is refreshed before package installs (see `docker/tasks.md`).

## Completion notes
- `upgrade: true` performs a full distro-style upgrade on every handler run. In a production role you'd often want `upgrade: safe` or `update_cache: true` only:
  ```yaml
  - name: Update apt cache
    ansible.builtin.apt:
      update_cache: true
  ```
- Handler names must exactly match what tasks `notify:` (case-insensitive). Current one matches.

## Verify
Run the role play; a successful run shows `RUNNING HANDLER [Upgrade and update]` once at the end.

## Gotchas
- Handlers only fire if a task actually reported a change — if `notify` fires on an already-unchanged repo, skip happens and cache may be stale.