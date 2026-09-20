# Role: Docker — variables (`docker/vars/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/docker`
- **File**: `vars/main.yaml`
- **What it is**: the package lists + user list consumed by the docker role tasks.

## Content
```yaml
docker_requirment:
  - ca-certificates
  - curl

docker_packages:
  - docker-ce
  - docker-ce-cli
  - containerd.io
  - docker-buildx-plugin
  - docker-compose-plugin

users:
  - ubuntu
  - jenkins
```

## What each variable drives
| Variable | Used by | Meaning |
|---|---|---|
| `docker_requirment` | task 2 (loop) | pre-reqs installed before the repo is added |
| `docker_packages` | install task (loop) | the actual Docker Engine packages |
| `users` | group task (loop) | OS users granted `docker` group membership |

## Completion notes
- `docker_requirment` and `docker_packages` could live in `defaults/` if you want to override them per environment; `vars/` is fine here since they rarely change.
- **Mirrors**: for air-gapped/alternative registries you could add repo overrides, but the packages themselves stay the same.

## Gotchas
- The variable is spelled `docker_requirment` (typo kept on purpose? it's used consistently in `tasks/main.yaml` — do **not** rename one side without the other, or the loop breaks with an *undefined variable* error). A cleaner name would be `docker_requirements`, but both files must be renamed together.