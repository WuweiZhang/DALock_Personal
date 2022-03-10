#prepare dataframe pwdRockyoyu, freqRockyou, TYPOSlist 2500 entries
import json
import random
import pandas as pd
import numpy as np
import sha3
from functools import reduce
import binascii
data = json.load(open("C:\\Users\\Alina\\Documents\\PWD\\TyposKNNscript\\typosranksfromrockyou2.json"))
#print(data)
#getting weighted random 2501 passwords from this data
r=random.random()
sum=0
for i in range(0, len(data)):
    sum=sum+data[i][2]
print(sum)
print(r)
print(len(data))
coef=0
count=0
pwdlist=[]
freqlist=[]
for i in range(0, 2501):
    r=random.random()
    #print(r)
    count=0
    coef=0
    while coef<sum*r:
        coef=coef+data[count][2]
        count=count+1
    pwdlist.append(data[count][1])
    freqlist.append(data[count][2])
#print(pwdlist)
#this is a dataframe that stores for 500 users 5 passwords from rockyou and typos intersection
#row has a correct pwd, its frequency from rockyou, and list of typos from typosdataset
df=pd.DataFrame(np.nan, index = range(0,2501), columns=['pwd', 'freq','list'])
df=df.astype(object)
for row in df.index:
    df.at[row,'list']=[]
typos = json.load(open("C:\\Users\\Alina\\Documents\\PWD\\TyposKNNscript\\mturk15-general.json"))
#print(typos)
pwdset=set(pwdlist)
#for i in range(0, len(pwdlist)):
df["pwd"]=pwdlist
df["freq"]=freqlist
#print(df)
klist=[]
for i in range(0,len(typos)):
    if typos[i]["rpw"] in pwdset:
        df.at[pwdlist.index(typos[i]["rpw"]),'list'].append(typos[i]["tpw"])
        #klist=df.at[pwdlist.index(typos[i]["rpw"]),'list']
        #df.loc[df['pwd']==typos[i]["rpw"], 'list']=df.loc[df['pwd']==typos[i]["rpw"], 'list']+typos[i]["tpw"].split(',')
        #print(klist, typos[i]["rpw"], typos[i]["tpw"])
    #print('before setting klist ', df.loc[df['pwd']==typos[i]["rpw"], 'list'])
    #df.loc[df['pwd']==typos[i]["rpw"], 'list']=list(klist)
    #print('after setting klist:',df.loc[df['pwd']==typos[i]["rpw"], 'list'])
df.list=df.list.apply(lambda y: np.nan if len(y)==0 else y)
df=df.groupby('pwd').ffill()
print(df)
#df.list=df.list.where(df.pwd==)
df.to_csv("alternatetyposmodel1.csv")       
