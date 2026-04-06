

```sh

```



```sh

```


```sh

```

---------------------------------------------------------------------------




---------------------------------------------------------------------------








sudo vim /etc/hosts

## ip for node
192.168.2.11 argocd.example.com 

---

vim argocd.yaml

apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: argocd-server-ingress
  namespace: argocd
  annotations:
    nginx.ingress.kubernetes.io/ssl-redirect: "false"
spec:
  rules:
  - host: argocd.example.com   # change this to your DNS or /etc/hosts entry
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: argocd-server
            port:
              number: 80

kubectl apply -f argocd.yaml 

---

kubectl delete -A ValidatingWebhookConfiguration ingress-nginx-admission

---

kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d

---

# not needed
kubectl port-forward svc/argocd-server -n argocd 8080:80

# on bastion 


sudo apt install -y nginx
sudo vim /etc/nginx/sites-available/argocd
server {
    listen 80;
    server_name _;   # or argocd.example.com if you have a domain

    location / {
        proxy_pass http://                 :31599;   # worker private IP + NodePort
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
}

sudo ln -s /etc/nginx/sites-available/argocd /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx

```sh
wget -O - https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg

echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(grep -oP '(?<=UBUNTU_CODENAME=).*' /etc/os-release || lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list

sudo apt update && sudo apt install terraform
```
#Kk159753
#KHkh159753