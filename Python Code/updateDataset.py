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

atfile = open("dataset1.txt", "r")
outfile = open("d1.txt", "a")
pretime=0
previd=0
for line in atfile:
    #print(line)
    line=line.strip()
    #print(line)
    items=line.split(',')    
    if len(items)==1:
        outfile.write(previd+','+line+',0,0,0\n') 
    if len(items)==2:
        if previd!=items[0]:
            outfile.write(items[0]+',0,'+items[1]+',0,0\n')
        else:
            outfile.write(items[0]+','+prevtime+','+items[1]+',0,0\n')
    if len(items)==5:
        prevtime=items[1]
        previd=items[0]
        outfile.write(line+"\n")
    
print('done')