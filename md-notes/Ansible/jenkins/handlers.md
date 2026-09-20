# Role: Jenkins — handlers (`jenkins/handlers/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/jenkins`
- **File**: `handlers/main.yaml`
- **What it is**: cache-refresh handler for the Jenkins apt repo.

## Content
```yaml
- name: Update
  ansible.builtin.apt:
    update_cache: true
```

## How it fits
Triggered by `notify: Update` in `jenkins/tasks/main.yml` right after the Jenkins apt repo is registered, so apt picks up the new package index before installing `jenkins`.

## Completion notes
- Good and minimal — matches the task name `Update` exactly (case-insensitive).
- Idempotent: only refreshes when the repo task actually changed.

## Verify
Handlers fire only on changed tasks; a fresh node shows `RUNNING HANDLER [Update]`.

## Gotchas
- Jenkins repo install depends on the key file step having run; if that task changes every run (e.g. file perms), the handler refires cache update repeatedly. Fine functionally, just noisy.