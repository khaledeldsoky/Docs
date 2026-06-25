# [kube Documention](https://kubernetes.io/docs/home/)

# [Cluster Architecture ](https://kubernetes.io/docs/concepts/architecture/)

## ▶️ Control plane components 

```yml
1. kube-apiserver
2. etcd
3. kube-scheduler 
4. kube-controller-manager 
5. cloud-controller-manager 
```

## ▶️ Node components
```yml
1. kubelet
2. kube-proxy (optional) 
3. Container runtime
```
---
# [Install and Set Up kubectl on Linux](https://kubernetes.io/docs/tasks/tools/install-kubectl-linux/#install-using-native-package-management)

## Installing Using kubeadm 
### ▶️ Update the apt package index
```sh 
# Update apt package index
sudo apt-get update 
   
# install packages needed to use the Kubernetes apt repository 
sudo apt-get install -y apt-transport-https ca-certificates curl gpg
   
# Download the public signing key for the Kubernetes package repositories.
# If the folder "/etc/apt/keyrings" does not exist, it should be created before the curl command, read the note below.
# sudo mkdir -p -m 755 /etc/apt/keyrings
curl -fsSL https://pkgs.k8s.io/core:/stable:/v1.28/deb/Release.key | sudo gpg --dearmor -o /etc/apt/keyrings/kubernetes-apt-keyring.gpg
   
# allow unprivileged APT programs to read this keyring
sudo chmod 644 /etc/apt/keyrings/kubernetes-apt-keyring.gpg
   
# Add the appropriate Kubernetes apt repository
echo 'deb [signed-by=/etc/apt/keyrings/kubernetes-apt-keyring.gpg] https://pkgs.k8s.io/core:/stable:/v1.28/deb/ /' | sudo tee /etc/apt/sources.list.d/kubernetes.list
   
# helps tools such as command-not-found to work correctly
sudo chmod 644 /etc/apt/sources.list.d/kubernetes.list
   
# Update apt package index, then install kubectl
sudo apt-get update
sudo apt-get install -y kubectl
   
# install kubelet, kubeadm and kubectl, and pin their version:
sudo apt-get install -y kubelet kubeadm kubectl
sudo apt-mark hold kubelet kubeadm kubectl
```
----
# after install kubernates do the coming
# [Container Runtimes](https://v1-31.docs.kubernetes.io/docs/setup/production-environment/container-runtimes/)

## Install and configure prerequisites 



1. #### Enable IPv4 packet forwarding
    ```bash
      # sysctl params required by setup, params persist across reboots
cat <<-EOF | sudo tee /etc/sysctl.d/k8s.conf
net.ipv4.ip_forward = 1
EOF
      # Apply sysctl params without reboot
      sudo sysctl --system
    

      #Verify that net.ipv4.ip_forward is set to 1 with:
      sysctl net.ipv4.ip_forward
      
    ```

2. #### Configuring the systemd cgroup driver
   ## Is very important to do For ALLLLLLLLLLLLLLLLLLLLLLLLLLl nodes

To use the systemd cgroup driver in /etc/containerd/config.toml with runc, set
  ```sh
  containerd config default > /etc/containerd/config.toml
  ```
  To use the systemd cgroup driver in /etc/containerd/config.toml with runc, set

  ```sh
  vim /etc/containerd/config.toml
  ```

   ```sh
    [plugins."io.containerd.grpc.v1.cri".containerd.runtimes.runc]
      ...
      [plugins."io.containerd.grpc.v1.cri".containerd.runtimes.runc.options]
        SystemdCgroup = true
   ```


1. #### If you apply this change, make sure to restart containerd:
   ```bash
       sudo systemctl restart containerd
   ```

2. 
```sh
   sudo kubeadm init
```

---

if you missing the token 
```
[text](https://kubernetes.io/docs/tasks/administer-cluster/kubeadm/adding-linux-nodes/)
```

kubectl apply -f https://reweave.azurewebsites.net/k8s/v1.29/net.yaml
------------------------------------------------------------
[to use dockerd isntead of containerd](https://www.mirantis.com/blog/how-to-install-cri-dockerd-and-migrate-nodes-from-dockershim/)
------------------------------------------------------------
```sh
kubectl describe pod <pod name>
kubectl describe deplyoment <deplyoment name>
kubectl describe service <service name>

kubectl get pods
kubectl get service

kubectl logs pod <pod name> 
```
---

# install ingress controller

## kubectl apply -f deployments/rbac/rbac.yaml
```sh
git clone https://github.com/nginxinc/kubernetes-ingress.git --branch v4.0.0

# Change the active directory.
cd kubernetes-ingress

# Create a namespace and a service account:
kubectl apply -f deployments/common/ns-and-sa.yaml

# Create a cluster role and binding for the service account:
kubectl apply -f deployments/rbac/rbac.yaml

# Create common resources
kubectl apply -f examples/shared-examples/default-server-secret/default-server-secret.yaml

# Create a ConfigMap to customize your NGINX settings:
kubectl apply -f examples/shared-examples/default-server-secret/default-server-secret.yaml

#Create an IngressClass resource. NGINX Ingress Controller won’t start without an IngressClass resource.
kubectl apply -f deployments/common/ingress-class.yaml

# Create core custom resources
# INSTALL CRDS FROM SINGLE YAML
kubectl apply -f https://raw.githubusercontent.com/nginxinc/kubernetes-ingress/v4.0.0/deploy/crds.yaml

# Deploy NGINX Ingress Controller
kubectl apply -f deployments/deployment/nginx-ingress.yaml

# Confirm NGINX Ingress Controller is running
kubectl get pods --namespace=nginx-ingress
```

-------------------------------------------------------------------

# For monotiring :-
[Monotiring Link](https://github.com/kubernetes-sigs/metrics-server)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: metrics-server
  namespace: kube-system
  labels:
    k8s-app: metrics-server
spec:
  selector:
    matchLabels:
      k8s-app: metrics-server
  template:
    metadata:
      name: metrics-server
      labels:
        k8s-app: metrics-server
    spec:
      serviceAccountName: metrics-server
      volumes:
      # mount in tmp so we can safely use from-scratch images and/or read-only containers
      - name: tmp-dir
        emptyDir: {}
      containers:
      - name: metrics-server
        image: k8s.gcr.io/metrics-server/metrics-server:v0.3.7
        imagePullPolicy: IfNotPresent
        args:
          - --kubelet-insecure-tls
          - --cert-dir=/tmp
          - --secure-port=4443
        ports:
        - name: main-port
          containerPort: 4443
          protocol: TCP
        securityContext:
          readOnlyRootFilesystem: true
          runAsNonRoot: true
          runAsUser: 1000
        volumeMounts:
        - name: tmp-dir
          mountPath: /tmp
      nodeSelector:
        kubernetes.io/os: linux
```
---------------------------------------------
you must install web server (apache or nginx)


# ----------------------------- eks_cluster ----------------------------- #
# خارجيه Ec2 هتحط الكومندات ع ال  eks بعدل لما تعمل ال 
aws configure      

# بتاعت الاكونت credentials حط هنا ال 
vim ~/.aws/credentials       

# عشان تثدر تتحكم ف الكلاستر بتاعك                
aws eks update-kubeconfig --region <region cluster in it> --name <cluster name> 

# عشن تقدر تتعامل مع الكلاتسر kubectl ل install هتعمل 
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
sudo install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl

# ------------------ auto completion kubectl ------------------ #

sudo apt-get install bash-completion                            # install bash-completion
echo 'source <(kubectl completion bash)' >>~/.bashrc            # Add the completion script to your .bashrc file
source ~/.bashrc                                                # Apply changes

# ------------------ auto completion kubectl ------------------ #
# install argocd موقع ل
   # https://www.digitalocean.com/community/tutorials/how-to-deploy-to-kubernetes-using-argo-cd-and-gitops

# خطوات التحميل
kubectl get nodes

# للارجو name space هتعمل 
kubectl create namespace argocd

# للفايل بتاع الارجو apply هتعمل 
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

#عشان تشوف البود الي جوه الارجو
watch kubectl get pods -n argocd

# هيخلي الارجو يعمل لود بلاسنر 
kubectl patch svc argocd-server -n argocd -p '{"spec": {"type": "LoadBalancer"}}'

# عشان تخلي الارجو يفتح ع البورت الي انت عاوزه
kubectl port-forward svc/argocd-server -n argocd 8081:443
    # افتح بورت 8081 فالسيكورتي جروب

# الرقم السري بتاع الارجو
kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d; echo
    # argo و هو هيطلعلك ال  eks  روح بقا ع اللود بلانسر يتاع ال 

# --  مش هيعملها هيشتغل ع الي ف الربو cluster هي الكود الاصلي , بمعني لو حد غير حاجه ف ال  git repo اهم حاجه بالنسبه ليه هي ال 
        # overwrite طبعا ف اوبشن جوا الارجو انك تخلي التغير ف الكلاستر يعمل 
# --  بيخليني اقدر اراقب حاله الكلاستر الي الجينكز مش بيثدر يوفرها

