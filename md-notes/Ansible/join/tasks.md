# Role: Join — tasks (`join/tasks/main.yml`)
## Source
- **Folder**: `Docs/Ansible/join`
- **File**: `tasks/main.yml`
- **What it is**: joins worker nodes to the cluster using facts cached from the master.

## Content (the active/second strategy)
```yaml
- name: Check if worker already joined
  ansible.builtin.stat:
    path: /etc/kubernetes/kubelet.conf
  register: kubelet_conf

- name: Join cluster
  ansible.builtin.command: "{{ hostvars['master'].join_cmd }}"
  register: join_cluster
  changed_when: join_cluster.rc == 0

- name: Make dir for kubeadm config
  ansible.builtin.file:
    state: directory
    path: "/home/ubuntu/.kube"
    mode: "0755"

- name: Make file for kubeadm config
  ansible.builtin.file:
    state: touch
    path: "/home/ubuntu/.kube/config"
    mode: '0644'

- name: Copy kubeconfig content to worker
  ansible.builtin.copy:
    dest: /home/ubuntu/.kube/config
    content: "{{ hostvars['master'].kubeconfig_raw }}"
    owner: ubuntu
    group: ubuntu
    mode: '0644'
```

Commented out at the top is Strategy 1 (read local `output.txt` containing the join command — the insecure path).

## The bug fixed in completion
`master-config/tasks/main.yaml` stores the kubeconfig fact as **`kubeconfig_content`**:
```yaml
- name: Set fact with kubeconfig content
  ansible.builtin.set_fact:
    kubeconfig_content: "{{ kubeconfig_raw.stdout }}"
```
But this role reads **`kubeconfig_raw`** — a historical fact name that was never cached. Result: on the worker, `hostvars['master'].kubeconfig_raw` is **undefined** and the copy task fails (`undefined variable`).

Fix: read the fact the master actually saves:
```yaml
    content: "{{ hostvars['master'].kubeconfig_content }}"
```
(Or rename the fact in `master-config` to `kubeconfig_raw` — either way, make both sides agree.)

## Completed version with the guard fixed
```yaml
- name: Check if worker already joined
  ansible.builtin.stat:
    path: /etc/kubernetes/kubelet.conf
  register: kubelet_conf

- name: Join cluster
  ansible.builtin.command: "{{ hostvars['master'].join_cmd }}"
  become: true
  when: not kubelet_conf.stat.exists          # don't rejoin an already-joined node
  changed_when: false

- name: Copy kubeconfig to worker
  ansible.builtin.copy:
    dest: /home/ubuntu/.kube/config
    content: "{{ hostvars['master'].kubeconfig_content }}"
    owner: ubuntu
    group: ubuntu
    mode: '0644'
```
The original `stat` result was registered but **never used** in a `when:` — that's why the original could re-run the join. Added `when: not kubelet_conf.stat.exists` + `become: true` (join needs root).

## Dependencies & prerequisites
- Run after `master-config` in the same play run (or a later play, thanks to cache).
- `fact_caching = jsonfile` configured (`ansible-cfg.md`).
- Master host literally named `master` in inventory.
- `/home/ubuntu` must exist on the worker (role runs as the `ubuntu` user context).

## Verify
```bash
kubectl get nodes                      # run from master after join
# on the worker:
sudo kubeadm reset --force             # cleanup a bad join, then rerun the role
```

## Gotchas
- If workers see `Error: token expired`, regenerate: `kubeadm token create --print-join-command`, then re-cache.
- `hostvars['master']` requires the master to have **finished** its play in this run — if the join-only play targets workers first, the fact is stale/absent.