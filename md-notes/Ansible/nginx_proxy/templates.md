# Role: nginx_proxy — templates (`nginx_proxy/templates/nginx_proxy.conf`)
## Source
- **Folder**: `Docs/Ansible/nginx_proxy`
- **File**: `templates/nginx_proxy.conf`
- **What it is**: the nginx server block that reverse-proxies port 80 to an upstream service.

## Content
```nginx
server {
    listen 80;
    location / {
        proxy_pass http://internal-privateLb-1615617916.us-east-1.elb.amazonaws.com;
    }
}
```

## What it does
Listens on :80 and forwards every request to the backend named in `proxy_pass` — keeping the client's `Host`/IP headers (default behaviour).

## Completion / improvement notes
- The backend is an **AWS internal ELB hostname** — hardcodid in the template on purpose, but as a *template* you can make it a variable:
  ```nginx
  server {
      listen 80;
      server_name <PROXY_DOMAIN_OR_>;

      location / {
          proxy_pass http://{{ nginx_backend }};
          proxy_set_header Host $host;
          proxy_set_header X-Real-IP $remote_addr;
          proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
          proxy_set_header X-Forwarded-Proto $scheme;
      }
  }
  ```
- The forwarded headers (added above) are important for apps that need the real client address (Django, Jenkins behind proxy, etc.).
- A more portable default backend: `proxy_pass http://<PRIVATE_LB_IP>;` or node IP.

## Verify
```bash
nginx -t && systemctl reload nginx
curl -H 'Host: <PROXY_DOMAIN>' http://<BASTION_IP>/ -v   # expect upstream handoff
```

## Gotchas
- `proxy_pass` without a trailing URI keeps the original request path — that's what you want for a plain reverse proxy.
- If the ELB name can't resolve from the bastion, nginx exits on boot (`(98) Address already in use`-style logs) — install the `DNS` role first.