sum = 0;
with open ('LinkedIn.txt') as f:
	lines = f.readlines()
	for line in lines:
		l = line.split()
		sum += int(l[1])

print(sum)
