#Wuwei's Universal HeatMap Ploting Tools
import sys
import pandas as pd
import seaborn as sns
import os
import matplotlib.pylab as plt 
import numpy as np
from matplotlib.colors import LogNorm
import math
CrackDiction = {}
UtilityDiction = {}
GuessDiction = {}
days = []
for i in range(0,182):
	days.append(i)

psiAxisIndex = [float('Inf'),0.015625, 0.000244140625]
kAxisIndex= [5, 20]
thisPath = "."
files = [f for f in os.listdir(thisPath)]
for datafile in files:

	print(datafile)
	if datafile[0] == '.' or '.py' in datafile or '.png' in datafile:
		continue
	else:
		with open(datafile) as f:
			CrackDiction = {}
			UtilityDiction = {}
			GuessDiction = {}
			CaptchaDiction = {}
			dw = [-1,-1,-1]
			lines = f.readlines()
			for line in lines:
				if "Password:" in line:
					continue
				l = line.split()

				if len(l) == 4:
					psiStr = ", psi: infinity"
					if float(l[1]) not in psiAxisIndex:
						continue
					if int(l[0]) not in kAxisIndex:
						continue
					if float(l[1]) != float('Inf'):
						psiStr = ", psi: 2e" + str(int(math.log(float(l[1]))/math.log(2)))


					dw = "k: "+str(l[0]) + psiStr
					if dw not in CrackDiction:
						CrackDiction[dw] = []
					if str(l[0]) == "5":
						CrackDiction[dw].append(1.1*float(l[3]))
					else:
						CrackDiction[dw].append(float(l[3]))
			for k in CrackDiction:
				print(k)
				plt.plot(days,CrackDiction[k],label = k)
			plt.legend(fontsize='small')
			plt.xlabel('Time (Days)')
			plt.ylabel('Locked Rate Users')
			plt.title('Unwanted Locked Out(180 days): ' + str(datafile))
			plt.savefig(datafile+ '_No_Attacker.png')
			plt.clf()

		




