## Why Do We Need Network Addons in Kubernetes?
  Kubernetes Networking Requirements: Kubernetes requires pods to be able to communicate with each other across nodes. For this to work:

  - Pods on different nodes must be able to communicate directly, as if they are on the same network.
  - Each pod gets its own IP address, and Kubernetes needs to manage routing traffic between these IPs.
  - 
---

## Network addons (like Weave Net, Calico, Flannel, etc.) implement the Kubernetes network model:

  - Create a virtual network overlay so pods can communicate across nodes.
  - Manage IP address assignment and ensure each pod has a unique, routable IP.
  - Handle cross-node routing and network isolation.

---

## Without a network addon:

- Pods Can’t Communicate Across Nodes: Only pods running on the same node may be able to communicate (depending on kube-proxy settings)and multi-node communication will fail.

---

## What is CNI (Container Network Interface)?
  - CNI is a standard specification for networking plugins in container runtimes (like Kubernetes).
  - CNI Plugins: Kubernetes does not directly manage the network itself. Instead, it uses CNI plugins (like Weave Net, Calico, Flannel, etc.) to manage pod networking.

---

## If You Don’t Install a CNI Network Addon
  - Kubernetes networking will not work as expected.
  - Pods will fail to communicate across nodes.
  - You may also see issues with services like CoreDNS failing because DNS relies on functional pod networking.
  
---

