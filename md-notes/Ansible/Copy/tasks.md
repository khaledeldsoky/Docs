# Role: Copy — ship folder to remote hosts (`Copy/tasks/main.yml`)

## Source
- **Folder**: `Docs/Ansible/Copy`
- **File**: `tasks/main.yml`
- **What it is**: copies the local `templetes` folder (k8s YAMLs) to the target machine.

## The original task
```yaml
- name: Copy my local folder to remote
  copy:
    src: /home/khaled/khaled/Project_DevOps/Infra/Ansible/Copy/templetes    # folder on your control machine
    dest: /home/ubuntu/
    owner: ubuntu
    group: ubuntu
    mode: '0755'
```

## Problems & fixes
1. **Hardcoded absolute source path** on the control machine — breaks when the repo moves. Use a path *relative to the role*, or a var.
2. **Trailing slash on `src`** — with `/home/ubuntu/` dest it's fine (copies the *contents* into dest); keep consistent.
3. **Missing dependency note**: dest `/home/ubuntu/` assumes run as/in user `ubuntu` exists.
4. The templates do not end up in a "kube files" location for later deployments — better to place them explicitly.

## Completed version
```yaml
- name: Copy k8s templates to remote
  ansible.builtin.copy:
    src: "{{ copy_source_dir }}"      # e.g. ../templetes relative to the role
    dest: "{{ copy_dest_dir }}"        # e.g. /home/ubuntu/templetes/
    owner: ubuntu
    group: ubuntu
    mode: '0755'
```
with defaults in `defaults/main.yml`:
```yaml
copy_source_dir: roles/Copy/templetes
copy_dest_dir: /home/ubuntu/templetes/
```

Or as a one-shot ad-hoc copy (fastest for a lab):
```bash
scp -i <KEY>.pem -r templetes ubuntu@<BASTION_IP>:~/
```

## Content of the templates (see related md files)
| Template | Purpose |
|---|---|
| `namespace.yml` | Creates namespace `devops` |
| `postgres.yml` | PostgreSQL Service + Deployment for SonarQube |
| `sonarqube.yml` | SonarQube NodePort Service + Deployment |

## Verify
```bash
ansible-playbook playbooks/copy.yml --limit private --check
# on target:
ssh <PRIVATE_IP> 'ls -la /home/ubuntu/templetes/'
```

## Gotchas
- The copy destination is where you will later run `kubectl apply -f templetes/*.yml`.
- Use `ansible.builtin.copy` (fully-qualified) to be lint/policy friendly — the original bare `copy:` still works but triggers warnings.