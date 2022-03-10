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
# Gain Per Account is 1 unit
COSTPERGUESS = {10e-4,10e-6,10e-8,10e-10}
COSTPERCaptCha = {0.01,0.001,0.0001}
USERS = 65424
for i in range(0,182):
	days.append(i)

psiAxisIndex = [float('Inf'),0.015625, 0.000244140625]
kAxisIndex= [5, 20]
CaptchaDiction = [180,50]
thisPath = "."
files = [f for f in os.listdir(thisPath)]
interestedCurve = ["k:5, psi: infinity, capatcha: infinity",
"k:20, psi: infinity, capatcha: infinity",
"k:5, psi: 2e-6, capatcha: infinity",
"k:20, psi: 2e-6, capatcha: infinity"
]
for datafile in files:
	print(datafile)
	if datafile[0] == '.' or '.py' in datafile or '.png' in datafile or 'Eco' == datafile:
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

				if len(l) == 8:
					psiStr = ", psi: inf"
					captchaStr = ", CA: inf"
					if float(l[1]) not in psiAxisIndex:
						continue
					if int(l[0]) not in kAxisIndex:
						continue
					if float(l[1]) != float('Inf'):
						psiStr = ", psi: 2e" + str(int(math.log(float(l[1]))/math.log(2)))
					if float(l[2]) != float('Inf'):
						captchaStr = ", CA: " + str(str(l[2]))

					dw = "k: "+str(l[0]) + psiStr+ captchaStr
					if dw not in CrackDiction:
						CrackDiction[dw] = []
					CrackDiction[dw].append(l[4])

					if dw not in UtilityDiction:
						UtilityDiction[dw] = []
					UtilityDiction[dw].append(min(100.0,100.0*float(l[5])))
					if dw not in GuessDiction:
						GuessDiction[dw] = []
					GuessDiction[dw].append(l[6])					
					if dw not in CaptchaDiction:
						CaptchaDiction[dw] = []
					CaptchaDiction[dw].append(l[7])


			for captchaCost in COSTPERCaptCha:
				for guessCost in COSTPERGUESS:
					titleStr = "_capCost_" + str(captchaCost) + "_guessCost_"+str(guessCost) 
					for k in CaptchaDiction:
						newL = []
						for i in range(0,len(CrackDiction[k])):
							#print(GuessDiction[k][i])
							
							CaptchaCost = float(CaptchaDiction[k][i])*captchaCost*USERS
							GuessCost = float(GuessDiction[k][i])*guessCost*USERS
							Gain = float(CrackDiction[k][i])*USERS
							Profit = Gain - CaptchaCost - GuessCost
							newL.append(Profit)
						plt.plot(days,newL, label = k)
					plt.legend(fontsize='small')
					plt.xlabel('Time (Days)')
					plt.ylabel('GainPerUser/CostPerCaptCha')
					plt.title('Gain-180 Days' + str(datafile)+titleStr)
					plt.savefig("Eco/"+datafile + titleStr +'_eco.png')
					
					plt.clf()






