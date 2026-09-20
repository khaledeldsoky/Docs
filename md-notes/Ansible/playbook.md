# Main Playbook (`playbook.yaml`)

## Source
- **Folder**: `Docs/Ansible`
- **File**: `playbook.yaml`
- **What it is**: the top-level entry point that maps **groups → roles**.

## The original file
```yaml
- name: Bastion
  hosts: bastion
  roles:
    # - jenkins
    # - docker
    - nginx_proxy

- name: Private
  hosts: private
  roles:
    # - jenkins
    # - docker
    # - nginx_proxy
    # - http
    - run_scripts
```

## How it works
Two plays run in order:
1. **Bastion** → installs `nginx_proxy` (reverse proxy on the jump host).
2. **Private** → runs `run_scripts` (deploys the web app).

Everything else is commented out — the roles `jenkins`, `docker`, `nginx_proxy`, and the missing `http` reside in this repo but aren't activated.

## Completed version — full topology
```yaml
- name: Bastion
  hosts: bastion
  roles:
    - nginx_proxy       # reverse proxy into private services

- name: Master
  hosts: master
  roles:
    - kubeadm           # container runtime + k8s tooling
    - master-config     # kubeadm init, join token, calico

- name: Workers
  hosts: workers
  roles:
    - kubeadm           # container runtime + k8s tooling
    - docker
    - jenkins
    - join              # joins the cluster using the master's token

- name: Private
  hosts: private
  roles:
    - docker
    - jenkins
    - nginx_proxy
    - run_scripts
```

> `run_scripts` must run **after** DNS/Docker/Jenkins dependencies exist in the private group; order in the list is order of execution.

## Notes on the original
- The play `hosts: private` also matches the master if the master is a private instance — use the dedicated `[master]`/`[workers]` groups (see `hosts.md`) to avoid double-provisioning.
- The commented `# - http` role was never created.

## Run it
```bash
ansible-playbook playbook.yaml
# limit to one group or a single host:
ansible-playbook playbook.yaml --limit bastion
ansible-playbook playbook.yaml --check        # dry run of changes
```

## Verify
```bash
ansible-playbook playbook.yaml --list-hosts    # shows what will run and on whom
ansible-playbook playbook.yaml --syntax-check
```

## Gotchas
- Play order matters: bastion first (so private hosts have a working tunnel), k8s master before workers.
- Cert/dep joining (`join` role) requires `fact_caching` enabled — see `ansible-cfg.md`.