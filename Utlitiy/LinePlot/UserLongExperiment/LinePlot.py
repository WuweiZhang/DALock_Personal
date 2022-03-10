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
colorsList = ['tab:blue', 'tab:green','tab:purple','tab:pink','olive']
colorsListForPsi = ['tab:green','tab:purple','tab:pink','olive']
lineStyleList = ['-',":"]
psiFirst = 0.00891800293815806
psiVDict = {}
CaptchaDiction = [float('Inf'),180,50]
thisPath = "."
files = [f for f in os.listdir(thisPath)]
for datafile in files:
	print(datafile)
	if datafile[0] == '.' or '.py' in datafile or '.png' in datafile:
		continue
	else:
		psi2Str = {}
		with open(datafile) as f:
			lines = f.readlines()
			for line in lines:
				l = line.split()
				if "Password:" in line:
					continue
				if len(l) == 8 and l[1] != '∞':
					psi2Str[l[1]] = l[1]
		kList = []
		for k in psi2Str:
			kList.append(k)

		kList.sort(reverse=True)
		print(kList)
		ranks = [6,7,8,9]
		rank= 0
		for k in kList:
			psi2Str[k] = "2^-" + str(ranks[rank])  + ""
			rank+=1

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
					psiStr = ",psi: ∞"
					captchaStr = "captcha_∞"
					if l[1] != '∞':
						psiStr = ",psi:" + psi2Str[l[1]]
						psiVDict[float(l[1])] = 1

					if l[2] != '∞':
						captchaStr = "captcha_" + str(str(l[2]))
					dw = captchaStr
					dw2 = "k: "+str(l[0])+ psiStr 

					if dw not in CrackDiction:
						CrackDiction[dw] = {}
					if dw not in UtilityDiction:
						UtilityDiction[dw] = {}
					if dw not in CaptchaDiction:
						CaptchaDiction[dw] = {}
					if dw not in GuessDiction:
						GuessDiction[dw] = {}


					if dw2 not in CrackDiction[dw]:
						CrackDiction[dw][dw2] = []
					CrackDiction[dw][dw2].append(float(l[4])*100.0)

					if dw2 not in UtilityDiction[dw]:
						UtilityDiction[dw][dw2] = []
					UtilityDiction[dw][dw2].append(l[5])


					if dw2 not in GuessDiction[dw]:
						GuessDiction[dw][dw2] = []
					GuessDiction[dw][dw2].append(l[6])					


					if dw2 not in CaptchaDiction[dw]:
						CaptchaDiction[dw][dw2] = []
					CaptchaDiction[dw][dw2].append(l[7])
			psiPlot = {}
			for k in psiVDict:
				psiPlot[k] = []
				for i in range(182):
					psiPlot[k].append(100.0*(k+ psiFirst))

			for captcha in CrackDiction:
				fig = plt.figure()
				ax = plt.subplot(111)
				#print(captcha)
				markerID = 2
				linestyleID = 0
				colorid = 0
				for k in CrackDiction[captcha]:
					print ("|--->" + k)
					#print(colorsList[colorid])
					ax.plot(days,CrackDiction[captcha][k],label = k,markerSize = 8, marker=markerID,color = colorsList[int(colorid/2)],linestyle=lineStyleList[linestyleID%2],markevery =20)
					markerID +=1
					colorid+=1
					linestyleID+=1
				colorid = 0
				for k in psiPlot:
					#print(colorsList[colorid])
					ax.plot(days,psiPlot[k],label = psi2Str[str(k)] +"+f(p1)",color = colorsListForPsi[colorid],linestyle = '--')
					colorid+=1
				plt.xlabel('Time (Days)')
				plt.ylabel('Cracked Users(%)')
				plt.title('RockYou')
				box = ax.get_position()
				ax.set_position([box.x0, box.y0, box.width*0.8 , box.height])

# Put a legend to the right of the current axis
				ax.legend(loc='center left', bbox_to_anchor=(1, 0.5),fancybox=True, shadow=True)
				plt.savefig(datafile+ '_Attacker' + captcha +'.png')
				plt.clf()


