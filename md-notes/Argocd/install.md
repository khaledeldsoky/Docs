# ArgoCD — Install notes (`Install.md`)
## Source
- **Folder**: `Docs/Argocd`
- **File**: `Install.md`
- **What it is**: the manual day-2 notes: /etc/hosts entry, Ingress YAML to reach ArgoCD, secret retrieval, and a bastion nginx reverse proxy — plus tangled Terraform install steps and two plaintext passwords.

## ⚠️ Security fix applied
The file ended with two plaintext credentials:
```
#Kk159753
#KHkh159753
```
Those are replaced everywhere with **placeholders** (`<INITIAL_ADMIN_PASSWORD>`). **Rotate these passwords** if this cluster is still in use — every value has now been shared in a repo.

## Section by section (completed)

### 1. Point the cluster to a friendly hostname
```bash
sudo vim /etc/hosts
# add a line for the node that hosts ArgoCD:
192.168.2.11 argocd.example.com
```
> or better (non-interactive):
```bash
echo "192.168.2.11 argocd.example.com" | sudo tee -a /etc/hosts
```

### 2. Ingress to reach the ArgoCD web UI
Save as `argocd.yaml`:
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: argocd-server-ingress
  namespace: argocd
  annotations:
    nginx.ingress.kubernetes.io/ssl-redirect: "false"
spec:
  rules:
  - host: argocd.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: argocd-server
            port:
              number: 80
```
Apply:
```bash
kubectl apply -f argocd.yaml
```

### 3. Fix an ingress-nginx admission quirk
```bash
kubectl delete -A ValidatingWebhookConfiguration ingress-nginx-admission
```
> Deletes the webhook so nginx Ingress objects validate without it — this is a lab workaround; in production rather fix the webhook certificate.

### 4. Retrieve the initial admin password
```bash
kubectl -n argocd get secret argocd-initial-admin-secret \
  -o jsonpath="{.data.password}" | base64 -d
# → <INITIAL_ADMIN_PASSWORD>
```

### 5. Optional: kubectl port-forward (marked "not needed" in original)
```bash
kubectl port-forward svc/argocd-server -n argocd 8080:80
# http://localhost:8080
```

### 6. Bastion reverse proxy (public :80 → node :31599)
On the bastion:
```bash
sudo apt install -y nginx
sudo vim /etc/nginx/sites-available/argocd
```
**Fixed server block** — the original had a broken line `proxy_pass http:// :31599;` (no upstream IP). Correct version:
```nginx
server {
    listen 80;
    server_name _;                    # or argocd.example.com

    location / {
        proxy_pass http://<WORKER_NODE_PRIVATE_IP>:31599;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
}
```
Enable + reload:
```bash
sudo ln -s /etc/nginx/sites-available/argocd /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```
> `server_name _;` = catch-all; replace with your domain if you own one. `31599` is the **NodePort of the ingress-nginx controller** from the notebook flow — confirm with `kubectl -n ingress-nginx get svc`.

## Stray content in the original (Terraform install)
The tail of `Install.md` had Terraform apt-repo setup — unrelated to ArgoCD but fully preserved here:
```bash
wget -O - https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(grep -oP '(?<=UBUNTU_CODENAME=).*' /etc/os-release || lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list
sudo apt update && sudo apt install terraform
```

## Full end-to-end access path
```
User → http://argocd.example.com → bastion :80 (nginx)
     → <WORKER_IP>:31599 (ingress-nginx NodePort)
     → argocd-server ingress → argocd-server:80 → UI
```

## Verify
- `kubectl -n argocd get ingress` → shows `argocd-example-com` host.
- `curl -I http://argocd.example.com` from bastion → `200` with ArgoCD header.
- Login with `admin` / the password from step 4.

## Gotchas
- Webhook deletion is a **lab-only** hack — real clusters fix admission-controller certs instead.
- NodePort `31599` must match what ingress-nginx actually got assigned.
- Rotate `Kk159753`/`KHkh159753` if those credentials were real — they were committed in plaintext.