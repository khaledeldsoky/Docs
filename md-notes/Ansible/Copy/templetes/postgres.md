# Template: PostgreSQL for SonarQube (`postgres.yml`)
## Source
- **Folder**: `Docs/Ansible/Copy/templetes`
- **File**: `postgres.yml`
- **What it is**: Service + Deployment for PostgreSQL 14 backing SonarQube, in namespace `devops`.

## Content
```yaml
apiVersion: v1
kind: Service
metadata:
  name: sonarqube-postgresql
  namespace: devops
spec:
  ports:
    - port: 5432
      targetPort: 5432
  selector:
    app: sonarqube-postgresql
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: sonarqube-postgresql
  namespace: devops
spec:
  replicas: 1
  selector:
    matchLabels:
      app: sonarqube-postgresql
  template:
    metadata:
      labels:
        app: sonarqube-postgresql
    spec:
      containers:
      - name: postgres
        image: postgres:14
        ports:
        - containerPort: 5432
        env:
        - name: POSTGRES_USER
          value: sonarUser
        - name: POSTGRES_PASSWORD
          value: sonarPass
        - name: POSTGRES_DB
          value: sonarDB
```

## Apply
```bash
kubectl apply -f postgres.yml
kubectl -n devops get svc,deploy,pods
```

## What this gives SonarQube
- **Service name**: `sonarqube-postgresql.devops.svc.cluster.local` (the JDBC URL SonarQube connects to).
- **Credentials**: user `sonarUser` / password `sonarPass`, database `sonarDB`.

## Completion suggestions
- **Secrets**: move `POSTGRES_PASSWORD` to a Secret instead of plain env:
  ```yaml
  - name: POSTGRES_PASSWORD
    valueFrom:
      secretKeyRef:
        name: postgres-secret
        key: password
  ```
- **Persistence**: add a PersistentVolumeClaim for `/var/lib/postgresql/data` so data survives pod restarts:
  ```yaml
  spec:
    volumes:
    - name: pgdata
      persistentVolumeClaim:
        claimName: postgres-pvc
    containers:
    - name: postgres
      volumeMounts:
      - name: pgdata
        mountPath: /var/lib/postgresql/data
  ```
- **Health checks**: add `livenessProbe`/`readinessProbe` on tcp 5432.

## Verify
```bash
kubectl -n devops exec deploy/sonarqube-postgresql -- \
  psql -U sonarUser -d sonarDB -c 'SELECT 1;'
```

## Gotchas
- Service selector must match the Deployment labels (`app: sonarqube-postgresql`) — they do here.
- Postgres 14 is fine for SonarQube 9.9; newer SonarQube may want a newer minor.