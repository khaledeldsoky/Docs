# [Roles](https://kubernetes.io/docs/reference/access-authn-authz/rbac/)

## 🟠 Role-based access control (RBAC)

**▶ Role :**

`developer-role.yaml`

```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: developer
rules:
  - apiGroups: [""]
    resources: ["pods"]
    verbs: ["list“, "get", “create“, “update“, “delete"]

  - apiGroups: [""]
    resources: [“ConfigMap"]
    verbs: [“create“]
```
```sh 
kubectl create -f developer-role.yaml
```

**▶ RoleBinding :**

`devuser-developer-binding.yaml`
```yml
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: devuser-developer-binding
subjects:
  - kind: User
  name: dev-user
  apiGroup: rbac.authorization.k8s.io
roleRef:
  kind: Role
  name: developer
  apiGroup: rbac.authorization.k8s.io
```
```sh
kubectl create -f devuser-developer-binding.yaml
```

---
### ➡️ View RBAC
```sh
kubectl get roles
kubectl get rolebindings
kubectl describe role developer
kubectl describe rolebinding devuser-developer-binding
```
---
### ➡️ Check Access

```sh
kubectl auth can-i create deployments
yes

kubectl auth can-i delete nodes
no

kubectl auth can-i create deployments --as dev-user
no

kubectl auth can-i create pods --as dev-user
yes

kubectl auth can-i create pods --as dev-user --namespace test
no
```
---
### ➡️ Resource Names

`developer-role.yaml`

```yml
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: developer
rules:
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", “create“, “update“]
  resourceNames: [“blue“, “orange”]
```

---
## 🟠 Cluster Roles

**▶ ClusterRole**

`cluster-admin-role.yaml`

```yml
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: cluster-administrator
rules:
- apiGroups: [""]
  resources: [“nodes"]
  verbs: ["list“, "get", “create“, “delete"]
```
```sh
kubectl create -f cluster-admin-role.yaml
```
**▶ ClusterRoleBinding**

`cluster-admin-role-binding.yaml`

```yml
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: cluster-admin-role-binding
subjects:
- kind: User
  name: cluster-admin
  apiGroup: rbac.authorization.k8s.io
roleRef:
  kind: ClusterRole
  name: cluster-administrator
  apiGroup: rbac.authorization.k8s.io
```

```sh
kubectl create –f cluster-admin-role-binding.yaml
```