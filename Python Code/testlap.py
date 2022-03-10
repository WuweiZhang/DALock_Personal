import numpy as np

file = open('littlerockyouNOISE.out','r')
data=np.genfromtxt('littlerockyouNOISE.out', delimiter=',', usecols=range(4000))
#data=data.reshape(4000,30000)
print (data)
