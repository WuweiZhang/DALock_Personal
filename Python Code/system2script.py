import json
import random
import pandas as pd
import numpy as np
import time
pwds = []
freq = []
str = ' 290729 123456'
w1, w2 = str.split()
#print ('first ', w1, ' second ', w2)
file = open("C:/LargeFiles/rockyou-withcount.txt",  encoding = "latin-1")
for line in file:
    try:
        line = line.strip()
        #print(line)
        n, w = line.split(' ', 1)
        freq.append(int(n))
        pwds.append(w)
        #print(n, w)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
rockyou = dict(zip(freq, pwds))
#hash = {k:v for k, v in zip(pwds, freq)}
#print (rockyou)
N=len(freq)
atfile = open("attackeddataset500new.txt")
df=pd.DataFrame(np.nan, index = range(0,500), columns=['Password', 'hcu','compr'])
df=df.astype('object')
for row in df.index:
    df['hcu']=0
   # df['attempts']=pd.Series([['1','2']]*df.shape[0])
    #df['attempts']=['s','l']
curr=0
userattempts=[]
for line in atfile:
    line=line.strip()
    #print(line)
    items=line.split(',')
    #print(len(items))
    if len(items)==2:
        print(items[0])
        if curr==int(items[0]):
            df.loc[curr, 'Password']=items[1]
            #print(df.loc[curr])
            curr=curr+1
            #print(curr,'curr')
            #userattempts=[]
        else:
            df.loc[curr-1, 'Password']=items[1]
            #print(curr,'curr')
            #df.loc[curr, 'hcu']=freq[pwds.index(items[1])]+df.loc[curr, 'hcu']
            #print(df.loc[curr-1])
            # if items[1]!=df.loc[curr, 'Password']:
                # print(df.loc[curr-1])
                # df.loc[curr, 'hcu']=freq[pwds.index(items[1])]+df.loc[curr, 'hcu']
                # print(df.loc[curr-1])
    else:
        if len(items)>2:
            if (items[2]!=df.loc[curr-1, 'Password']):
                print(items[0])
                #check if current attempted password is in attempts list
                #if items[2] not in userattempts:
                    #print('userattempts', userattempts)
                    #userattempts.append(items[2])
                    #print('userattempts again ', userattempts)
                   # df.at[curr-1, 'attempts']=userattempts
                df.loc[curr-1,'Password']=items[2]
                # if hash.has_key(items[2]):
                    # index1=hash.get(items[2])
                # else:
                    # index1=0.5
                flag=0
                t0=time.clock()
                index1=None
                while (time.clock()-t0)<10:
                    try:
                        index1=1/freq[pwds.index(items[2])]
                    # #df.loc[curr-1, 'hcu']=freq[pwds.index(items[2])]+df.loc[curr-1, 'hcu']
                    except ValueError:
                    # #df.loc[curr-1, 'hcu']=0.5+df.loc[curr-1, 'hcu']
                        index1=1/(2*N)
                    #print('before here', df.loc[curr-1,'hcu'])
                    if index1!=None:
                        break
                if index1==None:
                    index1=1/(2*N)
                df.loc[curr-1, 'hcu']=index1+df.loc[curr-1, 'hcu']
                    #print(curr, items[2], index1)
                    #print(df)
                #print(df.loc[curr-1])
        if len(items)==1:
            df.loc[curr-1, 'compr']=1
df.to_csv('system2attack.csv')
print('done')