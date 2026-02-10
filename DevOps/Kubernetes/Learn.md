
**copy a file from a container inside a pod to your local machine**
```sh
kubectl cp <namespace>/<pod-name>:<path-inside-container> <local-destination>
kubectl cp my-pod:/app/output.txt ./output.txt
```

**copy a file from your local machine to a pod**
```sh
kubectl cp <local-file-path> <namespace>/<pod-name>:<target-path-in-pod>
kubectl cp ./myfile.txt my-pod:/tmp/myfile.txt
```

**Tells nginx inside the container to reload its configuration (without stopping the server).**
```sh
kubectl exec web -- nginx -s reload
```
