tot = 0
pwd = []
dist = []
unique = {}
with open('LinkedIn-withcount.txt', encoding = "ISO-8859-1") as f:
	lines = f.readlines()
	for line in lines:
		l = line.split()
		if len(l) < 2:
			continue
		tot += int(l[0])
		unique[l[1]] = 0
	for i in range(5):
		line = lines[i]
		l = line.split()
		pwd.append(l[1])
		dist.append(float(l[0]))
	for i in range(5):
		line = lines[-1*i]
		l = line.split()
		pwd.append(l[1])
		dist.append(float(l[0]))
for i in range(5):
	print(i+1,pwd[i],dist[i],dist[i]/float(tot))
for i in range(5):
	print(len(unique) - i,"--",pwd[-1*i],dist[-1*i],dist[-1*i]/float(tot))
