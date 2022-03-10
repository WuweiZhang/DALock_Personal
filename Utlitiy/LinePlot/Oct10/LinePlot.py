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
GAINPERACCOUNTOVERCAPTCHA = {1,10,100,1000}
USERS = 65424
for i in range(0,182):
	days.append(i)

psiAxisIndex = [float('Inf'),0.015625, 0.000244140625]
kAxisIndex= [5, 20]
CaptchaDiction = [float('Inf'),180,50]
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

				if len(l) == 8:
					psiStr = ", psi: infinity"
					captchaStr = ", captcha: infinity"
					if float(l[1]) not in psiAxisIndex:
						continue
					if int(l[0]) not in kAxisIndex:
						continue
					if float(l[1]) != float('Inf'):
						psiStr = ", psi: 2e" + str(int(math.log(float(l[1]))/math.log(2)))
					if float(l[2]) != float('Inf'):
						captchaStr = ", captcha: " + str(str(l[2]))

					dw = "k: "+str(l[0]) + psiStr
					if captchaStr ==  ", captcha: infinity":
						if dw not in CrackDiction:
							CrackDiction[dw] = []
						CrackDiction[dw].append(l[4])

						if dw not in UtilityDiction:
							UtilityDiction[dw] = []
						UtilityDiction[dw].append(min(100.0,100.0*float(l[5])))
						if dw not in GuessDiction:
							GuessDiction[dw] = []
						GuessDiction[dw].append(l[6])					

					dw += captchaStr
					if dw not in CaptchaDiction:
						CaptchaDiction[dw] = []
					CaptchaDiction[dw].append(l[7])

			for k in CrackDiction:
				print(k)
				plt.plot(days,CrackDiction[k],label = k)
			plt.legend(fontsize='small')
			plt.xlabel('Time (Days)')
			plt.ylabel('Cracked Users(%)')
			plt.title('Progress of Attack Over 180 Days: ' + str(datafile))
			plt.savefig(datafile+ '_Attacker.png')
			plt.clf()

			for k in UtilityDiction:
				plt.plot(days,UtilityDiction[k],label = k)
			plt.legend(fontsize='small')
			plt.xlabel('Time (Days)')
			plt.ylabel('Locked Users(%)')
			plt.title('Usr:' + str(datafile))
			plt.savefig(datafile+'_user.png')
			plt.clf()




			for k in CaptchaDiction:
				newL = []
				for i in range(0,len(CaptchaDiction[k])):
					if (float(CaptchaDiction[k][i]) == 0):
						newL.append(0)
					else:
						newL.append(float(CaptchaDiction[k][i]))
				plt.plot(days,newL, label = k)
			plt.xlabel('Time (Days)')
			plt.ylabel('Captcha Per User')
			plt.title('captcha_' + str(datafile))
			plt.legend(fontsize='small')
			plt.savefig(datafile+'_Captcha.png')
			plt.clf()




