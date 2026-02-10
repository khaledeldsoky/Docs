# Why we use "include "argocd.fullname" ." Not the name direct?
  - Everyvinstall of this chart will create a resource with that exact name.
  - That means you could not install the same chart twice in the same cluster

---
How Think to create Helm structure
- Fisrt, Build all the resouces by ordinary way
- Second, Create Dir templates and create same resource file but put the varbiables name
- Third, Create the values file and enter the value 
- Fourth, Create 