cat <<EOF > ~/.ssh/config 
host bastion
   HostName $1
   User <REMOTE_USER>
   IdentityFile <PATH_TO_PRIVATE_FILE>
   StrictHostKeyChecking=no

host <ANY_NAME_YOU_NEED>
   HostName  $2
   user  <REMOTE_USER>
   IdentityFile <PATH_TO_PRIVATE_FILE>
   ProxyCommand ssh -q -W %h:%p  bastion
   StrictHostKeyChecking=no
EOF
