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
rockyou = list(zip(freq, pwds))
print ('len of rockyou ',len(rockyou))
expandedrockyou=[]
rnd = random.random()
scale = 0
for i in range (0, len(freq)):
    scale = scale + freq[i]
print (rnd, ', NN number: ', scale)
frsum=0
testwords = []
k=1
#the while loop is written to select randomly from rockyou 5000 passwords (weighted random!)
while k<5001:
    rnd=random.random()
    scale1=round(scale*rnd)
    frsum=0
    #print (rnd, scale, scale1)
    l = 0
    while (frsum<scale1):
        frsum=frsum+freq[l]
        l=l+1
   # print (l)
    word = pwds[l]
    #print (rnd, scale1, word)
    testwords.append(word)
    k = k+1
        #print (testwords)
	
#print (testwords)
print (len(testwords))
#print (word)

#print(typopass)

