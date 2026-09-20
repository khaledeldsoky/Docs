# Docker notes (`Docker.ipynb`)
## Source
- **Folder**: `Docs/Docker`
- **File**: `Docker.ipynb`
- **What it is**: study notes on Docker storage, images, and the CRI/CNI/CSI acronyms, plus ready-to-run build/Jenkins snippets.

## 1. Docker storage

### Volumes (managed by Docker)
```bash
docker volume create data_volume
# → stored under /var/lib/docker/volumes/data_volume
docker run -v data:/var/lib/mysql
```
- Data survives container death/removal (`docker rm`).
- The volume name is just `data` — Docker resolves it to `/var/lib/docker/volumes/data`.

### Bind mounts (your own directory)
```bash
mkdir /data/mysql
# must write the FULL path to bind (absolute path)
docker run -v /data/mysql:/var/lib/mysql
```
- Shares a host folder directly with the container; changes reflect both ways.
- Any file under bind mount appearing in container hides underlying image content at that path.

> 📁 Volume vs Bind: **named volume = Docker manages storage** (safe, portable, `docker volume` commands); **bind mount = you provide the path** (useful for configs/dev).

## 2. Images

### Commit a running container into an image
```bash
docker commit id_container myUbuntuVersion:1.0
```

## 3. The acronyms (what talks to what)
| Acronym | Full name | Handled by |
|---|---|---|
| CRI | Container Runtime Interface | runtime fetch/unpack (containerd) |
| CNI | Container Network Interface | networking between containers/pods |
| CSI | Container Storage Interface | storage provisioning for containers |

## 4. Build & run an app
```bash
docker build -t <dockerhub_user>/<image_name> .
docker build -t khaledmohamedatia/app .

docker run -itd --name <container_name> \
  -p <expose_port>:<docker_port> <dockerhub_user>/<image_name>
docker run -itd --name app -p 8087:80 khaledmohamedatia/app
```
- `-itd`: interactive + tty + detached.
- `-p 8087:80` maps host 8087 → container 80.

## 5. Jenkins inside Docker (with Docker access)
```bash
docker run -itd \
  --name jenkins \
  -v /path/you/want/to/map/:/var/jenkins_home:Z \
  -v /var/run/docker.sock:/var/run/docker.sock:Z \
  -p 8001:8080 -p 50000:50000 \
  jenkins_docker_client_im

docker exec -it jenkins bash
```
- `:Z` = SELinux relabel (needed on Fedora/RHEL/Ubuntu-with-SELinux hosts).
- `/var/run/docker.sock` mount lets Jenkins run `docker` directly (docker-outside-of-docker). See the Ansible jenkins/docker roles for provisioning.

## Completion notes
- Volumes can also name their mount path as an **absolute container path**; the shorthand `-v name:path` is the volume form.
- To see/clean storage: `docker volume ls`, `docker volume prune`.
- Port publishing collision: add `-p` per port; `-p 8001:8080` for UI + `-p 50000:50000` for JNLP agents.

## Verify
```bash
docker volume ls
docker ps --filter name=app
docker logs -f app
```

## Gotchas
- Reusing the same volume after `docker run` gives you the previous data — intended, but surprising on redeploys.
- `docker commit` is fine for quick snapshots; production should use a Dockerfile (`docker build`).