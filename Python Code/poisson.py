import random
import csv
lst = []
t=0
lambdas = [1,1/7, 1/31, 1/62, 1/180]
i=0
j=0
l1=[]
with open('arrivals.csv','w') as csvfile:
    writer = csv.writer(csvfile, delimiter=',')
    for j in range(0,500):
        l = random.choice(lambdas)
        i=0
        l1=[]
        t=0
        while t<366*5:
            i=i+1
            t+= int(random.expovariate(l))
            l1.append((i,t))
        writer.writerow((1/l,l1))
