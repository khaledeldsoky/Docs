# Role: Docker — tasks (`docker/tasks/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/docker`
- **File**: `tasks/main.yaml`
- **What it is**: installs Docker Engine (CE) on Ubuntu via Docker's official apt repo and configures users + service.

## Content
```yaml
- name: Upgrade and update
  ansible.builtin.apt:
    upgrade: true
    update_cache: true

- name: Install dependencies
  ansible.builtin.apt:
    name: "{{ item }}"
    state: present
  loop: "{{ docker_requirment }}"

- name: Add Docker GPG apt Key
  ansible.builtin.apt_key:
    url: https://download.docker.com/linux/ubuntu/gpg
    state: present

- name: Add source repository into sources list
  ansible.builtin.apt_repository:
    repo: "deb [arch=amd64] https://download.docker.com/linux/ubuntu {{ ansible_distribution_release }} stable "
    state: present

- name: Install docker packages
  ansible.builtin.apt:
    name: "{{ item }}"
    state: present
  loop: "{{ docker_packages }}"
  notify: upgrade and update

- name: Start and enable docker
  ansible.builtin.service:
    name: docker
    state: started
    enabled: true

- name: Add the users to docker group
  ansible.builtin.user:
    name: "{{ item }}"
    groups: docker
    append: true
  loop: "{{ users }}"

# - name: open docker.sock to everyone
#   file:
#     path: /var/run/docker.sock
#     mode: 0760
```

See `docker/vars.md` for the `docker_requirment`, `docker_packages`, and `users` variables.

## What each step does
1. Full `apt upgrade` (base sys packages).
2. Install `ca-certificates` + `curl` (GPG/repo tooling).
3. Import Docker's GPG key.
4. Add `docker-ce stable` repo for the detected Ubuntu release.
5. Install `docker-ce`, `docker-ce-cli`, `containerd.io`, buildx + compose plugin; **notify** the handler to refresh cache first.
6. Start + enable the docker service.
7. Add `ubuntu` and `jenkins` users to the `docker` group so they can run docker without sudo (re-login required).

## Completion / improvement notes
- **GPG**: on modern Ubuntu prefer a keyring file instead of `apt_key` (deprecated warnings):
  ```yaml
  - name: Save Docker GPG key
    ansible.builtin.get_url:
      url: https://download.docker.com/linux/ubuntu/gpg
      dest: /etc/apt/keyrings/docker.asc
      mode: '0644'
  - name: Add Docker apt repo
    ansible.builtin.apt_repository:
      repo: "deb [arch=amd64 signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu {{ ansible_distribution_release }} stable"
      state: present
  ```
- **Locked group note**: to use the docker group immediately without logout, run `newgrp docker` on the target.
- The commented docker.sock task is intentionally left out — opening the socket broadly is a security risk. Jenkins gets access by mounting `/var/run/docker.sock` explicitly instead (see `Docker.md`).

## Verify
```bash
docker --version
docker compose version
docker run --rm hello-world
# group check:
id jenkins          # expect: groups=... jenkins
```

## Gotchas
- `{{ ansible_distribution_release }}` makes it distro-correct (noble/jammy/focal) automatically.
- If the private hosts are air-gapped, this role can't reach `download.docker.com` — run the `DNS` role first or mirror the repo.