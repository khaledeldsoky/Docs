# Template: SonarQube (`sonarqube.yml`)
## Source
- **Folder**: `Docs/Ansible/Copy/templetes`
- **File**: `sonarqube.yml`
- **What it is**: NodePort Service + Deployment for SonarQube Community 9.9, namespace `devops`, backed by the PostgreSQL template.

## Content (corrected)
```yaml
apiVersion: v1
kind: Service
metadata:
  name: sonarqube
  namespace: devops
spec:
  type: NodePort
  ports:
    - port: 9000
      targetPort: 9000
      nodePort: 32000   # change if needed
  selector:
    app: sonarqube
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: sonarqube
  namespace: devops
spec:
  replicas: 1
  selector:
    matchLabels:
      app: sonarqube
  template:
    metadata:
      labels:
        app: sonarqube
    spec:
      containers:
      - name: sonarqube
        image: sonarqube:9.9-community
        ports:
        - containerPort: 9000
        env:
        - name: SONAR_JDBC_URL
          value: jdbc:postgresql://sonarqube-postgresql.devops.svc.cluster.local:5432/sonarDB
        - name: SONAR_JDBC_USERNAME
          value: sonarUser
        - name: SONAR_JDBC_PASSWORD
          value: sonarPass
```

## The bug fixed
The original image line was:
```yaml
image: sonarqube=sonarqube:9.9-community     # typo — '=' instead of ':'
```
`sonarqube=sonarqube:...` is an invalid image reference and the Deployment would never pull. Fixed to:
```yaml
image: sonarqube:9.9-community
```

## Apply
```bash
kubectl apply -f sonarqube.yml
kubectl -n devops get svc sonarqube,deploy/sonarqube,pods -o wide
```

## Access
SonarQube is exposed on port **32000** of any cluster node:
```
http://<NODE_IP>:32000
```
Default login: `admin` / `admin` (change on first login).

## Completion suggestions
- **JDBC password**: use a Secret (shared with `postgres.yml`) rather than a literal.
- **Resource limits** (SonarQube is RAM-hungry):
  ```yaml
  resources:
    requests: { memory: 2Gi }
    limits:   { memory: 4Gi }
  ```
- **Storage**: SonarQube keeps plugins/es data in `/opt/sonarqube/data`, `/extensions`, `/logs` — add PVCs for durability.
- **Probes**: readiness on `/api/system/status` helps the pod only accept traffic when healthy.

## Verify
```bash
kubectl -n devops get pod -l app=sonarqube -o wide
curl -s http://<NODE_IP>:32000/api/system/status
# expect: {"status":"UP"} once healthy (may take a couple minutes on first boot)
```

## Gotchas
- Wait for the DB pod to be Ready **before** SonarQube starts or the migration fails.
- `nodePort: 32000` must not collide with another service on the cluster.