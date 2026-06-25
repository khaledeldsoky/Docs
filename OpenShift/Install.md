[OpenShift Downloads](https://mirror.openshift.com/)


# Prerequisite

- VMware
- RHCOS ISO
- OpenShift installer


1. Go to [Red Hat OpenShift](wsl --list --verbosehttps://console.redhat.com/openshift/install/metal/user-provisioned)

  - Install RHCOS (rhcos-live.x86_64.iso) from RedHat.
  - Install OpenShift installer (openshift-install-linux.tar.gz) from REdHat
  - Download pull secret Pull

```sh
tar -xvzf openshift-install-linux.tar.gz 

chmod +x openshift-install

./openshift-install create install-config --dir=.

```

2. Install VMvare to convert the ISO format to RAQ or QCOW2 or vhd to able to upload it as custom ISO in cloud


Create ssh key
- ssh-keygen -t ed25519 -f ~/.ssh/id_ed25519 -N ""
- Public key path /home/khaled/.ssh/id_ed25519.pub


## Download the OpenShift Installer
```sh
wget https://mirror.openshift.com/pub/openshift-v4/clients/ocp/latest/openshift-install-linux.tar.gz
tar -xvf openshift-install-linux.tar.gz
chmod +x openshift-install
sudo mv openshift-install /usr/local/bin/
```

## Download the OpenShift Client (oc):
```sh
wget https://mirror.openshift.com/pub/openshift-v4/clients/ocp/latest/openshift-client-linux.tar.gz
tar -xvf openshift-client-linux.tar.gz
sudo mv oc /usr/local/bin/
```

## Download RHCOS Images:

```sh
wget https://mirror.openshift.com/pub/openshift-v4/x86_64/dependencies/rhcos/4.12/4.12.10/rhcos-4.12.10-x86_64-metal.x86_64.raw.gz
wget https://mirror.openshift.com/pub/openshift-v4/x86_64/dependencies/rhcos/4.12/4.12.10/rhcos-4.12.10-x86_64-live.x86_64.iso
```

## Download Pull Secret:
https://console.redhat.com/openshift/install/pull-secret

## Install Apache
```sh
sudo apt install -y apache2
sudo systemctl start apache2
```

## Copy Files:
```sh
sudo mkdir -p /var/www/html/ocp4
sudo cp ~/ocp-install/*.ign /var/www/html/ocp4/
sudo cp ~/ocp-install/rhcos-4.12.10-x86_64-live.x86_64.iso  /var/www/html/ocp4/rhcos
sudo chown -R www-data:www-data /var/www/html/ocp4
sudo chmod -R 755 /var/www/html/ocp4
```