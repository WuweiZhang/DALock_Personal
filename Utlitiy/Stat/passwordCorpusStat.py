inputFiles = ['Yahoo-all.txt','LinkedIn.txt','rockyou-withcount.txt']
for inputFile in inputFiles:
    numberOfUniquePasswords = 0
    numberOfAccounts = 0
    with open (inputFile,encoding='utf8') as f:
        isYahooStyle = True
        if inputFile == 'rockyou-withcount.txt':
            isYahooStyle = False
        lines = f.readlines()
        for line in lines:
            passwordTuple = line.split()
            if isYahooStyle:
                numberOfAccounts += int(passwordTuple[0]) * int(passwordTuple[1])
                numberOfUniquePasswords  += int(passwordTuple[1])
            else:
                numberOfAccounts += int(passwordTuple[0]) 
                numberOfUniquePasswords  += 1
    print(inputFile ," unique passwords: " ,(numberOfUniquePasswords) , " number of accounts: " ,(numberOfAccounts))
