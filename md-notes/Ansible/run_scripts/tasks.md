# Role: run_scripts — deploy the web app (`run_scripts/tasks/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/run_scripts`
- **File**: `tasks/main.yaml`
- **What it is**: copies a bootstrap script to the target, runs it, deletes it — used to stand up the Django web app.

## Content
```yaml
- name: Copy shell script to remote server
  copy:
    src: /mnt/linux/data/project/final-project/bash_script/web_app.sh
    dest: /tmp/myscript.sh
    mode: 0755

- name: Run shell script on remote server
  shell: sh /tmp/myscript.sh

- name: Remove shell script from remote server
  file:
    path: /tmp/myscript.sh
    state: absent
```

## The `web_app.sh` it runs (from `bash-helper/copy_db.md`)
That script updates apt, installs Python/pip/MySQL dev libs, clones the Django repo, injects DB settings via `sed`, migrates, and launches `runserver 0.0.0.0:80`.

## Issues in the original
1. `src:` hardcodid to the old mount path — the script no longer lives there in this layout. Point it at the script present in `Docs/bash-helper/web_app.sh`-style locations, or better, generate it on the fly (see `bash-helper/copy_db.md`).
2. `run_scripts` never passes the DB parameters (`$1..$4`) the script expects — the run would use empty args.
3. Runs without `become` — script has `sudo` inside, so it survives; but cleaner to trust it.

## Completed version
```yaml
- name: Copy bootstrap script to remote
  ansible.builtin.copy:
    src: "{{ webapp_script_path }}"     # e.g. ../bash-helper/web_app.sh or a generated file
    dest: /tmp/myscript.sh
    mode: 0755

- name: Run bootstrap script with DB settings
  ansible.builtin.shell: >
    sh /tmp/myscript.sh
    "{{ webapp_db_name }}"
    "{{ webapp_db_user }}"
    "{{ webapp_db_pass }}"
    "{{ webapp_db_host }}"

- name: Remove script
  ansible.builtin.file:
    path: /tmp/myscript.sh
    state: absent
```
where `webapp_*` vars live in `vars/main.yaml` (placeholders, not secrets):
```yaml
webapp_script_path: <REPO_PATH>/web_app.sh
webapp_db_name: <DB_NAME>
webapp_db_user: <DB_USER>
webapp_db_pass: <DB_PASSWORD>
webapp_db_host: <DB_HOST>
```

## Verify
```bash
# on the target:
curl -s http://localhost/            # expect the Django app
systemctl status postgresql          # if local db
```

## Gotchas
- The `sed -i` in `web_app.sh` injects DB settings at the *generation* time (template placeholders in the Django config).
- If the app must bind `0.0.0.0:80`, run the script as a root-capable user; the target's firefall/security group must allow :80.
- Ensure the git URL in `web_app.sh` is reachable (DNS role if not).