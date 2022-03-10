using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Zxcvbn;

using KeePassLib.Cryptography;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace ConsoleApp5
{

    class User
    {
        public static RNGCryptoServiceProvider cryptoRand = new RNGCryptoServiceProvider();
        public const double TYPORATE = 2;
        public const double COMMON = 21.5;
        public const double MISSLASTSHIFT = 0.2;
        public const double SWITCHSYMBOL = 10.9 + MISSLASTSHIFT;
        public const double ADDENDLETTER = 4.6 + SWITCHSYMBOL;
        public const double SWITCHFISTCASE = 4.5 + ADDENDLETTER;
        public const double ADDFIRSTLETTER = 1.3 + SWITCHFISTCASE;
        public const double CAPTCHA_SOLVER_COST = 1.0;
        public string passwordFromAnotherService;
        public static int ONEDAY = 1;
        public string pwd;
        public double psi;
        int maxConsecutiveIncorrect;
        public Boolean isCompr;
        public Boolean isRegistered;
        Queue<int> incorrectLoginTimes;
        Queue<Tuple<int, double>> incorrectLoginRecords;

        double psiThreshold;

        public double maxKForCaptcha;
        public List<Tuple<int, List<String>>> timeLineList;

        public Tuple<int, double> getThrottlingBudgetForAdversary()
        {

            return new Tuple<int, double>(maxConsecutiveIncorrect - incorrectLoginRecords.Count , psiThreshold - psi);
        }

        public void setPasswordFromAnotherService(string passwordFromAnotherService)
        {
            this.passwordFromAnotherService = String.Copy(passwordFromAnotherService);
        }
        public void Register(string pas, double threshold, int maxIncorrect, Boolean Compr)
        {
            this.pwd = pas;
            psiThreshold = threshold;
            isCompr = false;
            isRegistered = true;
            maxConsecutiveIncorrect = maxIncorrect;
            incorrectLoginTimes = new Queue<int>();
            incorrectLoginRecords = new Queue<Tuple<int, double>>();
            timeLineList = new List<Tuple<int, List<String>>>();

        }





        public void setThreshold(double psiThreshold, int maxIncorrect, double maxKForCaptcha)
        {
            maxConsecutiveIncorrect = maxIncorrect;
            this.psiThreshold = psiThreshold;
            this.maxKForCaptcha = maxKForCaptcha;
            this.psi = 0;
            while (incorrectLoginRecords.Count() > 0)
            {
                incorrectLoginRecords.Dequeue();
            }
        }
        public double getHCT()
        {
            return psiThreshold;
        }
        public int getRC()
        {
            return incorrectLoginRecords.Count();
        }


        /// <summary>
        /// k-Strikes policy within last 24 hours
        /// </summary>
        /// <param name="passwordGuess">the current password that the user/attacker is checking</param>
        /// <param name="time">time of login attempt (change format e.g., minutes)</param>
        /// <param name="IsAttacker">boolean value used only for record keeping, indicates whether or not present login attempt is attacker/legit user</param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int login(string passwordGuess, int time, bool IsAttacker)
        {
            int OneDayAgo = time - ONEDAY;
            if (OneDayAgo < 0) { OneDayAgo = 0; }
            if (incorrectLoginRecords.Count > 0)
            {
                //Console.WriteLine( incorrectLoginTimes.Count);
                while (incorrectLoginTimes.Count > 0 && incorrectLoginTimes.Peek() > OneDayAgo) { incorrectLoginTimes.Dequeue(); }
            }
            if (incorrectLoginTimes.Count >= maxConsecutiveIncorrect) return -1;
            else if (passwordGuess == pwd)
            {
                incorrectLoginTimes = new Queue<int>();
                return 1;
            }
            /// We can create several different versions based on the way psi is updated
            incorrectLoginTimes.Enqueue(time);
            return 0;

        }
        /// <summary>
        /// "either or" k strikes/threshold policy within last 24 hours
        /// </summary>
        /// <param name="typedPassword">the current password that the user/attacker is checking</param>
        /// <param name="time">time of login attempt (change format e.g., minutes)</param>
        /// <param name="SC">boolean value used only for record keeping, indicates whether or not present login attempt is attacker/legit user</param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int login(string typedPassword, int time, SketchCount SC)
        {
            int OneDayAgo = time - ONEDAY;
            if (OneDayAgo < 0) { OneDayAgo = 0; }
            if (incorrectLoginRecords.Count > 0)
            {
                //Console.WriteLine(incorrectLoginRecords.Count);
                while (incorrectLoginRecords.Count > 0 && incorrectLoginRecords.Peek().Item1 > OneDayAgo)
                {
                    Tuple<int, double> dequeuedItem = incorrectLoginRecords.Dequeue();
                    psi -= dequeuedItem.Item2;
                }
            }
            if (incorrectLoginRecords.Count >= maxConsecutiveIncorrect || psi > psiThreshold) return -1;

            if (!String.Equals(typedPassword, pwd))
            {
                double thre = SC.EstimateMedian(typedPassword) / SC.getSize();
                Tuple<int, double> newAttempt = new Tuple<int, double>(time, thre);


                incorrectLoginRecords.Enqueue(newAttempt);

                psi += thre;

                return 0;
            }
            psi = 0;
            while (incorrectLoginRecords.Count() > 0)
            {
                incorrectLoginRecords.Dequeue();
            }
            return 1;
        }

        /// <summary>
        /// "either or" k strikes/threshold policy within last 24 hours
        /// </summary>
        /// <param name="typedPassword">the current password that the user/attacker is checking</param>
        /// <param name="time">time of login attempt (change format e.g., minutes)</param>
        /// <param name="SC">boolean value used only for record keeping, indicates whether or not present login attempt is attacker/legit user</param>
        /// <returns>Tuple<int, Boolean>:
        ///             first value: 1 on succeess, 0 on incorrect password, -1 on accountLocked
        ///             second value: true with captcha, false without captcah 
        /// </returns>
        public Tuple<int, Boolean> loginNeverResetPsi(string typedPassword, int time, SketchCount SC)
        {
            //int OneDayAgo = time - ONEDAY;
            //if (OneDayAgo < 0) { OneDayAgo = 0; }
            /*
            if (incorrectLoginRecords.Count > 0)
            {
                //Console.WriteLine(incorrectLoginRecords.Count);
                while (incorrectLoginRecords.Count > 0 && incorrectLoginRecords.Peek().Item1 > OneDayAgo)
                {
                    Tuple<int, double> dequeuedItem = incorrectLoginRecords.Dequeue();
                }
            } */

            /* Note for captcha: Here we assume the person who login can always solve captcah, therefore it always return a tuple*/
            Boolean requireCaptcha = maxKForCaptcha < 0;

            if (incorrectLoginRecords.Count >= maxConsecutiveIncorrect || this.psi >= psiThreshold) {
                //if (this.psi >= psiThreshold) Console.WriteLine("Locked psi["+ (psiThreshold - this.psi) + "], k [" + (maxConsecutiveIncorrect - incorrectLoginRecords.Count));
                return new Tuple<int, Boolean>(-1, requireCaptcha);
            }

            if (!String.Equals(typedPassword, pwd))
            {
                //Console.WriteLine("     Wrong pwd attempted[" + typedPassword+"] , k:"+ (maxConsecutiveIncorrect - incorrectLoginRecords.Count));
             
                double thre = SC.EstimatePopularityByMedian(typedPassword);
                Tuple<int, double> newAttempt = new Tuple<int, double>(time, thre);
                maxKForCaptcha -= 1;
                incorrectLoginRecords.Enqueue(newAttempt);
                psi += thre;
                return new Tuple<int, Boolean>(0, requireCaptcha);
            }
            while (incorrectLoginRecords.Count() > 0)
            {
                incorrectLoginRecords.Dequeue();
            }
            return new Tuple<int, Boolean>(1, requireCaptcha);
        }


        public Tuple<int, Boolean> loginNeverResetPsi(string typedPassword, int time,Dictionary<string,double> pgsEst, double aggregatedSum, double maxInc)
        {
            //int OneDayAgo = time - ONEDAY;
            //if (OneDayAgo < 0) { OneDayAgo = 0; }
            /*
            if (incorrectLoginRecords.Count > 0)
            {
                //Console.WriteLine(incorrectLoginRecords.Count);
                while (incorrectLoginRecords.Count > 0 && incorrectLoginRecords.Peek().Item1 > OneDayAgo)
                {
                    Tuple<int, double> dequeuedItem = incorrectLoginRecords.Dequeue();
                }
            } */

            /* Note for captcha: Here we assume the person who login can always solve captcah, therefore it always return a tuple*/
            Boolean requireCaptcha = maxKForCaptcha < 0;

            if (incorrectLoginRecords.Count >= maxConsecutiveIncorrect || this.psi >= psiThreshold)
            {
                //if (this.psi >= psiThreshold) Console.WriteLine("Locked psi["+ (psiThreshold - this.psi) + "], k [" + (maxConsecutiveIncorrect - incorrectLoginRecords.Count));
                return new Tuple<int, Boolean>(-1, requireCaptcha);
            }

            if (!String.Equals(typedPassword, pwd))
            {
                //Console.WriteLine("     Wrong pwd attempted[" + typedPassword+"] , k:"+ (maxConsecutiveIncorrect - incorrectLoginRecords.Count));

                var result = maxInc;
                if (pgsEst.ContainsKey(typedPassword) && pgsEst[typedPassword] > 0)
                    result = pgsEst[typedPassword];

                double thre = 1.0 / (result + 1.0);
                thre /= aggregatedSum;
                Tuple<int, double> newAttempt = new Tuple<int, double>(time, thre);
                maxKForCaptcha -= 1;
                incorrectLoginRecords.Enqueue(newAttempt);
                psi += thre;
                return new Tuple<int, Boolean>(0, requireCaptcha);
            }
            while (incorrectLoginRecords.Count() > 0)
            {
                incorrectLoginRecords.Dequeue();
            }
            return new Tuple<int, Boolean>(1, requireCaptcha);
        }


        public Tuple<int, Boolean> loginNeverResetPsi(string typedPassword, int time, Zxcvbn.Zxcvbn zx, double aggregatedSum)
        {
            //int OneDayAgo = time - ONEDAY;
            //if (OneDayAgo < 0) { OneDayAgo = 0; }
            /*
            if (incorrectLoginRecords.Count > 0)
            {
                //Console.WriteLine(incorrectLoginRecords.Count);
                while (incorrectLoginRecords.Count > 0 && incorrectLoginRecords.Peek().Item1 > OneDayAgo)
                {
                    Tuple<int, double> dequeuedItem = incorrectLoginRecords.Dequeue();
                }
            } */

            /* Note for captcha: Here we assume the person who login can always solve captcah, therefore it always return a tuple*/
            Boolean requireCaptcha = maxKForCaptcha < 0;

            if (incorrectLoginRecords.Count >= maxConsecutiveIncorrect || this.psi >= psiThreshold)
            {
                //if (this.psi >= psiThreshold) Console.WriteLine("Locked psi["+ (psiThreshold - this.psi) + "], k [" + (maxConsecutiveIncorrect - incorrectLoginRecords.Count));
                return new Tuple<int, Boolean>(-1, requireCaptcha);
            }

            if (!String.Equals(typedPassword, pwd))
            {
                //Console.WriteLine("     Wrong pwd attempted[" + typedPassword+"] , k:"+ (maxConsecutiveIncorrect - incorrectLoginRecords.Count));
                var result = zx.EvaluatePassword(typedPassword);
                double thre = 1.0 / (result.Entropy + 1.0);
                thre /= aggregatedSum;
                Tuple<int, double> newAttempt = new Tuple<int, double>(time, thre);
                maxKForCaptcha -= 1;
                incorrectLoginRecords.Enqueue(newAttempt);
                psi += thre;
                return new Tuple<int, Boolean>(0, requireCaptcha);
            }
            while (incorrectLoginRecords.Count() > 0)
            {
                incorrectLoginRecords.Dequeue();
            }
            return new Tuple<int, Boolean>(1, requireCaptcha);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordGuess"></param>
        /// <param name="SC"></param>
        /// <param name="attacker"></param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int login(string passwordGuess, SketchCount SC, bool attacker)
        {
            if (psi > psiThreshold) return -1;
            else if (passwordGuess == pwd)
            {
                return 1;
            }
            /// We can create several different versions based on the way psi is updated
            psi += SC.EstimateMedian(passwordGuess) / SC.getSize();
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordGuess"></param>
        /// <param name="SC"></param>
        /// <param name="attacker"></param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        /*public int login(string passwordGuess, ref Zxcvbn.Zxcvbn zx, bool attacker)
        {
            if (psi > psiThreshold) return -1;
            else if (passwordGuess == pwd)
            {
                return 1;
            }
            var result = zx.EvaluatePassword(passwordGuess);
            double freqEst = Math.Pow(2.0,-result.Entropy);
            /// We can create several different versions based on the way psi is updated
            psi += freqEst;
            return 0;

        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordGuess"></param>
        /// <param name="SC"></param>
        /// <param name="attacker"></param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int loginKeePass(string passwordGuess, bool attacker)
        {
            if (psi > psiThreshold) return -1;
            else if (passwordGuess == pwd)
            {
                return 1;
            }
            var result = QualityEstimation.EstimatePasswordBits(passwordGuess.ToCharArray());
            double freqEst = Math.Pow(2.0, -result);
            /// We can create several different versions based on the way psi is updated
            psi += freqEst;
            return 0;

        }

        public void changepwd(string newPassword)
        {
            pwd = newPassword;
            psi = 0.0;
            incorrectLoginTimes = new Queue<int>();
        }


    }
}
