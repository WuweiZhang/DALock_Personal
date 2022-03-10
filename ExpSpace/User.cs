using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Zxcvbn;

using KeePassLib.Cryptography;
namespace ConsoleApp5
{
   
    class User
    {
        
        public static int ONEDAY = 1;
        public string pwd;
        public double hitcount;
        int maxConsecutiveIncorrect;
        public Boolean isCompr;
        public Boolean isRegistered;
        Queue<int> incorrectLoginTimes;
        public string[] favoritePasswords;
        public double loginfrequency;

        double hitCountThreshold;

        public void Register(string pas, double threshold, int maxIncorrect, Boolean Compr)
        {
            this.pwd = pas;
            hitCountThreshold = threshold;
            isCompr = false;
            isRegistered = true;
            maxConsecutiveIncorrect = maxIncorrect;
            incorrectLoginTimes = new Queue<int>();
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
            if ( OneDayAgo<0) { OneDayAgo = 0; }
            if (incorrectLoginTimes.Count > 0 )
            {
                Console.WriteLine( incorrectLoginTimes.Count);
                while (incorrectLoginTimes.Count >0 && incorrectLoginTimes.Peek() > OneDayAgo) { incorrectLoginTimes.Dequeue(); }
            }
            if (incorrectLoginTimes.Count >= maxConsecutiveIncorrect) return -1;
            else if (passwordGuess == pwd)
            {
                incorrectLoginTimes = new Queue<int>();
                return 1;
            }
            /// We can create several different versions based on the way hitcount is updated
            incorrectLoginTimes.Enqueue(time);
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordGuess"></param>
        /// <param name="SC"></param>
        /// <param name="attacker"></param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int login(string passwordGuess, ref SketchCount SC, bool attacker)
        {
            if (hitcount > hitCountThreshold) return -1;
            else if (passwordGuess == pwd)
            {
                return 1;
            }
            /// We can create several different versions based on the way hitcount is updated
            hitcount += SC.EstimateMedian(passwordGuess) / SC.getSize();
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordGuess"></param>
        /// <param name="SC"></param>
        /// <param name="attacker"></param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int login(string passwordGuess, ref Zxcvbn.Zxcvbn zx, bool attacker)
        {
            if (hitcount > hitCountThreshold) return -1;
            else if (passwordGuess == pwd)
            {
                return 1;
            }
            var result = zx.EvaluatePassword(passwordGuess);
            double freqEst = Math.Pow(2.0,-result.Entropy);
            /// We can create several different versions based on the way hitcount is updated
            hitcount += freqEst;
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordGuess"></param>
        /// <param name="SC"></param>
        /// <param name="attacker"></param>
        /// <returns>1 on succeess, 0 on incorrect password, -1 on accountLocked</returns>
        public int loginKeePass(string passwordGuess,  bool attacker)
        {
            if (hitcount > hitCountThreshold) return -1;
            else if (passwordGuess == pwd)
            {
                return 1;
            }
            var result = QualityEstimation.EstimatePasswordBits(passwordGuess.ToCharArray());
            double freqEst = Math.Pow(2.0, -result);
            /// We can create several different versions based on the way hitcount is updated
            hitcount += freqEst;
            return 0;

        }

        public  void changepwd(string newPassword)
        {
            pwd = newPassword;
            hitcount = 0.0;
            incorrectLoginTimes = new Queue<int>();
        }
    }
}
