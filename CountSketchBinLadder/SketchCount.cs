using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp5
{



    /// <summary>
    /// 
    /// </summary>
    public class SketchCount
    {
        public static System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
        public static bool debugFlag = false;



        public static double[,] sketchCount;

        /// <summary>
        ///       Useful for faster experiments, allows us to alter privacy level by re-generating noise
        /// </summary>
        public static double[,] laplaceNoise;
        public static UInt32 width = 100;
        public static UInt32 depth = 100;
        public static double size = 0;
        public static double InitialSize = 0;

        /// <summary>
        /// privacy parameter, smaller epsesilon = stronger privacy, set epsilon = double.PositiveInfinity for no noise (no differential privacy guarantee)
        /// </summary>
        public static double epsilon;
        public UInt32 getWidth() { return width; }
        public UInt32 getDepth() { return depth; }
        public double getSize() { return size; }
        public  double getEpsilon() { return epsilon;  }

        

        /// <summary>
        /// Constructs an empty sketch count with given parameters
        /// </summary>
        /// <param name="widthOfSketch">width paramter</param>
        /// <param name="depthOfSketch">depth parameter</param>
        /// <param name="eps">privacy parameter</param>
        public SketchCount(UInt32 widthOfSketch, UInt32 depthOfSketch, double eps)
        {
           
            width = widthOfSketch;
            depth = depthOfSketch;
            sketchCount = new double[depth, width];
            laplaceNoise = new double[depth, width];
            size = 0;
            if (eps > 0)
            {
                size = Math.Abs(HelperFunctions.LaplaceNoise(eps, 1));
            }
            InitialSize = size;
            epsilon = eps;
            if (epsilon > 0)
            {
                for (int i = 0; i < depth; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        
                        laplaceNoise[i,j]= HelperFunctions.LaplaceNoise(epsilon, depth);
                        sketchCount[i, j] = laplaceNoise[i, j];
                    }
                }
            }
        }

        /// <summary>
        /// Reading SketchCount from a file
        /// </summary>
        /// <param name="file">path of the file</param>
        public void loadSketchCount(string file)
        {
            StreamReader sr = new StreamReader(file);
            width = Convert.ToUInt32(sr.ReadLine());
            depth = Convert.ToUInt32(sr.ReadLine());
            sketchCount = new double[depth, width];
           // SketchCount p = new SketchCount(width, depth,0);
            String line;
            int j = 2;
            while ((line = sr.ReadLine()) != null)
            {
               // Console.WriteLine(line);
               // Console.WriteLine(j);
                line.TrimEnd(',');
                List<string> ia = line.Split(',').ToList<string>();
                List<double> countsketchlist = new List<double>();
                for (int i1=0; i1 < ia.Capacity-1 ; i1++)
                {
                    countsketchlist.Add(Convert.ToDouble(ia[i1]));
                }
                for (int i = 0; i < width-1; i++)
                {
                    sketchCount[j-2, i] = countsketchlist[i];
                }
                j++;
            }
           // Console.WriteLine("created from file");
          //  Console.WriteLine(sketchCount);
        }

        /// <summary>
        /// Subtracts out old noise and refreshes noise with fresh laplace noise according to new privacy parameter
        /// </summary>
        /// <param name="eps">privacy paramter</param>
        public void ChangePrivacyLevel(double eps)
        {
            if (epsilon > 0)
            {
                size -= InitialSize;
                size += Math.Abs(HelperFunctions.LaplaceNoise(eps, 1));
                for (int i = 0; i < depth; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        sketchCount[i, j] -= laplaceNoise[i,j];
                        laplaceNoise[i, j] = HelperFunctions.LaplaceNoise(epsilon, depth);
                    }
                }
            }
        }

        /// <summary>
        /// Wrapper for SHA256 hash function
        /// </summary>
        /// <param name="randomString">input string to be hashed</param>
        /// <returns>SHA256 hash of input</returns>
        static UInt32 m_Sha256(string randomString)
        {

            
            //var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            UInt32 cr1 = BitConverter.ToUInt32(crypto, 0);
            //cr1 = cr1 % M;
            //foreach (byte theByte in crypto)
            //{
            //    hash.Append(theByte.ToString("x2"));
            //}
            return cr1;
        }



        /// <summary>
        /// Estimates the frequency of item x in the count sketch using the Mean estimation rule
        /// </summary>
        /// <param name="x">item x</param>
        /// <returns>estimated frequency of x (median rule)</returns>
        public double EstimateMedian(String x)
        {
            UInt32 h1 = 0;
            UInt32 val = 0;
            int p = -1;
            h1 = (UInt32)x.GetHashCode(); //m_Sha256(x);
            val = h1 % 2;
            if (val == 1) { p = 1; };
            double[] frequencies = new double[depth];
            for (int i = 0; i < depth; i++)
            {
                String temp = "The index is " + i + ":" + x;
                h1 = (UInt32)temp.GetHashCode();
                h1 = h1 % width;
                //sketchCount[i, h1] += p * n;
                // UInt32 col = m_Sha256(x) % width;
                //val = m_Sha256(x) % 2;
                //if (val == 1) { p = 1; };
                frequencies[i] = sketchCount[i, h1] * p;
               // total = total + sketchCount[i, h1] * p;
                // p = -1;
            }
            Array.Sort(frequencies);
            if ((frequencies.Length % 2) ==1)
            {
                return frequencies[(frequencies.Length-1)/2];
            }
            else
            {
                return 0.5 * frequencies[(frequencies.Length - 2) / 2] + 0.5 * frequencies[1 + (frequencies.Length - 2) / 2];
            }
 
        }

        public double EstimateMedianDebug(String x, StreamWriter sw)
        {
            UInt32 h1 = 0;
            UInt32 val = 0;
            int p = -1;
            h1 = (UInt32)x.GetHashCode(); //m_Sha256(x);
            val = h1 % 2;
            if (val == 1) { p = 1; };
            double[] frequencies = new double[depth];
            for (int i = 0; i < depth; i++)
            {
                String temp = "The index is " + i + ":" + x;
                h1 = (UInt32)temp.GetHashCode();
                h1 = h1 % width;
                //sketchCount[i, h1] += p * n;
                // UInt32 col = m_Sha256(x) % width;
                //val = m_Sha256(x) % 2;
                //if (val == 1) { p = 1; };
                frequencies[i] = sketchCount[i, h1] * p;
                // total = total + sketchCount[i, h1] * p;
                // p = -1;
            }
            Array.Sort(frequencies);
            string debug = "        -Evaluation[" + x + "] - ";
            foreach (double frequency in frequencies)
            {
                debug += frequency + ",";
            }
            sw.WriteLine(debug);
            Console.WriteLine(debug);
            if ((frequencies.Length % 2) == 1)
            {
                return frequencies[(frequencies.Length - 1) / 2];
            }
            else
            {
                return 0.5 * frequencies[(frequencies.Length - 2) / 2] + 0.5 * frequencies[1 + (frequencies.Length - 2) / 2];
            }

        }

        /// <summary>
        /// Estimates the frequency of item x in the count sketch using the Mean estimation rule
        /// </summary>
        /// <param name="x">item x</param>
        /// <returns>estimated frequency of x (median rule)</returns>
        public double EstimatePopularityByMedian(String x)
        {
// Console.WriteLine(EstimateMedian(x).ToString() + " " + size.ToString());
            return Math.Abs(EstimateMedian(x) / (size));

        }

        /// <summary>
        /// Estimates the frequency of item x in the count sketch using the Mean estimation rule
        /// </summary>
        /// <param name="x">item x</param>
        /// <returns>estimated frequency of x (mean rule)</returns>
        public double EstimateMean(String x)
        {
            UInt32 h1 = 0;
            double total = 0;
            UInt32 val = 0;
            int p = -1;
            h1 = (UInt32)x.GetHashCode(); //m_Sha256(x);
            val = h1 % 2;
            if (val == 1) { p = 1; };

            for (int i=0; i < depth; i++)
            {
                String temp = "The index is " + i + ":" + x;
                h1 = (UInt32)temp.GetHashCode();
                h1 = h1 % width;
                //sketchCount[i, h1] += p * n;
               // UInt32 col = m_Sha256(x) % width;
                //val = m_Sha256(x) % 2;
                //if (val == 1) { p = 1; };
                total = total + sketchCount[i, h1] *p;
               // p = -1;
            }
            return total/depth;
        }

        //public int[,] amn = new int[M, N];
        /// <summary>
        /// Increase the sketchCount value of x by n
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        public void AddSketchCount(String x, int n)
        {
            UInt32 h1 = 0;
            UInt32 val = 0;
            int p = -1;
            h1 = (UInt32) x.GetHashCode(); //m_Sha256(x);
            //h1 = h1 % width;
            val = h1 % 2;
            if (val == 1) { p = 1; };
            for (int i = 0; i < depth; i++)
            {
                String temp = "The index is " + i + ":" + x;
                //h1 = m_Sha256(temp);
                h1 = (UInt32)temp.GetHashCode();
                h1 = h1 % width;
                sketchCount[i, h1] += p * n;
            }
            p = -1;
            size += n;

        }

        //public int[,] amn = new int[M, N];
        /// <summary>
        /// Increase the sketchCount value of x by n
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        public void AddSketchMinCount(String x, int n)
        {
            UInt32 h1 = 0;
            UInt32 val = 0;
            int p = -1;
            h1 = (UInt32)x.GetHashCode(); //m_Sha256(x);
            //h1 = h1 % width;
            val = h1 % 2;
            if (val == 1) { p = 1; };
            for (int i = 0; i < depth; i++)
            {
                String temp = "The index is " + i + ":" + x;
                //h1 = m_Sha256(temp);
                h1 = (UInt32)temp.GetHashCode();
                h1 = h1 % width;
                sketchCount[i, h1] += p * n;
            }
            p = -1;
            size += n;

        }
        /// <summary>
        /// Assumes input file is formatted as follows
        ///   pwd1 f_1
        ///   pwd2 f_2
        ///    ...
        ///    
        ///  where f_i is the number of observations of pwd_i
        /// </summary>
        /// 


        public Dictionary<String,int> FillSketchCount(string file, bool isYahoo,double sampleProbability, Random rnd)
        {
            //Console.WriteLine("heere");

            String line;
            Dictionary<String, int> passwordMapping = new Dictionary<String, int>();
            try
            {

                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(file);
                if (isYahoo == false)
                {
                    //Read the first line of text
                    line = sr.ReadLine();
                    if (debugFlag) Console.WriteLine(line);
                    // char[] splitchar = { ' ' };
                    //SketchCount y = new SketchCount(100, 100, 0.25);
                    char[] sep = { ' ' };
                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        Tuple<int, string> parsedLine = HelperFunctions.ParseRockYouLine(line);
                        int frequency = parsedLine.Item1;

                        // Convert.ToInt32(line.Substring(0, splitPoint).Trim());
                        string password = parsedLine.Item2;
                        //string[] result = line.Trim().Split(sep);
                        //write the lie to console window

                        if (!passwordMapping.ContainsKey(password))
                        {
                            if (sampleProbability < 1.0)
                            {
                                int maxF = frequency;
                                for (int temp = 0; temp < maxF; temp++)
                                {
                                    if (rnd.NextDouble() > sampleProbability)
                                        frequency -= 1;
                                }
                            }
                            passwordMapping.Add(password, frequency);
                        }
                        else {
                            frequency = passwordMapping[password];
                        }

                        //int num = Convert.ToInt32(result[0]);
                        this.AddSketchCount(password, frequency);

                        if (debugFlag) Console.WriteLine(frequency);
                        if (debugFlag) Console.WriteLine(line);
                        if (debugFlag) Console.WriteLine(EstimateMean(password));
                        //Console.WriteLine(result[1]);
                        //Read the next line
                        line = sr.ReadLine();
                        //Console.WriteLine(line);
                        //result = null;
                    }
                    if (debugFlag)
                    {
                        Console.WriteLine("end");
                        Console.ReadLine();
                    }
                    //close the file
                    sr.Close();

                    if (debugFlag) Console.ReadLine();
                    return null;
                }
                else// this is yahoo dataset
                {
                    //String referenceRockYouFile = "rockyou-withcount.txt";
                    // Step 0: Map Pwd From RockYou to Yahoo/LinkedIn



                    StreamReader rockYouReader = new StreamReader("LinkedIn-withcount.txt");
                    Console.WriteLine("entering here");
                    Queue<String> rockYouPwds = new Queue<String>();
                    string rockYouLine =rockYouReader.ReadLine();
                    while (rockYouLine != null) {
                        //Console.WriteLine(rockYouLine);
                        Tuple<int, String> rockYouPasswordPopularityTupleArray = HelperFunctions.ParseRockYouLine(rockYouLine);
                        rockYouPwds.Enqueue(rockYouPasswordPopularityTupleArray.Item2);
                        rockYouLine = rockYouReader.ReadLine();
                    }


                    int dequeued = 0;
                    line = sr.ReadLine();
                    if (debugFlag) Console.WriteLine("fill sketch count");
                    // char[] splitchar = { ' ' };
                    //SketchCount y = new SketchCount(100, 100, 0.25);
                    char[] sep = { ' ' };
                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        Tuple<int, int> parsedPasswords = HelperFunctions.ParseYahooLineHelper(line);
                        int frequency = parsedPasswords.Item1;
                        int count = parsedPasswords.Item2;
                        for (int i=0; i < count; i++)
                        {
                            int thisFrequency = frequency;
                            string pwd = HelperFunctions.YahooTupleToPassword(thisFrequency, i);
                            if (rockYouPwds.Count > 0){
                                String oripwd = String.Copy(pwd);
                                pwd = rockYouPwds.Dequeue();
                                //Console.WriteLine(pwd);
                                while (passwordMapping.ContainsKey(pwd) && rockYouPwds.Count > 0)
                                    pwd = rockYouPwds.Dequeue();
                                if (!passwordMapping.ContainsKey(pwd))
                                {
                                    /* Real Distribution */
                                    passwordMapping.Add(pwd, thisFrequency);
                                    dequeued += 1;
                                }
                            }
                            if (sampleProbability < 1.0)
                            {
                                int maxF = thisFrequency;
                                for (int temp = 0; temp < maxF; temp++)
                                {
                                    if (rnd.NextDouble() > sampleProbability)
                                        thisFrequency -= 1;
                                }
                            }
                            this.AddSketchCount(pwd, thisFrequency);
                        }

                        //string[] result = line.Trim().Split(sep);
                        //write the lie to console window
                        line = sr.ReadLine();

                        //int num = Convert.ToInt32(result[0]);


           
                            //Console.WriteLine(result[1]);
                            //Read the next line
                       }
                         
                        //Console.WriteLine(line);
                        //result = null;
                    }
                    if (debugFlag)
                    {
                        Console.WriteLine("end");
                        Console.ReadLine();
                    }
                    //close the file
                    sr.Close();
                    if (debugFlag) Console.ReadLine();
                return passwordMapping; 
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                if (debugFlag) Console.WriteLine("Executing finally block.");
                
            }
            return passwordMapping;
        }

        /// <summary>
        /// Assumes input file is formatted as follows
        ///   pwd1 f_1
        ///   pwd2 f_2
        ///    ...
        ///    
        ///  where f_i is the number of observations of pwd_i
        /// </summary>
        public void FillSketchCountFromFrequencyList(int[,] frequencies)
        {
            for(int j=0; j < frequencies.GetLength(0);j++)
            {
                int frequency = frequencies[j, 0];
                int count = frequencies[j, 1];
                for (int i = 0; i < count; i++)
                {

                    string pwd = HelperFunctions.YahooTupleToPassword(frequency, i);

                    this.AddSketchCount(pwd, frequency);
                }
            }
        }



        /// <summary>
        /// save the current sketchCount to file
        /// </summary>
        public void SaveSketchCount(string file)
        {  //writing to file
            /// Would also need to save the size of the SketchCount
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(file);
            string output = "";
            //streamWriter.WriteLine(size);
            streamWriter.WriteLine(width);
            streamWriter.WriteLine(depth);
            for (int i = 0; i < sketchCount.GetUpperBound(0); i++)
            {
                for (int j = 0; j < sketchCount.GetUpperBound(1); j++)
                {
                    output += sketchCount[i, j].ToString()+",";
                }
              //  output = output + "\n";
                streamWriter.WriteLine(output);
                output = "";
            }
            streamWriter.Close();
        }
       /// <summary>
       /// TODO: Write this function and add summary description here
       /// Specify anticipated file format
       /// </summary>
       /// <param name="file"></param>
       
        
    }
}
