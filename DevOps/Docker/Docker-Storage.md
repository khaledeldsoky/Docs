# Docker Storage

📁 /var/lib/docker/volumes

### ▶️ Volume mounting :

```sh 
docker volume create data_volume
# 📁 /var/lib/docker/volumes/data_volume
docker run -v data:/var/lib/mysql
```

▶ if the container is die , the data is still found

---
### ▶️ bind mount

```sh
mkdir /data/mysql
# 📁 /data/mysql
# write the full path for the file
docker run -v /data/mysql:/var/lib/mysql
```
---
🔎 Storage drive 🆚 Volume drive 

