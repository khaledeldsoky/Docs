alias khaled="ls -a"
unalias khaled

ps -ef | awk '{print $1}' | sort | uniq 