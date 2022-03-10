using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;


namespace ConsoleApp5
{

    class HelperFunctions
    {

        static Random r = new Random();
        /* Setting of Password Typos Constant 
        static double CHANCE_OF_MAKE_MISTAKE = 701 / 9440;
        static double CHANCE_OF_MAKE_TYPO = 484 / 701 * CHANCE_OF_MAKE_MISTAKE;
        static double CHANCE_OF_PICK_WRONG = (701 - 484) / 701 * CHANCE_OF_MAKE_MISTAKE;
        static double SWITCH_ALL_CASE = 10.9;
        static double SWITCH_FIRST_CHAR = 4.5 + SWITCH_ALL_CASE;
        static double REMOVE_LAST_CHARACTER = 4.65 + SWITCH_FIRST_CHAR;
        static double REMOVE_FIRST_CHARACTER = 1.3 + REMOVE_LAST_CHARACTER;
        static double N2S_LAST_CHAR = 0.2 + REMOVE_FIRST_CHARACTER;
        */
        /* Setting of TypTop Constant */
        static double CHANCE_OF_MAKE_MISTAKE = 701 / 9440.0;
        static double CHANCE_OF_TYPE_WRONG_PASSWORD_GIVEN_MISTAKE = 1.0 - 484 / 701.0;
        static double CHANCE_OF_MAKE_TYPOS_GIVEN_MISTAKE = 484 / 701.0;
        static double CHANCE_OF_WRONG_PASSWORD = CHANCE_OF_MAKE_MISTAKE * CHANCE_OF_TYPE_WRONG_PASSWORD_GIVEN_MISTAKE;  /* CALCULATED BASED ON CONDITIONAL PROBABILITY */
        static double CHANCE_OF_TYPO = CHANCE_OF_MAKE_MISTAKE * CHANCE_OF_MAKE_TYPOS_GIVEN_MISTAKE;  /* CALCULATED BASED ON CONDITIONAL PROBABILITY */
        /* The following probability are listed in cumulative interval fashion */
        static double CAP_LOCK = 0.14 ;
        static double SHIFT_FIRST_CHAR = 0.04  + CAP_LOCK;
        static double ONE_INSERTION = 0.12  + SHIFT_FIRST_CHAR;
        static double ONE_DELETION = 0.12  + ONE_INSERTION;
        static double ONE_REPLACEMENT = 0.31  + ONE_DELETION;
        static double TRANSPOSITION = 0.04 + ONE_REPLACEMENT;
        static double TWO_INSERTION = 0.03  + TRANSPOSITION;
        static double TWO_DELETION = 0.03  + TWO_INSERTION;
        static double TWO_REPLACEMENT = 0.1  + TWO_DELETION;
        static string ALLKEYBOARDCHARACTER = "`1234567890-=qwertyuiop[]\\asdfghjkl;\'zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";
        /// <summary>
        /// Converts a YahooTuple (freq, index) to a password (string)
        /// </summary>
        /// <param name="frequency">the frequency of the password</param>
        /// <param name="index">an integer i \leq count ranging over the number of passwords of given frequency</param>
        /// <returns>a unique password for the tuple for sketch count experiments</returns>
        public static string YahooTupleToPassword(int frequency, int index)
        {
            return frequency + ":" + index;
        }


        public static RNGCryptoServiceProvider cryptoRand = new RNGCryptoServiceProvider();



        /// <summary>
        /// Binary search for the best password to attempt, such password maximize usage of psi.
        /// </summary>
        /// <param name="passwordCostGain">Gain Cost Dicitonary</param>
        /// <param name="avoidThisPassword">avoid a password  to pick</param>
        /// <returns>the index of password for appromximately optimal guessing<returns>



        public static int getBestPasswordToTry(double psiBudget, List<Tuple<String,double,double>> passwordCostGain, string avoidThisPassword)
        {

            for (int i = 0; i < passwordCostGain.Count; i++) {
                
                if (passwordCostGain[i].Item2 < psiBudget && !passwordCostGain[i].Item1.Equals(avoidThisPassword)) return i;
            }
            return -1;
        }

        public static Queue<int> getKBestPasswordToTry(int k, double psiBudget, List<Tuple<String, double, double,double> > passwordCostGain, string avoidThisPassword)
        {

            Queue<int> returnValue = new Queue<int>();
            List<int> passwordToTry = new List<int>();
            if (psiBudget > 10.0) {
                for (int i = 0; i < passwordCostGain.Count && returnValue.Count < k; i++) {
                    returnValue.Enqueue(0);
                }
                return returnValue;
            }


            double remainingPsiBudget = psiBudget;
            Boolean added = true;
            while (passwordToTry.Count < k && added == true && remainingPsiBudget > 0) {
                added = false;
                double totalProfit = 0.0;
                double totalWeight = 0.0;
                List<int> itemsAddThisTime = new List<int>();
                for (int i = 0; i < passwordCostGain.Count; i++)
                {
                    /* If the ith password is not selected */
                    if (!passwordCostGain[i].Item1.Equals(avoidThisPassword) && passwordToTry.Contains(i) == false)
                    {
                        /* Add this one is not a problem */
                        if (passwordCostGain[i].Item2 + totalWeight < remainingPsiBudget)

                        {
                            itemsAddThisTime.Add(i);
                            totalProfit += passwordCostGain[i].Item4;
                            totalWeight += passwordCostGain[i].Item2;
                        }
                        /* Add this one is a problem (assume addedable) */
                        else if (passwordCostGain[i].Item2 < remainingPsiBudget)
                        {
                            /* Comapre the current package with new one can't fit in, choose the better one into bag of K*/
                            added = true;
                            if (totalProfit > passwordCostGain[i].Item4)
                            {
                                foreach (int itemIndex in itemsAddThisTime)
                                {
                                    passwordToTry.Add(itemIndex);
                                }
                                remainingPsiBudget -= totalWeight;
                            }
                            else
                            {
                                passwordToTry.Add(i);
                                remainingPsiBudget -= passwordCostGain[i].Item2;
                            }
                            itemsAddThisTime.Clear();
                        }
                    }
                    /* If added something, break from the for loop*/
                    if (added) break;
                }
                foreach (int i in itemsAddThisTime) {
                    passwordToTry.Add(i);
                    added = true;
                }

            }
            //Console.WriteLine(passwordToTry.Count);


            if (passwordToTry.Count > k) {
                passwordToTry.OrderByDescending(i =>passwordCostGain[i].Item4).ToList();
                passwordToTry.RemoveRange(k, passwordToTry.Count - k);
            }
            passwordToTry.Sort();
            int offset = 0;
            //Console.WriteLine("----------------");
            foreach (int originalIndex in passwordToTry) {
                //Console.WriteLine(originalIndex);
                returnValue.Enqueue(originalIndex - offset);
                offset++;
            }
            return returnValue;
        }


        public static Queue<int> getKBestPasswordToTryTraditional(int k, double psiBudget, List<Tuple<String, double, double, double>> passwordCostGain, string avoidThisPassword)
        {

            Queue<int> returnValue = new Queue<int>();
            List<int> passwordToTry = new List<int>();
            if (psiBudget > 10.0)
            {
                for (int i = 0; i < passwordCostGain.Count && returnValue.Count < k; i++)
                {
                    returnValue.Enqueue(0);
                }
                return returnValue;
            }


            double remainingPsiBudget = psiBudget;
            for (int i = 0; i < passwordCostGain.Count && passwordToTry.Count < k; i++)
            {
                if (passwordCostGain[i].Item2 < remainingPsiBudget && !passwordCostGain[i].Item1.Equals(avoidThisPassword))
                {
                    passwordToTry.Add(i);
                    remainingPsiBudget -= passwordCostGain[i].Item2;
                }
            }
            int offset = 0;
            //Console.WriteLine("----------------");
            foreach (int originalIndex in passwordToTry)
            {
                //Console.WriteLine(originalIndex);
                returnValue.Enqueue(originalIndex - offset);
                offset++;
            }
            return returnValue;
        }


        public static int[,] fillFrequencyArrayYahoo(string file, out int total)
        {
            total = 0;
            List<Tuple<int, int>> frequencyList = new List<Tuple<int, int>>();
            StreamReader sr = new StreamReader(file);
            string line = "";
            line = sr.ReadLine();
            while (line != null)
            {
                Tuple<int,int> T = ParseYahooLineHelper(line);
                frequencyList.Add(T);
                line = sr.ReadLine();
            }
            int[,] frequencyArray = new int[frequencyList.Count, 2];
            int i = 0;
            foreach(Tuple<int,int> T in frequencyList)
            {
                frequencyArray[i, 0] = T.Item1;
                frequencyArray[i, 1] = T.Item2;
                total += T.Item1 * T.Item2;
                i++;
            }
            return frequencyArray;
        }
        /// <summary>
        /// Samples noise from the laplace distribution with parameter b = sensitivity*k/epsilon
        /// to achieve epsilon group differential privacy with group size k. Assumes global sensitivity of the function is 1.0
        /// 
        /// </summary>
        /// <param name="epsilon">privacy parameter, smaller epsilon = stronger privacy, set epsilon = double.PositiveInfinity for no noise</param>
        /// <param name="k">size of group privacy parameter (typically depth of count-sketch)</param>
        /// <returns>double (random noise sampled from Laplace Distribution)</returns>
        public static double LaplaceNoise(double epsilon, double k, double sensitivity = 1.0)
        {
            if (double.IsPositiveInfinity(epsilon)) return 0.0;
            double b = sensitivity * k * 1.0 / epsilon;

            double u = r.NextDouble() - 0.5;
            double X = -b * Math.Log(1.0 - 2.0 * Math.Abs( u));
            if (u < 0.0) X = -X;
            return X;
        }

        /// <summary>
        /// Construct Typo Dictionary Based on Mtruk Result. 
        /// 
        /// </summary>
        ///
        /// <returns>a tuple containing password pattern </returns>
        /// 
        public static Dictionary<string, List<string>> MturkDict()
        {
            Dictionary<string, List<string>> ret = new Dictionary<string, List<string>>();
            string currentKey = null;
            using (StreamReader r = new StreamReader("mturk15-general.json"))
            {
                while (true)
                {
                    string line = r.ReadLine();
                    //Console.WriteLine(line);
                    if (line == null) break;
                    if (line.Contains("rpw"))
                    {
                        string sub1 = line.Substring(12);
                        currentKey = sub1.Substring(0, sub1.Length - 3);
                    }
                    else if (line.Contains("\"tpw\":"))
                    {
                        string sub1 = line.Substring(12);
                        string sub2 = sub1.Substring(0, sub1.Length - 3);
                        if (!ret.ContainsKey(currentKey))
                        {
                            ret.Add(currentKey, new List<string>());
                        }
                        ret[currentKey].Add(sub2);
                    }
                }
            }
            return ret;
        }



        public static string TypePassword(String password, String passwordFromAnotherService) {

            double chanceOfChooseWrongPassword = r.NextDouble();
            if (chanceOfChooseWrongPassword <= CHANCE_OF_WRONG_PASSWORD) {
                return TypePasswordBasedOnAmazonMturkTypoDistribution(passwordFromAnotherService);
            }
            return TypePasswordBasedOnAmazonMturkTypoDistribution(password);

        }



        /// <summary>
        /// Construct Typo Dictionary Based on Mtruk Result. 
        /// 
        /// </summary>
        /// <param name="password">string encoding a line read from RockYou--withCount  </param>
        /// <returns>a tuple containing password pattern </returns>
        ///
        public static string TypePasswordBasedOnAmazonMturkTypoDistribution(String password)
        {

            /*
             * 
             *         static double SHIFT_FIRST_CHAR = 0.04  + CAP_LOCK;
                static double ONE_INSERTION = 0.12  + SHIFT_FIRST_CHAR;
                    static double ONE_DELETION = 0.12  + ONE_INSERTION;
        static double ONE_REPLACEMENT = 0.31  + ONE_DELETION;
        static double TRANSPOSITION = 0.04 + ONE_REPLACEMENT;
        static double TWO_INSERTION = 0.03  + TRANSPOSITION;
        static double TWO_DELETION = 0.03  + TWO_INSERTION;
        static double TWO_REPLACEMENT = 0.1  + TWO_DELETION;
        */
            double chanceOfTypo = r.NextDouble() ;

            if (chanceOfTypo > CHANCE_OF_TYPO)
            {
                return password;
            }

            double typoType = r.NextDouble();
            //Console.Out.WriteLine("Should have a typo here" + typoType.ToString());
            if (typoType <= CAP_LOCK)
            {
                //Console.Out.WriteLine("CAPLock");
                return new string(
                    password.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ?
                    char.ToLower(c) : char.ToUpper(c)) : c).ToArray());
            }

            else if (typoType <= SHIFT_FIRST_CHAR)
            {
                //Console.Out.WriteLine("SHIFTCHAR");
                if (char.IsLetter(password[0]))
                {
                    if (char.IsUpper(password[0])) return char.ToLower(password[0]) + password.Substring(1);
                    return char.ToUpper(password[0]) + password.Substring(1);
                }
                return password;
            }
            else if (typoType <= ONE_INSERTION)
            {
               // Console.Out.WriteLine("INSERT");
                return insertRandomChar(password);
            }
            else if (typoType <= ONE_DELETION)
            {
               // Console.Out.WriteLine("1 DELETE");
                return deleteRandomChar(password);
            }

            else if (typoType <= ONE_REPLACEMENT)
            {
               // Console.Out.WriteLine("1 REPLACE");
                return replaceRandomChar(password, -1).Item1;

            }
            else if (typoType <= TRANSPOSITION)
            {
               // Console.Out.WriteLine("TRANS");
                int randomIndex1 = r.Next(0, password.Length);
                int randomIndex2 = randomIndex1;
                while (randomIndex2 == randomIndex1)
                {
                    randomIndex2 = r.Next(0, password.Length);
                }
                int smallerIndex = Math.Min(randomIndex1, randomIndex2);
                int largerIndex = Math.Max(randomIndex1, randomIndex2);
                string firstPart = "";
                string secondPart = "";
                string thirdPart = "";
                if (smallerIndex != 0)
                {
                    firstPart = password.Substring(0, smallerIndex);
                }
                if (largerIndex != password.Length - 1)
                {

                    secondPart = password.Substring(smallerIndex + 1, largerIndex - smallerIndex - 1);
                    thirdPart = password.Substring(largerIndex + 1, password.Length - largerIndex - 1);
                }
                return firstPart + password[largerIndex] + secondPart + password[smallerIndex] + thirdPart;
            }
            else if (typoType <= TWO_INSERTION)
            {
              //  Console.Out.WriteLine("2 INS");
                return insertRandomChar(insertRandomChar(password));
            }
            else if (typoType <= TWO_DELETION)
            {
             //   Console.Out.WriteLine("2 DELE");
                return deleteRandomChar(deleteRandomChar(password));
            }
            else if (typoType <= TWO_REPLACEMENT)
            {
              //  Console.Out.WriteLine("2 REPLACE");
                Tuple<String, int> resultOfFirstReplacement = replaceRandomChar(password, -1);
                return replaceRandomChar(resultOfFirstReplacement.Item1, resultOfFirstReplacement.Item2).Item1;
            }
            else
            {
                /* Make up to two edit of inset, delete, replace */
            String passwordAfterFirstEdit = "";
                int firstRandomOperation = r.Next(0, 3);
                if (firstRandomOperation == 0) {
                    /* insertion */
                    passwordAfterFirstEdit = insertRandomChar(password);
                }
                else if (firstRandomOperation == 1)
                {
                    /* insertion */
                    passwordAfterFirstEdit = deleteRandomChar(password);
                }
                else
                {
                    /* insertion */
                    passwordAfterFirstEdit = replaceRandomChar(password,-1).Item1;
                }

                int secondRandomOperation = r.Next(0, 3);
                while (secondRandomOperation == firstRandomOperation) {
                    secondRandomOperation = r.Next(0, 3);
                }

                if (secondRandomOperation == 0)
                {
                    /* insertion */
                    return  insertRandomChar(passwordAfterFirstEdit);
                }
                else if (secondRandomOperation == 1)
                {
                    /* insertion */
                    return deleteRandomChar(passwordAfterFirstEdit);
                }
                else
                {
                    /* insertion */
                    return replaceRandomChar(passwordAfterFirstEdit, -1).Item1;
                }

            }

        }

        public static String insertRandomChar(string password) {


            int randomIndex = r.Next(0, password.Length + 1);
            char randomChar = ALLKEYBOARDCHARACTER[r.Next(0, ALLKEYBOARDCHARACTER.Length - 1)];
            if (randomIndex == password.Length)
                return password + randomChar;
            else if (randomIndex == 0)
                return randomChar + password;

            return password.Substring(0, randomIndex) + randomChar + password.Substring(randomIndex);
        }

        public static String deleteRandomChar(string password)
        {
            if (password.Length <= 1)
                return password;
            int randomIndex = r.Next(0, password.Length);
            if (randomIndex == password.Length - 1)
                return password.Substring(0, password.Length - 1);
            else if (randomIndex == 0)
                return password.Substring(1);

            return password.Substring(0, randomIndex) + password.Substring(randomIndex + 1);
        }



        public static Tuple<String,int> replaceRandomChar(string password, int alreadyReplaceIndex)
        {

            int randomIndex = alreadyReplaceIndex;

            while (randomIndex == alreadyReplaceIndex) {
                randomIndex = r.Next(0,password.Length);
            }

            char randomChar = ALLKEYBOARDCHARACTER[r.Next(0, ALLKEYBOARDCHARACTER.Length - 1)];
            if (randomIndex == password.Length)
                return new Tuple<String, int>(password.Substring(0, randomIndex) + randomChar,randomChar);
            else if (randomIndex == 0)
                return new Tuple<String, int>(randomChar + password.Substring(1, randomIndex),randomChar);

            return new Tuple<String,int>(password.Substring(0, randomIndex) + randomChar + password.Substring(randomIndex + 1), randomIndex);
        }
        /// <summary>
        /// Parses a line from the RockYou Input File. 
        /// Expected format: frequency password
        /// </summary>
        /// <param name="line">string encoding a line read from RockYou--withCount  </param>
        /// <returns>a tuple containing the frequency (int) and the password (string) </returns>
        /// 
        public static Tuple<int, string> ParseRockYouLine(string line)
        {
            string trimedLine = line.TrimStart();
            int firstSpaceIndex = trimedLine.IndexOf('\t');
            string password = trimedLine.Substring(firstSpaceIndex+1,trimedLine.Length-firstSpaceIndex-1);
            int frequency;
            if (Int32.TryParse(trimedLine.Substring(0,firstSpaceIndex), out frequency))
            {
                return new Tuple<int, string>(frequency, password);
            }
            else
            {
                Console.WriteLine("error parsing" + line);
                return null;
            }
        }

        public static Tuple<double, string> ParsePGSLine(string line)
        {
            string trimedLine = line.TrimStart();
            int firstSpaceIndex = trimedLine.IndexOf('\t');
            string password = trimedLine.Substring(0, firstSpaceIndex);
            double frequency;
            if (Double.TryParse(trimedLine.Substring(firstSpaceIndex + 1, trimedLine.Length - firstSpaceIndex - 1), out frequency))
            {
                return new Tuple<double, string>(frequency, password);
            }
            else
            {
                Console.WriteLine("error parsing" + line);
                return null;
            }
        }
        /// <summary>
        /// Parses a line from the Yahoo! frequency corpus 
        /// Expected format: frequency #passwords
        ///   where #passwords is an integer denoting the number of distinct passwords which occur with the given frequency 
        ///   e.g., the line "3 10" would indicate that there were 10 distinct passwords each of which were chosen by three users
        /// </summary>
        /// <param name="line">string encoding a line read from Yahoo! frequency corpus  </param>
        /// <returns>an array tuple containing the frequency (int) and the placeholders for the passwords (string) </returns>
        public static Tuple<int, string>[] ParseYahooLine(string line)
        {
            string[] parts = line.TrimStart().Split();
            int numPasswords;
            int frequency;
            if (Int32.TryParse(parts[0], out frequency) && Int32.TryParse(parts[1], out numPasswords))
            {
                Tuple<int, string>[] A = new Tuple<int, string>[numPasswords];
                for (int i = 0; i < numPasswords; i++)
                {
                  A[i] = new Tuple<int, string>(frequency, frequency + ":" + i);
                }
                return A;
            }
            else
            {
                Console.WriteLine("error parsing" + line);
                return null;
            }
        }


        /// <summary>
        /// Generate Random password string of length 6 to 20
        /// </summary>
        /// <param name="line">string encoding a line read from Yahoo! frequency corpus  </param>
        /// <returns>an array tuple containing the frequency (int) and the placeholders for the passwords (string) </returns>
        public static string generateRandomPassword() {


            int passwordLength = r.Next(14) + 6;
            string password = "";
            for (int i = 0; i <= passwordLength; i++)
            {

                password = password + ALLKEYBOARDCHARACTER[r.Next((ALLKEYBOARDCHARACTER.Length))];
            }
            return password;

        }



        /// <summary>
        /// Parses a line from the Yahoo Input File. 
        /// Expected format: frequency number of passwords
        /// </summary>
        /// <param name="line">string encoding a line read from yahoo-all  </param>
        /// <returns>an tuple </returns>
        public static Tuple<int, int> ParseYahooLineHelper(string line)
        {
            string[] parts = line.TrimStart().Split();
            int pwdn = Convert.ToInt32(parts[1]);
            int frequency = Convert.ToInt32(parts[0]);
            return new Tuple<int, int>(frequency, pwdn);
        }

        /// <summary>
        /// Filp charToBeFlip based on US Keyboard layout.
        /// e.g. 1 -> ! a -> A
        /// </summary>
        /// <param name="charToBeFlip">The char needs to be flipped  </param>

        public static char flipCharacter(char charToBeFlip) {
            if (char.IsLetter(charToBeFlip)){
                if (char.IsUpper(charToBeFlip)) return char.ToLower(charToBeFlip);
                return char.ToUpper(charToBeFlip);
            }
            switch (charToBeFlip){

                case '1':
                    return '!';
                case '2':
                    return '@';
                case '3':
                    return '#';
                case '4':
                    return '$';
                case '5':
                    return '%';
                case '6':
                    return '^';
                case '7':
                    return '&';
                case '8':
                    return '*';
                case '9':
                    return '(';
                case '0':
                    return ')';
                case '-':
                    return '_';
                case '=':
                    return '+';
                case '!':
                    return '1';
                case '@':
                    return '2';
                case '#':
                    return '3';
                case '$':
                    return '4';
                case '%':
                    return '5';
                case '^':
                    return '6';
                case '&':
                    return '7';
                case '*':
                    return '8';
                case '(':
                    return '9';
                case ')':
                    return '0';
                case '_':
                    return '-';
                case '+':
                    return '=';
                case '{':
                    return '[';
                case '[':
                    return '{';
                case '}':
                    return ']';
                case ']':
                    return '}';
                case '\\':
                    return '|';
                case '|':
                    return '\\';
                case ';':
                    return ':';
                case ':':
                    return ';';
                case '\'':
                    return '\"';
                case '\"':
                    return '\'';
                case '<':
                    return ',';
                case ',':
                    return '<';
                case '>':
                    return '.';
                case '.':
                    return '>';
                case '/':
                    return '?';
                case '?':
                    return '/';
                default:
                    return charToBeFlip;
            }
        }
    }
}
