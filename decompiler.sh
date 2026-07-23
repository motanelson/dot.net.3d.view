printf "\ec\e[45;36m\ngive me a file to decompile ? \n"
read d
objdump -d $d -M intel
