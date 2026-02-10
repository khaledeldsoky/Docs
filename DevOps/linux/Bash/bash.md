## BASH
If examples:

if [ "$var" == "value" ]; then
    echo "Match"
else
    echo "No match"
fi
Example 1: Check if a file exists
#!/bin/bash
file="test.txt"
if [ -f "$file" ]; then
    echo "$file exists"
else
    echo "$file does not exist"
fi
Example 2: Compare numbers 
#!/bin/bash
num=15
if [ $num -gt 10 ]; then
    echo "$num is greater than 10"
elif [ $num -eq 10 ]; then
    echo "$num equals 10"
else
    echo "$num is less than 10"
fi
Example 3: Check string equality 
#!/bin/bash
user="Alice"
if [ "$user" = "Alice" ]; then
    echo "Welcome, $user!"
else
    echo "Unknown user"
fi
Example 4: Check if a directory is empty 
#!/bin/bash
dir="myfolder"
if [ -z "$(ls -A $dir)" ]; then
    echo "$dir is empty"
else
    echo "$dir is not empty"
fi

Loops :




For
for i in 1 2 3; do
    echo $i
done
Example 1: Iterate over a list of words 
#!/bin/bash
for fruit in apple banana orange; do
    echo "Fruit: $fruit"
done
Example 2: Loop over files in a directory 
#!/bin/bash
for file in *.txt; do
    echo "Found file: $file"
done
Example 3: Iterate over a range 
#!/bin/bash
for i in {1..5}; do
    echo "Number: $i"
done
Example 4: Loop with command output 
#!/bin/bash
for line in $(cat names.txt); do
    echo "Name: $line"
done

While
count=1
while [ $count -le 3 ]; do
    echo $count
    ((count++))
done
Example 1: Count from 1 to 5 
#!/bin/bash
count=1
while [ $count -le 5 ]; do
    echo "Count: $count"
    ((count++))
done
Example 2: Read a file line by line
#!/bin/bash
file="input.txt"
while IFS= read -r line; do
    echo "Line: $line"
done < "$file"
Example 3: Wait for a file to exist
#!/bin/bash
file="data.txt"
while [ ! -f "$file" ]; do
    echo "Waiting for $file to appear..."
    sleep 2
done
echo "$file found!"
Example 4: Infinite loop with user input 
#!/bin/bash
while true; do
    echo "Enter 'quit' to exit: "
    read input
    if [ "$input" = "quit" ]; then
        break
    fi
    echo "You entered: $input"
done



Function
my_function() {
    echo "Hello, $1!"
}
my_function "User"

