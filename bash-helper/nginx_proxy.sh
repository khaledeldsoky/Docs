cat <<OEF > <PATH_TO_NGINX_FILE>/nginx_proxy.conf
server {
    listen 80;
    location / {
        proxy_pass http://$1;
    }
}
OEF