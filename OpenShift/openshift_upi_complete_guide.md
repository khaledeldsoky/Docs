
# دليل شامل: تثبيت OpenShift 4 UPI على VMware vSphere
## UPI (User-Provided Infrastructure) - Static IPs - Local Storage - CentOS LB/NFS

---

## 📋 محتويات الدليل

1. [المتطلبات](#1-المتطلبات)
2. [البنية التحتية](#2-البنية-التحتية)
3. [الشبكة و IPs](#3-الشبكة-و-ips)
4. [الخطوة 1: إعداد DNS](#4-الخطوة-1-إعداد-dns)
5. [الخطوة 2: إعداد LB/NFS Server](#5-الخطوة-2-إعداد-lbnfs-server)
6. [الخطوة 3: Bastion Host + OpenShift Tools](#6-الخطوة-3-bastion-host--openshift-tools)
7. [الخطوة 4: RHCOS Template](#7-الخطوة-4-rhcos-template)
8. [الخطوة 5: إنشاء VMs](#8-الخطوة-5-إنشاء-vms)
9. [الخطوة 6: التثبيت](#9-الخطوة-6-التثبيت)
10. [الخطوة 7: Post-Install](#10-الخطوة-7-post-install)
11. [المشاكل الشائعة وحلولها](#11-المشاكل-الشائعة-وحلولها)
12. [Checklist نهائي](#12-checklist-نهائي)

---

## 1. المتطلبات

### 1.1 متطلبات vSphere

| المكون | الإصدار | ملاحظة |
|--------|---------|--------|
| vCenter | 7.0 U2+ | إجباري |
| ESXi | 7.0 U2+ | إجباري |
| Virtual Hardware | 15+ | إجباري |
| CPU | x86-64-v2+ | نادراً ما يكون مشكلة |

### 1.2 متطلبات VMs

| Role | عدد | vCPU | RAM | Disk | ملاحظة |
|------|-----|------|-----|------|--------|
| Bootstrap | 1 | 4 | 16 GB | 120 GB | يتم حذفه بعد التثبيت |
| Control Plane (Masters) | 3 | 4 | 16 GB | 120 GB | إجباري |
| Compute (Workers) | 2+ | 2 | 8 GB | 120 GB | 2 minimum |
| LB/NFS Server | 1 | 2 | 4 GB | 50 GB + 150 GB NFS | CentOS/RHEL |

### 1.3 متطلبات الشبكة

| المكون | الوصف |
|--------|-------|
| DNS Server | Forward + Reverse (PTR) - **إجباري** |
| Load Balancer | HAProxy - **إجباري** |
| NFS Server | للـ Registry Storage |
| DHCP | **غير مطلوب** (Static IPs) |
| Shared Storage | **غير مطلوب** (Local Storage كافي) |

### 1.4 متطلبات التخزين المستمر (Persistent Storage)

بعد التثبيت، الـ Image Registry يحتاج storage:

| الخيار | للـ Production | للـ Lab |
|--------|---------------|--------|
| OpenShift Data Foundation (ODF) | ✅ موصى به | ❌ |
| NFS | ⚠️ مش recommended | ✅ يكفي |
| Local Storage Operator | ✅ | ✅ |

---

## 2. البنية التحتية

### 2.1 Diagram

```
                    ┌─────────────────┐
                    │   DNS Server    │
                    │  (Windows DNS)  │
                    │   172.16.6.70   │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │  LB/NFS Server  │
                    │    (CentOS)     │
                    │   172.20.0.11   │
                    │  HAProxy + NFS  │
                    └────────┬────────┘
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
   ┌────▼────┐         ┌────▼────┐         ┌────▼────┐
   │  ESXi 1 │         │  ESXi 2 │         │ vCenter │
   │172.20.0.│         │172.20.0.│         │172.20.0.│
   │   151   │         │   152   │         │   101   │
   └────┬────┘         └────┬────┘         └─────────┘
        │                    │
   ┌────▼────────────────────▼────┐
   │      OpenShift Cluster       │
   │                              │
   │  Bootstrap: 172.20.0.230    │
   │  Master-0:  172.20.0.231    │
   │  Master-1:  172.20.0.232    │
   │  Master-2:  172.20.0.233    │
   │  Worker-0: 172.20.0.234    │
   │  Worker-1: 172.20.0.235    │
   │                              │
   │  API VIP:    172.20.0.220   │
   │  Ingress VIP: 172.20.0.221  │
   └──────────────────────────────┘
```

### 2.2 VMs و أدوارها

| VM | IP | Role | Location |
|----|-----|------|----------|
| DNS Server | 172.16.6.70 | DNS (Windows) | Existing |
| LB/NFS | 172.20.0.11 | HAProxy + NFS | New VM on vSphere |
| API VIP | 172.20.0.220 | Virtual IP | On LB/NFS Server |
| Ingress VIP | 172.20.0.221 | Virtual IP | On LB/NFS Server |
| ocp4-bootstrap | 172.20.0.230 | Bootstrap | vSphere VM |
| ocp4-master-0 | 172.20.0.231 | Control Plane | vSphere VM |
| ocp4-master-1 | 172.20.0.232 | Control Plane | vSphere VM |
| ocp4-master-2 | 172.20.0.233 | Control Plane | vSphere VM |
| ocp4-worker-0 | 172.20.0.234 | Compute | vSphere VM |
| ocp4-worker-1 | 172.20.0.235 | Compute | vSphere VM |

---

## 3. الشبكة و IPs

### 3.1 Network Details

| البيان | القيمة |
|--------|--------|
| Network | 172.20.0.0/16 |
| Gateway | 172.20.0.254 |
| DNS Server | 172.16.6.70 |
| Domain | ocp4.example.com |

### 3.2 IPs Table

| المكون | IP | الوصف |
|--------|-----|--------|
| DNS Server | 172.16.6.70 | Windows DNS |
| LB/NFS Server | 172.20.0.11 | CentOS VM |
| API VIP | 172.20.0.220 | HAProxy Frontend |
| Ingress VIP | 172.20.0.221 | HAProxy Frontend |
| Bootstrap | 172.20.0.230 | يتم حذفه بعد التثبيت |
| Master-0 | 172.20.0.231 | etcd member |
| Master-1 | 172.20.0.232 | etcd member |
| Master-2 | 172.20.0.233 | etcd member |
| Worker-0 | 172.20.0.234 | Compute |
| Worker-1 | 172.20.0.235 | Compute |

---

## 4. الخطوة 1: إعداد DNS

### ⚠️ مهم جداً: PTR Records إجبارية!

بدون PTR records، الـ etcd مش هيعمل initialize والتثبيت هيفشل.

### 4.1 Forward Lookup Zone (`ocp4.example.com`)

افتح **DNS Manager** على Windows Server → Your Domain → New Host (A):

```
api.ocp4          A    172.20.0.220
api-int.ocp4      A    172.20.0.220
*.apps.ocp4       A    172.20.0.221
bootstrap.ocp4    A    172.20.0.230
master-0.ocp4     A    172.20.0.231
master-1.ocp4     A    172.20.0.232
master-2.ocp4     A    172.20.0.233
worker-0.ocp4     A    172.20.0.234
worker-1.ocp4     A    172.20.0.235
```

### 4.2 Reverse Lookup Zone (`0.20.172.in-addr.arpa`)

**⚠️ ده إجباري للـ etcd!**

```
220    PTR    api.ocp4.example.com.
220    PTR    api-int.ocp4.example.com.
221    PTR    *.apps.ocp4.example.com.
230    PTR    bootstrap.ocp4.example.com.
231    PTR    master-0.ocp4.example.com.
232    PTR    master-1.ocp4.example.com.
233    PTR    master-2.ocp4.example.com.
234    PTR    worker-0.ocp4.example.com.
235    PTR    worker-1.ocp4.example.com.
```

### 4.3 التحقق من DNS

من **CentOS LB/NFS Server**:

```bash
# Forward
nslookup api.ocp4.example.com
nslookup master-0.ocp4.example.com

# Reverse - إجباري!
dig -x 172.20.0.231 +short
# لازم يرجع: master-0.ocp4.example.com.
```

---

## 5. الخطوة 2: إعداد LB/NFS Server

### 5.1 إنشاء VM على vSphere

| البيان | القيمة |
|--------|--------|
| OS | CentOS Stream 9 / RHEL 9 / Rocky 9 |
| vCPU | 2 |
| RAM | 4 GB |
| Disk 1 (OS) | 50 GB |
| Disk 2 (NFS) | 150 GB (اختياري) |
| IP | 172.20.0.11/24 |
| Gateway | 172.20.0.254 |
| DNS | 172.16.6.70 |

### 5.2 Static IP على CentOS

```bash
sudo nmcli connection modify "ens192"   ipv4.addresses 172.20.0.11/24   ipv4.gateway 172.20.0.254   ipv4.dns "172.16.6.70"   ipv4.method manual

sudo nmcli connection up "ens192"

# التحقق
ip addr show ens192
ip route | grep default
```

### 5.3 تثبيت Packages

```bash
sudo dnf install -y haproxy nfs-utils firewalld
sudo systemctl enable --now haproxy nfs-server firewalld
```

### 5.4 إعداد HAProxy

**⚠️ مهم: الـ API VIP و Ingress VIP لازم يكونوا على الـ LB Server!**

```bash
sudo ip addr add 172.20.0.220/24 dev ens192
sudo ip addr add 172.20.0.221/24 dev ens192

# نخليها دائمة
sudo nmcli connection modify "ens192" +ipv4.addresses 172.20.0.220/24
sudo nmcli connection modify "ens192" +ipv4.addresses 172.20.0.221/24
sudo nmcli connection up "ens192"
```

**ملف `/etc/haproxy/haproxy.cfg`:**

```haproxy
global
    log         127.0.0.1 local2
    chroot      /var/lib/haproxy
    pidfile     /var/run/haproxy.pid
    maxconn     4000
    user        haproxy
    group       haproxy
    daemon

defaults
    mode                    tcp
    log                     global
    option                  tcplog
    option                  dontlognull
    option                  redispatch
    retries                 3
    timeout queue           1m
    timeout connect         10s
    timeout client          1m
    timeout server          1m
    timeout check           10s

# API (Port 6443)
frontend api
    bind *:6443
    default_backend api

backend api
    balance source
    server ocp4-bootstrap 172.20.0.230:6443 check
    server ocp4-master-0  172.20.0.231:6443 check
    server ocp4-master-1  172.20.0.232:6443 check
    server ocp4-master-2  172.20.0.233:6443 check

# Machine Config Server (Port 22623)
frontend machineconfig
    bind *:22623
    default_backend machineconfig

backend machineconfig
    balance source
    server ocp4-bootstrap 172.20.0.230:22623 check
    server ocp4-master-0  172.20.0.231:22623 check
    server ocp4-master-1  172.20.0.232:22623 check
    server ocp4-master-2  172.20.0.233:22623 check

# HTTP Ingress (Port 80)
frontend ingress_http
    bind *:80
    default_backend ingress_http

backend ingress_http
    balance source
    server ocp4-master-0  172.20.0.231:80 check
    server ocp4-master-1  172.20.0.232:80 check
    server ocp4-master-2  172.20.0.233:80 check
    server ocp4-worker-0  172.20.0.234:80 check
    server ocp4-worker-1  172.20.0.235:80 check

# HTTPS Ingress (Port 443)
frontend ingress_https
    bind *:443
    default_backend ingress_https

backend ingress_https
    balance source
    server ocp4-master-0  172.20.0.231:443 check
    server ocp4-master-1  172.20.0.232:443 check
    server ocp4-master-2  172.20.0.233:443 check
    server ocp4-worker-0  172.20.0.234:443 check
    server ocp4-worker-1  172.20.0.235:443 check
```

```bash
sudo systemctl reload haproxy
sudo ss -tlnp | grep -E '6443|22623|80|443'
```

### 5.5 إعداد NFS Server

```bash
sudo mkdir -p /exports/registry
sudo chmod 777 /exports/registry

echo '/exports/registry 172.20.0.0/24(rw,sync,no_root_squash,no_all_squash)' | sudo tee -a /etc/exports

sudo exportfs -arv
sudo showmount -e localhost
```

### 5.6 Firewall

```bash
sudo firewall-cmd --permanent --add-service=nfs
sudo firewall-cmd --permanent --add-service=rpc-bind
sudo firewall-cmd --permanent --add-service=mountd
sudo firewall-cmd --permanent --add-port=6443/tcp
sudo firewall-cmd --permanent --add-port=22623/tcp
sudo firewall-cmd --permanent --add-port=80/tcp
sudo firewall-cmd --permanent --add-port=443/tcp
sudo firewall-cmd --reload
```

---

## 6. الخطوة 3: Bastion Host + OpenShift Tools

### 6.1 مكان التثبيت

**نفس CentOS LB/NFS Server (172.20.0.11)**

### 6.2 تحميل OpenShift Tools

من **Red Hat Hybrid Cloud Console**:
- https://console.redhat.com/openshift/downloads

أو من command line:

```bash
cd ~/ocp4-install

wget https://mirror.openshift.com/pub/openshift-v4/clients/ocp/latest/openshift-install-linux.tar.gz
wget https://mirror.openshift.com/pub/openshift-v4/clients/ocp/latest/openshift-client-linux.tar.gz

tar -xzf openshift-install-linux.tar.gz
tar -xzf openshift-client-linux.tar.gz

sudo mv openshift-install oc kubectl /usr/local/bin/

# التحقق
openshift-install version
oc version
```

### 6.3 إنشاء SSH Key

```bash
ssh-keygen -t ed25519 -N '' -f ~/.ssh/ocp4

eval "$(ssh-agent -s)"
ssh-add ~/.ssh/ocp4

cat ~/.ssh/ocp4.pub
```

### 6.4 إنشاء `install-config.yaml`

**⚠️ مهم: لازم تعمل Cluster في vCenter!**

```yaml
apiVersion: v1
baseDomain: example.com
metadata:
  name: ocp4
compute:
- architecture: amd64
  hyperthreading: Enabled
  name: worker
  platform: {}
  replicas: 0
controlPlane:
  architecture: amd64
  hyperthreading: Enabled
  name: master
  platform: {}
  replicas: 3
platform:
  vsphere:
    apiVIPs:
    - 172.20.0.220
    ingressVIPs:
    - 172.20.0.221
    vcenters:
    - datacenters:
      - Datacenter
      server: 172.20.0.101
      user: administrator@vsphere.local
      password: YourPassword
      port: 443
    failureDomains:
    - name: fd-1
      region: main-site
      server: 172.20.0.101
      topology:
        computeCluster: /Datacenter/host/Openshift
        datacenter: Datacenter
        datastore: /Datacenter/datastore/hdd
        networks:
        - VM Network
        resourcePool: /Datacenter/host/Openshift/Resources
        folder: /Datacenter/vm
      zone: zone-1
networking:
  clusterNetwork:
  - cidr: 10.128.0.0/14
    hostPrefix: 23
  machineNetwork:
  - cidr: 172.20.0.0/16
  networkType: OVNKubernetes
  serviceNetwork:
  - 172.30.0.0/16
publish: External
pullSecret: '{"auths": ...}'
sshKey: |
  ssh-ed25519 AAAA...
```

### 6.5 توليد Ignition Files

```bash
cd ~/ocp4-install

# احفظ نسخة
# install-config.yaml يتم حذفه بعد create manifests
cp install-config.yaml install-config.yaml.bak

# توليد Manifests
./openshift-install create manifests --dir=.

# (اختياري) اجعل Masters غير قابلة للـ Scheduling
# sed -i 's/mastersSchedulable: true/mastersSchedulable: false/' manifests/cluster-scheduler-02-config.yaml

# توليد Ignition Files
./openshift-install create ignition-configs --dir=.

# النتيجة:
# auth/
# bootstrap.ign
# master.ign
# worker.ign
# metadata.json
```

---

## 7. الخطوة 4: RHCOS Template

### 7.1 تحميل RHCOS OVA

```bash
# جرب الـ URL ده
wget https://mirror.openshift.com/pub/openshift-v4/dependencies/rhcos/4.15/latest/rhcos-4.15.23-x86_64-vmware.x86_64.ova

# لو مش شغال، حمل من Red Hat Console
```

### 7.2 رفع OVA على vSphere

من **vSphere Client**:
1. Right-click على Datacenter → **Deploy OVF Template**
2. Local file → اختار الـ OVA
3. Name: `rhcos-template`
4. Storage: `hdd`
5. Network: `VM Network`
6. Finish

### 7.3 تحويل لـ Template

1. Right-click على `rhcos-template` VM
2. Template → **Convert to Template**

---

## 8. الخطوة 5: إنشاء VMs

### 8.1 تثبيت govc

```bash
wget https://github.com/vmware/govmomi/releases/latest/download/govc_Linux_x86_64.tar.gz
tar -xzf govc_Linux_x86_64.tar.gz
chmod +x govc
sudo mv govc /usr/local/bin/
```

### 8.2 متغيرات البيئة

```bash
export GOVC_URL=172.20.0.101
export GOVC_USERNAME=administrator@vsphere.local
export GOVC_PASSWORD=Root@123
export GOVC_INSECURE=1
export GOVC_DATACENTER=Datacenter
export GOVC_DATASTORE=hdd
export GOVC_NETWORK="VM Network"
export GOVC_RESOURCE_POOL="/Datacenter/host/Openshift/Resources"
```

### 8.3 سكريبت إنشاء VMs

**⚠️ مهم جداً:**
- `disk.EnableUUID=TRUE` إجباري
- Ignition data كبيرة ومش بتنفع في command line
- نستخدم Python مع pyvmomi

```bash
# تثبيت pyvmomi
pip3 install pyvmomi
```

```python
#!/usr/bin/env python3
# create_ocp_vms.py
import base64
import ssl
from pyVim.connect import SmartConnect, Disconnect
from pyVmomi import vim

VCENTER = '172.20.0.101'
USERNAME = 'administrator@vsphere.local'
PASSWORD = 'YourPassword'

GATEWAY = '172.20.0.254'
NETMASK = '255.255.255.0'
DNS = '172.16.6.70'

vms = {
    'ocp4-bootstrap': {
        'ip': '172.20.0.230',
        'hostname': 'bootstrap.ocp4.example.com',
        'cpu': 4,
        'ram': 16384,
        'disk': 120,
        'ign': 'bootstrap.ign'
    },
    'ocp4-master-0': {
        'ip': '172.20.0.231',
        'hostname': 'master-0.ocp4.example.com',
        'cpu': 4,
        'ram': 16384,
        'disk': 120,
        'ign': 'master.ign'
    },
    'ocp4-master-1': {
        'ip': '172.20.0.232',
        'hostname': 'master-1.ocp4.example.com',
        'cpu': 4,
        'ram': 16384,
        'disk': 120,
        'ign': 'master.ign'
    },
    'ocp4-master-2': {
        'ip': '172.20.0.233',
        'hostname': 'master-2.ocp4.example.com',
        'cpu': 4,
        'ram': 16384,
        'disk': 120,
        'ign': 'master.ign'
    },
    'ocp4-worker-0': {
        'ip': '172.20.0.234',
        'hostname': 'worker-0.ocp4.example.com',
        'cpu': 2,
        'ram': 8192,
        'disk': 120,
        'ign': 'worker.ign'
    },
    'ocp4-worker-1': {
        'ip': '172.20.0.235',
        'hostname': 'worker-1.ocp4.example.com',
        'cpu': 2,
        'ram': 8192,
        'disk': 120,
        'ign': 'worker.ign'
    }
}

context = ssl.SSLContext(ssl.PROTOCOL_TLS_CLIENT)
context.check_hostname = False
context.verify_mode = ssl.CERT_NONE

si = SmartConnect(host=VCENTER, user=USERNAME, pwd=PASSWORD, sslContext=context)
content = si.RetrieveContent()

def get_vm(name):
    obj_view = content.viewManager.CreateContainerView(content.rootFolder, [vim.VirtualMachine], True)
    for vm in obj_view.view:
        if vm.name == name:
            return vm
    return None

for vm_name, config in vms.items():
    print(f"
Processing {vm_name}...")

    # Find template
    template = get_vm('rhcos-template')
    if not template:
        print("ERROR: Template not found!")
        break

    # Clone
    clone_spec = vim.vm.CloneSpec()
    clone_spec.location = vim.vm.RelocateSpec()
    clone_spec.location.pool = get_vm('rhcos-template').resourcePool

    task = template.Clone(name=vm_name, folder=template.parent, spec=clone_spec)
    while task.info.state == 'running':
        pass

    vm = get_vm(vm_name)
    if not vm:
        print(f"ERROR: Failed to create {vm_name}")
        continue

    # Configure
    spec = vim.vm.ConfigSpec()
    spec.numCPUs = config['cpu']
    spec.memoryMB = config['ram']

    # disk.EnableUUID
    spec.extraConfig = [
        vim.option.OptionValue(key='disk.EnableUUID', value='TRUE')
    ]

    task = vm.ReconfigVM_Task(spec)
    while task.info.state == 'running':
        pass

    # Read ignition
    with open(config['ign'], 'rb') as f:
        ign_data = base64.b64encode(f.read()).decode('utf-8')

    # Set ignition
    spec = vim.vm.ConfigSpec()
    spec.extraConfig = [
        vim.option.OptionValue(key='guestinfo.ignition.config.data', value=ign_data),
        vim.option.OptionValue(key='guestinfo.ignition.config.data.encoding', value='base64'),
        vim.option.OptionValue(key='guestinfo.afterburn.initrd.network-kargs', 
            value=f"ip={config['ip']}::{GATEWAY}:{NETMASK}:{config['hostname']}:ens192:off nameserver={DNS}")
    ]

    task = vm.ReconfigVM_Task(spec)
    while task.info.state == 'running':
        pass

    print(f"  ✓ {vm_name} created")

print("
All VMs created!")
print("Power on VMs in order: Bootstrap → Masters → Workers")
Disconnect(si)
```

### 8.4 تشغيل VMs بالترتيب

```bash
# Bootstrap أولاً
govc vm.power -on ocp4-bootstrap
sleep 120

# Masters
govc vm.power -on ocp4-master-0 ocp4-master-1 ocp4-master-2
sleep 60

# Workers
govc vm.power -on ocp4-worker-0 ocp4-worker-1
```

---

## 9. الخطوة 6: التثبيت

### 9.1 انتظر Bootstrap

```bash
cd ~/ocp4-install
./openshift-install wait-for bootstrap-complete --dir=. --log-level=info
```

**⚠️ بياخد 30-40 دقيقة!**

### 9.2 بعد Bootstrap Complete

```bash
# 1. أوقف Bootstrap
govc vm.power -off ocp4-bootstrap
govc vm.destroy ocp4-bootstrap

# 2. أزل Bootstrap من HAProxy
sudo sed -i '/ocp4-bootstrap/d' /etc/haproxy/haproxy.cfg
sudo systemctl reload haproxy

# 3. وافق على CSRs
export KUBECONFIG=~/ocp4-install/auth/kubeconfig

watch -n 5 "oc get csr -o go-template='{{range .items}}{{if not .status}}{{.metadata.name}}{{\n}}{{end}}{{end}}' | xargs oc adm certificate approve"

# 4. انتظر الاكتمال
./openshift-install wait-for install-complete --dir=. --log-level=info
```

---

## 10. الخطوة 7: Post-Install

### 10.1 Registry Storage (NFS)

```bash
export KUBECONFIG=~/ocp4-install/auth/kubeconfig

# PV
cat <<EOF | oc apply -f -
apiVersion: v1
kind: PersistentVolume
metadata:
  name: registry-pv
spec:
  capacity:
    storage: 100Gi
  accessModes:
    - ReadWriteMany
  persistentVolumeReclaimPolicy: Retain
  nfs:
    path: /exports/registry
    server: 172.20.0.11
EOF

# PVC
cat <<EOF | oc apply -f -
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: image-registry-storage
  namespace: openshift-image-registry
spec:
  accessModes:
    - ReadWriteMany
  resources:
    requests:
      storage: 100Gi
  volumeName: registry-pv
EOF

# تفعيل Registry
oc patch configs.imageregistry.operator.openshift.io cluster --type=merge --patch '{"spec":{"managementState":"Managed","storage":{"pvc":{"claim":"image-registry-storage"}},"rolloutStrategy":"RollingUpdate"}}'
```

### 10.2 vSphere CSI Driver

```bash
# StorageClass
cat <<EOF | oc apply -f -
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: thin-csi
  annotations:
    storageclass.kubernetes.io/is-default-class: "true"
provisioner: csi.vsphere.vmware.com
parameters:
  StoragePolicyName: ""
  csi.storage.k8s.io/fstype: ext4
reclaimPolicy: Delete
volumeBindingMode: WaitForFirstConsumer
allowVolumeExpansion: true
EOF
```

---

## 11. المشاكل الشائعة وحلولها

### 11.1 `cluster '/Datacenter/host/xxx' not found`

**السبب:** OpenShift بيحتاج vSphere Cluster، مش standalone ESXi hosts.

**الحل:**
```bash
# اعمل Cluster في vCenter
govc cluster.create -dc=Datacenter Openshift

# أضف الـ ESXi hosts
govc cluster.add -cluster=Openshift -hostname=172.20.0.151 -username=root -password=ESXi_PASSWORD -noverify
govc cluster.add -cluster=Openshift -hostname=172.20.0.152 -username=root -password=ESXi_PASSWORD -noverify
```

### 11.2 `Argument list too long`

**السبب:** Ignition data كبيرة جداً للـ command line.

**الحل:** استخدم Python مع pyvmomi بدل govc.

### 11.3 `dial tcp 172.20.0.220:6443: i/o timeout`

**السبب:** API VIP مش موجود على الـ LB Server.

**الحل:**
```bash
sudo ip addr add 172.20.0.220/24 dev ens192
sudo ip addr add 172.20.0.221/24 dev ens192
```

### 11.4 `cannot bind socket (Permission denied)`

**السبب:** SELinux بيمنع HAProxy.

**الحل:**
```bash
sudo setsebool -P haproxy_connect_any 1
# أو مؤقتاً
sudo setenforce 0
```

### 11.5 `no such host` - DNS

**السبب:** CentOS مش بيستخدم الـ DNS Server الصحيح.

**الحل:**
```bash
sudo sed -i '1i nameserver 172.16.6.70' /etc/resolv.conf
```

### 11.6 `Permission denied (publickey)`

**السبب:** SSH key مش متطابق.

**الحل:** تأكد إن الـ SSH key في `install-config.yaml` هو نفسه اللي بتحاول تستخدمه.

### 11.7 Ignition data مش متضافت

**السبب:** govc مش بيقدر يستقبل data كبيرة.

**الحل:** استخدم Python مع pyvmomi.

---

## 12. Checklist نهائي

### قبل البدء:

| ☐ | الخطوة | الموقع |
|---|--------|--------|
| ☐ | vCenter 7.0 U2+ | vSphere |
| ☐ | ESXi 7.0 U2+ | vSphere |
| ☐ | Cluster في vCenter | vSphere |
| ☐ | DNS Forward + Reverse (PTR) | Windows DNS |
| ☐ | CentOS VM للـ LB/NFS | vSphere |
| ☐ | HAProxy شغال | CentOS |
| ☐ | NFS Export متاح | CentOS |
| ☐ | API VIP (172.20.0.220) على LB | CentOS |
| ☐ | Ingress VIP (172.20.0.221) على LB | CentOS |

### أثناء التثبيت:

| ☐ | الخطوة | الموقع |
|---|--------|--------|
| ☐ | `install-config.yaml` صحيح | CentOS |
| ☐ | Ignition files متولدة | CentOS |
| ☐ | RHCOS Template موجود | vSphere |
| ☐ | VMs متعملة | vSphere |
| ☐ | `disk.EnableUUID=TRUE` | كل VM |
| ☐ | Ignition data متضافت | كل VM |
| ☐ | Bootstrap Complete | - |
| ☐ | CSRs معتمدة | - |
| ☐ | Install Complete | - |

### بعد التثبيت:

| ☐ | الخطوة | الموقع |
|---|--------|--------|
| ☐ | Registry Storage | NFS |
| ☐ | vSphere CSI Driver | OpenShift |
| ☐ | StorageClass | OpenShift |

---

## ملاحظات مهمة

1. **Ignition files صلاحية 24 ساعة** - لازم تخلص التثبيت في الـ 24 ساعة.
2. **Bootstrap يتم حذفه** - مش جزء من الـ Cluster.
3. **PTR Records إجبارية** - بدونها الـ etcd مش هيعمل.
4. **disk.EnableUUID=TRUE** - مطلوب للـ vSphere CSI.
5. **NFS مش recommended للـ Production** - استخدم ODF للـ Production.
