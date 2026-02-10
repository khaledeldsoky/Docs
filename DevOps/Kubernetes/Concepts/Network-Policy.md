# 🔐 [Network Policy](https://kubernetes.io/docs/concepts/services-networking/network-policies/)


**▶ NetworkPolicy**

`policy-definition.yaml`

```yml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
	name: db-policy
spec:
	podSelector:
		matchLabels:
			role: db
	policyTypes:
	- Ingress
	- Egress
# you can use podSelector and namespaceSelector and ipBlock or use them seperate by put - before them
	ingress:	
	- from:
		- podSelector:
			matchLabels:
				name: api-pod
	
			namespaceSelector: 
				matchLabels:
					name: prod

# 	-	namespaceSelector: 
# 			matchLabels:
# 			name: prod

		- ipBlock:
				cidr: 192.168.5.10/32

		ports:
		-	protocol: TCP
			port: 3306
# ---------------- egress ----------------
	egress:
	- to:
		- ipBlock:
			cidr: 192.168.5.10/32

	ports:
	- protocol: TCP
		port: 80 80
```

```sh
kubectl create f policy-definition.yaml
```
