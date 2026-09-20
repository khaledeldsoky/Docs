# Role: kubeadm — base Kubernetes tooling (`kubeadm/tasks/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/kubeadm`
- **File**: `tasks/main.yaml`
- **What it is**: installs kubelet/kubeadm/kubectl (v1.31), configures containerd with systemd cgroups, IPv4 forwarding, bash autocomplete. Runs on **both** master and workers.

## Content
```yaml
- name: Install packages needed
  ansible.builtin.apt:
    name:
      - apt-transport-https
      - ca-certificates
      - curl
      - gpg

- name: Make dir for key
  ansible.builtin.file:
    path: /etc/apt/keyrings
    state: directory
    mode: "0755"

- name: Download the public signing key
  ansible.builtin.apt_key:
    url: https://pkgs.k8s.io/core:/stable:/v1.31/deb/Release.key
    state: present

- name: Add the appropriate Kubernetes apt repository
  ansible.builtin.apt_repository:
    repo: deb https://pkgs.k8s.io/core:/stable:/v1.31/deb/ /
    state: present

- name: Update packages
  ansible.builtin.apt:
    update_cache: true
    force: true

- name: Install kubelet, kubeadm and kubectl
  ansible.builtin.apt:
    name:
      - kubelet
      - kubeadm
      - kubectl

- name: Enable the kubelet service before running kubeadm
  ansible.builtin.service:
    name: kubelet
    enabled: true

# ------- Kube network
- name: Enable IPv4 packet forwarding
  ansible.posix.sysctl:
    name: net.ipv4.ip_forward
    value: '1'
    sysctl_file: /etc/sysctl.d/k8s.conf
    state: present
    reload: true

- name: Install containerd
  ansible.builtin.apt:
    name: containerd
    state: present
    update_cache: true

- name: Generate default containerd config
  ansible.builtin.command: sudo containerd config default
  register: containerd_config
  changed_when: false

- name: Create /etc/containerd directory
  ansible.builtin.file:
    path: /etc/containerd
    state: directory
    owner: root
    group: root
    mode: '0755'

- name: Write containerd config to /etc/containerd/config.toml
  ansible.builtin.copy:
    content: "{{ containerd_config.stdout }}"
    dest: /etc/containerd/config.toml
    owner: root
    group: root
    mode: '0644'

- name: Update SystemdCgroup to true in containerd config
  ansible.builtin.lineinfile:
    path: /etc/containerd/config.toml
    regexp: '^\s*SystemdCgroup\s*=\s*false'
    line: '            SystemdCgroup = true'
    insertafter: '[plugins\."io\.containerd\.grpc\.v1\.cri"\.containerd\.runtimes\.runc\.options]'

- name: Restart containerd
  ansible.builtin.service:
    name: containerd
    state: restarted

# ----------------------- Auto-Complete

- name: Auto Complete kube
  ansible.builtin.apt:
    name: bash-completion
    state: present

- name: Add kubectl completion script to .bashrc
  ansible.builtin.lineinfile:
    path: /home/ubuntu/.bashrc
    line: 'source <(kubectl completion bash)'

- name: Apply changes to .bashrc
  ansible.builtin.shell: 'source /home/ubuntu/.bashrc'
  args:
    executable: /bin/bash
  register: my_output
  changed_when: false

# ----------------------- reload Daemon
- name: Reload Daemon
  ansible.builtin.systemd_service:
    daemon_reload: true
```

## The one thing that makes the whole cluster work
`SystemdCgroup = true` — without it, containerd and kubelet fight over cgroup drivers and pods never become Ready. The `lineinfile` flips the default containerd config. (Modern k8s prefers the `SystemdCgroup = true` path.)

## Prerequisites
`ansible.posix` collection on the control node:
```bash
ansible-galaxy collection install ansible.posix
```

## Completion / improvement notes
- **Held packages**: pin kube* so `apt upgrade` can't bump the version mid-cluster:
  ```yaml
  - name: Hold kubeadm kubelet kubectl
    ansible.builtin.dpkg_selections:
      name: '{{ item }}'
      selection: hold
    loop: [kubelet, kubeadm, kubectl]
  ```
- **Requires swap off + kernel modules** for full bootstrap; add the standard preflight tasks (`swapoff -a`, `modprobe br_netfilter`) if targets fail `kubeadm init` pre-flight.
- **arch**: the repo is for amd64/arm64 via dpkg arch; fine for this lab.
- Version pinned at **1.31** in the repo URL — bump deliberately when you want a new minor.
- `.bashrc` edit only affects *new* shells; the `source` in the task applies it for the run only.

## Verify
```bash
kubeadm version
kubelet --version
containerd config dump | grep -i SystemdCgroup    # expect: SystemdCgroup = true
```

## Gotchas
- This role installs prerequisites only — it does **not** run `kubeadm init` (that's `master-config`).
- If `containerd` masks the runc config path, the `insertafter` regex may need the exact option block to match (it does for containerd 1.7 / 2.x).