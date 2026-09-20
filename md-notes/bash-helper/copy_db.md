# bash-helper: `copy_db.sh` — web_app.sh generator
## Source
- **Folder**: `Docs/bash-helper`
- **File**: `copy_db.sh`
- **What it is**: generates a remote bootstrap script (`web_app.sh`) that deploys a Django app and injects database settings via `sed`.

## What the generated script does
1. `apt update` + install Python/pip, MySQL dev libs & build tools.
2. Clone `https://github.com/khaledeldsoky/SimpleDjangoApp.git`.
3. `pip3 install -r requirements.txt`.
4. `sed`-replace 4 placeholders in `config/settings.py` (database_name/username/password/host).
5. `migrate`.
6. Launch `manage.py runserver 0.0.0.0:80` in background.

## Original content
```bash
cat <<EOF > /mnt/linux/data/project/final-project/bash_script/web_app.sh
#!/bin/bash
sudo apt update
sudo apt install -y python3-pip > /dev/null 2>&1
sudo apt install python3-dev default-libmysqlclient-dev build-essential pkg-config -y > /dev/null 2>&1
git clone https://github.com/khaledeldsoky/SimpleDjangoApp.git /home/ubuntu/project
cd /home/ubuntu/project/
sudo pip3 install -r requirements.txt
sed -i "s/database_name/$1/g" config/settings.py
sed -i "s/database_username/$2/g" config/settings.py
sed -i "s/database_password/$3/g" config/settings.py
sed -i "s/database_host/$4/g" config/settings.py
sudo python3 manage.py migrate
sudo nohup python3 manage.py runserver 0.0.0.0:80 
EOF
```

## How to use it
```bash
# generates the script using positional args $1..$4 (db name, user, password, host):
./copy_db.sh mydb myuser mypass dbhost.example.com
# web_app.sh is now created at the path inside the heredoc
```

## Completed version (portable, no hardcoded path)
```bash
#!/bin/bash
# Usage: ./copy_db.sh <DB_NAME> <DB_USER> <DB_PASSWORD> <DB_HOST> [OUTPUT_PATH]
OUT="${5:-/tmp/web_app.sh}"

cat <<EOF > "$OUT"
#!/bin/bash
set -e
sudo apt update
sudo apt install -y python3-pip python3-dev default-libmysqlclient-dev build-essential pkg-config
git clone https://github.com/khaledeldsoky/SimpleDjangoApp.git /home/ubuntu/project || true
cd /home/ubuntu/project/
sudo pip3 install -r requirements.txt
sed -i "s/database_name/$1/g" config/settings.py
sed -i "s/database_username/$2/g" config/settings.py
sed -i "s/database_password/$3/g" config/settings.py
sed -i "s/database_host/$4/g" config/settings.py
sudo python3 manage.py migrate
sudo nohup python3 manage.py runserver 0.0.0.0:80 &
EOF

chmod +x "$OUT"
echo "Generated: $OUT"
```
Changes: configurable output path, `set -e`, idempotent clone (`|| true`), ampersand instead of trailing-space newline for nohup, explicit usage comment.

## Security notes
- DB password is **embedded in the generated script and in shell history** (`$3`). Prefer env vars/`--password-stdin` or `sed` from a `.env` that you `chmod 600`.
- The generated script uses `sudo pip3` (system python) — a venv is safer: `python3 -m venv /home/ubuntu/project/.venv`.

## Verify
```bash
bash -n copy_db.sh
./copy_db.sh mydb myuser 'P@ssw0rd' dbhost.example.com /tmp/web_app.sh
sed -n '8,12p' /tmp/web_app.sh   # placeholders replaced and shellcheck-clean
```

## Gotchas
- The `sed` patterns are plain strings — if a password contains `/`, the sed breaks; escape it first (`printf '%s' "$3" | sed 's|/|\\/|g'`).
- `runserver` is a dev server (no TLS/workers) — swap for gunicorn/uwsgi + nginx for anything real.