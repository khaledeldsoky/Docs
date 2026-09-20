# Role: Join — Readme (`join/Readme`)
## Source
- **Folder**: `Docs/Ansible/join`
- **File**: `Readme` (no extension)
- **What it is**: the author's own prose explaining two ways to join worker nodes to the cluster.

## The original text (verbatim)
> I use this role to join the worker node to the cluster. There is two ways:
>
> 1. We copy the output coming from the master (join command) to our local machine and apply this output to the worker node — but this way is not secure, the join command has credentials data.
>
> 2. We save the output coming from the master (join command) in ansible cache.

## Why each way works / fails
| Way | How | Risk |
|---|---|---|
| 1. Copy join output locally → push to workers | `kubeadm token create --print-join-command` on master, save to `output.txt`, then run it on each worker | The full **token + CA hash** sits in plaintext on disk/SCP'd around |
| 2. Persist in Ansible cache | `set_fact(..., cacheable: true)` on the master, then workers pull `hostvars['master'].join_cmd` | No plaintext file; needs `fact_caching` configured (`ansible-cfg.md`) |

Way 2 is what the role implements and is the recommended one.

## Completed explanation (what the role's tasks do, in plain English)
1. **Check** if the worker already joined: look for `/etc/kubernetes/kubelet.conf`.
2. **Join**: run the master's saved join command (`hostvars['master'].join_cmd`).
3. **Create** `/home/ubuntu/.kube` directory + empty `config`.
4. **Copy** the master's kubeconfig (`hostvars['master'].kubeconfig_raw`) to `/home/ubuntu/.kube/config`
   → gives the worker `kubectl` access as cluster admin.

## Dependencies
- Roles must run in order: `kubeadm` → `master-config` → `join`.
- Master host must be literally named `master` in the inventory (`hosts.md`).
- `fact_caching = jsonfile` must be on in `ansible.cfg` (`ansible-cfg.md`).

## Gotchas
- The join **token expires** (default 24h) — if `join` runs long after init, regenerate via `kubeadm token create --print-join-command`.
- Running the join command twice on an already-joined worker is an error → the `stat` guard exists for this.