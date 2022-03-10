using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Zxcvbn;
//using System.BitConverter;

namespace ConsoleApp5
{
    class loginAttempt
    {
        /// <summary>
        /// Index of user in array
        /// </summary>
        public int userId;

        /// <summary>
        /// time (minutes ellapsed since beginning of experiment)
        /// </summary>
        public int loginTime;

        /// <summary>
        /// string passwordGuess
        /// </summary>
        public string passwordGuess;
        /// <summary>
        /// boolean is this the attacker
        /// </summary>
        public bool IsAttack;

        /// <summary>
        /// Initialized login attempt
        /// </summary>
        /// <param name="user">Index of user in array</param>
        /// <param name="time">time (minutes ellapsed since beginning of experiment)</param>
        /// <param name="guess">passwordGuess</param>
        /// <param name="attack">is this the attacker</param>
        public loginAttempt(int user, int time, string guess, bool attack)
        {
            userId = user;
            loginTime = time;
            passwordGuess = guess;
            IsAttack = attack;
        }

    }

    class Program
    {
        public static RNGCryptoServiceProvider cryptoRand = new RNGCryptoServiceProvider();
        /* Login pattern in terms of hours */
        static float[] loginPatterns = { 12, 24, 24 * 3, 24 * 7, 24 * 14, 24 * 30 };
        static int CHANCEOFGENERATEUSER = 8250; // RockYou: 1250, LinkedIn 8500. Yahoo 2500

        static Random rnd;

        /// <summary>
        /// Generates an array of loginAttempts sorted by login time for numUsers under attack
        /// </summary>
        /// <param name="numUsers">Number of Users</param>
        /// <returns>array of login attempts sorted by login times</returns>
        public Tuple<loginAttempt[], User[]> GenerateLoginAttemptsWithAttacker(int numUsers)
        {
            //double a = GeneratePoisson(rate);
            return null;
        }

        static void GenerateUserFile(String outputUserFileLocation, String inputFileLocation, Boolean isYahooStyle, Random random)
        {
            String line;
            int totalNumberPasswordsInFile = 0;
            if (File.Exists(outputUserFileLocation))
            {
                File.Delete(outputUserFileLocation);
            }



            StreamReader rockYouReader = new StreamReader("LinkedIn-withcount.txt");
            Queue<String> rockYouPwds = new Queue<String>();
            if (isYahooStyle)
            {
                string rockYouLine = rockYouReader.ReadLine();
                while (rockYouLine != null)
                {
                    //Console.WriteLine(rockYouLine);
                    Tuple<int, String> rockYouPasswordPopularityTupleArray = HelperFunctions.ParseRockYouLine(rockYouLine);
                    rockYouPwds.Enqueue(rockYouPasswordPopularityTupleArray.Item2);
                    rockYouLine = rockYouReader.ReadLine();
                }
            }

            StreamReader sr = new StreamReader(inputFileLocation);

            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            int maxFrequency = 0;
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string>[] passwordPopularityTupleArray;
                // passwordPopularityTuple: (number of users, password)
                if (isYahooStyle)
                {
                    passwordPopularityTupleArray = HelperFunctions.ParseYahooLine(line);
                }
                else
                {
                    passwordPopularityTupleArray = new Tuple<int, string>[1];
                    passwordPopularityTupleArray[0] = HelperFunctions.ParseRockYouLine(line);
                }
                foreach (Tuple<int, string> passwordPopularityTuple in passwordPopularityTupleArray)
                {

                    totalNumberPasswordsInFile += passwordPopularityTuple.Item1;
                    if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;
                    }

                    int frequency = passwordPopularityTuple.Item1;
                    string pwd = passwordPopularityTuple.Item2;
                    if (isYahooStyle)
                    {
                        if (rockYouPwds.Count > 0)
                        {
                            String oripwd = String.Copy(pwd);
                            pwd = rockYouPwds.Dequeue();
  
                        }
                    }
                    passwordDistributionList.Add(new Tuple<int, string>(frequency, pwd));
                    maxFrequency += frequency;
                }
            }
            int OverallTime = 24 * 30 * 6;//24 hours in a day * 30 day in a month * 6 month in a year
            int MAX_AMOUNT_USER = 100000;

            StreamWriter sw = File.CreateText(outputUserFileLocation);
            while (MAX_AMOUNT_USER > 0)
            {
                MAX_AMOUNT_USER -= 1;
                int firstPasswordIdx = random.Next(maxFrequency);
                int rndpwdidx = 0;
                for (rndpwdidx = 0; rndpwdidx < passwordDistributionList.Count && firstPasswordIdx > passwordDistributionList[rndpwdidx].Item1; rndpwdidx++)
                    firstPasswordIdx -= passwordDistributionList[rndpwdidx].Item1;
                String originalPassword = String.Copy(passwordDistributionList[rndpwdidx].Item2);
                String passwordFromAnotherService = String.Copy(originalPassword);
                while (String.Equals(passwordFromAnotherService, originalPassword))
                {
                    int remainingCount = random.Next(maxFrequency);
                    for (rndpwdidx = 0; rndpwdidx < passwordDistributionList.Count && remainingCount > passwordDistributionList[rndpwdidx].Item1; rndpwdidx++)
                        remainingCount -= passwordDistributionList[rndpwdidx].Item1;
                    passwordFromAnotherService = String.Copy(passwordDistributionList[rndpwdidx].Item2);
                }

                int loginPattern = random.Next(0, loginPatterns.Length);
                float loginRate = loginPatterns[loginPattern];

                int userTimes = 0;
                double loginTime = 0;



                sw.WriteLine(MAX_AMOUNT_USER.ToString());
                sw.WriteLine(originalPassword);
                sw.WriteLine(passwordFromAnotherService);

                /* 2.2.2 user activities */

                while (userTimes < OverallTime)
                {
                    String userTypedPassword = HelperFunctions.TypePassword(originalPassword, passwordFromAnotherService);

                    /* 2.2.1 Generate single login time for users */

                    loginTime = GeneratePoisson(1 / loginRate);
                    userTimes += Convert.ToInt32(Math.Round(loginTime));
                    int userTimeInDay = userTimes / 24;

                    /* 2.2.3 If User make a mistake, type password again and update budget.
                     * If Lockout happens, increment locked variable on date: userTimeInDay*/
                    int maxLock = 11;
                    do
                    {
                        userTypedPassword = HelperFunctions.TypePassword(originalPassword, passwordFromAnotherService);
                        sw.WriteLine(userTimes.ToString());
                        sw.WriteLine(userTypedPassword);
                        maxLock -= 1;
                    } while (!userTypedPassword.Equals(originalPassword) && maxLock >= 0);
                }
                sw.WriteLine("----------------------------------\n");
            }
        }


        static void zxcvbnOverRockYou() {
            StreamReader rockYouReader = new StreamReader("LinkedIn-withcount.txt");
            string OUTPUT_FILE_PATH = @"c:\DALock\OptimalGuessing\LinkedNormalCount.txt";
            var zx = new Zxcvbn.Zxcvbn();
            string rockYouLine = rockYouReader.ReadLine();
           
            using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
            {

                while (rockYouLine != null)
                {
                    //Console.WriteLine(rockYouLine);
                    Tuple<int, String> rockYouPasswordPopularityTupleArray = HelperFunctions.ParseRockYouLine(rockYouLine);
                    var result = zx.EvaluatePassword(rockYouPasswordPopularityTupleArray.Item2);
                    //double estimatedCostInPsi = rockYouPasswordPopularityTupleArray.Item1 / 32603388.0;
                    //sw.WriteLine(estimatedCostInPsi);
                    sw.WriteLine(1.0/(result.Entropy+1.0));
                    rockYouLine = rockYouReader.ReadLine();
                }
            }
        }


        static void Main()
        {

            //Console.WriteLine(result.Entropy);
            //zxcvbnOverRockYou();
            uint COUNTSKETCH_WIDTH = 100000;
            uint COUNTSKETCH_DEPTH = 5;
            int[] tries = { 3, 10 };
            double[] privacyBudgetList = { double.PositiveInfinity, 0.1 };
            double[] psiList = { double.PositiveInfinity, Math.Pow(2, -6), Math.Pow(2, -12) };
            string TESTFILE = "Yahoo-all.txt"; //"LinkedIn-withcount.txt"; //LinkedIn.txt  //Yahoo-all.txt //LinkedIn-withcount.txt
            string USERFILE = "YU.txt"; // 
            string[] PGSFILELIST = { "Markov","PCFG" };
            //string PGSFILE = "Markov.txt";
            bool ISYAHOOSTYLE = true;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            rnd = new Random(seed);
            //
            double[] samplingRates = { 0.01, 0.05, 0.1, 1.0 };
            
            Console.WriteLine("Wait till SketchCount is populated...");

            foreach (double randomSample in samplingRates) {
                SketchCount SC = new SketchCount(COUNTSKETCH_WIDTH, COUNTSKETCH_DEPTH, 0);
                Dictionary<String,int> passwordMapping = SC.FillSketchCount(TESTFILE, ISYAHOOSTYLE, randomSample,rnd);
                //GenerateUserFile(USERFILE, TESTFILE, ISYAHOOSTYLE, rnd);
                string suffix = "all";
                if (randomSample < 1)
                    suffix = (Convert.ToInt32(randomSample * 100.0)).ToString() + "%";
                Console.WriteLine(suffix);
                ForeSeerOptimalOnCaptchaNotKnapsackExperimentBasedOnUserFile(tries, privacyBudgetList, 50, ISYAHOOSTYLE, TESTFILE, USERFILE, SC,suffix);
                UserLongTermStudyExperimentBasedOnUserFile(tries, privacyBudgetList, ISYAHOOSTYLE, TESTFILE, SC, passwordMapping,USERFILE,suffix);
            }
            //ZXCVBNUserLongTermStudyExperimentBasedOnUserFile(tries, privacyBudgetList, ISYAHOOSTYLE, TESTFILE,USERFILE);
            //ZXCVBNForeSeerOptimalOnCaptchaExperimentBasedOnUserFile(tries, privacyBudgetList, 50, ISYAHOOSTYLE, TESTFILE, USERFILE);
            /*
            foreach (string pgsfile in PGSFILELIST) {
                Console.WriteLine(pgsfile);
                PGSUserLongTermStudyExperimentBasedOnUserFile(tries, privacyBudgetList, ISYAHOOSTYLE, TESTFILE, USERFILE, pgsfile);
                PGSForeSeerOptimalOnCaptchaExperimentBasedOnUserFile(tries, privacyBudgetList, 50, ISYAHOOSTYLE, TESTFILE, USERFILE, pgsfile);
                Console.WriteLine("---- Experiment Done----");
            }*/
            Console.WriteLine("---- All Done----");
            while (true) System.Threading.Thread.Sleep(500000);

            //Console.WriteLine(result);
            //lockOutExp1(1000);

        }

        static double GeneratePoisson(float rate)
        {

            float res = ((float)rnd.Next(100) / 101.0f);

            var a = -Math.Log(1.0f - res) / rate;

            return a;
        }


        static void ForeSeerOptimalOnCaptchaNotKnapsackExperimentBasedOnUserFile(int[] maxKBudget, double[] privacyBudgetList, double rewardPerAccount, bool isYahooStyle, string passwordFileName, string userFileName, SketchCount SC,string suffix)
        {


            /* Hash Table for Key: Password Value:<Cost, Benefit> Dicitonary
             * The Cost is defined as the Psi Increment in the SC
             * The Benefit/gain is defined as real password popularity.
             */
            Dictionary<string, Tuple<double, double>> passwordToCostAndBenefit = new Dictionary<string, Tuple<double, double>>();


            /*
             * Define number of passwords interested. 18000 ~= 365 * 50. 
             * Which means adversaries can try 100 passwords per day for 180 day
             * period
            */
            double MAX_NUMBER_OF_PASSWORDS_INTERESTED = 50000;  //2*20*180 = 7200
            //double COSTPERGUESS = (0.011/3600.0)/2.6e9; // 2.6e9 guess per second, AWS server rate: 0.011 per hour


            // Step 1: Load SC and process
            /* No captcha: infinity
             * sloppy typer: 1 mistake per day on average
             * enough to memorize: 50 mistakes
              * */
            double[] captchaList = { double.PositiveInfinity };
            List<double> psiList = new List<double>();
            if (suffix.Equals("all"))
                psiList.Add(double.PositiveInfinity);
            psiList.Add(Math.Pow(2, -7));

            //psiList.Add(Math.Pow(2, -9.375));
            //psiList.Add(Math.Pow(2, -9.5));
            //for (int i = 9; i <= 12; i++)
            //{
            //    psiList.Add(Math.Pow(2, -1 * i));
            //
            //}
            //psiList.Add(Math.Pow(2, -9.375));
            //psiList.Add(Math.Pow(2, -9.5));
            //psiList.Add(double.PositiveInfinity);





            /* Variables For Experiment */
            String line;
            int maxFrequency = 0;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random random = new Random(seed);
            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            List<User> users = new List<User>();
            List<Tuple<int, string>> advGuessUnsorted = new List<Tuple<int, string>>();
            double totalNumberOfUsers = 100000.0;
            long totalNumberPasswordsInFile = 0;
            StreamReader rockYouReader = new StreamReader("LinkedIn-withcount.txt");
            Queue<String> rockYouPwds = new Queue<String>();
            if (isYahooStyle)
            {
                string rockYouLine = rockYouReader.ReadLine();
                while (rockYouLine != null)
                {
                    //Console.WriteLine(rockYouLine);
                    Tuple<int, String> rockYouPasswordPopularityTupleArray = HelperFunctions.ParseRockYouLine(rockYouLine);
                    rockYouPwds.Enqueue(rockYouPasswordPopularityTupleArray.Item2);
                    rockYouLine = rockYouReader.ReadLine();
                }
            }

            /* Read the password file */
            StreamReader sr = new StreamReader(passwordFileName);


            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string>[] passwordPopularityTupleArray;
                // passwordPopularityTuple: (number of users, password)
                if (isYahooStyle)
                {
                    passwordPopularityTupleArray = HelperFunctions.ParseYahooLine(line);
                }
                else
                {
                    passwordPopularityTupleArray = new Tuple<int, string>[1];
                    passwordPopularityTupleArray[0] = HelperFunctions.ParseRockYouLine(line);
                }
                foreach (Tuple<int, string> passwordPopularityTuple in passwordPopularityTupleArray)
                {
                    totalNumberPasswordsInFile += passwordPopularityTuple.Item1;
                    if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;
                    }
                    if (!isYahooStyle)
                    {
                        passwordDistributionList.Add(passwordPopularityTuple);
                        maxFrequency += passwordPopularityTuple.Item1;
                        if (advGuessUnsorted.Count < MAX_NUMBER_OF_PASSWORDS_INTERESTED)
                        {
                            advGuessUnsorted.Add(passwordPopularityTuple);
                        }
                    }
                    else
                    {
                        string pwd = String.Copy(passwordPopularityTuple.Item2);
                        if (rockYouPwds.Count > 0)
                        {
                            pwd = String.Copy(rockYouPwds.Dequeue());
                            //Console.WriteLine("Convert: [" + passwordPopularityTuple.Item2 +"] To" + pwd);
                        }
                        passwordDistributionList.Add(new Tuple<int, string>(passwordPopularityTuple.Item1, pwd));
                        if (advGuessUnsorted.Count < MAX_NUMBER_OF_PASSWORDS_INTERESTED)
                        {
                            advGuessUnsorted.Add(new Tuple<int, string>(passwordPopularityTuple.Item1, pwd));
                        }
                    }
                }
            }

            /* Create a 2nd password for each user */
            // Step 2. Generate User Arrival Info
            // Define each login as Tuple:
            //      <time,User,typedpassword>

            Console.WriteLine("In Foreseer Experiment: Users: " + Convert.ToString(totalNumberOfUsers));

            /* Experiment Part. For each psi k budget */


            /* Time Complexity #ofPB * #of psi * #ofK
             * Each iteration takes N users
             * 
             */

            foreach (double privacyBudget in privacyBudgetList)
            {
                SC.ChangePrivacyLevel(privacyBudget);
                string OUTPUT_FILE_PATH = @"c:\DALock\OptimalGuessing\Yahoo\Yahoo_CS" + suffix + privacyBudget.ToString();
                if (File.Exists(OUTPUT_FILE_PATH))
                {
                    File.Delete(OUTPUT_FILE_PATH);
                }
                // Create a file to write to.

                long cumulativeAttemptedPasswordPopulation = 0;
                //List<Tuple<string, double, double, double>> advGuess = new List<Tuple<string, double, double, double>>();
                List<Tuple<string, double, double, double>> advGuessForTraditional = new List<Tuple<string, double, double, double>>();
                int attempedCaptchaGuesses = 0;
                //int optimalAttemptForCaptCha = 0;
                //double greatValueForCaptCha = double.NegativeInfinity;

                foreach (Tuple<int, string> passwordTuple in advGuessUnsorted)
                {
                    string password = passwordTuple.Item2;
                    double estimatedCostInPsi = SC.EstimatePopularityByMedian(password);
                    double estimateCost = Math.Abs(SC.EstimateMedian(password));
                    attempedCaptchaGuesses += 1;
                    cumulativeAttemptedPasswordPopulation += passwordTuple.Item1;
                    double cumulativeSuccessProbability = cumulativeAttemptedPasswordPopulation / (1.0 * totalNumberPasswordsInFile);

                    //double cost = cumulativeSuccessProbability * (1 + attempedCaptchaGuesses) * attempedCaptchaGuesses / 2.0 + (1.0 - cumulativeSuccessProbability) * attempedCaptchaGuesses;
                    //cost *= COSTPERCAPTCHA;
                    //double reward = (cumulativeSuccessProbability) * rewardPerAccount;
                    //double utility = reward - cost;
                    //if (utility > greatValueForCaptCha)
                    //{
                    //    greatValueForCaptCha = utility;
                    //    optimalAttemptForCaptCha = attempedCaptchaGuesses;
                    //}

                    if (estimateCost < 0)
                        estimateCost = double.PositiveInfinity;
                    //advGuess.Add(new Tuple<string, double, double, double>(password, estimatedCostInPsi, passwordTuple.Item1 / estimateCost, passwordTuple.Item1));
                    advGuessForTraditional.Add(new Tuple<string, double, double, double>(password, estimatedCostInPsi, passwordTuple.Item1, passwordTuple.Item1));
                    //Console.WriteLine(password+"["+estimatedCostInPsi+"]");
                }
                //advGuess.Sort((x, y) => y.Item3.CompareTo(x.Item3));

                int OverallTime = 24 * 30 * 6;//24 hours in a day * 30 day in a month * 6 month in a year

                using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
                {
                    sw.WriteLine("Experiment For Foreseer");
                    //sw.WriteLine("Optimal Captcha: " + optimalAttemptForCaptCha);
                }
                foreach (double psiBudget in psiList)
                {
                    Console.WriteLine("psiBudget: " + Convert.ToString(psiBudget));
                    foreach (int kBudget in maxKBudget)
                    {
                        foreach (double captchaBudget in captchaList)
                        {

                            Console.WriteLine("-->kBudget: " + Convert.ToString(kBudget));
                            String bigStepPassword = passwordDistributionList[0].Item2;
                            if (psiBudget.Equals(double.PositiveInfinity))
                                bigStepPassword = "";
                            /* Account static dictionary: Key is date and value is the corresponding number of accounts*/
                            Dictionary<int, double> cracked = new Dictionary<int, double>();
                            Dictionary<int, double> locked = new Dictionary<int, double>();
                            Dictionary<int, double> guessed = new Dictionary<int, double>();
                            Dictionary<int, double> captchaed = new Dictionary<int, double>();
                            int totGuessPUser = 0;
                            StreamReader ufr = new StreamReader(userFileName);
                            while (true)
                            {
                                /* Retrieve users's acitvity from file */
                                String userID = ufr.ReadLine();
                                if (userID == null) break;
                                String password = ufr.ReadLine();
                                String altPassword = ufr.ReadLine();
                                //if (password.Equals("123456")){
                                //    Console.WriteLine("uid:" + userID);
                                //    Console.WriteLine("pwd:" + password);
                                //
                                //}
                                //Console.WriteLine("altpwd:" + altPassword);
                                //if (userID.Equals("99999")) Console.WriteLine("pwd\t" + password + "alt\t" + altPassword);
                                User user = new User();
                                user.Register(password, 1.0, 0, false);
                                user.setPasswordFromAnotherService(altPassword);
                                user.setThreshold(psiBudget, kBudget, captchaBudget);
                                List<String> loginActivity = new List<String>();
                                int lineCount = 0;
                                int loginTime = -1;
                                while (true)
                                {
                                    String activityLine = ufr.ReadLine();
                                    if (activityLine == null || activityLine.Equals("----------------------------------"))
                                    {
                                        //Console.WriteLine("next user");
                                        if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                        if (Convert.ToInt32(userID) > 0) ufr.ReadLine();
                                        break;
                                    }
                                    if (lineCount % 2 == 0)
                                    {
                                        int thisLoginTime = Convert.ToInt32(activityLine);
                                        if (thisLoginTime != loginTime)
                                        {
                                            if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                            loginActivity = new List<String>();
                                        }
                                        loginTime = thisLoginTime;

                                    }
                                    else
                                    {
                                        loginActivity.Add(activityLine);
                                    }
                                    lineCount += 1;
                                }




                                List<Tuple<string, double, double, double>> untriedPassword;

                                untriedPassword = new List<Tuple<string, double, double, double>>(advGuessForTraditional);
                                //Console.WriteLine("use traditional");


                                double remainPsiBudget = psiBudget;
                                int remainKBudget = kBudget;
                                bool thisUserDone = false;
                                bool thisUserLocked = false;

                                foreach (Tuple<int, List<String>> userLoginActivity in user.timeLineList)
                                {
                                    int userTimes = userLoginActivity.Item1;
                                    int userTimeInDay = userTimes / 24;



                                    /* 2.2.2 Calculate the budget next time */


                                    Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();
                                    remainKBudget = budget.Item1;
                                    remainPsiBudget = budget.Item2;

                                    double remainingPsiBudgetBeforeFirstFailure = budget.Item2;
                                    foreach (String passwordTypedThisTime in userLoginActivity.Item2)
                                    {
                                        //Console.WriteLine("Type:"+passwordTypedThisTime);
                                        if (!passwordTypedThisTime.Equals(user.pwd) && remainPsiBudget > 0 && remainKBudget > 0)
                                        {
                                            remainPsiBudget -= SC.EstimatePopularityByMedian(passwordTypedThisTime);
                                        }
                                        remainKBudget -= 1;
                                    }
                                    /* 2.3.1 Calculate attack budget for this step */

                                    /* run out of dictionary stop attack */
                                    if (untriedPassword.Count <= 0) break;
                                    /* 2.3.2 Real Attack Starts */
                                    /* 2.3.2-1 Real Attack Starts: User locked their account 
                                     *  - Case 1: User Gonna lockout his/her account: thisUserLocked = True
                                     *  - Case 2: remaining psi budget is not high enough for any password attempt
                                        - Case 3: Captcha Give up
                                       */
                                    if (remainKBudget <= 0 || remainPsiBudget <= 0)
                                    {

                                        remainKBudget = kBudget - 1; /*  not leting user to try, save 1 for the biggest gain in terms of population*/
                                        remainPsiBudget = remainingPsiBudgetBeforeFirstFailure;

                                        /* Get One giant step password: this password is almost certain to exceed the budget */


                                        if (thisUserDone) break;
                                        /* Optimize the rest password choices by knapsacks*/
                                    
                                        if ( remainKBudget > 0 && untriedPassword.Count > 0)
                                        {
                                            Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget, remainPsiBudget, untriedPassword, bigStepPassword);

                                            while (passwordIndices.Count > 0)
                                            {
                                                int passwordIndex = passwordIndices.Dequeue();
                                                string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                                double thisThreshold = untriedPassword[passwordIndex].Item2;

                                                Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, userTimes, SC);
                                                int loginSuccess = loginFeedback.Item1;
                                                Boolean requireCaptcha = loginFeedback.Item2;

                                                if (!guessed.ContainsKey(userTimeInDay))
                                                {
                                                    guessed.Add(userTimeInDay, 0);
                                                }
                                                guessed[userTimeInDay] += 1;
                                                //if (userID.Equals("99999")) Console.WriteLine("CASE 3.2.2-1 guessed @" + Convert.ToString(userTimes)+ "is" + Convert.ToString(guessed[userTimeInDay]) + "\t" + nextPasswordToGuess);


                                                if (loginSuccess == 1)
                                                {
                                                    thisUserDone = true;
                                                    if (!cracked.ContainsKey(userTimeInDay))
                                                    {
                                                        cracked.Add(userTimeInDay, 0);
                                                    }
                                                    cracked[userTimeInDay] += 1;
                                                }
                                                untriedPassword.Remove(untriedPassword[passwordIndex]);
                                                if (thisUserDone) break;

                                            }
                                        }

                                        if (!thisUserDone)
                                        {

                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(bigStepPassword, userTimes, SC);
                                            int loginSuccess = loginFeedback.Item1;
                                            if (!guessed.ContainsKey(userTimeInDay))
                                            {
                                                guessed.Add(userTimeInDay, 0);
                                            }
                                            guessed[userTimeInDay] += 1;
                                            //if (userID.Equals("99999")) Console.WriteLine("CASE 3.2.2-1 guessed @" + Convert.ToString(userTimes) + "is" + Convert.ToString(guessed[userTimeInDay]) + "\t" + bigStepPassword + "\tDone");

                                            if (loginSuccess == 1)
                                            {
                                                if (!cracked.ContainsKey(userTimeInDay))
                                                {
                                                    cracked.Add(userTimeInDay, 0);
                                                }
                                                cracked[userTimeInDay] += 1;
                                            }
                                        }
                                        thisUserDone = true;
                                    }
                                    if (thisUserDone) break;


                                    /* 2.3.2-2 Real Attack Starts: User login successfully: Insert attack before  */
                                    if ( remainKBudget > 0 && untriedPassword.Count > 0)
                                    {
                                        //if (userID.Equals("99999")) Console.WriteLine("Remain budget " + Convert.ToString(remainKBudget) + "\t" + Convert.ToString(remainPsiBudget));
                                        Queue <int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget, remainPsiBudget, untriedPassword, bigStepPassword);

                                        while (passwordIndices.Count > 0)
                                        {
                                            if (passwordIndices.Count > kBudget)
                                            {
                                                Console.WriteLine("Error!!!");
                                            }
                                            int passwordIndex = passwordIndices.Dequeue();
                                            string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                            double thisThreshold = untriedPassword[passwordIndex].Item2;

                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, userTimes, SC);
                                            int loginSuccess = loginFeedback.Item1;
                                            Boolean requireCaptcha = loginFeedback.Item2;



                                            if (!guessed.ContainsKey(userTimeInDay))
                                            {
                                                guessed.Add(userTimeInDay, 0);
                                            }
                                            guessed[userTimeInDay] += 1;
                                            //if (userID.Equals("99999")) Console.WriteLine("CASE 3.2.2-2 guessed @"  +Convert.ToString(userTimes) + "is" + Convert.ToString(guessed[userTimeInDay]) + "  " + nextPasswordToGuess + "");



                                            if (loginSuccess == 1)
                                            {
                                                thisUserDone = true;
                                                if (!cracked.ContainsKey(userTimeInDay))
                                                {
                                                    cracked.Add(userTimeInDay, 0);
                                                }
                                                cracked[userTimeInDay] += 1;
                                            }
                                            untriedPassword.Remove(untriedPassword[passwordIndex]);
                                            if (thisUserDone) break;

                                        }
                                    }

                                    /* If cracked, stop attacking */
                                    if (thisUserDone) break;


                                    /* User's activities */
                                    foreach (String passwordTypedByUserThisTime in userLoginActivity.Item2)
                                    {
                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(passwordTypedByUserThisTime, userTimes, SC);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;
                                        if (loginSuccess == -1)
                                        {
                                            if (!thisUserLocked)
                                            {
                                                if (!locked.ContainsKey(userTimeInDay))
                                                {
                                                    locked.Add(userTimeInDay, 0);
                                                }
                                                locked[userTimeInDay] += 1;
                                                thisUserLocked = true;
                                            }
                                        }
                                        if (loginSuccess == 1) break;
                                    }

                                }

                                /*2.4 End of Day Attack. If user does not lock their account by the end of the day.
                                 * To simply the process, assume user gonna login once after the 180 day period later.*/
                                if (untriedPassword.Count > 0 && !thisUserDone)
                                {

                                    int overAllTimeInDay = OverallTime / 24;
                                    if (remainPsiBudget > 0)
                                    {
                                        Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();
                                        remainKBudget = budget.Item1 - 1; /* Save 1 for the biggest gain in terms of population*/


                                        remainPsiBudget = budget.Item2;
                                        if (remainKBudget > 0 && untriedPassword.Count > 0)
                                        {
                                            Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget, remainPsiBudget, untriedPassword, bigStepPassword);
                                            //Console.WriteLine("1Iteration----");
                                            while (passwordIndices.Count > 0)
                                            {

                                                int passwordIndex = passwordIndices.Dequeue();
                                                //Console.WriteLine(passwordIndex);
                                                string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                                double thisThreshold = untriedPassword[passwordIndex].Item2;

                                                Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, OverallTime, SC);
                                                int loginSuccess = loginFeedback.Item1;


                                                if (!guessed.ContainsKey(overAllTimeInDay))
                                                {
                                                    guessed.Add(overAllTimeInDay, 0);
                                                }
                                                guessed[overAllTimeInDay] += 1;
                                                //if (userID.Equals("99999")) Console.WriteLine("CASE 3.2.4 guessed is" + Convert.ToString(guessed[overAllTimeInDay]));


                                                if (loginSuccess == 1)
                                                {
                                                    thisUserDone = true;
                                                    if (!cracked.ContainsKey(overAllTimeInDay))
                                                    {
                                                        cracked.Add(overAllTimeInDay, 0);
                                                    }
                                                    cracked[overAllTimeInDay] += 1;
                                                }
                                                untriedPassword.Remove(untriedPassword[passwordIndex]);
                                                if (thisUserDone) break;

                                            }
                                        }
                                        if (!thisUserDone)
                                        {
                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(bigStepPassword, OverallTime, SC);
                                            int loginSuccess = loginFeedback.Item1;


                                            if (!guessed.ContainsKey(overAllTimeInDay))
                                            {
                                                guessed.Add(overAllTimeInDay, 0);
                                            }

                                            guessed[overAllTimeInDay] += 1;
                                            //if (userID.Equals("99999")) Console.WriteLine("CASE 3.2.4 guessed is" + Convert.ToString(guessed[overAllTimeInDay]));
                                            if (loginSuccess == 1)
                                            {
                                                if (!cracked.ContainsKey(overAllTimeInDay))
                                                {
                                                    cracked.Add(overAllTimeInDay, 0);
                                                }
                                                cracked[overAllTimeInDay] += 1;
                                            }
                                            thisUserDone = true;
                                        }

                                        /* Not done, this account locked */
                                        if (!thisUserDone && !thisUserLocked)
                                        {
                                            if (!locked.ContainsKey(overAllTimeInDay))
                                            {
                                                locked.Add(overAllTimeInDay, 0);
                                            }
                                            locked[overAllTimeInDay] += 1;
                                        }

                                        thisUserDone = true;
                                    }
                                }



                            }
                            double cuLocked = 0.0;
                            double cuCracked = 0.0;
                            double cuGuessed = 0.0;
                            double cuCaptcha = 0.0;



                            // This text is added only once to the file.

                            using (StreamWriter sw = File.AppendText(OUTPUT_FILE_PATH))
                            {


                                for (int i = 0; i <= 30 * 6 + 1; i++)
                                {
                                    if (cracked.ContainsKey(i)) cuCracked += cracked[i];
                                    if (locked.ContainsKey(i)) cuLocked += locked[i];
                                    if (guessed.ContainsKey(i)) cuGuessed += guessed[i];
                                    if (captchaed.ContainsKey(i)) cuCaptcha += captchaed[i];
                                    sw.WriteLine(Convert.ToString(kBudget) + "\t" + Convert.ToString(psiBudget) + "\t" + Convert.ToString(captchaBudget) + "\t" + Convert.ToString(i) + "\t" + Convert.ToString(cuCracked / totalNumberOfUsers) + "\t" + Convert.ToString(cuLocked / totalNumberOfUsers) + "\t" + Convert.ToString(cuGuessed / totalNumberOfUsers) + "\t" + Convert.ToString(cuCaptcha / totalNumberOfUsers) + "\t");
                                }
                            }
                        }
                    }
                }
            }
        }



        static void ZXCVBNForeSeerOptimalOnCaptchaExperimentBasedOnUserFile(int[] maxKBudget, double[] privacyBudgetList, double rewardPerAccount, bool isYahooStyle, string passwordFileName, string userFileName)
        {

            var zx = new Zxcvbn.Zxcvbn();
            /* Hash Table for Key: Password Value:<Cost, Benefit> Dicitonary
             * The Cost is defined as the Psi Increment in the SC
             * The Benefit/gain is defined as real password popularity.
             */
            Dictionary<string, Tuple<double, double>> passwordToCostAndBenefit = new Dictionary<string, Tuple<double, double>>();


            /*
             * Define number of passwords interested. 18000 ~= 365 * 50. 
             * Which means adversaries can try 100 passwords per day for 180 day
             * period
            */
            double MAX_NUMBER_OF_PASSWORDS_INTERESTED = 20000;  //2*20*180 = 7200
            //double COSTPERGUESS = (0.011/3600.0)/2.6e9; // 2.6e9 guess per second, AWS server rate: 0.011 per hour



            // Step 1: Load SC and process
            /* No captcha: infinity
             * sloppy typer: 1 mistake per day on average
             * enough to memorize: 50 mistakes
              * */
            double[] captchaList = { double.PositiveInfinity };
            List<double> psiList = new List<double>();
            //psiList.Add(double.PositiveInfinity);
            psiList.Add(Math.Pow(2, -9.375));
            //psiList.Add(Math.Pow(2, -9.4375));
            psiList.Add(Math.Pow(2, -9.5));
            //psiList.Add(Math.Pow(2, -9.625));
            //psiList.Add(Math.Pow(2, -1 * 9.75));
            //psiList.Add(double.PositiveInfinity);
            //for (int i = 6; i <= 11; i++)
            //{
            //    psiList.Add(Math.Pow(2, -1 * i));
            //
            //}
            //psiList.Add(double.PositiveInfinity);


            /* Read the password file */
            StreamReader sr = new StreamReader(passwordFileName);




            /* Variables For Experiment */
            String line;
            int maxFrequency = 0;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random random = new Random(seed);
            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            List<User> users = new List<User>();
            List<Tuple<int, string>> advGuessUnsorted = new List<Tuple<int, string>>();
            double totalNumberOfUsers = 100000.0;
            long totalNumberPasswordsInFile = 0;
            /* Step 1: Create Users */
            double aggregatedSum = 0.0;
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string>[] passwordPopularityTupleArray;
                // passwordPopularityTuple: (number of users, password)
                if (isYahooStyle)
                {
                    passwordPopularityTupleArray = HelperFunctions.ParseYahooLine(line);
                }
                else
                {
                    passwordPopularityTupleArray = new Tuple<int, string>[1];
                    passwordPopularityTupleArray[0] = HelperFunctions.ParseRockYouLine(line);
                }
                foreach (Tuple<int, string> passwordPopularityTuple in passwordPopularityTupleArray)
                {
                    totalNumberPasswordsInFile += passwordPopularityTuple.Item1;
                    if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;

                    }
                    passwordDistributionList.Add(passwordPopularityTuple);
                    maxFrequency += passwordPopularityTuple.Item1;
                    if (advGuessUnsorted.Count < MAX_NUMBER_OF_PASSWORDS_INTERESTED)
                    {
                        advGuessUnsorted.Add(passwordPopularityTuple);
                        var result = zx.EvaluatePassword(passwordPopularityTuple.Item2);
                        double estimatedCostInPsi = 1.0 / (result.Entropy + 1.0);
                        //if (advGuessUnsorted.Count < 200){
                            aggregatedSum += estimatedCostInPsi;
                        //}
                    }
                }
            }

            /* Create a 2nd password for each user */
            // Step 2. Generate User Arrival Info

            Console.WriteLine("In Foreseer Experiment: Users: " + Convert.ToString(totalNumberOfUsers));
            //double aggregatedSum = 0.0;
            List<Tuple<string, double, double, double>> advGuessForTraditional = new List<Tuple<string, double, double, double>>();
            List<Tuple<string, double, double, double>> advGuess = new List<Tuple<string, double, double, double>>();




            string OUTPUT_FILE_PATH = @"c:\DALock\OptimalGuessing\Yahoo\Yahoo_ZXCVBN_Normal";
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                File.Delete(OUTPUT_FILE_PATH);
            }
            // Create a file to write to.


            int optimalAttemptForCaptCha = int.MaxValue;


            foreach (Tuple<int, string> passwordTuple in advGuessUnsorted)
            {
                string password = passwordTuple.Item2;
                var result = zx.EvaluatePassword(password);
                double estimatedCostInPsi = 1.0 / (result.Entropy + 1.0);
                estimatedCostInPsi /= aggregatedSum;
                //Console.WriteLine("Entropy: ["+password+"]:" + estimatedCostInPsi);
                advGuess.Add(new Tuple<string, double, double, double>(password, estimatedCostInPsi, passwordTuple.Item1 / estimatedCostInPsi, passwordTuple.Item1));
                advGuessForTraditional.Add(new Tuple<string, double, double, double>(password, estimatedCostInPsi, passwordTuple.Item1, passwordTuple.Item1));
                //Console.WriteLine(password+"["+estimatedCostInPsi+"]");
            }
            advGuess.Sort((x, y) => y.Item3.CompareTo(x.Item3));

            int OverallTime = 24 * 30 * 6;//24 hours in a day * 30 day in a month * 6 month in a year

            using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
            {
                sw.WriteLine("Experiment For Foreseer");
                sw.WriteLine("Optimal Captcha: " + optimalAttemptForCaptCha);
            }
            foreach (double psiBudget in psiList)
            {
                Console.WriteLine("psiBudget: " + Convert.ToString(psiBudget));
                foreach (int kBudget in maxKBudget)
                {
                    foreach (double captchaBudget in captchaList)
                    {

                        Console.WriteLine("-->kBudget: " + Convert.ToString(kBudget));
                        String bigStepPassword =String.Copy(passwordDistributionList[0].Item2);
                        if (psiBudget.Equals(double.PositiveInfinity))
                            bigStepPassword = "";
                        /* Account static dictionary: Key is date and value is the corresponding number of accounts*/
                        Dictionary<int, double> cracked = new Dictionary<int, double>();
                        Dictionary<int, double> locked = new Dictionary<int, double>();
                        Dictionary<int, double> guessed = new Dictionary<int, double>();
                        Dictionary<int, double> captchaed = new Dictionary<int, double>();

                        StreamReader ufr = new StreamReader(userFileName);
                        while (true)
                        {
                            /* Retrieve users's acitvity from file */
                            String userID = ufr.ReadLine();
                            if (userID == null) break;
                            String password = ufr.ReadLine();
                            String altPassword = ufr.ReadLine();

                            User user = new User();
                            user.Register(password, 1.0, 0, false);
                            user.setPasswordFromAnotherService(altPassword);
                            user.setThreshold(psiBudget, kBudget, captchaBudget);
                            List<String> loginActivity = new List<String>();
                            int lineCount = 0;
                            int loginTime = -1;
                            while (true)
                            {
                                String activityLine = ufr.ReadLine();
                                if (activityLine == null || activityLine.Equals("----------------------------------"))
                                {
                                    //Console.WriteLine("next user");
                                    if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                    if (Convert.ToInt32(userID) > 0) ufr.ReadLine();
                                    break;
                                }
                                if (lineCount % 2 == 0)
                                {
                                    int thisLoginTime = Convert.ToInt32(activityLine);
                                    if (thisLoginTime != loginTime)
                                    {
                                        if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                        loginActivity = new List<String>();
                                    }
                                    loginTime = thisLoginTime;

                                }
                                else
                                {
                                    loginActivity.Add(activityLine);
                                }
                                lineCount += 1;
                            }

                            double maxCaptchaWillingToSolve = optimalAttemptForCaptCha;



                            List<Tuple<string, double, double, double>> untriedPassword;

                            untriedPassword = new List<Tuple<string, double, double, double>>(advGuess);
                            //Console.WriteLine("use traditional");


                            double remainPsiBudget = psiBudget;
                            int remainKBudget = kBudget - 1;
                            bool thisUserDone = false;
                            bool thisUserLocked = false;

                            foreach (Tuple<int, List<String>> userLoginActivity in user.timeLineList)
                            {
                                int userTimes = userLoginActivity.Item1;
                                int userTimeInDay = userTimes / 24;



                                /* 2.2.2 Calculate the budget next time */


                                Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();
                                remainKBudget = budget.Item1;
                                remainPsiBudget = budget.Item2;

                                double remainingPsiBudgetBeforeFirstFailure = budget.Item2;
                                foreach (String passwordTypedThisTime in userLoginActivity.Item2)
                                {
                                    //Console.WriteLine("Type:"+passwordTypedThisTime);
                                    if (!passwordTypedThisTime.Equals(user.pwd) && remainPsiBudget > 0 && remainKBudget > 0)
                                    {
                                        remainKBudget -= 1;
                                        var tempresult = zx.EvaluatePassword(password);
                                        double temppctg = 1.0 / (tempresult.Entropy + 1.0);
                                        temppctg /= aggregatedSum;
                                        remainPsiBudget -= temppctg;
                                    }
                                }
                                /* 2.3.1 Calculate attack budget for this step */

                                /* run out of dictionary stop attack */
                                if (untriedPassword.Count <= 0) break;
                                /* 2.3.2 Real Attack Starts */
                                /* 2.3.2-1 Real Attack Starts: User locked their account 
                                 *  - Case 1: User Gonna lockout his/her account: thisUserLocked = True
                                 *  - Case 2: remaining psi budget is not high enough for any password attempt
                                    - Case 3: Captcha Give up
                                   */
                                if (remainKBudget <= 0 || remainPsiBudget <= 0)
                                {

                                    remainKBudget = kBudget - 1; /*  not leting user to try, save 1 for the biggest gain in terms of population*/
                                    remainPsiBudget = remainingPsiBudgetBeforeFirstFailure;

                                    /* Get One giant step password: this password is almost certain to exceed the budget */

                                    if (maxCaptchaWillingToSolve > 0)
                                    {
                                        if (user.maxKForCaptcha <= 0) maxCaptchaWillingToSolve -= 1;
                                    }
                                    else
                                    {
                                        thisUserDone = true;
                                    }
                                    if (thisUserDone) break;
                                    /* Optimize the rest password choices by knapsacks*/
                                    if (maxCaptchaWillingToSolve > 0 && remainKBudget > 0 && untriedPassword.Count > 0)
                                    {
                                        Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget, remainPsiBudget, untriedPassword, bigStepPassword);

                                        while (passwordIndices.Count > 0)
                                        {
                                            int passwordIndex = passwordIndices.Dequeue();
                                            string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                            //Console.WriteLine(nextPasswordToGuess);
                                            double thisThreshold = untriedPassword[passwordIndex].Item2;

                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, userTimes, zx,aggregatedSum);
                                            int loginSuccess = loginFeedback.Item1;
                                            Boolean requireCaptcha = loginFeedback.Item2;

                                            if (requireCaptcha)
                                            {
                                                maxCaptchaWillingToSolve -= 1;
                                                if (!captchaed.ContainsKey(userTimeInDay))
                                                {
                                                    captchaed.Add(userTimeInDay, 0);
                                                }
                                                captchaed[userTimeInDay] += 1;
                                            }

                                            if (!guessed.ContainsKey(userTimeInDay))
                                            {
                                                guessed.Add(userTimeInDay, 0);
                                            }
                                            guessed[userTimeInDay] += 1;



                                            if (loginSuccess == 1)
                                            {
                                                thisUserDone = true;
                                                if (!cracked.ContainsKey(userTimeInDay))
                                                {
                                                    cracked.Add(userTimeInDay, 0);
                                                }
                                                cracked[userTimeInDay] += 1;
                                            }
                                            untriedPassword.Remove(untriedPassword[passwordIndex]);
                                            if (thisUserDone) break;

                                        }
                                    }

                                    if (!thisUserDone)
                                    {

                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(bigStepPassword, userTimes, zx,aggregatedSum);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;
                                        if (requireCaptcha)
                                        {
                                            if (!captchaed.ContainsKey(userTimeInDay))
                                            {
                                                captchaed.Add(userTimeInDay, 0);
                                            }
                                            captchaed[userTimeInDay] += 1;
                                        }
                                        if (!guessed.ContainsKey(userTimeInDay))
                                        {
                                            guessed.Add(userTimeInDay, 0);
                                        }
                                        guessed[userTimeInDay] += 1;

                                        if (loginSuccess == 1)
                                        {
                                            if (!cracked.ContainsKey(userTimeInDay))
                                            {
                                                cracked.Add(userTimeInDay, 0);
                                            }
                                            cracked[userTimeInDay] += 1;
                                        }
                                    }
                                    thisUserDone = true;
                                }
                                if (thisUserDone) break;


                                /* 2.3.2-2 Real Attack Starts: User login successfully: Insert attack before  */
                                if (maxCaptchaWillingToSolve > 0 && remainKBudget > 0 && untriedPassword.Count > 0)
                                {
                                    Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget - 1, remainPsiBudget, untriedPassword, bigStepPassword);

                                    while (passwordIndices.Count > 0)
                                    {
                                        if (passwordIndices.Count > kBudget)
                                        {
                                            Console.WriteLine("Error!!!");
                                        }
                                        int passwordIndex = passwordIndices.Dequeue();
                                        string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                        double thisThreshold = untriedPassword[passwordIndex].Item2;

                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, userTimes, zx, aggregatedSum);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;

                                        if (requireCaptcha)
                                        {
                                            maxCaptchaWillingToSolve -= 1;
                                            if (!captchaed.ContainsKey(userTimeInDay))
                                            {
                                                captchaed.Add(userTimeInDay, 0);
                                            }
                                            captchaed[userTimeInDay] += 1;
                                        }

                                        if (!guessed.ContainsKey(userTimeInDay))
                                        {
                                            guessed.Add(userTimeInDay, 0);
                                        }
                                        guessed[userTimeInDay] += 1;



                                        if (loginSuccess == 1)
                                        {
                                            thisUserDone = true;
                                            if (!cracked.ContainsKey(userTimeInDay))
                                            {
                                                cracked.Add(userTimeInDay, 0);
                                            }
                                            cracked[userTimeInDay] += 1;
                                        }
                                        untriedPassword.Remove(untriedPassword[passwordIndex]);
                                        if (thisUserDone) break;

                                    }
                                }

                                /* If cracked, stop attacking */
                                if (thisUserDone) break;


                                /* User's activities */
                                foreach (String passwordTypedByUserThisTime in userLoginActivity.Item2)
                                {
                                    Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(passwordTypedByUserThisTime, userTimes, zx, aggregatedSum);
                                    int loginSuccess = loginFeedback.Item1;
                                    Boolean requireCaptcha = loginFeedback.Item2;
                                    if (loginSuccess == -1)
                                    {
                                        if (!thisUserLocked)
                                        {
                                            if (!locked.ContainsKey(userTimeInDay))
                                            {
                                                locked.Add(userTimeInDay, 0);
                                            }
                                            locked[userTimeInDay] += 1;
                                            thisUserLocked = true;
                                        }
                                    }
                                    if (loginSuccess == 1) break;
                                }

                            }

                            /*2.4 End of Day Attack. If user does not lock their account by the end of the day.
                             * To simply the process, assume user gonna login once after the 180 day period later.*/
                            if (untriedPassword.Count > 0 && !thisUserDone)
                            {

                                int overAllTimeInDay = OverallTime / 24;
                                if (remainPsiBudget > 0 && maxCaptchaWillingToSolve > 0)
                                {
                                    Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();
                                    remainKBudget = budget.Item1 - 1; /* Save 1 for the biggest gain in terms of population*/


                                    if (user.maxKForCaptcha <= 0) maxCaptchaWillingToSolve -= 1;

                                    remainPsiBudget = budget.Item2;
                                    if (maxCaptchaWillingToSolve > 0 && remainKBudget > 0 && untriedPassword.Count > 0)
                                    {
                                        Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional((int)Math.Min(remainKBudget, maxCaptchaWillingToSolve), remainPsiBudget, untriedPassword, bigStepPassword);
                                        //Console.WriteLine("1Iteration----");
                                        while (passwordIndices.Count > 0)
                                        {

                                            int passwordIndex = passwordIndices.Dequeue();
                                            //Console.WriteLine(passwordIndex);
                                            string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                            double thisThreshold = untriedPassword[passwordIndex].Item2;

                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, OverallTime, zx, aggregatedSum);
                                            int loginSuccess = loginFeedback.Item1;
                                            Boolean requireCaptcha = loginFeedback.Item2;

                                            if (requireCaptcha)
                                            {
                                                maxCaptchaWillingToSolve -= 1;
                                                if (!captchaed.ContainsKey(overAllTimeInDay))
                                                {
                                                    captchaed.Add(overAllTimeInDay, 0);
                                                }
                                                captchaed[overAllTimeInDay] += 1;
                                            }

                                            if (!guessed.ContainsKey(overAllTimeInDay))
                                            {
                                                guessed.Add(overAllTimeInDay, 0);
                                            }
                                            guessed[overAllTimeInDay] += 1;



                                            if (loginSuccess == 1)
                                            {
                                                thisUserDone = true;
                                                if (!cracked.ContainsKey(overAllTimeInDay))
                                                {
                                                    cracked.Add(overAllTimeInDay, 0);
                                                }
                                                cracked[overAllTimeInDay] += 1;
                                            }
                                            untriedPassword.Remove(untriedPassword[passwordIndex]);
                                            if (thisUserDone) break;

                                        }
                                    }
                                    if (!thisUserDone)
                                    {
                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(bigStepPassword, OverallTime, zx, aggregatedSum);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;

                                        if (requireCaptcha)
                                        {
                                            if (!captchaed.ContainsKey(overAllTimeInDay))
                                            {
                                                captchaed.Add(overAllTimeInDay, 0);
                                            }
                                            captchaed[overAllTimeInDay] += 1;
                                        }

                                        if (!guessed.ContainsKey(overAllTimeInDay))
                                        {
                                            guessed.Add(overAllTimeInDay, 0);
                                        }

                                        guessed[overAllTimeInDay] += 1;
                                        if (loginSuccess == 1)
                                        {
                                            if (!cracked.ContainsKey(overAllTimeInDay))
                                            {
                                                cracked.Add(overAllTimeInDay, 0);
                                            }
                                            cracked[overAllTimeInDay] += 1;
                                        }
                                        thisUserDone = true;
                                    }

                                    /* Not done, this account locked */
                                    if (!thisUserDone && !thisUserLocked)
                                    {
                                        if (!locked.ContainsKey(overAllTimeInDay))
                                        {
                                            locked.Add(overAllTimeInDay, 0);
                                        }
                                        locked[overAllTimeInDay] += 1;
                                    }

                                    thisUserDone = true;
                                }
                            }



                        }
                        double cuLocked = 0.0;
                        double cuCracked = 0.0;
                        double cuGuessed = 0.0;
                        double cuCaptcha = 0.0;



                        // This text is added only once to the file.

                        using (StreamWriter sw = File.AppendText(OUTPUT_FILE_PATH))
                        {


                            for (int i = 0; i <= 30 * 6 + 1; i++)
                            {
                                if (cracked.ContainsKey(i)) cuCracked += cracked[i];
                                if (locked.ContainsKey(i)) cuLocked += locked[i];
                                if (guessed.ContainsKey(i)) cuGuessed += guessed[i];
                                if (captchaed.ContainsKey(i)) cuCaptcha += captchaed[i];
                                sw.WriteLine(Convert.ToString(kBudget) + "\t" + Convert.ToString(psiBudget) + "\t" + Convert.ToString(captchaBudget) + "\t" + Convert.ToString(i) + "\t" + Convert.ToString(cuCracked / totalNumberOfUsers) + "\t" + Convert.ToString(cuLocked / totalNumberOfUsers) + "\t" + Convert.ToString(cuGuessed / totalNumberOfUsers) + "\t" + Convert.ToString(cuCaptcha / totalNumberOfUsers) + "\t");
                            }
                        }
                    }
                }
            }
        }



        static void UserLongTermStudyExperimentBasedOnUserFile(int[] maxKBudget, double[] privacyBudgetList, bool isYahooStyle, string passwordFileName, SketchCount SC, Dictionary<string,int> passwordMapping, string userFileName,string suffix)
        {



            // Step 1: Load SC and process
            List<double> psiList = new List<double>();
            psiList.Add(double.PositiveInfinity);
            psiList.Add(Math.Pow(2, -7));
            //for (int i = 9; i <= 12; i++)
            //{
            //   psiList.Add(Math.Pow(2, -1 * i));

            //}
            //psiList.Add(Math.Pow(2, -9.375));
            //psiList.Add(Math.Pow(2, -9.5));
            //psiList.Add(Math.Pow(2, -9.375));
            //psiList.Add(Math.Pow(2, -9.5));


            StreamReader sr = new StreamReader("LinkedIn-withcount.txt");

            String line;
            int maxFrequency = 0;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random random = new Random(seed);
            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            List<User> users = new List<User>();
            List<string> advGuess = new List<string>();

            double totalNumberOfUsers = 100000.0;

/*
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string> passwordPopularityTuple;
                // passwordPopularityTuple: (number of users, password)
                //passwordPopularityTuple = new Tuple<int, string>();
                passwordPopularityTuple = HelperFunctions.ParseRockYouLine(line);
                int frequency = passwordPopularityTuple.Item1;
                //Console.WriteLine(line);
                if (passwordMapping == null){
                    maxFrequency += passwordPopularityTuple.Item1;
                    passwordDistributionList.Add(new Tuple<int, string>(frequency, passwordPopularityTuple.Item2));
                }
                else if (passwordMapping.ContainsKey(passwordPopularityTuple.Item2))
                {
                    frequency = passwordMapping[passwordPopularityTuple.Item2];
                    maxFrequency += passwordMapping[passwordPopularityTuple.Item2];
                    passwordDistributionList.Add(new Tuple<int, string>(passwordMapping[passwordPopularityTuple.Item2], passwordPopularityTuple.Item2));
                }
 
                if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;
                    }

            f
                    for (int i = 0; i < frequency; i++)
                    {
                        // 1/5000 * p chance to create a user who use this password,
                         // where p is the number of users who are using
                         // this password
                        int chanceOfGenerateUsers = random.Next(CHANCEOFGENERATEUSER);
                        if (chanceOfGenerateUsers > 1)
                        {

                            continue;
                        }
                        User user = new User();
                        user.Register(passwordPopularityTuple.Item2, 1.0, 0, false);
                        users.Add(user);
                        totalNumberOfUsers += 1;
                    }
                
            }
            // This text is added only once to the file.
*/


            foreach (double privacyBudget in privacyBudgetList)
            {
                SC.ChangePrivacyLevel(privacyBudget);


                string OUTPUT_FILE_PATH = @"c:\DALock\UserLongExperiment\Yahoo\Yahoo_CS"+suffix + privacyBudget.ToString() + ".txt";
                if (File.Exists(OUTPUT_FILE_PATH))
                {
                    File.Delete(OUTPUT_FILE_PATH);
                }
                using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
                {
                    sw.WriteLine("Experiment For User LongtermStudy");
                }



                foreach (double psiBudget in psiList)
                {
                    Console.WriteLine("psiBudget: " + Convert.ToString(psiBudget));
                    foreach (int kBudget in maxKBudget)
                    {
                        Console.WriteLine("-->kBudget: " + Convert.ToString(kBudget));

                        /* Account static dictionary: Key is date and value is the corresponding number of accounts*/
                        Dictionary<int, double> locked = new Dictionary<int, double>();
                        Double averagePsiIncrement = 0.0;
                        Double averageIncorrectAttempt = 0.0;
                        Double maxPsiIncrement = 0.0;
                        StreamReader ufr = new StreamReader(userFileName);
                        /* Step 1: Register Users */
                        while (true)
                        {
                            /* Retrieve users's acitvity from file */
                            String userID = ufr.ReadLine();
                            if (userID == null) break;
                            String password = ufr.ReadLine();
                            String altPassword = ufr.ReadLine();
                            //Console.WriteLine("uid:" + userID);
                            //Console.WriteLine("pwd:" + password);
                            //Console.WriteLine("altpwd:" + altPassword);
                            User user = new User();
                            user.Register(password, 1.0, 0, false);
                            user.setPasswordFromAnotherService(altPassword);
                            user.setThreshold(psiBudget, kBudget, double.PositiveInfinity);
                            List<String> loginActivity = new List<String>();
                            int lineCount = 0;
                            int loginTime = -1;
                            while (true)
                            {
                                String activityLine = ufr.ReadLine();
                                //Console.WriteLine("[" + lineCount.ToString() + "]|[" + activityLine);
                                if (activityLine == null || activityLine.Equals("----------------------------------"))
                                {
                                    //Console.WriteLine("next user");
                                    if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                    if (Convert.ToInt32(userID) > 0) ufr.ReadLine();
                                    break;
                                }
                                if (lineCount % 2 == 0)
                                {
                                    int thisLoginTime = Convert.ToInt32(activityLine);
                                    if (thisLoginTime != loginTime)
                                    {
                                        if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                        loginActivity = new List<String>();
                                    }
                                    loginTime = thisLoginTime;

                                }
                                else
                                {
                                    loginActivity.Add(activityLine);
                                }
                                lineCount += 1;
                            }
                            Boolean thisUserLocked = false;
                            foreach (Tuple<int, List<String>> userLoginActivity in user.timeLineList)
                                {
                                    if (thisUserLocked) break;
                                    int userTimes = userLoginActivity.Item1;
                                    int userTimeInDay = userTimes / 24;
                                    /* User's activities */
                                    foreach (String passwordTypedByUserThisTime in userLoginActivity.Item2)
                                    {
                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(passwordTypedByUserThisTime, userTimes, SC);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;
                                        if (loginSuccess == -1)
                                        {
                                            if (!thisUserLocked)
                                            {
                                                if (!locked.ContainsKey(userTimeInDay))
                                                {
                                                    locked.Add(userTimeInDay, 0);
                                                }
                                                locked[userTimeInDay] += 1;
                                                thisUserLocked = true;
                                            }
                                            break;
                                        }
                                        if (loginSuccess == 1) break;
                                    }

                                }

                        }

            

                        double cuLocked = 0.0;



                        using (StreamWriter sw = File.AppendText(OUTPUT_FILE_PATH))
                        {
                            sw.WriteLine("Average Psi Increment on Incorrect Attempt:" + Convert.ToString(averagePsiIncrement / totalNumberOfUsers) + "Average Incorrect Login:" + Convert.ToString(averageIncorrectAttempt / totalNumberOfUsers));

                            for (int i = 0; i <= 30 * 6 + 1; i++)
                            {
                                if (locked.ContainsKey(i))
                                {
                                    //Console.WriteLine(cuLocked + " " + locked[i]);
                                    cuLocked += locked[i];
                                }
                                sw.WriteLine(Convert.ToString(kBudget) + "\t" + Convert.ToString(psiBudget) + "\t" + Convert.ToString(i) + "\t" + Convert.ToString(cuLocked / totalNumberOfUsers) + "\t");
                            }
                        }

                    }
                }
            }
        }



        static void ZXCVBNUserLongTermStudyExperimentBasedOnUserFile(int[] maxKBudget, double[] privacyBudgetList, bool isYahooStyle, string passwordFileName,string userFileName)
        {

            var zx = new Zxcvbn.Zxcvbn();
            double aggregatedSum = 0.0;
            // Step 1: Load SC and process
            List<double> psiList = new List<double>();
            //psiList.Add(double.PositiveInfinity);
            //psiList.Add(Math.Pow(2, -9));
            psiList.Add(Math.Pow(2, -9.375));
            psiList.Add(Math.Pow(2, -9.4375));
            psiList.Add(Math.Pow(2, -9.5));
            //psiList.Add(Math.Pow(2, -9.625));
            //psiList.Add(Math.Pow(2, -9.75));
            //for (int i = 9; i <= 10; i++)
            //{
            //    psiList.Add(Math.Pow(2, -1 * i));
            //
            //}
            


            StreamReader sr = new StreamReader(passwordFileName);

            String line;
            int maxFrequency = 0;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random random = new Random(seed);
            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            List<User> users = new List<User>();
            List<string> advGuess = new List<string>();

            double totalNumberOfUsers = 100000.0;

            /* Step 1: Register Users */
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string>[] passwordPopularityTupleArray;
                // passwordPopularityTuple: (number of users, password)
                if (isYahooStyle)
                {
                    passwordPopularityTupleArray = HelperFunctions.ParseYahooLine(line);
                }
                else
                {
                    passwordPopularityTupleArray = new Tuple<int, string>[1];
                    passwordPopularityTupleArray[0] = HelperFunctions.ParseRockYouLine(line);
                }
                foreach (Tuple<int, string> passwordPopularityTuple in passwordPopularityTupleArray)
                {
                    if (passwordDistributionList.Count < 20000) {
                        var result = zx.EvaluatePassword(passwordPopularityTuple.Item2);
                        double estimatedCostInPsi = 1.0 / (result.Entropy + 1.0);
                        //if (passwordDistributionList.Count < 200)
                            aggregatedSum += estimatedCostInPsi;
                     }
                    passwordDistributionList.Add(passwordPopularityTuple);
                    maxFrequency += passwordPopularityTuple.Item1;

                    if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;
                    }
                }
            }
            // This text is added only once to the file.
            Console.WriteLine("aggregated sum is " + aggregatedSum);


            int OverallTime = 24 * 30 * 6;//24 hours in a day * 30 day in a month * 12 month in a year * 5 years of login hours

            int usertestId = 0;



            string OUTPUT_FILE_PATH = @"c:\DALock\UserLongExperiment\Yahoo\Yahoo_ZXCVBN_Normal.txt";
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                File.Delete(OUTPUT_FILE_PATH);
            }
            using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
            {
                sw.WriteLine("Experiment For User LongtermStudy");
            }



            foreach (double psiBudget in psiList)
            {
                Console.WriteLine("psiBudget: " + Convert.ToString(psiBudget));
                foreach (int kBudget in maxKBudget)
                {

                        Console.WriteLine("-->kBudget: " + Convert.ToString(kBudget));
                        String bigStepPassword = String.Copy(passwordDistributionList[0].Item2);
                        if (psiBudget.Equals(double.PositiveInfinity))
                            bigStepPassword = "";
                        /* Account static dictionary: Key is date and value is the corresponding number of accounts*/
                        Dictionary<int, double> cracked = new Dictionary<int, double>();
                        Dictionary<int, double> locked = new Dictionary<int, double>();
                        Dictionary<int, double> guessed = new Dictionary<int, double>();
                        Dictionary<int, double> captchaed = new Dictionary<int, double>();

                        StreamReader ufr = new StreamReader(userFileName);
                        while (true)
                        {
                            /* Retrieve users's acitvity from file */
                            String userID = ufr.ReadLine();
                            if (userID == null) break;
                            String password = ufr.ReadLine();
                            String altPassword = ufr.ReadLine();

                            User user = new User();
                            user.Register(password, 1.0, 0, false);
                            user.setPasswordFromAnotherService(altPassword);
                            user.setThreshold(psiBudget, kBudget, double.PositiveInfinity);
                            List<String> loginActivity = new List<String>();
                            int lineCount = 0;
                            int loginTime = -1;
                            while (true)
                            {
                                String activityLine = ufr.ReadLine();
                                //Console.WriteLine("[" + lineCount.ToString() + "]|[" + activityLine);
                                if (activityLine == null || activityLine.Equals("----------------------------------"))
                                {
                                    //Console.WriteLine("next user");
                                    if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                    if (Convert.ToInt32(userID) > 0) ufr.ReadLine();
                                    break;
                                }
                                if (lineCount % 2 == 0)
                                {
                                    int thisLoginTime = Convert.ToInt32(activityLine);
                                    if (thisLoginTime != loginTime)
                                    {
                                        if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                        loginActivity = new List<String>();
                                    }
                                    loginTime = thisLoginTime;

                                }
                                else
                                {
                                    loginActivity.Add(activityLine);
                                }
                                lineCount += 1;
                            }

                            double maxCaptchaWillingToSolve = double.PositiveInfinity;



                            List<Tuple<string, double, double, double>> untriedPassword;



                            double remainPsiBudget = psiBudget;
                            int remainKBudget = kBudget - 1;
                            bool thisUserDone = false;
                            bool thisUserLocked = false;

                            foreach (Tuple<int, List<String>> userLoginActivity in user.timeLineList)
                            {
                                int userTimes = userLoginActivity.Item1;
                                int userTimeInDay = userTimes / 24;



                                /* 2.2.2 Calculate the budget next time */


                                Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();


                                /* User's activities */
                                foreach (String passwordTypedByUserThisTime in userLoginActivity.Item2)
                                {
                                    Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(passwordTypedByUserThisTime, userTimes, zx, aggregatedSum);
                                    int loginSuccess = loginFeedback.Item1;
                                    Boolean requireCaptcha = loginFeedback.Item2;
                                    if (loginSuccess == -1)
                                    {
                                        if (!thisUserLocked)
                                        {
                                            if (!locked.ContainsKey(userTimeInDay))
                                            {
                                                locked.Add(userTimeInDay, 0);
                                            }
                                            locked[userTimeInDay] += 1;
                                            thisUserLocked = true;
                                        }
                                    }
                                    if (loginSuccess == 1) break;
                                }

                            }


                        }
                        double cuLocked = 0.0;




                        // This text is added only once to the file.

                        using (StreamWriter sw = File.AppendText(OUTPUT_FILE_PATH))
                        {


                            for (int i = 0; i <= 30 * 6 + 1; i++)
                            {
 
                                if (locked.ContainsKey(i)) cuLocked += locked[i];
                                sw.WriteLine(Convert.ToString(kBudget) + "\t" + Convert.ToString(psiBudget) + Convert.ToString(i) +  "\t" + Convert.ToString(cuLocked / totalNumberOfUsers));
                            }
                        }
                    
                }
            }
        }




        static void PGSForeSeerOptimalOnCaptchaExperimentBasedOnUserFile(int[] maxKBudget, double[] privacyBudgetList, double rewardPerAccount, bool isYahooStyle, string passwordFileName, string userFileName, string PGSFile)
        {
            StreamReader pgsreader = new StreamReader(PGSFile+".txt");
            Dictionary<String, double> pgsEstimation = new Dictionary<string, double>();
            
            while (true)
            {
                String pgsline = pgsreader.ReadLine();
                if (pgsline == null) break;
                Tuple<double, string> passwordPopularityTupleArray =HelperFunctions.ParsePGSLine(pgsline);
                pgsEstimation.Add(passwordPopularityTupleArray.Item2, passwordPopularityTupleArray.Item1);

            }

                /* Hash Table for Key: Password Value:<Cost, Benefit> Dicitonary
                 * The Cost is defined as the Psi Increment in the SC
                 * The Benefit/gain is defined as real password popularity.
                 */
           Dictionary<string, Tuple<double, double>> passwordToCostAndBenefit = new Dictionary<string, Tuple<double, double>>();


            /*
             * Define number of passwords interested. 18000 ~= 365 * 50. 
             * Which means adversaries can try 100 passwords per day for 180 day
             * period
            */
            double MAX_NUMBER_OF_PASSWORDS_INTERESTED = 20000;  //2*20*180 = 7200
            //double COSTPERGUESS = (0.011/3600.0)/2.6e9; // 2.6e9 guess per second, AWS server rate: 0.011 per hour



            // Step 1: Load SC and process
            /* No captcha: infinity
             * sloppy typer: 1 mistake per day on average
             * enough to memorize: 50 mistakes
              * */
            double[] captchaList = { double.PositiveInfinity };
            List<double> psiList = new List<double>();
            //psiList.Add(double.PositiveInfinity);
            //psiList.Add(Math.Pow(2, -7.5));
            //psiList.Add(Math.Pow(2, -9.125));
            psiList.Add(Math.Pow(2, -7.0));
            //psiList.Add(Math.Pow(2, -9.4375));
            //hashpsiList.Add(Math.Pow(2, -9.5));
            //psiList.Add(Math.Pow(2, -9.625));
            //psiList.Add(Math.Pow(2, -1 * 9.75));
            //psiList.Add(double.PositiveInfinity);
            //for (int i = 6; i <= 11; i++)
            //{
            //    psiList.Add(Math.Pow(2, -1 * i));
            //
            //}
            //psiList.Add(double.PositiveInfinity);


            /* Read the password file */
            StreamReader sr = new StreamReader(passwordFileName);




            /* Variables For Experiment */
            String line;
            int maxFrequency = 0;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random random = new Random(seed);
            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            List<User> users = new List<User>();
            List<Tuple<int, string>> advGuessUnsorted = new List<Tuple<int, string>>();
            double totalNumberOfUsers = 100000.0;
            long totalNumberPasswordsInFile = 0;
            /* Step 1: Create Users */
            double aggregatedSum = 0.0;
            double maxInc = 0.0;
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string>[] passwordPopularityTupleArray;
                // passwordPopularityTuple: (number of users, password)
                if (isYahooStyle)
                {
                    passwordPopularityTupleArray = HelperFunctions.ParseYahooLine(line);
                }
                else
                {
                    passwordPopularityTupleArray = new Tuple<int, string>[1];
                    passwordPopularityTupleArray[0] = HelperFunctions.ParseRockYouLine(line);
                }
                foreach (Tuple<int, string> passwordPopularityTuple in passwordPopularityTupleArray)
                {
                    totalNumberPasswordsInFile += passwordPopularityTuple.Item1;
                    if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;

                    }
                    passwordDistributionList.Add(passwordPopularityTuple);
                    maxFrequency += passwordPopularityTuple.Item1;
                    if (advGuessUnsorted.Count < MAX_NUMBER_OF_PASSWORDS_INTERESTED)
                    {
                        advGuessUnsorted.Add(passwordPopularityTuple);
                        if (pgsEstimation.ContainsKey(passwordPopularityTuple.Item2))
                        {
                            var result = pgsEstimation[passwordPopularityTuple.Item2];

                            if (result > 0)
                            {
                                double estimatedCostInPsi = 1.0 / (result + 1.0);
                                aggregatedSum += estimatedCostInPsi;
                                maxInc += result;
                            }

                            //if (passwordDistributionList.Count < 200)
                        }
                        else
                        {
                            pgsEstimation.Add(passwordPopularityTuple.Item2, -5.0);
                        }
                    }
                }
            }
            List<string> keys = new List<string>(pgsEstimation.Keys);
            foreach (string entry in keys) {
                if (pgsEstimation[entry] < 0)
                    pgsEstimation[entry] =maxInc ;
            }
            /* Create a 2nd password for each user */
            // Step 2. Generate User Arrival Info

            Console.WriteLine("In Foreseer Experiment: Users: " + Convert.ToString(totalNumberOfUsers));
            //double aggregatedSum = 0.0;
            List<Tuple<string, double, double, double>> advGuessForTraditional = new List<Tuple<string, double, double, double>>();
            List<Tuple<string, double, double, double>> advGuess = new List<Tuple<string, double, double, double>>();




            string OUTPUT_FILE_PATH = @"c:\DALock\OptimalGuessing\Yahoo\Yahoo_" + PGSFile + "all∞";
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                File.Delete(OUTPUT_FILE_PATH);
            }
            // Create a file to write to.


            int optimalAttemptForCaptCha = int.MaxValue;


            foreach (Tuple<int, string> passwordTuple in advGuessUnsorted)
            {
                string password = passwordTuple.Item2;
                var result = pgsEstimation[password];
                double estimatedCostInPsi = 1.0 / (result + 1.0);
                estimatedCostInPsi /= aggregatedSum;
                //Console.WriteLine("Entropy: ["+password+"]:" + estimatedCostInPsi);
                advGuess.Add(new Tuple<string, double, double, double>(password, estimatedCostInPsi, passwordTuple.Item1 / estimatedCostInPsi, passwordTuple.Item1));
                advGuessForTraditional.Add(new Tuple<string, double, double, double>(password, estimatedCostInPsi, passwordTuple.Item1, passwordTuple.Item1));
                //Console.WriteLine(password+"["+estimatedCostInPsi+"]");
            }
            advGuess.Sort((x, y) => y.Item3.CompareTo(x.Item3));

            int OverallTime = 24 * 30 * 6;//24 hours in a day * 30 day in a month * 6 month in a year

            using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
            {
                sw.WriteLine("Experiment For Foreseer");
                sw.WriteLine("Optimal Captcha: " + optimalAttemptForCaptCha);
            }
            foreach (double psiBudget in psiList)
            {
                Console.WriteLine("psiBudget: " + Convert.ToString(psiBudget));
                foreach (int kBudget in maxKBudget)
                {
                    foreach (double captchaBudget in captchaList)
                    {

                        Console.WriteLine("-->kBudget: " + Convert.ToString(kBudget));
                        String bigStepPassword = String.Copy(passwordDistributionList[0].Item2);
                        if (psiBudget.Equals(double.PositiveInfinity))
                            bigStepPassword = "";
                        /* Account static dictionary: Key is date and value is the corresponding number of accounts*/
                        Dictionary<int, double> cracked = new Dictionary<int, double>();
                        Dictionary<int, double> locked = new Dictionary<int, double>();
                        Dictionary<int, double> guessed = new Dictionary<int, double>();
                        Dictionary<int, double> captchaed = new Dictionary<int, double>();

                        StreamReader ufr = new StreamReader(userFileName);
                        while (true)
                        {
                            /* Retrieve users's acitvity from file */
                            String userID = ufr.ReadLine();
                            if (userID == null) break;
                            String password = ufr.ReadLine();
                            String altPassword = ufr.ReadLine();

                            User user = new User();
                            user.Register(password, 1.0, 0, false);
                            user.setPasswordFromAnotherService(altPassword);
                            user.setThreshold(psiBudget, kBudget, captchaBudget);
                            List<String> loginActivity = new List<String>();
                            int lineCount = 0;
                            int loginTime = -1;
                            while (true)
                            {
                                String activityLine = ufr.ReadLine();
                                if (activityLine == null || activityLine.Equals("----------------------------------"))
                                {
                                    //Console.WriteLine("next user");
                                    if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                    if (Convert.ToInt32(userID) > 0) ufr.ReadLine();
                                    break;
                                }
                                if (lineCount % 2 == 0)
                                {
                                    int thisLoginTime = Convert.ToInt32(activityLine);
                                    if (thisLoginTime != loginTime)
                                    {
                                        if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                        loginActivity = new List<String>();
                                    }
                                    loginTime = thisLoginTime;

                                }
                                else
                                {
                                    loginActivity.Add(activityLine);
                                }
                                lineCount += 1;
                            }

                            double maxCaptchaWillingToSolve = optimalAttemptForCaptCha;



                            List<Tuple<string, double, double, double>> untriedPassword;

                            untriedPassword = new List<Tuple<string, double, double, double>>(advGuess);
                            //Console.WriteLine("use traditional");


                            double remainPsiBudget = psiBudget;
                            int remainKBudget = kBudget - 1;
                            bool thisUserDone = false;
                            bool thisUserLocked = false;

                            foreach (Tuple<int, List<String>> userLoginActivity in user.timeLineList)
                            {
                                int userTimes = userLoginActivity.Item1;
                                int userTimeInDay = userTimes / 24;



                                /* 2.2.2 Calculate the budget next time */


                                Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();
                                remainKBudget = budget.Item1;
                                remainPsiBudget = budget.Item2;

                                double remainingPsiBudgetBeforeFirstFailure = budget.Item2;
                                foreach (String passwordTypedThisTime in userLoginActivity.Item2)
                                {
                                    //Console.WriteLine("Type:"+passwordTypedThisTime);
                                    if (!passwordTypedThisTime.Equals(user.pwd) && remainPsiBudget > 0 && remainKBudget > 0)
                                    {
                                        remainKBudget -= 1;
                                        var tempresult = aggregatedSum;
                                        if ( pgsEstimation.ContainsKey(password) && pgsEstimation[password] > 0)
                                            tempresult = pgsEstimation[password];
                                        double temppctg = 1.0 / (tempresult + 1.0);
                                        temppctg /= aggregatedSum;
                                        remainPsiBudget -= temppctg;
                                    }
                                }
                                /* 2.3.1 Calculate attack budget for this step */

                                /* run out of dictionary stop attack */
                                if (untriedPassword.Count <= 0) break;
                                /* 2.3.2 Real Attack Starts */
                                /* 2.3.2-1 Real Attack Starts: User locked their account 
                                 *  - Case 1: User Gonna lockout his/her account: thisUserLocked = True
                                 *  - Case 2: remaining psi budget is not high enough for any password attempt
                                    - Case 3: Captcha Give up
                                   */
                                if (remainKBudget <= 0 || remainPsiBudget <= 0)
                                {

                                    remainKBudget = kBudget - 1; /*  not leting user to try, save 1 for the biggest gain in terms of population*/
                                    remainPsiBudget = remainingPsiBudgetBeforeFirstFailure;

                                    /* Get One giant step password: this password is almost certain to exceed the budget */

                                    if (maxCaptchaWillingToSolve > 0)
                                    {
                                        if (user.maxKForCaptcha <= 0) maxCaptchaWillingToSolve -= 1;
                                    }
                                    else
                                    {
                                        thisUserDone = true;
                                    }
                                    if (thisUserDone) break;
                                    /* Optimize the rest password choices by knapsacks*/
                                    if (maxCaptchaWillingToSolve > 0 && remainKBudget > 0 && untriedPassword.Count > 0)
                                    {
                                        Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget, remainPsiBudget, untriedPassword, bigStepPassword);

                                        while (passwordIndices.Count > 0)
                                        {
                                            int passwordIndex = passwordIndices.Dequeue();
                                            string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                            //Console.WriteLine(nextPasswordToGuess);
                                            double thisThreshold = untriedPassword[passwordIndex].Item2;

                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, userTimes, pgsEstimation, aggregatedSum,maxInc);
                                            int loginSuccess = loginFeedback.Item1;
                                            Boolean requireCaptcha = loginFeedback.Item2;

                                            if (requireCaptcha)
                                            {
                                                maxCaptchaWillingToSolve -= 1;
                                                if (!captchaed.ContainsKey(userTimeInDay))
                                                {
                                                    captchaed.Add(userTimeInDay, 0);
                                                }
                                                captchaed[userTimeInDay] += 1;
                                            }

                                            if (!guessed.ContainsKey(userTimeInDay))
                                            {
                                                guessed.Add(userTimeInDay, 0);
                                            }
                                            guessed[userTimeInDay] += 1;



                                            if (loginSuccess == 1)
                                            {
                                                thisUserDone = true;
                                                if (!cracked.ContainsKey(userTimeInDay))
                                                {
                                                    cracked.Add(userTimeInDay, 0);
                                                }
                                                cracked[userTimeInDay] += 1;
                                            }
                                            untriedPassword.Remove(untriedPassword[passwordIndex]);
                                            if (thisUserDone) break;

                                        }
                                    }

                                    if (!thisUserDone)
                                    {

                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(bigStepPassword, userTimes, pgsEstimation, aggregatedSum, maxInc);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;
                                        if (requireCaptcha)
                                        {
                                            if (!captchaed.ContainsKey(userTimeInDay))
                                            {
                                                captchaed.Add(userTimeInDay, 0);
                                            }
                                            captchaed[userTimeInDay] += 1;
                                        }
                                        if (!guessed.ContainsKey(userTimeInDay))
                                        {
                                            guessed.Add(userTimeInDay, 0);
                                        }
                                        guessed[userTimeInDay] += 1;

                                        if (loginSuccess == 1)
                                        {
                                            if (!cracked.ContainsKey(userTimeInDay))
                                            {
                                                cracked.Add(userTimeInDay, 0);
                                            }
                                            cracked[userTimeInDay] += 1;
                                        }
                                    }
                                    thisUserDone = true;
                                }
                                if (thisUserDone) break;


                                /* 2.3.2-2 Real Attack Starts: User login successfully: Insert attack before  */
                                if (maxCaptchaWillingToSolve > 0 && remainKBudget > 0 && untriedPassword.Count > 0)
                                {
                                    Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional(remainKBudget - 1, remainPsiBudget, untriedPassword, bigStepPassword);

                                    while (passwordIndices.Count > 0)
                                    {
                                        if (passwordIndices.Count > kBudget)
                                        {
                                            Console.WriteLine("Error!!!");
                                        }
                                        int passwordIndex = passwordIndices.Dequeue();
                                        string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                        double thisThreshold = untriedPassword[passwordIndex].Item2;

                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, userTimes, pgsEstimation, aggregatedSum, maxInc);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;

                                        if (requireCaptcha)
                                        {
                                            maxCaptchaWillingToSolve -= 1;
                                            if (!captchaed.ContainsKey(userTimeInDay))
                                            {
                                                captchaed.Add(userTimeInDay, 0);
                                            }
                                            captchaed[userTimeInDay] += 1;
                                        }

                                        if (!guessed.ContainsKey(userTimeInDay))
                                        {
                                            guessed.Add(userTimeInDay, 0);
                                        }
                                        guessed[userTimeInDay] += 1;



                                        if (loginSuccess == 1)
                                        {
                                            thisUserDone = true;
                                            if (!cracked.ContainsKey(userTimeInDay))
                                            {
                                                cracked.Add(userTimeInDay, 0);
                                            }
                                            cracked[userTimeInDay] += 1;
                                        }
                                        untriedPassword.Remove(untriedPassword[passwordIndex]);
                                        if (thisUserDone) break;

                                    }
                                }

                                /* If cracked, stop attacking */
                                if (thisUserDone) break;


                                /* User's activities */
                                foreach (String passwordTypedByUserThisTime in userLoginActivity.Item2)
                                {
                                    Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(passwordTypedByUserThisTime, userTimes, pgsEstimation, aggregatedSum, maxInc);
                                    int loginSuccess = loginFeedback.Item1;
                                    Boolean requireCaptcha = loginFeedback.Item2;
                                    if (loginSuccess == -1)
                                    {
                                        if (!thisUserLocked)
                                        {
                                            if (!locked.ContainsKey(userTimeInDay))
                                            {
                                                locked.Add(userTimeInDay, 0);
                                            }
                                            locked[userTimeInDay] += 1;
                                            thisUserLocked = true;
                                        }
                                    }
                                    if (loginSuccess == 1) break;
                                }

                            }

                            /*2.4 End of Day Attack. If user does not lock their account by the end of the day.
                             * To simply the process, assume user gonna login once after the 180 day period later.*/
                            if (untriedPassword.Count > 0 && !thisUserDone)
                            {

                                int overAllTimeInDay = OverallTime / 24;
                                if (remainPsiBudget > 0 && maxCaptchaWillingToSolve > 0)
                                {
                                    Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();
                                    remainKBudget = budget.Item1 - 1; /* Save 1 for the biggest gain in terms of population*/


                                    if (user.maxKForCaptcha <= 0) maxCaptchaWillingToSolve -= 1;

                                    remainPsiBudget = budget.Item2;
                                    if (maxCaptchaWillingToSolve > 0 && remainKBudget > 0 && untriedPassword.Count > 0)
                                    {
                                        Queue<int> passwordIndices = HelperFunctions.getKBestPasswordToTryTraditional((int)Math.Min(remainKBudget, maxCaptchaWillingToSolve), remainPsiBudget, untriedPassword, bigStepPassword);
                                        //Console.WriteLine("1Iteration----");
                                        while (passwordIndices.Count > 0)
                                        {

                                            int passwordIndex = passwordIndices.Dequeue();
                                            //Console.WriteLine(passwordIndex);
                                            string nextPasswordToGuess = untriedPassword[passwordIndex].Item1;
                                            double thisThreshold = untriedPassword[passwordIndex].Item2;

                                            Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(nextPasswordToGuess, OverallTime, pgsEstimation, aggregatedSum, maxInc);
                                            int loginSuccess = loginFeedback.Item1;
                                            Boolean requireCaptcha = loginFeedback.Item2;

                                            if (requireCaptcha)
                                            {
                                                maxCaptchaWillingToSolve -= 1;
                                                if (!captchaed.ContainsKey(overAllTimeInDay))
                                                {
                                                    captchaed.Add(overAllTimeInDay, 0);
                                                }
                                                captchaed[overAllTimeInDay] += 1;
                                            }

                                            if (!guessed.ContainsKey(overAllTimeInDay))
                                            {
                                                guessed.Add(overAllTimeInDay, 0);
                                            }
                                            guessed[overAllTimeInDay] += 1;



                                            if (loginSuccess == 1)
                                            {
                                                thisUserDone = true;
                                                if (!cracked.ContainsKey(overAllTimeInDay))
                                                {
                                                    cracked.Add(overAllTimeInDay, 0);
                                                }
                                                cracked[overAllTimeInDay] += 1;
                                            }
                                            untriedPassword.Remove(untriedPassword[passwordIndex]);
                                            if (thisUserDone) break;

                                        }
                                    }
                                    if (!thisUserDone)
                                    {
                                        Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(bigStepPassword, OverallTime, pgsEstimation, aggregatedSum, maxInc);
                                        int loginSuccess = loginFeedback.Item1;
                                        Boolean requireCaptcha = loginFeedback.Item2;

                                        if (requireCaptcha)
                                        {
                                            if (!captchaed.ContainsKey(overAllTimeInDay))
                                            {
                                                captchaed.Add(overAllTimeInDay, 0);
                                            }
                                            captchaed[overAllTimeInDay] += 1;
                                        }

                                        if (!guessed.ContainsKey(overAllTimeInDay))
                                        {
                                            guessed.Add(overAllTimeInDay, 0);
                                        }

                                        guessed[overAllTimeInDay] += 1;
                                        if (loginSuccess == 1)
                                        {
                                            if (!cracked.ContainsKey(overAllTimeInDay))
                                            {
                                                cracked.Add(overAllTimeInDay, 0);
                                            }
                                            cracked[overAllTimeInDay] += 1;
                                        }
                                        thisUserDone = true;
                                    }

                                    /* Not done, this account locked */
                                    if (!thisUserDone && !thisUserLocked)
                                    {
                                        if (!locked.ContainsKey(overAllTimeInDay))
                                        {
                                            locked.Add(overAllTimeInDay, 0);
                                        }
                                        locked[overAllTimeInDay] += 1;
                                    }

                                    thisUserDone = true;
                                }
                            }



                        }
                        double cuLocked = 0.0;
                        double cuCracked = 0.0;
                        double cuGuessed = 0.0;
                        double cuCaptcha = 0.0;



                        // This text is added only once to the file.

                        using (StreamWriter sw = File.AppendText(OUTPUT_FILE_PATH))
                        {


                            for (int i = 0; i <= 30 * 6 + 1; i++)
                            {
                                if (cracked.ContainsKey(i)) cuCracked += cracked[i];
                                if (locked.ContainsKey(i)) cuLocked += locked[i];
                                if (guessed.ContainsKey(i)) cuGuessed += guessed[i];
                                if (captchaed.ContainsKey(i)) cuCaptcha += captchaed[i];
                                sw.WriteLine(Convert.ToString(kBudget) + "\t" + Convert.ToString(psiBudget) + "\t" + Convert.ToString(captchaBudget) + "\t" + Convert.ToString(i) + "\t" + Convert.ToString(cuCracked / totalNumberOfUsers) + "\t" + Convert.ToString(cuLocked / totalNumberOfUsers) + "\t" + Convert.ToString(cuGuessed / totalNumberOfUsers) + "\t" + Convert.ToString(cuCaptcha / totalNumberOfUsers) + "\t");
                            }
                        }
                    }
                }
            }
        }




        static void PGSUserLongTermStudyExperimentBasedOnUserFile(int[] maxKBudget, double[] privacyBudgetList, bool isYahooStyle, string passwordFileName, string userFileName, string PGSFile)
        {
            StreamReader pgsreader = new StreamReader(PGSFile+".txt");
            Dictionary<String, double> pgsEstimation = new Dictionary<string, double>();

            while (true)
            {
                String pgsline = pgsreader.ReadLine();
                if (pgsline == null) break;
                Tuple<double, string> passwordPopularityTupleArray = HelperFunctions.ParsePGSLine(pgsline);
                pgsEstimation.Add(passwordPopularityTupleArray.Item2, passwordPopularityTupleArray.Item1);

            }
            double aggregatedSum = 0.0;
            // Step 1: Load SC and process
            List<double> psiList = new List<double>();
            //psiList.Add(double.PositiveInfinity);
            //psiList.Add(Math.Pow(2, -8));
            psiList.Add(Math.Pow(2, -7));
            //psiList.Add(Math.Pow(2, -9));
            //psiList.Add(Math.Pow(2, -9.125));
            //psiList.Add(Math.Pow(2, -9.375));

            //psiList.Add(Math.Pow(2, -9.5));
            //psiList.Add(Math.Pow(2, -9.625));
            //psiList.Add(Math.Pow(2, -9.75));
            //for (int i = 9; i <= 10; i++)
            //{
            //    psiList.Add(Math.Pow(2, -1 * i));
            //
            //}



            StreamReader sr = new StreamReader(passwordFileName);

            String line;
            int maxFrequency = 0;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random random = new Random(seed);
            List<Tuple<int, string>> passwordDistributionList = new List<Tuple<int, string>>();
            List<User> users = new List<User>();
            List<string> advGuess = new List<string>();

            double totalNumberOfUsers = 100000.0;
            double maxInc = 0.0;
            /* Step 1: Register Users */
            while (true)
            {
                line = sr.ReadLine();
                if (line == null) break;

                Tuple<int, string>[] passwordPopularityTupleArray;
                // passwordPopularityTuple: (number of users, password)
                if (isYahooStyle)
                {
                    passwordPopularityTupleArray = HelperFunctions.ParseYahooLine(line);
                }
                else
                {
                    passwordPopularityTupleArray = new Tuple<int, string>[1];
                    passwordPopularityTupleArray[0] = HelperFunctions.ParseRockYouLine(line);
                }

                foreach (Tuple<int, string> passwordPopularityTuple in passwordPopularityTupleArray)
                {
                    if (passwordDistributionList.Count < 20000)
                    {
                        if (pgsEstimation.ContainsKey(passwordPopularityTuple.Item2))
                        {
                            var result = pgsEstimation[passwordPopularityTuple.Item2];

                            if (result > 0)
                            {
                                double estimatedCostInPsi = 1.0 / (result + 1.0);
                                aggregatedSum += estimatedCostInPsi;
                                maxInc += result;
                            }

                            //if (passwordDistributionList.Count < 200)
                        }
                        else {
                            pgsEstimation.Add(passwordPopularityTuple.Item2, -5.0);
                        }
                    }
                    passwordDistributionList.Add(passwordPopularityTuple);
                    maxFrequency += passwordPopularityTuple.Item1;

                    if (passwordPopularityTuple.Item2.Length == 0)
                    {
                        Console.WriteLine("empty password found");
                        continue;
                    }
                }
            }


            List<string> keys = new List<string>(pgsEstimation.Keys);
            foreach (string entry in keys)
            {
                if (pgsEstimation[entry] < 0)
                    pgsEstimation[entry] = maxInc ;
            }
            // This text is added only once to the file.
            Console.WriteLine("aggregated sum is " + aggregatedSum);


            int OverallTime = 24 * 30 * 6;//24 hours in a day * 30 day in a month * 12 month in a year * 5 years of login hours

            int usertestId = 0;



            string OUTPUT_FILE_PATH = @"c:\DALock\UserLongExperiment\Yahoo\Yahoo_"+ PGSFile  + "_all∞.txt";
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                File.Delete(OUTPUT_FILE_PATH);
            }
            using (StreamWriter sw = File.CreateText(OUTPUT_FILE_PATH))
            {
                sw.WriteLine("Experiment For User LongtermStudy");
            }



            foreach (double psiBudget in psiList)
            {
                Console.WriteLine("psiBudget: " + Convert.ToString(psiBudget));
                foreach (int kBudget in maxKBudget)
                {

                    Console.WriteLine("-->kBudget: " + Convert.ToString(kBudget));
                    String bigStepPassword = String.Copy(passwordDistributionList[0].Item2);
                    if (psiBudget.Equals(double.PositiveInfinity))
                        bigStepPassword = "";
                    /* Account static dictionary: Key is date and value is the corresponding number of accounts*/
                    Dictionary<int, double> cracked = new Dictionary<int, double>();
                    Dictionary<int, double> locked = new Dictionary<int, double>();
                    Dictionary<int, double> guessed = new Dictionary<int, double>();
                    Dictionary<int, double> captchaed = new Dictionary<int, double>();

                    StreamReader ufr = new StreamReader(userFileName);
                    while (true)
                    {
                        /* Retrieve users's acitvity from file */
                        String userID = ufr.ReadLine();
                        if (userID == null) break;
                        String password = ufr.ReadLine();
                        String altPassword = ufr.ReadLine();

                        User user = new User();
                        user.Register(password, 1.0, 0, false);
                        user.setPasswordFromAnotherService(altPassword);
                        user.setThreshold(psiBudget, kBudget, double.PositiveInfinity);
                        List<String> loginActivity = new List<String>();
                        int lineCount = 0;
                        int loginTime = -1;
                        while (true)
                        {
                            String activityLine = ufr.ReadLine();
                            //Console.WriteLine("[" + lineCount.ToString() + "]|[" + activityLine);
                            if (activityLine == null || activityLine.Equals("----------------------------------"))
                            {
                                //Console.WriteLine("next user");
                                if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                if (Convert.ToInt32(userID) > 0) ufr.ReadLine();
                                break;
                            }
                            if (lineCount % 2 == 0)
                            {
                                int thisLoginTime = Convert.ToInt32(activityLine);
                                if (thisLoginTime != loginTime)
                                {
                                    if (loginTime != -1) user.timeLineList.Add(new Tuple<int, List<String>>(loginTime, loginActivity));
                                    loginActivity = new List<String>();
                                }
                                loginTime = thisLoginTime;

                            }
                            else
                            {
                                loginActivity.Add(activityLine);
                            }
                            lineCount += 1;
                        }





                        double remainPsiBudget = psiBudget;
                        int remainKBudget = kBudget - 1;
                        bool thisUserLocked = false;

                        foreach (Tuple<int, List<String>> userLoginActivity in user.timeLineList)
                        {
                            int userTimes = userLoginActivity.Item1;
                            int userTimeInDay = userTimes / 24;



                            /* 2.2.2 Calculate the budget next time */


                            Tuple<int, double> budget = user.getThrottlingBudgetForAdversary();


                            /* User's activities */
                            foreach (String passwordTypedByUserThisTime in userLoginActivity.Item2)
                            {
                                Tuple<int, Boolean> loginFeedback = user.loginNeverResetPsi(passwordTypedByUserThisTime, userTimes, pgsEstimation, aggregatedSum, maxInc);
                                int loginSuccess = loginFeedback.Item1;
                                Boolean requireCaptcha = loginFeedback.Item2;
                                if (loginSuccess == -1)
                                {
                                    if (!thisUserLocked)
                                    {
                                        if (!locked.ContainsKey(userTimeInDay))
                                        {
                                            locked.Add(userTimeInDay, 0);
                                        }
                                        locked[userTimeInDay] += 1;
                                        thisUserLocked = true;
                                    }
                                }
                                if (loginSuccess == 1) break;
                            }

                        }


                    }
                    double cuLocked = 0.0;




                    // This text is added only once to the file.

                    using (StreamWriter sw = File.AppendText(OUTPUT_FILE_PATH))
                    {


                        for (int i = 0; i <= 30 * 6 + 1; i++)
                        {

                            if (locked.ContainsKey(i)) cuLocked += locked[i];
                            sw.WriteLine(Convert.ToString(kBudget) + "\t" + Convert.ToString(psiBudget) + Convert.ToString(i) + "\t" + Convert.ToString(cuLocked / totalNumberOfUsers));
                        }
                    }

                }
            }
        }



    }




}
