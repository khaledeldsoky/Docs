**scores.txt**
```
1 7.4
2 0.4
3 1.6
4 6.2
5 7.6
6 7.7
7 5.6
8 4.4
```

## get sum for the second part (floadting numbers)

```sh
awk '{sum += $2} END {print sum}' scores.txt
awk '{sum += $2} END {printf "%.2f\n", sum}' scores.txt
```
## summ is not a function , it is a ordinary name , you can replace it with "khaled"

## --------------------------------------------------------------- ##

## to get the avg
```sh
awk '{avg += $2} END {printf "%.2f\n", avg / NR}' scores.txt > solution
```
awk already knows how many lines there are using **NR** (number of records)