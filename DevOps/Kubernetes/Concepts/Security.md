# Secure Kubernetes

▶ Who can access ? (**Authentication**)

- Files – Username and Passwords
- Files – Username and Tokens
- Certificates
- External Authentication providers - LDAP
- Service Accounts
---
▶ What can they do ? (**Authorization**)
- RBAC Authorization
- ABAC Authorization
- Node Authorization
- Webhook Mode
---
**▶ Accounts :** 
1. User Accounts
2. Service Accounts

**For Service Accounts ✔️**

```sh
kubectl create serviceaccount sa1
Service Account sa1 Created

kubectl list serviceaccount
ServiceAccount
sa1
```

**For User Accounts ❌**

```sh
kubectl create user user1
user1 Created

kubectl list users
Username
user1
user2
```
---
### ▶️ CERTIFICATE AUTHORITY (CA)

```sh
# Generate Keys
openssl genrsa -out ca.key 2048
ca.key

# Certificate Signing Request
openssl req -new -key ca.key -subj "/CN=KUBERNETES-CA" -out ca.csr
ca.csr

# Sign Certificates
openssl x509 -req -in ca.csr -signkey ca.key -out ca.crt
ca.crt
```
----
### ▶️ ADMIN USER

```sh
# Generate Keys
openssl genrsa -out admin.key 2048
admin.key

# Certificate Signing Request
openssl req -new -key admin.key -subj "/CN=kube-admin" -out admin.csr "/CN=kube-admin/OU=system:masters" -out admin.csr
admin.csr

#Sign Certificates
openssl x509 -req -in admin.csr –CA ca.crt -CAkey ca.key -out admin.crt
admin.crt
```
---
### ▶️ Client Certificates for Clients
```sh
curl https://kube-apiserver:6443/api/v1/pods --key admin.key --cert admin.crt --cacert ca.crt
```