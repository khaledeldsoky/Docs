# bash-helper: `nginx_proxy.sh` — reverse-proxy config generator
## Source
- **Folder**: `Docs/bash-helper`
- **File**: `nginx_proxy.sh`
- **What it is**: generates an nginx `server` block that proxies everything on :80 to a backend given as `$1`.

## Original content
```bash
cat <<OEF > <PATH_TO_NGINX_FILE>/nginx_proxy.conf
server {
    listen 80;
    location / {
        proxy_pass http://$1;
    }
}
OEF
```

## Completed version
```bash
#!/bin/bash
# Usage: ./nginx_proxy.sh <BACKEND_URL_OR_IP> [CONF_PATH]
BACKEND="${1:?backend URL required}"
OUT="${2:-/tmp/nginx_proxy.conf}"

cat <<EOF > "$OUT"
server {
    listen 80;
    server_name _;

    location / {
        proxy_pass http://$BACKEND;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
}
EOF
echo "nginx config written to $OUT"
```
Changes: arg validation, output path param, sanitised `$host` refs (escaped `\$` so nginx — not bash — expands them), and proper forwarding headers that apps expect.

## Install the generated config (Debian/Ubuntu)
```bash
./nginx_proxy.sh internal-privateLB.example.com /tmp/nginx_proxy.conf
sudo cp /tmp/nginx_proxy.conf /etc/nginx/sites-available/proxy.conf
sudo ln -s /etc/nginx/sites-available/proxy.conf /etc/nginx/sites-enabled/ 2>/dev/null || true
sudo rm -f /etc/nginx/sites-enabled/default
sudo nginx -t && sudo systemctl reload nginx
```

## Verify
```bash
nginx -t
curl -H 'Host: example.com' http://localhost/ -v
```

## Gotchas
- **Escaping**: `proxy_set_header Host $host;` — `$host` is an *nginx* variable; in the heredoc it must be `\$host` so bash doesn't empty it.
- If `$1` is an IP, add `http://` in the call or `proxy_pass http://$BACKEND;` doubles it — call it as `./nginx_proxy.sh 10.0.2.160:31599`.
- On RHEL the config dir is `/etc/nginx/conf.d/*.conf`, not `sites-available/enabled`.