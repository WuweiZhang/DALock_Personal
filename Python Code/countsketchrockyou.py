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
d=4000
w=30000
count=0
#sketch = count_min_sketch.CountMinSketch(d, w)
matrix_a=np.zeros((d,w))
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
        #print(matrix_a)
def EstimateSketchCount(x):
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
file = list(open("rockyou-withcount.txt",  encoding = "latin-1"))
for line in file:
    try:
        line = line.strip()
        #print(line)
        n,w1 = line.split(' ', 2)
        #freq.append(int(n))
        #pwds.append(w)
        AddSketchCount(w1,int(n))
        print('added ',w1,n)
        uu=EstimateSketchCount(w1)
        print('estimated ',w1,uu)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
rockyou = zip(pwds,freq)

np.savetxt('test.out', matrix_a, delimiter=',')
print('done')