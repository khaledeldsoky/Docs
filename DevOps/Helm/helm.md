# What is Helm?
` The package manager for Kubernetes , like apt and yum for linux` 

` A Repository is the place where charts can be collected and shared`

---
# [Helm Doc](https://helm.sh/docs/) 

# ▶️ Install Helm

```bash
curl https://baltocdn.com/helm/signing.asc | gpg --dearmor | sudo tee /usr/share/keyrings/helm.gpg > /dev/null
sudo apt-get install apt-transport-https --yes
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/helm.gpg] https://baltocdn.com/helm/stable/debian/ all main" | sudo tee /etc/apt/sources.list.d/helm-stable-debian.list
sudo apt-get update
sudo apt-get install helm
```

## ⏩ helm completion bash
  ```sh
     echo "source <(helm completion bash)" >> ~/.bashrc
     source ~/.bashrc
  ```

***Creates a chart directory***
```sh 
# helm create <name>
helm create Helm-charts
```

like this 
```sh
📁Helm-charts/
    Chart.yaml          # A YAML file containing information about the chart                        [Not used]
    LICENSE             # OPTIONAL: A plain text file containing the license for the chart          [Not used]
    README.md           # OPTIONAL: A human-readable README file  ---------------------------------> [Not used]
    values.yaml         # The default configuration values for this chart                           
    values.schema.json  # OPTIONAL: A JSON Schema for imposing a structure on the values.yaml file  [Not used]
    charts/             # A directory containing any charts upon which this chart depends.
    crds/               # Custom Resource Definitions                                               [Not used]
    templates/          # A directory of templates where the actual yaml file is created
    templates/NOTES.txt # OPTIONAL: A plain text file containing short usage notes                  [Not used]
```
---
# ▶️ [Cheat Sheet](https://helm.sh/docs/howto/charts_tips_and_tricks/)

## ⏩ Chart Management 
```bash 
# Creates a chart directory along with the common files and directories used in a chart.
helm create <name>  

# Packages a chart into a versioned chart archive file.                    
helm package <chart-path> 

# Run tests to examine a chart and identify possible issues:             
helm lint <chart>  

# Inspect a chart and list its contents:
helm show all <chart>  

# Displays the contents of the values.yaml file
helm show values <chart>        

# Download/pull chart        
helm pull <chart>         

# If set to true, will untar the chart after downloading it
helm pull <chart> --untar=true 

# Verify the package before using it
helm pull <chart> --verify

# Default-latest is used, specify a version constraint for the chart version to use
helm pull <chart> --version <number>

# Display a list of a chart’s dependencies:
helm dependency list <chart>            
```

## ⏩ Install and Uninstall Apps

```sh 
# Install the chart with a name
helm install <name> <chart>

# Install the chart in a specific namespace                        
helm install <name> <chart> --namespace <namespace>   

# Set values on the command line (can specify multiple or separate values with commas)
helm install <name> <chart> --set key1=val1,key2=val2 

# Install the chart with your specified values
helm install <name> <chart> --values <yaml-file/url> 

# Run a test installation to validate chart (p)
helm install <name> <chart> --dry-run --debug  

# Verify the package before using it 
helm install <name> <chart> --verify   

# update dependencies if they are missing before installing the chart
helm install <name> <chart> --dependency-update    

# Uninstall a release
helm uninstall <name>                                 
```

---
# ▶️ [Chart Development Tips and Tricks](https://helm.sh/docs/howto/charts_tips_and_tricks/)

## ⏩ Quote Strings, Don't Quote Integers

When you are working with string data, you are always safer quoting the strings
```yml
name: {{ .Values.MyName | quote }}
```

But when working with integers do not quote the values.
```yml
port: {{ .Values.Port }}
```

---

# ▶️ [The Chart File Structure](https://helm.sh/docs/topics/charts/)

```yml
wordpress/
  Chart.yaml          # A YAML file containing information about the chart
  LICENSE             # OPTIONAL: A plain text file containing the license for the chart
  README.md           # OPTIONAL: A human-readable README file
  values.yaml         # The default configuration values for this chart
  values.schema.json  # OPTIONAL: A JSON Schema for imposing a structure on the values.yaml file
  charts/             # A directory containing any charts upon which this chart depends.
  crds/               # Custom Resource Definitions
  templates/          # A directory of templates that, when combined with values,
                      # will generate valid Kubernetes manifest files.
  templates/NOTES.txt # OPTIONAL: A plain text file containing short usage notes
```

# ▶️ [flow control](https://helm.sh/docs/chart_template_guide/control_structures/)

## ⏩ If/Else
```yml
{{ if PIPELINE }}
  # Do something
{{ else if OTHER PIPELINE }}
  # Do something else
{{ else }}
  # Default case
{{ end }}
```

```yml
env:
{{- range .Values.env -}}
  - name: {{ .name }}
    value: {{ .value }}
{{- end -}}  
```

after this you will make value dir and make file for every service and put the values

after this you will make helm for redis
```sh
helm create redis
```
make the service and deployment files for redis in the templete dir 

and make file for redis in values dir



```sh
# to show the values when put it in the chart
helm templete -f <file-values-for-one-chart> <path for helm chart dir>
helm template -f values/adservice-values.yml helm/microservice

#for examines a chart for possible issues
helm lint -f <file-values-for-one-chart> <path for helm chart dir>
helm lint -f values/adservice-values.yml helm/microservice/

# to create a release from my values in the chart name
# this is actual use for run the chart in kube
helm install -f myvalues.yaml release_name chart_name

#i think it is like templete
helm install --dry-run -f myvalues.yaml release_name chart_name

#delete release
helm uninstall release_name
```
---

## 📁 helmfile

```sh
apt install helmfile
helmfile sync
helmfile list
helmfile destroy
```

