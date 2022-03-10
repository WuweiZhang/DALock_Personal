#Wuwei's Universal HeatMap Ploting Tools
import sys
import pandas as pd
import seaborn as sns
import os
import matplotlib.pylab as plt 
import numpy as np
from matplotlib.colors import LogNorm
import math
# @param FilePath: It contains multiple folders, the name for each folder will be considered as 
# the name of the algorithm
# @return algoNames: a list of algorithm names
def getAlgoNames(FilePath):
	#os.getcwd()
	ret =  [d for d in os.listdir(FilePath ) if os.path.isdir(FilePath + '/'+str(d))]
	#print (os.listdir(FilePath ))
	#print (os.path)
	#print(ret)
	return ret
	

def fillMap(algoMaps, FilePath,algoNames):
	# For each file, we assume the following format:
	# Line 1: Z Parameter, X parameter, Y parameter, Metric Score
	for algo in algoNames:
		#print(algo)
		thisPath = FilePath+'/'+str(algo)
		files = [f for f in os.listdir(thisPath)]
		for datafile in files:
			if datafile[0] == '.':
				continue
			with open(thisPath+'/'+str(datafile)) as f:
				#print ("file",FilePath+'/'+str(algo))
				lines = f.readlines()
				#print lines
				for line in lines:
					#print(line)
					l = line.split()
					if len(l) != 6:
						continue
					if int(l[2]) != 180:
						continue
					x = int(l[0])
					y = float(math.log(float(l[1]),2))
					if (x,y) not in algoMaps[algo]:
						algoMaps[algo][(x,y)] = 0
					algoMaps[algo][(x,y)] += round(float(l[4]),4)


def plotOneNum(dict_sim_scores, xTitle,yTitle,zTitle,name ):
	ser = pd.Series(list(dict_sim_scores.values()),index=pd.MultiIndex.from_tuples(dict_sim_scores.keys()))
	df = ser.unstack()#.fillna(1500)
	c = np.array(df.columns)
	print(df.values)
	fig, ax = plt.subplots(1,1)
	pc = ax.pcolormesh(df.values, cmap="Blues",norm=LogNorm(df.min(),df.max()),cbar_kws={"ticks":[0,2e-2,2e-1,2e0]})
	fig.colorbar(pc)
	#ax.scatter(s=100)
	plt.show()
def plotOne(dict_sim_scores, xTitle,yTitle,zTitle,name ):
	print(list(dict_sim_scores.values()))
	#for (x,y) in dict_sim_scores:
		#print(x,y)
	print (pd.MultiIndex.from_tuples(dict_sim_scores.keys()))
	#plt.xticks(rotation=‌​45)
	ser = pd.Series(list(dict_sim_scores.values()),
                  index=pd.MultiIndex.from_tuples(dict_sim_scores.keys()))
	df = ser.unstack()#.fillna(1500)
	df.shape
	svm = sns.heatmap(df,cmap="Blues",norm=LogNorm(df.min(),df.max()),cbar_kws={"ticks":[0,2e-5,2e-4,2e-3,2e-2,2e-1,2e0]},square = True,xticklabels="auto", yticklabels="auto",annot=True)
	svm.invert_yaxis()
	fmt = '{:%.2f}'

	xticklabels = []
	for item in svm.get_xticklabels():
		item.set_text(float((item.get_text())) )
		xticklabels += [item]
	yticklabels = []
	for item in svm.get_yticklabels():
		item.set_text(float((item.get_text())) )
		yticklabels += [item]

	svm.set_xticklabels(xticklabels,rotation=90)
	svm.set_yticklabels(yticklabels,rotation=0)
	figure = svm.get_figure()    
	figure.savefig(name + '.jpg', dpi=400)
	print(name)



def plotAll(algoMaps,xTitle,yTitle,zTitle):
	#print("here")
	for algoDict in algoMaps:

		thisMap = algoMaps[algoDict]
		plotOne(thisMap,xTitle,yTitle,zTitle,str(algoDict)+ '_')


def main():
	thirdParameterList = [] 
	filePath = sys.argv[1]
	xTitle = sys.argv[2]
	yTitle = sys.argv[3]
	zTitle = sys.argv[4]

	algoNames = getAlgoNames(filePath)
	algoMaps = {} # This dictionary takes algorithn name as key and stores a dictionary (of its heatmaps) as value
	# For each dictionary under an algorithm name, the key is pair (x,y,z), for example, privacy budget for z, width for x
	# Depth for y, the value will be the metric score
	# A dictionary where the value is the metric score
								# For example, z = epsilon, x = w, y = d
	for algo in algoNames:
		algoMaps[algo] = {}

	fillMap(algoMaps, filePath, algoNames)

	plotAll(algoMaps,xTitle,yTitle,zTitle)


main()
