import json
import random
import pandas as pd
import numpy as np
pwds = []
freq = []
pw1=[]
fr1=[]
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
        pw1.append(w)
        #print(n, w)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
#hash = {k:v for k, v in zip(pwds, freq)}
#print (rockyou)
attacked=[]
file2=open("attackeddataset500new.txt")
for line in file2:
    line=line.strip()
    #print(line)
    items=line.split(',')
    if len(items)==2:
        attacked.append(items[1])
        #print(items[1])
    if len(items)==5:
        attacked.append(items[2])
        #print(items[2])
intersect=[]
attacked1=set(attacked)
intersect=set(pwds).intersection(attacked1)
print(len(pwds), len(freq))
print(len(attacked1), len(intersect))
c=0
pw2=[]
for item in pw1:
    if item in intersect:
        pw2.append(item)
        fr1.append(freq[c])
    c=c+1
final = zip(pw2, fr1)
file3=open('rockyouintersection.txt','w')
for item in final:
    file3.write("%s,%d\n" % item)
    