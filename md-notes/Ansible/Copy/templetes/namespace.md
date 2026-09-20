# Template: `namespace.yml`
## Source
- **Folder**: `Docs/Ansible/Copy/templetes`
- **File**: `namespace.yml`
- **What it is**: creates the `devops` Kubernetes namespace used by SonarQube/PostgreSQL.

## Content
```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: devops
```

## Apply
```bash
kubectl apply -f namespace.yml
kubectl get ns devops
```

## Gotchas
- No changes needed — this is complete.
- All other templates in this folder assume this namespace exists first (apply order matters: `namespace.yml` → `postgres.yml` → `sonarqube.yml`).