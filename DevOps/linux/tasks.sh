# --------  Print Numbers from 1 to 10 -------- #
# #! /bin/bash
# num=1
# while [ $num -le 10 ];do
# echo "$num"
#   ((num++))
# done


# -------- List Files in Current Directory -------- #
# #!/bin/bash
# for file in *; do
#   echo "File: $file"
# done

#! /bin/bash

filename=$1

if [ -f "$filename" ]; then
  echo "file is exist"
else
  echo "file not exist"
fi
