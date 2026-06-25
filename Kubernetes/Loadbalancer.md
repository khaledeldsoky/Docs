NodePort
External Access: As you said, a NodePort exposes the service on a port on each node. You can access the service from outside the cluster by using any node’s IP address and the NodePort.
Manual Traffic Routing: You would need to manually handle traffic distribution, and if the nodes are scaled or their IPs change, you need to update your routing mechanism.
Limited to Cluster Node IPs: Traffic is routed directly to the node IP and port. If the node is down or fails, users will not be able to access the service via that node unless you manually reroute or adjust settings.
Scalability: While you can manually handle traffic through multiple nodes (by specifying each node's IP and NodePort), it’s not as clean or scalable as a load balancer, especially when dealing with changes like node failures.
2. LoadBalancer
External Access: A LoadBalancer automatically provisions an external IP address (in cloud environments) and routes external traffic to the appropriate pods.
Automatic Load Balancing: A load balancer provides traffic distribution, monitoring, and retries automatically. It can handle traffic distribution across multiple nodes and pods, even as the cluster scales.
Single External IP: Users can access the service using a single, static IP provided by the load balancer, regardless of which node the traffic is routed to.
Built-in Features: Many cloud providers offer advanced load balancing features like SSL termination, health checks, and scaling, which you would have to manage manually with a NodePort setup.
More Reliable and Scalable: Load balancers are designed to handle failures, rerouting traffic to healthy instances and distributing traffic efficiently.
Why choose LoadBalancer over NodePort?
Ease of Use: With LoadBalancer, you don’t need to manually configure traffic routing or worry about node IPs. The cloud provider automatically takes care of this.
Failover and Scaling: Load balancers handle failover, ensuring that traffic is routed to healthy nodes/pods, unlike NodePort, where you'd have to manage traffic routing manually.
Advanced Features: Load balancers often come with built-in features like SSL termination, rate limiting, and session persistence.
Simpler Management: Using LoadBalancer, especially in managed cloud environments, simplifies the overall service exposure and scaling processes.
In essence, while NodePort can expose services to the public, LoadBalancer provides a more robust, automated, and scalable way to manage traffic, especially as your services grow or require higher availability.