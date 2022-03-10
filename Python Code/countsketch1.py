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
str = ' 290729 123456'
w1, w2 = str.split()
#print ('first ', w1, ' second ', w2)
d=100
w=600
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
def AddSketchCount(x):
    for i in range(1,d):
        col=H_w(x,i,w)
        val=Hash1(x,i)
        matrix_a[i][col]=matrix_a[i][col]+val
        #print(matrix_a)
def EstimateSketchCount(x):
    total=0
    for i in range(1,d):
        col=H_w(x,i,w)
        total=total+matrix_a[i][col]*Hash1(x,i)
    return total/d
#hash = {k:v for k, v in zip(pwds, freq)}
#print (rockyou)
atfile = open("d1.txt")
df1=pd.DataFrame(np.nan, index = range(0,500), columns=['Password', 'hcu','compr'])
df1=df1.astype('object')
df2=pd.DataFrame(np.nan, index = range(0,500), columns=['Password', 'hcu','compr'])
df2=df1.astype('object')
for row in df1.index:
    df1['hcu']=0
    df2['hcu']=0
   # df['attempts']=pd.Series([['1','2']]*df.shape[0])
    #df['attempts']=['s','l']
curr=0
userattempts=[]
my_cols=['userID','Time','Pwd','Hon/At','Suc/Fail']
df1=pd.read_table("d1.txt", sep=',',names=my_cols)
df1.replace({'ACCOUNT HACKED':'-1'}, inplace=True)
df1.Time = pd.to_numeric(df1.Time, errors='coerce').fillna(0).astype(np.int64)
df1.userID = pd.to_numeric(df1.userID, errors='coerce').fillna(0).astype(np.int64)

df1=df1.sort_values(bgy=['Time'])
df1.to_csv("d2.txt")
for index, row in df1.iterrows():
    print (row['userID'], row['Time'],row['Pwd'])
df2.to_csv('2018system1primeattack2.csv')
#print(df1)
print('done')