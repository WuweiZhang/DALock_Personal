#countsketch Rockyou save to file
import json
import random
import pandas as pd
import numpy as np
import sha3
from functools import reduce
import binascii
#from countminsketch import CountMinSketch
pwds = []
freq = []
#str = ' 290729 123456'
#w1, w2 = str.split()
#print ('first ', w1, ' second ', w2)
d=400
w=3000
count=0
#sketch = count_min_sketch.CountMinSketch(d, w)
matrix_a=np.zeros((d,w))
matrix_a1=np.zeros((d,w))
loc, scale=0., d/4
for i in range (0, d):
    for j in range(0,w):
        s=np.random.laplace(loc, scale)
        matrix_a[i,j]=s

print(matrix_a)
def H_w(x,i,w):
    #b=bytearray()
    #b.extend(map(ord,x))
    #print(b)
    #b=b|i
    #print(x)
    b=bin(int.from_bytes(x.encode(), 'big'))    #x=x|i
    #print(b)
    b1=int(b,2)
    #print(b1,i)
    b2=bin(b1|i)
    #print(b2)
    temp = int(sha3.sha3_224(b2.encode('utf-8')).hexdigest(),16)
    #temp= temp&i
    #print(type(temp))
    #print(type(i))
    temp=temp % w
    #print(temp)
    return temp
def Hash1(x,i):
    result =-1
    b=bin(int.from_bytes(x.encode(), 'big'))    #x=x|i
    #print('hash1 ',b)
    b1=int(b,2)
    #print(b1,i)
    b2=bin(b1|i)
    #print(b2)
    temp = int(sha3.sha3_224(b2.encode('utf-8')).hexdigest(),16)
    #temp= temp&i
    #print(type(temp))
    #print(type(i))
    temp=temp % 2
    if temp==1: 
        result=1
    return result
def AddSketchCount(x,n):
    for i in range(1,d):
        col=H_w(x,i,w)
        val=Hash1(x,i)
        matrix_a[i][col]=matrix_a[i][col]+val*n
        matrix_a1[i][col]=matrix_a1[i][col]+val*n

        #print(matrix_a)
def EstimateSketchCount(x):
    total=0
    for i in range(1,d):
        col=H_w(x,i,w)
        total=total+matrix_a1[i][col]*Hash1(x,i)
    #print('total ',total,', d ',d)
    return total/d
def EstimateSketchCountLap(x):
    total=0
    for i in range(1,d):
        col=H_w(x,i,w)
        total=total+matrix_a[i][col]*Hash1(x,i)
    #print('total ',total,', d ',d)
    return total/d

#hash = {k:v for k, v in zip(pwds, freq)}
#print (rockyou)
#atfile = open("outfile3.txt")
#df=pd.DataFrame(np.nan, index = range(0,500), columns=['Password', 'hcu','compr'])
#df=df.astype('object')
#for row in df.index:
#    df['hcu']=0
   # df['attempts']=pd.Series([['1','2']]*df.shape[0])
    #df['attempts']=['s','l']
curr=0
cumerror=0
cumerrorlap=0
error=0
errorLap=0
file = list(open("rockyoutop500.txt",  encoding = "latin-1"))
for line in file:
    try:
        line = line.strip()
        #print(line)
        n,w1 = line.split(' ', 2)
        #freq.append(int(n))
        #pwds.append(w)
        #cumerror=cumerror+int(n)
        AddSketchCount(w1,int(n))
        print('added ',w1,n)
        #uu=EstimateSketchCount(w1)
        #cumerrorlap=cumerrorlap+uu
        #error=error + abs(int(n)-uu)
        #print('estimated ',w1,uu)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
rockyou = zip(pwds,freq)
file1 = list(open("rockyouintersection.txt",  encoding = "latin-1"))
for line in reversed(file1):
    try:
        line = line.strip()
        #print(line)
        w1,n = line.split(',', 1)
        #freq.append(int(n))
        #cumerror=cumerror+int(n)
        #pwds.append(w1)
        AddSketchCount(w1,int(n))
        print('added ',w1,n)
        #uu=EstimateSketchCount(w1)
        #cumerrorlap=cumerrorlap+uu
        #error=error + abs(int(n)-uu)
        #print('estimated ',w1,uu)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
for line in file:
    try:
        line = line.strip()
        #print(line)
        n,w1 = line.split(' ', 2)
        #freq.append(int(n))
        #pwds.append(w)
        #cumerror=cumerror+int(n)
        #AddSketchCount(w1,int(n))
        #print('added ',w1,n)
        uu=EstimateSketchCount(w1)
        uu1=EstimateSketchCountLap(w1)
        errorLap=errorLap+abs(int(n)-uu1)
        #cumerrorlap=cumerrorlap+uu
        error=error + abs(int(n)-uu)
        print('estimated ',w1,uu)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
for line in reversed(file1):
    try:
        line = line.strip()
        #print(line)
        w1,n = line.split(',', 1)
        #freq.append(int(n))
        #cumerror=cumerror+int(n)
        #pwds.append(w1)
        #AddSketchCount(w1,int(n))
        #print('added ',w1,n)
        uu=EstimateSketchCount(w1)
        uu1=EstimateSketchCountLap(w1)
        #cumerrorlap=cumerrorlap+uu
        error=error + abs(int(n)-uu)
        errorLap=errorLap+abs(int(n)-uu1)
        print('estimated ',w1,uu)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass

#np.savetxt('littlerockyouNOISEEps1.out', matrix_a)
print('Eps4 countsketch, countsketch with laplacian noise: ', error/32600000, errorLap/32600000)