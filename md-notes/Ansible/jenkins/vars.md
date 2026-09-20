# Role: Jenkins — variables (`jenkins/vars/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/jenkins`
- **File**: `vars/main.yaml`
- **What it is**: the apt packages Jenkins prerequisites need.

## Content
```yaml
jenkins_var:
    - openjdk-17-jdk
    - openssh-server
```

## Notes
- `openjdk-17-jdk` is installed early (build/compat tooling), and `openjdk-17-jre` again in tasks (harmless duplicate; JRE is a subset of JDK, apt dedupes).
- **`openssh-server` is not needed by Jenkins** — it comes along from the original lab intent (ssh into targets). Safe to remove from this list if the machine already has sshd (it almost always does).

## Completion
```yaml
jenkins_packages:
  - openjdk-17-jdk
```
Rename to `jenkins_packages` while you're touching tasks — but remember both files must change together (`tasks/main.yml` loops over `jenkins_var`).

## Gotchas
- Keep the name consistent: if you only edit `vars/`, playback fails with `'jenkins_var' is undefined`.