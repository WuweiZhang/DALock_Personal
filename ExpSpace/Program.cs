using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

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

        /// <summary>
        /// Generates an array of loginAttempts sorted by login time for numUsers absent of attack
        /// </summary>
        /// <param name="numUsers">Number of Users</param>
        /// <returns>array of login attempts sorted by login times</returns>
        public Tuple<loginAttempt[], User[]> GenerateLoginAttemptsHonest(int numUsers, int numActive, int numMedium, int num, double typoRateMin, double typoRateMax, int maxTime)
        {
            /// Todo: Sample 5-10 passwords for each user from RockYou or equivalent dataset
            /// Poison Parameters for each user
            Random rnd = new Random();
            Random rnd1 = new Random();

            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("rockyou-withcount.txt");

                //Read the first line of text
                line = sr.ReadLine();
                int maxFrequency = 0;
                List<Tuple<int, string>> rockyoudata = new List<Tuple<int, string>>();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the lie to console window
                    //Console.WriteLine(line);
                    //Read the next line
                    line = sr.ReadLine();
                    Tuple<int, string> tuple;

                    tuple = HelperFunctions.ParseRockYouLine(line);
                    maxFrequency = maxFrequency + tuple.Item1;
                    rockyoudata.Add(tuple);
                }
                int r = rnd.Next(maxFrequency);
                int s = 0;
                int rockyouindex = 0;
                while (s < r)
                {
                    rockyouindex = rockyouindex + 1;
                    s = s + rockyoudata[rockyouindex].Item1;
                }

                //close the file
                sr.Close();


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
            List<int> frequencylist = new List<int> { 1, 3, 7, 14, 30, 180 };
            for (int i = 0; i < numUsers; i++)
            {
                int r = rnd.Next(frequencylist.Count);
                User u = new User();
                u.loginfrequency = r;

            }
            int j = 0;
            while (j < maxTime)
            {

            }

            return null;
        }

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

        static void Main()
        {
            DWExperiment();
        }

        static void DWExperiment()
        {
            int totalPasswords;
            string[] pwd500 = { "bubbles", "12345", "elisha", "100591", "hiphop", "paula", "love123", "greenwood", "barbie", "nicole06", "131313", "1234567", "carly", "dancer", "walker1", "shadow", "stefan", "michelle", "ilovedanny", "remember", "bellamy", "jimjamjoop", "yesenia", "secret", "Brandon", "hotness2", "cristina", "123456y", "danielle", "popcorn", "chandler", "iloveu", "angelbaby", "654321", "nipper", "princesa", "zxcvbn", "samantha1", "cheng", "skinny", "portugal", "miamor", "pasaway", "666666", "lucky13", "stupid", "nirvana", "chocolate", "yvonne1", "adelaide", "lashay", "eatme69", "alejandra", "rainbow", "genius", "celtic", "belle", "ashleigh", "scooter", "monica", "sayangku", "alexis", "viewsonic", "1234567", "babes1", "carolina1", "erskine", "princess", "argentina", "smelly", "manunited", "people", "redstar", "slipknot", "madrid", "******", "12345", "fuckyou2", "liverpool", "rockyou", "swordfish", "yankees1", "paris", "qwerty", "Scooby", "password", "preciosa", "jordan", "brooklyn", "samsung", "sexyme", "696969", "russell", "viridiana", "angely", "buddy", "12345", "hiphop", "ranger", "654321", "123456789", "babygirl", "mylove", "hockey", "football", "hailey", "sophie", "michael", "delacruz", "party", "forever", "ILOVEYOU", "password", "rose16", "marion", "colombia", "mullet", "sheila", "12351235", "212121", "aaaaaa", "ashley", "fatima", "cheyenne1", "mariposa", "princess", "patricio", "tanner", "sammy", "12345678", "newpassword", "rodrigo1", "mountain", "canada", "donkey1", "bernadette", "nicki", "12345", "abercrombie", "zidane", "spooky", "mustang", "pelusa", "manman5", "jackson", "indigo", "password", "smiles", "francheska", "cheer", "26468675", "stephany", "santiago", "lourdes", "castillo", "jesus", "maggie", "1234567", "simpsons", "malago", "alfredo", "connor", "ikhouvanje", "102010", "friendster", "password2", "12345", "boricua", "scooter", "cherry1", "jackson", "minniemouse", "letmein", "katty", "camila", "999999999", "brandon", "lynnette", "password", "raiders10", "nicky", "dominique", "python", "12345", "eminem", "serena", "eminem", "rockstar", "hello", "eminem", "fernando", "family", "love15", "1234567890", "victor1", "livethelife", "fuckme69", "mom123", "sexy123", "password", "sassy21", "patches1", "warrior", "destiny", "12345", "fuckyou!", "casper", "angelica", "lorena", "imcool1", "cowboys", "654321", "jesus", "1fuckyou", "caroline", "mathew", "ashley1", "violeta", "forever", "princess", "christopher", "morgan", "12345", "hawaiian", "cassie", "rachel", "bleach", "PASSWORD", "cristi", "pembroke", "mahalkita", "kelly", "santos", "harmony", "anghel", "nintendo", "beckham", "kevin", "lorenz", "mariposa", "12345", "armani", "maggie", "annette", "hawaii", "china", "82468246", "rocker", "kenny1", "12345", "12345", "peewee", "princesa", "yolanda", "samantha", "rockyou", "chasity", "redhot", "emanuel", "123456789", "princess", "twiztid", "123456789", "iloveyou", "12345", "aninha", "sister", "rocks69", "lovemyson", "BRANDON", "sandra", "lawrence", "annmarie", "a12345", "vanessa", "ayleen", "monkey", "danica", "purple", "butterfly", "asshole", "janeth", "bently", "marvel", "shadow", "angel1995", "010304", "orlando", "mypics", "ximena", "shay15", "coffee", "stephanie", "123456789", "dominoes", "jonathan", "hooker", "love12", "yankees", "soccer11", "tinkerbell", "brandy", "emotional", "dddddd", "dj4ever", "popcorn", "1234567", "heart", "roxana", "billy", "040288", "carpediem", "natasha", "rose09", "mandy", "phelps", "12345", "metal", "iloveyou23", "shadow", "young1", "playboy", "theking", "lampard", "giovanni", "angelina", "annette", "159357", "ilovehim", "alexander", "daniel", "winter", "789456123", "florida1", "family", "hottie", "jesus", "shabba", "wishful", "mahal", "harley", "789456123", "fuckme1", "susana", "12345", "pretty", "januari", "herjunot", "superstar", "eeyore", "johncena", "fuckyou", "miamor", "david", "cuteako", "juan01", "danielle", "green", "valley", "4getmenot", "poohbear", "abcdefg", "suckmydick", "mother2", "ariana", "fofinha", "cute", "wrestling", "pookie1", "celtic", "bailey", "angels", "oranges", "12345", "123456789", "playgirl", "leticia", "michelle", "timothy", "energy", "angel", "21101989", "mother", "december24", "marian", "lovers", "joel12", "fatass", "princess2", "tigger1", "valentina", "gerard", "MONKEY", "angel13", "super", "12345", "elizabeth1", "cheese!", "12345", "espiritu", "sissy1", "pierre", "misterio", "murray", "mexican1", "butterfly", "hotmama", "159753", "dakota", "short5", "mydear", "password", "leigh", "candy1", "loveme1", "megan", "yankee", "dallas", "pisces", "powerball", "iloveyou", "vincent", "lovemom", "alyssa", "jackass", "lorena", "january", "pandora", "birdman", "badgirl", "annabelle", "celtic", "angel03", "palemon4", "madison", "red123", "kevin", "gatita", "monkey", "bitch03", "rebecca1", "harley", "pogiako", "manutd", "frumusica", "ashleigh", "butterfly", "chelsea", "alexandre", "12345", "starwars", "angels", "samantha", "147258369", "bobesponja", "fudge_tigger", "pinklady", "donkey", "princess", "guadalupe", "tweety", "mississippi", "cherry", "tekiero", "123456789", "angelica", "junior1", "celtic", "qwerty", "skyline", "bebito", "password", "traveon03", "angel", "lovely", "gemini", "fuckme69", "pinguino", "010289", "tweety", "slideshow", "falloutboy", "karina", "pinky", "lollol", "colombia", "jayjay", "titans", "turtle", "jaguares", "paulina", "12345", "david1", "soccer", "preciosa", "bautista", "anthony", "katie", "flowers", "player12" };
            int[,] mat = new int[SketchCount.width, SketchCount.depth];
            double[] eps = {4.0, 6.0 };
            uint[] tableSize = {2000000 };///, 5 * 20000, 100 * 20000
            float[] lambdas = { 12, 24, 24 * 3, 24 * 7, 24 * 14, 24 * 30, 24 * 60, 24 * 120, 24 * 30 * 6, 24 * 30 * 12 };

            Random random = new Random();
            DataTable dt = new DataTable();
            SketchCount SC = new SketchCount(10000, 50, 0.1);
            int[,] frequencies = HelperFunctions.fillFrequencyArrayYahoo("yahoo-all.txt", out totalPasswords);
            Benchmark.DWTradeOffExp(frequencies, totalPasswords, tableSize, eps, 5);
        }
        static void AllTest()
        {
            int totalPasswords;
            string[] pwd500 = { "bubbles", "12345", "elisha", "100591", "hiphop", "paula", "love123", "greenwood", "barbie", "nicole06", "131313", "1234567", "carly", "dancer", "walker1", "shadow", "stefan", "michelle", "ilovedanny", "remember", "bellamy", "jimjamjoop", "yesenia", "secret", "Brandon", "hotness2", "cristina", "123456y", "danielle", "popcorn", "chandler", "iloveu", "angelbaby", "654321", "nipper", "princesa", "zxcvbn", "samantha1", "cheng", "skinny", "portugal", "miamor", "pasaway", "666666", "lucky13", "stupid", "nirvana", "chocolate", "yvonne1", "adelaide", "lashay", "eatme69", "alejandra", "rainbow", "genius", "celtic", "belle", "ashleigh", "scooter", "monica", "sayangku", "alexis", "viewsonic", "1234567", "babes1", "carolina1", "erskine", "princess", "argentina", "smelly", "manunited", "people", "redstar", "slipknot", "madrid", "******", "12345", "fuckyou2", "liverpool", "rockyou", "swordfish", "yankees1", "paris", "qwerty", "Scooby", "password", "preciosa", "jordan", "brooklyn", "samsung", "sexyme", "696969", "russell", "viridiana", "angely", "buddy", "12345", "hiphop", "ranger", "654321", "123456789", "babygirl", "mylove", "hockey", "football", "hailey", "sophie", "michael", "delacruz", "party", "forever", "ILOVEYOU", "password", "rose16", "marion", "colombia", "mullet", "sheila", "12351235", "212121", "aaaaaa", "ashley", "fatima", "cheyenne1", "mariposa", "princess", "patricio", "tanner", "sammy", "12345678", "newpassword", "rodrigo1", "mountain", "canada", "donkey1", "bernadette", "nicki", "12345", "abercrombie", "zidane", "spooky", "mustang", "pelusa", "manman5", "jackson", "indigo", "password", "smiles", "francheska", "cheer", "26468675", "stephany", "santiago", "lourdes", "castillo", "jesus", "maggie", "1234567", "simpsons", "malago", "alfredo", "connor", "ikhouvanje", "102010", "friendster", "password2", "12345", "boricua", "scooter", "cherry1", "jackson", "minniemouse", "letmein", "katty", "camila", "999999999", "brandon", "lynnette", "password", "raiders10", "nicky", "dominique", "python", "12345", "eminem", "serena", "eminem", "rockstar", "hello", "eminem", "fernando", "family", "love15", "1234567890", "victor1", "livethelife", "fuckme69", "mom123", "sexy123", "password", "sassy21", "patches1", "warrior", "destiny", "12345", "fuckyou!", "casper", "angelica", "lorena", "imcool1", "cowboys", "654321", "jesus", "1fuckyou", "caroline", "mathew", "ashley1", "violeta", "forever", "princess", "christopher", "morgan", "12345", "hawaiian", "cassie", "rachel", "bleach", "PASSWORD", "cristi", "pembroke", "mahalkita", "kelly", "santos", "harmony", "anghel", "nintendo", "beckham", "kevin", "lorenz", "mariposa", "12345", "armani", "maggie", "annette", "hawaii", "china", "82468246", "rocker", "kenny1", "12345", "12345", "peewee", "princesa", "yolanda", "samantha", "rockyou", "chasity", "redhot", "emanuel", "123456789", "princess", "twiztid", "123456789", "iloveyou", "12345", "aninha", "sister", "rocks69", "lovemyson", "BRANDON", "sandra", "lawrence", "annmarie", "a12345", "vanessa", "ayleen", "monkey", "danica", "purple", "butterfly", "asshole", "janeth", "bently", "marvel", "shadow", "angel1995", "010304", "orlando", "mypics", "ximena", "shay15", "coffee", "stephanie", "123456789", "dominoes", "jonathan", "hooker", "love12", "yankees", "soccer11", "tinkerbell", "brandy", "emotional", "dddddd", "dj4ever", "popcorn", "1234567", "heart", "roxana", "billy", "040288", "carpediem", "natasha", "rose09", "mandy", "phelps", "12345", "metal", "iloveyou23", "shadow", "young1", "playboy", "theking", "lampard", "giovanni", "angelina", "annette", "159357", "ilovehim", "alexander", "daniel", "winter", "789456123", "florida1", "family", "hottie", "jesus", "shabba", "wishful", "mahal", "harley", "789456123", "fuckme1", "susana", "12345", "pretty", "januari", "herjunot", "superstar", "eeyore", "johncena", "fuckyou", "miamor", "david", "cuteako", "juan01", "danielle", "green", "valley", "4getmenot", "poohbear", "abcdefg", "suckmydick", "mother2", "ariana", "fofinha", "cute", "wrestling", "pookie1", "celtic", "bailey", "angels", "oranges", "12345", "123456789", "playgirl", "leticia", "michelle", "timothy", "energy", "angel", "21101989", "mother", "december24", "marian", "lovers", "joel12", "fatass", "princess2", "tigger1", "valentina", "gerard", "MONKEY", "angel13", "super", "12345", "elizabeth1", "cheese!", "12345", "espiritu", "sissy1", "pierre", "misterio", "murray", "mexican1", "butterfly", "hotmama", "159753", "dakota", "short5", "mydear", "password", "leigh", "candy1", "loveme1", "megan", "yankee", "dallas", "pisces", "powerball", "iloveyou", "vincent", "lovemom", "alyssa", "jackass", "lorena", "january", "pandora", "birdman", "badgirl", "annabelle", "celtic", "angel03", "palemon4", "madison", "red123", "kevin", "gatita", "monkey", "bitch03", "rebecca1", "harley", "pogiako", "manutd", "frumusica", "ashleigh", "butterfly", "chelsea", "alexandre", "12345", "starwars", "angels", "samantha", "147258369", "bobesponja", "fudge_tigger", "pinklady", "donkey", "princess", "guadalupe", "tweety", "mississippi", "cherry", "tekiero", "123456789", "angelica", "junior1", "celtic", "qwerty", "skyline", "bebito", "password", "traveon03", "angel", "lovely", "gemini", "fuckme69", "pinguino", "010289", "tweety", "slideshow", "falloutboy", "karina", "pinky", "lollol", "colombia", "jayjay", "titans", "turtle", "jaguares", "paulina", "12345", "david1", "soccer", "preciosa", "bautista", "anthony", "katie", "flowers", "player12" };
            int[,] mat = new int[SketchCount.width, SketchCount.depth];
            bool Y = false;
            double[] eps = { 2.0, 4.0, 6.0 };
            uint[] tableSize = { 100 * 100, 5 * 20000, 100 * 20000 };
            float[] lambdas = { 12, 24, 24 * 3, 24 * 7, 24 * 14, 24 * 30, 24 * 60, 24 * 120, 24 * 30 * 6, 24 * 30 * 12 };
            int attackerFreq = 24;
            int attackerNumAttempts = 2;
            uint w = 10000;
            uint wmax = 20000;
            uint wmin = 100;
            uint dmax = 100;
            uint dmin = 5;
            string str;
            string[] strArray;
            int NumUsers = 50; //number of users. default = 500
            int OverallTime = 24 * 30 * 12 * 5;//24 hours in a day * 30 day in a month * 12 month in a year * 5 years of login hours

            Random random = new Random();
            DataTable dt = new DataTable();
            SketchCount SC = new SketchCount(10000, 50, 0.1);
            int[,] frequencies = HelperFunctions.fillFrequencyArrayYahoo("yahoo-all.txt", out totalPasswords);


            Benchmark.DWTradeOffExp(frequencies, totalPasswords, tableSize, eps, 5);
            Console.WriteLine("Done with gridsearch");


            double GeneratePoisson(float rate)
            {
                Random rnd = new System.Random();
                float res = ((float)rnd.Next(100) / 101.0f);

                var a = -Math.Log(1.0f - res) / rate;

                return a;
            }
            //Here I am creating an array with rockyou passwords that an attacker will use for his login attempts
            StreamReader sr = new StreamReader("rockyou-withcount.txt");

            //Read the first line of text
            String line;
            line = sr.ReadLine();
            int maxFrequency = 0;
            List<Tuple<int, string>> rockyoudata = new List<Tuple<int, string>>();
            //Continue to read until you reach end of file
            while (rockyoudata.Count < 100000)
            {
                //write the lie to console window
                //Console.WriteLine(line);
                //Read the next line
                line = sr.ReadLine();
                Tuple<int, string> tuple;

                tuple = HelperFunctions.ParseRockYouLine(line);
                maxFrequency = maxFrequency + tuple.Item1;
                rockyoudata.Add(tuple);
            }
            Console.WriteLine("Wait till SketchCount is populated...");
            SC.FillSketchCount("rockyou-withcount.txt", false);
            Console.WriteLine("sketch count is ready, saving it to file...");
            SC.SaveSketchCount("outputSketchCount.txt");
            Console.WriteLine("uploading it back...");
            SketchCount sc1 = new SketchCount(10000, 50, 0.1);
            SC.loadSketchCount("outputSketchCount.txt");
            // sc1.SaveSketchCount("C:\\pwd\\outputSketchCount1.txt");

            Console.WriteLine("done");
            List<loginAttempt> lattempts = new List<loginAttempt>();
            List<User> users = new List<User>();

            Console.WriteLine("end!!!");
            // for loop for the number of users
            for (int u = 0; u < NumUsers; u++)
            {
                User user = new User();
                user.Register(pwd500[u + 5 * u], 100, 3, false);
                int curtime = 0;
                int lastUserTime = 0;
                int lastAttackerTime = 0;
                int lambdaindex = random.Next(0, lambdas.Length);

                float rate = lambdas[lambdaindex];
                List<Tuple<int, char>> attackerArrivals = new List<Tuple<int, char>>();
                List<Tuple<int, char>> honestArrivals = new List<Tuple<int, char>>();
                List<Tuple<int, char>> allArrivals = new List<Tuple<int, char>>();
                //generating lambda per user for poisson arrivals
                //generate list of tuples of honest user arrivings
                int userTimes = 0;
                double a = 0;
                while (userTimes < OverallTime)
                {
                    a = GeneratePoisson(1 / rate);
                    userTimes = userTimes + Convert.ToInt32(Math.Round(a));
                    Tuple<int, char> t = new Tuple<int, char>(userTimes, 'H');

                    // Console.WriteLine(a);
                    // Console.WriteLine(rate);
                    // Console.WriteLine(t);
                    honestArrivals.Add(t);
                }
                Console.WriteLine("done generating honest arrivals");
                // Console.WriteLine(honestArrivals);
                //generate list of tuples of attacker arrivings
                int attTimes = 0;
                while (attTimes < OverallTime)
                {
                    Tuple<int, char> t = new Tuple<int, char>(attTimes, 'A');
                    attackerArrivals.Add(t);
                    attTimes = attTimes + Convert.ToInt32(attackerFreq / attackerNumAttempts);
                    // Console.WriteLine(t);
                }
                // Console.WriteLine(attackerArrivals);
                //concat two lists
                allArrivals = honestArrivals.Concat(attackerArrivals).ToList();
                allArrivals.Sort();
                for (int i3 = 0; i3 < allArrivals.Count; i3++)
                { Console.WriteLine(allArrivals[i3]); }
                int i4 = 0;
                while (curtime < OverallTime)
                {
                    //take arrival from mingled list
                    //login as attacker or honest user depending on credentials
                    if (allArrivals[i4].Item2.Equals("H"))
                    {
                        user.login(pwd500[u + 5 * u], allArrivals[i4].Item1, false);
                        user.login(pwd500[u + 5 * u], ref SC, false);
                    }
                    else
                    {
                        user.login(rockyoudata[u].Item2, allArrivals[i4].Item1, true);
                        user.login(rockyoudata[u].Item2, ref SC, true);
                    }
                    i4 = i4 + 1;
                    curtime++;
                }
                users.Add(user);
            }
            Console.ReadLine();
            //Console.ReadLine();


            // BinomialLadder b = new BinomialLadder(100, 48, 48, 1);
            //b.FillBinLadder();

        }
    }
}

