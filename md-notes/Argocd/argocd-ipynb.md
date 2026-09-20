# ArgoCD — Notebook install + troubleshooting (`Argocd.ipynb`)
## Source
- **Folder**: `Docs/Argocd`
- **File**: `Argocd.ipynb`
- **What it is**: the original cell-by-cell recipe: install ArgoCD, fix DNS on the nodes, and add ingress-nginx on the master (private-subnet workaround).

## The full flow (completed from the notebook)

### 1. Install ArgoCD (namespace + official manifest + NodePort)
```bash
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
kubectl -n argocd patch svc argocd-server -p '{"spec": {"type": "NodePort"}}'
```
> Patching to **NodePort** is required when there is **no cloud LoadBalancer** available (no `elb/lb`) — the private-subnet cluster can't allocate a public LB.

### 2. Fix DNS (private nodes couldn't resolve the manifest URLs)
```bash
# edit /etc/resolv.conf  → add the public resolvers:
#   nameserver 8.8.8.8
#   nameserver 1.1.1.1
sudo systemctl restart systemd-resolved
```
> In the notebook these two `nameserver` lines are typed as if they *are* shell commands there — in a real shell you write them **inside** `/etc/resolv.conf`, not at the prompt. The proper idempotent way is the `DNS` Ansible role (`Docs/Ansible/DNS`).

### 3. Problem: master sits on the private subnet → no public ingress
The notebook's idea: install **ingress-nginx on the master** and expose it via NodePort.
```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.9.5/deploy/static/provider/cloud/deploy.yaml

# change the ingress-nginx Service to NodePort:
kubectl -n ingress-nginx edit svc ingress-nginx-controller
#   spec:
#     type: NodePort

# see the assigned NodePorts:
kubectl -n ingress-nginx get svc ingress-nginx-controller
```

## Full working recipe this completes into
1. **Cluster-side** (from a machine with kubectl access):
   - Apply ArgoCD manifests, patch svc → NodePort.
   - Install ingress-nginx (`provider/cloud/deploy.yaml` is fine — the Service type is what matters), switch it to NodePort.
2. **DNS**: ensure nodes resolve `argocd` registrar + GitHub (DNS role / resolv.conf above).
3. **Route to ArgoCD**: create an Ingress in `argocd` ns pointing at `argocd-server:80` (see `Docs/Argocd/Install.md`).
4. **Access**: browser → `http://<NODE_IP>:<NGINX_NODEPORT>` → ArgoCD UI.

## Get the admin password
```bash
kubectl -n argocd get secret argocd-initial-admin-secret \
  -o jsonpath="{.data.password}" | base64 -d
```

## Verify
```bash
kubectl -n argocd get pods        # argocd-server, -repo-server, -redis, -applications-controller Running
kubectl -n argocd get svc argocd-server   # note the NodePort
kubectl -n ingress-nginx get svc ingress-nginx-controller
```

## Gotchas / fixes applied
- **Manifest in the private network**: `raw.githubusercontent.com` must resolve (DNS fix step).
- **NodePort choice**: pick a port not used by other services (ingress-nginx 30000–32767 range).
- **No LoadBalancer**: that's the whole reason for NodePort + reverse proxy; the bastion nginx config in `Install.md` bridges public :80 → node NodePort.