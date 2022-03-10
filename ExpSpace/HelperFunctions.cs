using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class HelperFunctions
    {
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


        public static int[,] fillFrequencyArrayYahoo(string file, out int total)
        {
            total = 0;
            List<Tuple<int, int>> frequencyList = new List<Tuple<int, int>>();
            StreamReader sr = new StreamReader(file);
            string line = "";
            line = sr.ReadLine();
            while (line != null)
            {
                Tuple<int,int> T =              ParseYahooLineHelper(line);
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
            if (epsilon == double.PositiveInfinity) return 0.0;
            double b = sensitivity * k * 1.0 / epsilon;
            byte[] temp = new byte[4];
            cryptoRand.GetBytes(temp);
            int seed = BitConverter.ToInt32(temp, 0);
            Random r = new Random(seed);
            double u = r.NextDouble() - 0.5;
            double X = -b * Math.Log(1.0 - 2.0 * Math.Abs( u));
            if (u < 0.0) X = -X;
            return X;
        }

        /// <summary>
        /// Parses a line from the RockYou Input File. 
        /// Expected format: frequency password
        /// </summary>
        /// <param name="line">string encoding a line read from RockYou--withCount  </param>
        /// <returns>a tuple containing the frequency (int) and the password (string) </returns>
        public static Tuple<int, string> ParseRockYouLine(string line)
        {
            string[] parts = line.TrimStart().Split();
            string password = parts[1];
            int frequency;
            if (Int32.TryParse(parts[0], out frequency))
            {
                return new Tuple<int, string>(frequency, password);
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

    }
}
