import matplotlib.pyplot as plt

n = 50
sumL = []
for i in range(10000)
	rnd = np.random.binomial(size=n, n=1, p= 0.5)
	sums = 0.0
	power = 1
	for j in rnd:
		sums += (2**(power)) * ((-1)**(rnd[j]))
		power+=1
	sumL.append(sums)

hist, bins = np.histogram(sumL, bins=100, normed=True)
bin_centers = (bins[1:]+bins[:-1])*0.5
plt.plot(bin_centers, hist)