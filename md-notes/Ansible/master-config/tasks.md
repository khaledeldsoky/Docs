# Role: master-config — init the cluster (`master-config/tasks/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/master-config`
- **File**: `tasks/main.yaml`
- **What it is**: runs `kubeadm init`, saves the join command as a cached fact, sets up kubectl for user `ubuntu`, and installs Calico CNI.

## Content
```yaml
- name: Initialize Kubernetes master
  ansible.builtin.command: kubeadm init --pod-network-cidr=10.244.0.0/16
  args:
    creates: /etc/kubernetes/admin.conf
  register: initialize
  changed_when: false

- name: Save kubeadm join command to file
  ansible.builtin.command: kubeadm token create --print-join-command
  register: join_command
  changed_when: false

# (commented out: copy the join command to ./output.txt on the control node — the insecure approach)

- name: Save join command in ansible cacheable
  ansible.builtin.set_fact:
    join_cmd: "{{ join_command.stdout }}"
    cacheable: true

- name: Make dir for kubeadm config
  ansible.builtin.file:
    state: directory
    path: "/home/ubuntu/.kube"
    mode: "0755"

- name: Copy config from etc to user dir
  ansible.builtin.copy:
    src: /etc/kubernetes/admin.conf
    dest: "/home/ubuntu/.kube/config"
    mode: '0644'
    remote_src: true
    owner: "ubuntu"
    group: "ubuntu"

- name: Read kubeconfig from master
  ansible.builtin.command: cat /etc/kubernetes/admin.conf
  register: kubeconfig_raw
  changed_when: false

- name: Set fact with kubeconfig content
  ansible.builtin.set_fact:
    kubeconfig_content: "{{ kubeconfig_raw.stdout }}"

- name: Apply Calico
  ansible.builtin.command:
    cmd: kubectl apply -f https://raw.githubusercontent.com/projectcalico/calico/v3.27.5/manifests/calico.yaml
  environment:
    KUBECONFIG: /etc/kubernetes/admin.conf
  changed_when: false
```

## Facts it publishes for the `join` role
| Fact | Content | Consumed by |
|---|---|---|
| `join_cmd` (cacheable) | `kubeadm join <api> --token ... --discovery-token-ca-cert-hash ...` | `join/tasks` → runs on workers |
| `kubeconfig_content` | the full `admin.conf` text | `join/tasks` → written to worker `.kube/config` |

## Details
- **Network**: `--pod-network-cidr=10.244.0.0/16` matches Calico's default IP pool. Don't change one without the other.
- **Idempotency**: `creates:` on init + `changed_when: false` on everything mean the play is safe to re-run; after a real init, kubeadm is skipped.
- **Kubeconfig for ubuntu**: copies `/etc/kubernetes/admin.conf` so the `ubuntu` user can `kubectl` without sudo.
- **Calico v3.27.5**: pinned manifest (matches the era of this cluster; v3.27 is fine for 1.31).

## Completion / improvement notes
- `kubeadm init` needs the control-plane IP/API endpoint; for a single master it defaults to the node IP. For HA you'd add `--control-plane-endpoint <LB_IP>:6443 --upload-certs`.
- The `kubectl apply` uses `KUBECONFIG=/etc/kubernetes/admin.conf` inline — good for root; the ubuntu user gets theirs from the copy step.
- **cacheable fact caveat**: `set_fact(cacheable: true)` only persists when `fact_caching = jsonfile` is enabled in `ansible.cfg` (`ansible-cfg.md`). Without it, `join` runs in the *same* play run fine, but a separate later play-run can't see the facts.

## Verify
```bash
# on the master:
kubectl get nodes        # expect master Ready (calico pending a few minutes)
kubectl -n kube-system get pods | grep calico
kubeadm token list
```

## Gotchas
- The join token **expires in 24h** by default; re-create with `kubeadm token create --print-join-command` before joining later nodes.
- If Calico can't reach `raw.githubusercontent.com` (DNS!), run the `DNS` role first (`DNS.md`).
- `--pod-network-cidr` must be free of any real subnet overlap on your network, or routing breaks.