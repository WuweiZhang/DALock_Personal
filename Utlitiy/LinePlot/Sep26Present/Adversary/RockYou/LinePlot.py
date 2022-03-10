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
			dw = [-1,-1]
			lines = f.readlines()
			for line in lines:
				if "Password:" in line:
					continue
				l = line.split()

				if len(l) == 6:
					if float(l[1]) not in psiAxisIndex:
						continue
					if int(l[0]) not in kAxisIndex:
						continue
					if float(l[1]) == float('Inf'):
						dw = "k: "+ str(l[0]) + ", psi: infinity" 
					else:

						dw = "k: "+ str(l[0]) + ", psi: 2e" + str(int(math.log(float(l[1]))/math.log(2)))
					if dw not in CrackDiction:
						CrackDiction[dw] = []
					CrackDiction[dw].append(l[3])

					if dw not in UtilityDiction:
						UtilityDiction[dw] = []
					UtilityDiction[dw].append(min(1.0,float(l[4])))


					if dw not in GuessDiction:
						GuessDiction[dw] = []
					GuessDiction[dw].append(l[5])					

			for k in CrackDiction:
				print(k)
				plt.plot(days,CrackDiction[k],label = k)
			plt.legend(fontsize='small')
			plt.xlabel('Time (Days)')
			plt.ylabel('Cracked Users')
			plt.title('Progress of Attack Over 180 Days: ' + str(datafile))
			plt.savefig(datafile+ '_Attacker.png')

			plt.clf()
			for k in CrackDiction:
				plt.plot(days,UtilityDiction[k],label = k)
			plt.legend(fontsize='small')
			plt.xlabel('Time (Days)')
			plt.ylabel('Locked Users')
			plt.title('Usr:' + str(datafile))
			plt.savefig(datafile+'_user.png')
			plt.clf()
			for k in CrackDiction:
				newL = []
				for i in range(0,len(CrackDiction[k])):
					if (float(GuessDiction[k][i]) == 0):
						newL.append(0)
					else:
						newL.append(float(CrackDiction[k][i])/float(GuessDiction[k][i]))
				plt.plot(days,newL, label = k)
			plt.xlabel('Time (Days)')
			plt.ylabel('Economic Value Per Guess')
			plt.title('Eco:' + str(datafile))
			plt.legend(fontsize='small')
			plt.savefig(datafile+'_advEco.png')
			plt.clf()




