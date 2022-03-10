using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    public class BinomialLadder
    {
        public static uint BinHeight;
        public static uint n;
        //public uint[] BitArray;
        public static byte[] BitArray;
        public static byte[] Visited;
        public static uint t;
        public static Random rnd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="N"></param>
        /// <param name="h"></param>
        /// <param name="threshold"></param>
        /// <param name="seed"></param>
        public BinomialLadder(uint N, uint h, uint threshold, int seed)
        {
            BinHeight = h;
            t = threshold;
            n = N;
            BitArray = new byte[N];
            Visited = new byte[N];
            for (int i = 0; i < (N >> 1); i++)
            {
                BitArray[i] = 1; Visited[i] = 0;
            }
            for (uint i = (N >> 1); i < N; i++) { BitArray[i] = 0; Visited[i] = 0; }
            rnd = new Random(seed);
            BitArray = BitArray.OrderBy(x => rnd.Next()).ToArray();
            //public double[] r, d;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public void IncreaseHeight(string x)
        {
            uint theHeight = 0;
            uint[] hashes = new uint[BinHeight];

            for (int i = 0; i < BinHeight; i++)
            {
                string xi = "i=" + i + ":" + x;
                hashes[i] = (uint)(xi.GetHashCode()) % n;
                int j = 0;
                while (Visited[hashes[i]] == 1)
                {
                    j++;
                    xi = "Attempt=" + j + ":i=" + i + ":" + x;
                    hashes[i] = (uint)(xi.GetHashCode()) % n;
                }
                Visited[hashes[i]] = 1;
                theHeight += BitArray[hashes[i]];
            }

            if (theHeight < BinHeight)
            {
                // Pick random  rungs to flip from 0 to 1
                int flipto1 = rnd.Next() % (int)theHeight;
                for (int i = 0; i < BinHeight; i++)
                {
                    if (BitArray[hashes[i]] == 1 && flipto1 == 0)
                    {
                        BitArray[hashes[i]] = 1;
                    }
                    flipto1 -= BitArray[hashes[i]];
                }
                // Select non-rung, to flip from  1 to 0 to maintain equality
                int flipTo0 = rnd.Next() % (int)n;
                while (Visited[flipTo0] == 1 || BitArray[flipTo0] == 0) flipTo0 = rnd.Next() % (int)n;
                BitArray[flipTo0] = 0;

            }
            for (int i = 0; i < BinHeight; i++)
            {
                Visited[hashes[i]] = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static uint GetHeight(string x)
        {
            uint theHeight = 0;
            uint[] hashes = new uint[BinHeight];

            for (int i = 0; i < BinHeight; i++)
            {
                string xi = "i=" + i + ":" + x;
                hashes[i] = (uint)(xi.GetHashCode()) % n;
                int j = 0;
                while (Visited[hashes[i]] == 1)
                {
                    j++;
                    xi = "Attempt=" + j + ":i=" + i + ":" + x;
                    hashes[i] = (uint)(xi.GetHashCode()) % n;
                }
                Visited[hashes[i]] = 1;
                theHeight += BitArray[hashes[i]];
            }
            for (int i = 0; i < BinHeight; i++)
            {
                Visited[hashes[i]] = 0;
            }
            return theHeight;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsHeightOverThreshold(string x)
        {
            return t <= GetHeight(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double BinLadStep()
        {

            return BinHeight;
        }

        /// <summary>
        /// Add summary, say something about expected file format
        /// </summary>
        /// <param name="file"></param>
        public void FillBinLadder(string file= "C:\\pwd\\rockyoutop500.txt")
        {
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(file);

                //Read the first line of text
                line = sr.ReadLine();
                Console.WriteLine(line);
                // char[] splitchar = { ' ' };
                BinomialLadder y = new BinomialLadder(100, 48, 48, 1);
                char[] sep = { ' ' };
                //Continue to read until you reach end of file
                while (line != null)
                {
                    line = line.TrimStart(sep);
                    int splitPoint = line.IndexOf(' ');
                    int num = Convert.ToInt32(line.Substring(0, splitPoint).Trim());
                    string password = line.Substring(splitPoint + 1);
                    //string[] result = line.Trim().Split(sep);
                    //write the lie to console window

                    // IMPORTANT: Need to randomize the order in which elements are added to BinomialLadder 
                    //int num = Convert.ToInt32(result[0]);
                    for (int i = 0; i < num; i++)
                        y.IncreaseHeight(password);

                    Console.WriteLine(num);
                    Console.WriteLine(line);
                    //Console.WriteLine(result[1]);
                    //Read the next line
                    line = sr.ReadLine();
                    //Console.WriteLine(line);
                    //result = null;
                }
                Console.WriteLine("end");
                Console.ReadLine();
                //close the file
                sr.Close();
                Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Binomial Ladder: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        /*
        public void SaveBinLadder()
        {  //writing to file
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("C:\\pwd\\outputBinLadder.txt");
            string output = "";
            for (int i = 0; i < arr.GetUpperBound(0); i++)
            {
                for (int j = 0; j < arr.GetUpperBound(1); j++)
                {
                    output += arr[i, j].ToString();
                }
                streamWriter.WriteLine(output);
                output = "";
            }
            streamWriter.Close();
        } */
    }

}
