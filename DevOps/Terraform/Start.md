Why use terraform (Iac)?
- If acount aws is delaited 
- the person build the infra is leave the company 
- the code is easer than GUI

---

The meaning og Idempotence 
مش كل مره تجي تعمل الحاجه يروح يعملها من تاني , بيشوف ايه الي مش معمول ويروح يعمله 

---

Terraform 
- Open source 
- Code Go language
- Build by Hash corp campany
- Build resources by it self (Don't depend on the order)

---

Why use Terrafrom ?
- can build infra in most infra provider (AWS , GCP , Azure , ...)

---

Terraform Core vs Plugins
- Core essiantial
- Plugins like AWS plugins 

---

Provider

Provider.tf

provider "ibm" {
  region = "us-south"
}

## Authentication
### Static credentials
provider "ibm" {
  region = "us-south"
  ibmcloud_api_key = ""
  iaas_classic_username = ""
  iaas_classic_api_key = ""
}

OR

### Environment variables
```sh
export IC_API_KEY="ibmcloud_api_key"
export IAAS_CLASSIC_USERNAME="iaas_classic_username"
export IAAS_CLASSIC_API_KEY="iaas_classic_api_key"
```

OR

### Shared file

----------------

Run Terraform

1. init

```sh
# backend initialize 
# install plugins
terraform init
```


2. Plan & Apply

```sh
# check the resources will build , destroy , change
terraform plan

# apply the changes if you conformed
terraform apply
```

----------------

## State file (Very Important file , حلقه الوصل بين الكود و الكلاود )
- Compare between resources in my code , state file 
  resources in Cloud , state file (If resource in cloud is not exist so create it ) 

- if it removed , it will create all resource again (even if it is found)

- If we team every one will have a file state , and it is wrong , this mean every one will make resources seperated 
  so we need to make sharable between us in cloud as a example

## backend 

backend.tf

- we used to store the state file , to be sharable
- If we change any thing in backend , we need to run `terraform init`

## State lock
- if anyone use the state file , it lock the state file , to prevent confilcts

----------------

## Terraform format (FMT)

- reforamt files to be comfortable for the eyes

```sh 
terraform fmt
```

----------------

## Destory

- destroy resources build by terraform 

```sh
terraform destroy
```

----------------
