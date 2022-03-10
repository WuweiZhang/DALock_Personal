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
d=40
w=300
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
#total needs to be replaced with array of hashes from each col, then return min from this array
    total=0
    for i in range(1,d):
        col=H_w(x,i,w)
        total=total+matrix_a[i][col]*Hash1(x,i)
    #print('total ',total,', d ',d)
    return total/d
#hash = {k:v for k, v in zip(pwds, freq)}
#print (rockyou)
atfile = open("outfile3.txt")
df=pd.DataFrame(np.nan, index = range(0,501), columns=['Password', 'hcu','compr', 'attempts'])
df=df.astype('object')
for row in df.index:
    df['hcu']=0
    df.at[row,'attempts']=[]
   # df['attempts']=pd.Series([['1','2']]*df.shape[0])
    #df['attempts']=['s','l']
curr=0
#df.at[1,'attempts'].append('test')
#print ('df' , df)
#df.at[1,'attempts'].append('test1')
#print ('df' , df)
file = list(open("rockyouintersection.txt",  encoding = "latin-1"))
for line in reversed(file):
    try:
        line = line.strip()
        #print(line)
        w1,n = line.split(',', 1)
        #freq.append(int(n))
        #pwds.append(w)
        AddSketchCount(w1,int(n))
        print('added ',w1,n)
        uu=EstimateSketchCount(w1)
        #print('estimated ',w1,uu)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
rockyou = zip(pwds,freq)
for line in atfile:
    line=line.strip()
    #print(line)
    items=line.split(',')
    #print(len(items))
    #print(items[2])
   # if len(items)==2:
    if items[2]=='-1':
        df.loc[int(items[1]), 'compr']=1
    if ((items[4] and items[5])=='0' and items[2]!='-1'):
        #it means that pwd was reset
        df.loc[int(items[1]), 'Password']=items[3]
        #print(items[1],items[3])
        #print(df.at[int(items[1]),'attempts'])
        df.at[int(items[1]),'attempts'].append(str(items[3]))
        #print(df.loc[int(items[1])])
        #print(items[1],'curr')
        #print(userattempts)
            #print(curr,'curr')
            #df.loc[curr, 'hcu']=freq[pwds.index(items[1])]+df.loc[curr, 'hcu']
            #print(df.loc[curr-1])
            # if items[1]!=df.loc[curr, 'Password']:
                # print(df.loc[curr-1])
                # df.loc[curr, 'hcu']=freq[pwds.index(items[1])]+df.loc[curr, 'hcu']
                # print(df.loc[curr-1])
    else:
                #print(items[2])
                #check if current attempted password is in attempts list
                    #print('userattempts', userattempts)
            #print('userattempts again ', userattempts)
                   # df.at[curr-1, 'attempts']=userattempts
            # df.loc[int(items[1]),'Password']=items[3]
                # if hash.has_key(items[2]):
                    # index1=hash.get(items[2])
                # else:
                    # index1=0.5
                #print('estimate ',items[2], ' ',EstimateSketchCount(items[2]))
                    #print('before here', df.loc[curr-1,'hcu'])
        #print(df.at[int(items[1]),'attempts'])
        #print('checking pwd ', items[3])
        if str(items[3]) not in df.at[int(items[1]),'attempts']:
            index1=EstimateSketchCount(items[3])
            #print('estimated ',items[3],index1)
            df.loc[int(items[1]), 'hcu']=index1+df.loc[int(items[1]), 'hcu']
            print('hh',curr, items[2], index1)
                    #print(df)
                #print(df.loc[curr-1])
        #df.loc[int(items[1]),'attempts']=
        else:
            pwdtostr=str(items[3])
            df.at[int(items[1]),'attempts'].append(pwdtostr)
header=['Password', 'hcu','compr']
df.to_csv('countSketchsys1prime.csv', columns=header)
print('done')