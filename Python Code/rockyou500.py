import json
import random
pwds = []
freq = []
str = ' 290729 123456'
w1, w2 = str.split()
#print ('first ', w1, ' second ', w2)
file = open("rockyou-withcount.txt",  encoding = "latin-1")
for line in file:
    try:
        line = line.strip()
        #print(line)
        n, w = line.split(' ', 1)
        freq.append(int(n))
        pwds.append(w)
    except UnicodeEncodeError:
        pass
    except ValueError:
        pass
    except UnicodeDecodeError:
        pass
print (pwds[:25000])