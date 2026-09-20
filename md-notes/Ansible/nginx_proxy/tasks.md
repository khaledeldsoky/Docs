# Role: nginx_proxy — tasks (`nginx_proxy/tasks/main.yaml`)
## Source
- **Folder**: `Docs/Ansible/nginx_proxy`
- **File**: `tasks/main.yaml`
- **What it is**: installs nginx on the bastion/proxy machine and enables a reverse-proxy vhost that forwards to a private/ELB backend.

## Content
```yaml
- name: update
  ansible.builtin.apt:
    update_cache: yes

- name: Install apache nginx
  ansible.builtin.apt:
    name: nginx
    state: present
  notify: update

- name: start and enable nginx
  ansible.builtin.service:
    name: nginx
    state: started

- name: Disable the Default Virtual Host
  ansible.builtin.file:
    path: /etc/nginx/sites-enabled/default
    state: absent

- name: Copy Nginx configuration template
  ansible.builtin.template:
    src: /mnt/linux/data/project/final-project/ansible/nginx_proxy/templates/nginx_proxy.conf
    dest: /etc/nginx/sites-available/proxy.conf

- name: Enable proxy site
  ansible.builtin.file:
    src: /etc/nginx/sites-available/proxy.conf
    dest: /etc/nginx/sites-enabled/proxy.conf
    state: link

- name: Restart Nginx
  ansible.builtin.service:
    name: nginx
    state: restarted
  register: nginx_restart_result
  ignore_errors: yes
```

## What it does, step by step
1. Refresh apt cache.
2. Install nginx (`notify: update` — redundant but harmless).
3. Start service.
4. Remove the default site so only your vhost listens.
5. Render the template (`nginx_proxy/templates/nginx_proxy.conf`) into `sites-available/proxy.conf`.
6. Symlink it into `sites-enabled` (that's how nginx enables a site).
7. Restart nginx. `ignore_errors: yes` + `register` — keeps the play alive even if a bad config would make nginx fail to start (the handler also restarts, so the rescue path is both).

## The bug fixed in completion
The `template.src` points at an **absolute path on the control machine's old mount**:
```yaml
src: /mnt/linux/data/project/final-project/ansible/nginx_proxy/templates/nginx_proxy.conf
```
That doesn't exist in this repo layout. A template's `src` should be passed the **relative role path**, e.g. `src: nginx_proxy.conf` (Ansible resolves it relative to the role's `templates/` dir). Fixed:
```yaml
- name: Copy Nginx configuration template
  ansible.builtin.template:
    src: nginx_proxy.conf
    dest: /etc/nginx/sites-available/proxy.conf
```

## Verify
```bash
nginx -t                              # config syntax OK
systemctl status nginx
curl -I http://localhost/              # expect the proxied app's headers
ls -l /etc/nginx/sites-enabled/proxy.conf   # symlink exists
```

## Gotchas
- `sites-enabled` symlink pattern is Debian/Ubuntu-specific; on RHEL use `/etc/nginx/conf.d/`.
- Backend in the template is an internal ELB (`internal-privateLb-1615617916.us-east-1.elb.amazonaws.com`) — that's AWS-specific; replace with your private LB or node IP in `templates.md`.
- If nginx has `proxy_pass` to an unresolvable hostname, `nginx -t` may pass but the service fails to start on boot (DNS!). See `DNS.md`.