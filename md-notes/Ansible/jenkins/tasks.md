# Role: Jenkins — tasks (`jenkins/tasks/main.yml`)
## Source
- **Folder**: `Docs/Ansible/jenkins`
- **File**: `tasks/main.yml`
- **What it is**: installs OpenJDK 17 + Jenkins (stable) from the official Jenkins apt repo, enables + starts the service.

## Content
```yaml
- name: Update
  ansible.builtin.apt:
    update_cache: true

- name: Install jenkins requirements
  ansible.builtin.apt:
    name: "{{ item }}"
    state: present
  loop: "{{ jenkins_var }}"

- name: Download and import the Jenkins repository key
  ansible.builtin.get_url:
    url: https://pkg.jenkins.io/debian-stable/jenkins.io-2023.key
    dest: /usr/share/keyrings/jenkins-keyring.asc
    mode: 777

- name: Setting up the Jenkins repository as a package source...
  ansible.builtin.apt_repository:
    repo: "deb [signed-by=/usr/share/keyrings/jenkins-keyring.asc]  https://pkg.jenkins.io/debian-stable binary/"
    filename: jenkins.list
    state: present
  notify: Update

- name: Install java
  ansible.builtin.apt:
    name:
      - fontconfig
      - openjdk-17-jre
  notify: Update

- name: Install jenkins
  ansible.builtin.apt:
    name: jenkins
    state: present

- name: Enable and start jenkins
  ansible.builtin.service:
    name: jenkins
    enabled: true
    state: started
```

`jenkins_var` comes from `jenkins/vars/main.yaml` → `openjdk-17-jdk`, `openssh-server`.

## What each step does
1. Refresh apt cache.
2. Install JDK 17 + ssh-server (the `openssh-server` is a leftover — Jenkins doesn't need it).
3. Fetch Jenkins' signing key into a keyring (`jenkins.io-2023.key`).
4. Register the Jenkins **stable** Debian repo.
5. Install JRE 17 (runtime for the war).
6. Install the `jenkins` package.
7. Enable + start the `jenkins` service (listen :8080).

## Completion / improvement notes
- **200, getter_domain-wise**: key URL rotates — newer setups use `https://pkg.jenkins.io/debian/jenkins-keyring.asc`. Keep the 2023 key if it still fits your distro; review yearly.
- **Versions**: `openjdk-17` matches modern Jenkins LTS. Newer LTS (2.426+) move to Java 17; fine.
- **Service port / GUI**: after start, unlock with the initial admin password:
  ```bash
  sudo cat /var/lib/jenkins/secrets/initialAdminPassword
  ```
- **Docker-in-Jenkins**: give the jenkins user docker access and mount the socket (see `Docker.md`).

## Verify
```bash
systemctl status jenkins
curl -I http://localhost:8080
java -version
```

## Gotchas
- `mode: 777` on a keyring is loose; `0644` suffices for apt signed-by.
- Jenkins repo + Docker repo must not fight over `apt_update` ordering — `notify: Update` handles it per-play.
- Only Debian-based targets; RHEL hosts need a different repo URL.