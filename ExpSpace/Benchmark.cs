using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Benchmark
    {
        /// <summary>
        /// TODO: Adhishree
        /// </summary>
        /// <param name="w"></param>
        /// <param name="depth"></param>
        /// <param name="epsilon"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static double score(uint w, uint depth, double epsilon, bool Y)
        {
            // Construct CountSketch with parameters w, depth, epsilon
            // Fill CountSketch with file filename
            // Read through file to obtain true frequences
            // obtain estimated frequencies from CountSketch and compute L1 error metrics (powers of 10)
            SketchCount t = new SketchCount(w, depth, epsilon);
            int total = 0;
            Console.WriteLine("width = " + t.getWidth() + "\n" + "depth=" + t.getDepth());
            Console.WriteLine("epsilon=" + t.getEpsilon());
            if (Y == true)
            {
                t.FillSketchCount("yahoo-all.txt", true);
                total = 70000000;
                Console.WriteLine("Yahoo dataset");

            }
            else
            {
                t.FillSketchCount("rockyou-withcount.txt", false);
                total = 32600000;
                Console.WriteLine("Rockyou dataset");
            }
            // t.SaveSketchCount();
            double errorMean = 0.0;
            double errorMedian = 0.0;
            int i = 0;
            int totalCount = 0;
            StreamReader reader;
            if (Y == true)
            {
                reader = new StreamReader("yahoo-all.txt");
            }
            else
            {
                reader = new StreamReader("rockyou-withcount.txt");
            }
            List<Tuple<int, string>> tuplelist = new List<Tuple<int, string>>();
            while (!reader.EndOfStream)
            {
                if (Y == true)
                {
                    string line = reader.ReadLine();
                    Tuple<int, int> parsedLine = HelperFunctions.ParseYahooLineHelper(line);
                    int frequency = parsedLine.Item1;
                    int count = parsedLine.Item2;
                    //string[] parts = line.TrimStart().Split();
                    //string password = parts[1];
                    for (int j = 0; j < count; j++)
                    {

                        i++;
                        string password = HelperFunctions.YahooTupleToPassword(frequency, j);
                        errorMean += Math.Abs(frequency - t.EstimateMean(password));
                        errorMedian += Math.Abs(frequency - t.EstimateMedian(password));
                        tuplelist.Add(Tuple.Create(frequency, password));
                        totalCount = totalCount + 1;
                        if (i == 1 || i == 10 || i == 100 || i == 1000 || i == 10000 || i == 100000)
                        {
                            Console.WriteLine("_________________ i=" + i + "____________");
                            Console.WriteLine("Total Count: " + totalCount);
                            Console.WriteLine("L1 Error (Mean): " + errorMean);
                            Console.WriteLine("L1 Error (Mean)/N: " + errorMean / total);
                            Console.WriteLine("L1 Error (Median): " + errorMedian);
                            Console.WriteLine("L1 Error (Median)/N: " + errorMedian / total);
                            //  Console.WriteLine("Score is " + Convert.ToString(Scoring(tuplelist, t)));
                            Console.WriteLine("Password is " + password);
                            Console.WriteLine("_________________________________");
                        }
                    }


                }
                else
                {
                    i++;
                    string line = reader.ReadLine();
                    Tuple<int, string> parsedLine = HelperFunctions.ParseRockYouLine(line);
                    //string[] parts = line.TrimStart().Split();
                    //string password = parts[1];
                    int frequency = parsedLine.Item1;
                    string password = parsedLine.Item2;
                    errorMean += Math.Abs(frequency - t.EstimateMean(password));
                    errorMedian += Math.Abs(frequency - t.EstimateMedian(password));
                    totalCount = totalCount + 1;



                    if (i == 1 || i == 10 || i == 100 || i == 1000 || i == 10000 || i == 100000)
                    {
                        Console.WriteLine("_________________ i=" + i + "____________");
                        Console.WriteLine("Total Count: " + totalCount);
                        Console.WriteLine("L1 Error (Mean): " + errorMean);
                        Console.WriteLine("L1 Error (Mean)/N: " + errorMean / total);
                        Console.WriteLine("L1 Error (Median): " + errorMedian);
                        Console.WriteLine("L1 Error (Median)/N: " + errorMedian / total);
                        Console.WriteLine("_________________________________");
                        // Console.WriteLine("Score is " + Convert.ToString(Scoring(tuplelist, t)));

                    }
                }
                //   Console.WriteLine("End of file");
            }
            Console.WriteLine("L1 Error (Mean) is " + errorMean / total);
            Console.WriteLine("L1 Error (Median) is " + errorMedian / total);
            reader.Close();
            return errorMean / total;
        }
        public static void DWTradeOffExp(int[,] frequencies, int totalPasswords, uint[] tableSize, double[] eps, int maxiter = 5)
        {
            foreach (uint t in tableSize)
            {
                Console.WriteLine("------Table Size ( " + Convert.ToString(t) + " ) ------");
                Console.WriteLine("Eps\t d\t w\t MeanS\t MedianS\t");
                foreach (double epsilon in eps)
                {

                    // Compute all possible d,w pairs. As They can ONLY Be Integers. Their can be at most O(sqrt(t)) pairs of them
                    List<Tuple<uint, uint>> dwlist = new List<Tuple<uint, uint>>();
                    for (uint w = 1; w <= t; w++)
                    {
                        if (t % w == 0)
                        {
                            Tuple<uint, uint> newDW = Tuple.Create(w, t / w);
                            dwlist.Add(newDW);
                        }
                    }
                    // Test the performance on all dw pairs for maxIter times
                    foreach (Tuple<uint, uint> dwTuple in dwlist)
                    {
                        uint d = dwTuple.Item1;
                        uint w = dwTuple.Item2;
                        List<Tuple<double, double>> ret;

                        ret = Benchmark.scoreL1Error(w, d, epsilon, frequencies, totalPasswords, maxiter);
                        foreach (Tuple<double, double> res in ret)
                        {
                            double meanS = res.Item1;
                            double medianS = res.Item2;
                            Console.WriteLine(Convert.ToString(epsilon) + "\t " + Convert.ToString(d) + "\t " + Convert.ToString(w) + "\t " + Convert.ToString(meanS) + "\t " + Convert.ToString(medianS) + "\t ");
                        }
                    }

                }
            }
        }
        public static void RunDPBenchmarkExperiment(int[,] frequencies, int totalPasswords, Tuple<uint, uint> dRange, Tuple<uint, uint> wRange, double[] eps, int iter = 5)
        {


            foreach (double epsilon in eps)
            {
                uint wmin = wRange.Item1; uint wmax = wRange.Item2;
                uint dmin = dRange.Item1; uint dmax = dRange.Item2;
                uint bestW = (wmin + wmax) / 2, bestD = (dmax + dmin) / 2;
                for (int i = 0; i < iter; i++)
                {

                    double s1, s2, s3, s4, s5, s;
                    uint w1 = wmin + 3 * (wmax - wmin) / 4;
                    uint d1 = dmin + 3 * (dmax - dmin) / 4;
                    uint w2 = wmin + 3 * (wmax - wmin) / 4;
                    uint d2 = dmin + 1 * (dmax - dmin) / 4;
                    uint w3 = wmin + 1 * (wmax - wmin) / 4;
                    uint d3 = dmin + 3 * (dmax - dmin) / 4;
                    uint w4 = wmin + 1 * (wmax - wmin) / 4;
                    uint d4 = dmin + 1 * (dmax - dmin) / 4;
                    uint d5 = (dmax + dmin) / 2;
                    uint w5 = (wmax + wmin) / 2;
                    Console.WriteLine("S1");



                    s1 = Benchmark.scoreL1ErrorMedian(w1, d1, epsilon, frequencies, totalPasswords);
                    Console.WriteLine("S1");

                    s2 = Benchmark.scoreL1ErrorMedian(w2, d2, epsilon, frequencies, totalPasswords);
                    s3 = Benchmark.scoreL1ErrorMedian(w3, d3, epsilon, frequencies, totalPasswords);
                    s4 = Benchmark.scoreL1ErrorMedian(w4, d4, epsilon, frequencies, totalPasswords);
                    s5 = Benchmark.scoreL1ErrorMedian(w5, d5, epsilon, frequencies, totalPasswords);
                    var list = new[] { s1, s2, s3, s4, s5 };
                    s = list.Min();
                    if (s == s1) { bestW = w1; bestD = d1; dmin = (dmax + dmin) / 2; wmin = (wmin + wmax) / 2; };
                    if (s == s2) { bestW = w2; bestD = d2; dmax = (dmax + dmin) / 2; wmin = (wmin + wmax) / 2; };
                    if (s == s3) { bestW = w3; bestD = d3; dmin = (dmax + dmin) / 2; wmax = (wmin + wmax) / 2; };
                    if (s == s4) { bestW = w4; bestD = d4; dmax = (dmax + dmin) / 2; wmax = (wmin + wmax) / 2; };
                    if (s == s5) { bestW = w5; bestD = d5; dmax = dmin + (dmax - dmin) * 3 / 4; dmin = dmin + (dmax - dmin) * 1 / 4; wmax = wmin + (wmax - wmin) * 3 / 4; wmin = wmin + (wmax - wmin) * 1 / 4; };
                    //if (d<10) { break; };
                    Console.WriteLine("Finished Iteration: " + i);
                }
                Console.WriteLine("for epsilon " + Convert.ToString(epsilon) + " best value of (d,w) is (" + bestD + ", " + bestW + ")");
            }
        }

        public static double score(SketchCount t, int[,] frequencies, int total)
        {
            double errorMean = 0.0;
            double errorMedian = 0.0;
            int i = 0;
            int totalCount = 0;

            List<Tuple<int, string>> tuplelist = new List<Tuple<int, string>>();
            for (int k = 0; k < frequencies.GetLength(0); k++)
            {
                int frequency = frequencies[k, 0];
                int count = frequencies[k, 1];
                for (int j = 0; j < count; j++)
                {

                    i++;
                    string password = HelperFunctions.YahooTupleToPassword(frequency, j);
                    errorMean += Math.Abs(frequency - t.EstimateMean(password));
                    errorMedian += Math.Abs(frequency - t.EstimateMedian(password));
                    tuplelist.Add(Tuple.Create(frequency, password));
                    totalCount = totalCount + 1;
                    if (i == 1 || i == 10 || i == 100 || i == 1000 || i == 10000 || i == 100000)
                    {
                        Console.WriteLine("_________________ i=" + i + "____________");
                        Console.WriteLine("Total Count: " + totalCount);
                        Console.WriteLine("L1 Error (Mean): " + errorMean);
                        Console.WriteLine("L1 Error (Mean)/N: " + errorMean / total);
                        Console.WriteLine("L1 Error (Median): " + errorMedian);
                        Console.WriteLine("L1 Error (Median)/N: " + errorMedian / total);
                        //  Console.WriteLine("Score is " + Convert.ToString(Scoring(tuplelist, t)));
                        Console.WriteLine("Password is " + password);
                        Console.WriteLine("_________________________________");
                    }
                }
            }

            Console.WriteLine("L1 Error (Mean) is " + errorMean / total);
            Console.WriteLine("L1 Error (Median) is " + errorMedian / total);

            return errorMean / total;
        }

        /// <summary>
        /// TODO: Adhishree
        /// </summary>
        /// <param name="w"></param>
        /// <param name="depth"></param>
        /// <param name="epsilon"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// 
        public static List<Tuple<double, double>> scoreL1Error(uint w, uint depth, double epsilon, int[,] frequencies, int total, int maxIter)
        {
            // Construct CountSketch with parameters w, depth, epsilon
            // Fill CountSketch with file filename
            // Read through file to obtain true frequences
            // obtain estimated frequencies from CountSketch and compute L1 error metrics (powers of 10)
            SketchCount t = new SketchCount(w, depth, epsilon);
            ///if (w > w + 1) { 
            ///}
            ///Console.WriteLine("new sketch" );
            t.FillSketchCountFromFrequencyList(frequencies);
            ///Console.WriteLine("Filled Done");
            List<Tuple<double, double>> ret = new List<Tuple<double, double>>();
            for (uint iter = 0; iter <= maxIter; iter += 1)
            {
                double errorMean = 0.0;
                double errorMedian = 0.0;
                int i = 0;
                int totalCount = 0;
                ///Console.WriteLine("New Iter");
                // List<Tuple<int, string>> tuplelist = new List<Tuple<int, string>>();

                for (int k = 0; k < frequencies.GetLength(0); k++)
                {
                    int frequency = frequencies[k, 0];
                    int count = frequencies[k, 1];
                    for (int j = 0; j < count; j++)
                    {

                        i++;
                        string password = HelperFunctions.YahooTupleToPassword(frequency, j);
                        errorMean += Math.Abs(frequency - t.EstimateMean(password));
                        errorMedian += Math.Abs(frequency - t.EstimateMedian(password));
                        // tuplelist.Add(Tuple.Create(frequency, password));
                        totalCount = totalCount + 1;
                    }
                }

                ///Console.WriteLine(errorMean / total);
                Tuple<double, double> res = Tuple.Create(errorMean / total, errorMedian / total);
                ret.Add(res);
                if (iter + 1 <= maxIter) { t.ChangePrivacyLevel(epsilon); }

            }
            return ret;
        }

        public static double scoreL1ErrorMedian(uint w, uint depth, double epsilon, int[,] frequencies, int total)
        {
            // Construct CountSketch with parameters w, depth, epsilon
            // Fill CountSketch with file filename
            // Read through file to obtain true frequences
            // obtain estimated frequencies from CountSketch and compute L1 error metrics (powers of 10)
            SketchCount t = new SketchCount(w, depth, epsilon);
            ///if (w > w + 1) { 
            Console.WriteLine("width = " + t.getWidth() + "\n" + "depth=" + t.getDepth());
            Console.WriteLine("epsilon=" + t.getEpsilon());
            ///}

            t.FillSketchCountFromFrequencyList(frequencies);

            // t.SaveSketchCount();
            double errorMean = 0.0;
            double errorMedian = 0.0;
            int i = 0;
            int totalCount = 0;

            // List<Tuple<int, string>> tuplelist = new List<Tuple<int, string>>();
            for (int k = 0; k < frequencies.GetLength(0); k++)
            {
                int frequency = frequencies[k, 0];
                int count = frequencies[k, 1];
                for (int j = 0; j < count; j++)
                {

                    i++;
                    string password = HelperFunctions.YahooTupleToPassword(frequency, j);
                    errorMean += Math.Abs(frequency - t.EstimateMean(password));
                    errorMedian += Math.Abs(frequency - t.EstimateMedian(password));
                    // tuplelist.Add(Tuple.Create(frequency, password));
                    totalCount = totalCount + 1;
                    if (i == 0 && i == 1 || i == 10 || i == 100 || i == 1000 || i == 10000 || i == 100000)
                    {
                        Console.WriteLine("_________________ i=" + i + "____________");
                        Console.WriteLine("Total Count: " + totalCount);
                        Console.WriteLine("L1 Error (Mean): " + errorMean);
                        Console.WriteLine("L1 Error (Mean)/N: " + errorMean / total);
                        Console.WriteLine("L1 Error (Median): " + errorMedian);
                        Console.WriteLine("L1 Error (Median)/N: " + errorMedian / total);
                        //  Console.WriteLine("Score is " + Convert.ToString(Scoring(tuplelist, t)));
                        Console.WriteLine("Password is " + password);
                        Console.WriteLine("_________________________________");
                    }
                }
            }

            Console.WriteLine("L1 Error (Mean) is " + errorMean / total);
            Console.WriteLine("L1 Error (Median) is " + errorMedian / total);

            return errorMedian / total;
        }


        /// <summary>
        /// TODO: Adhishree
        /// </summary>
        /// <param name="w"></param>
        /// <param name="depth"></param>
        /// <param name="epsilon"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static double scorePartialL1ErrorMedian(uint w, uint depth, double epsilon, int[,] frequencies, int total)
        {
            // Construct CountSketch with parameters w, depth, epsilon
            // Fill CountSketch with file filename
            // Read through file to obtain true frequences
            // obtain estimated frequencies from CountSketch and compute L1 error metrics (powers of 10)
            SketchCount t = new SketchCount(w, depth, epsilon);

            Console.WriteLine("width = " + t.getWidth() + "\n" + "depth=" + t.getDepth());
            Console.WriteLine("epsilon=" + t.getEpsilon());

            t.FillSketchCountFromFrequencyList(frequencies);

            // t.SaveSketchCount();
            double errorMean = 0.0;
            double errorMedian = 0.0;
            int i = 0;
            int totalCount = 0;

            // List<Tuple<int, string>> tuplelist = new List<Tuple<int, string>>();
            for (int k = 0; i < 100000 && k < frequencies.GetLength(0); k++)
            {
                int frequency = frequencies[k, 0];
                int count = frequencies[k, 1];
                for (int j = 0; i < 100000 && j < count; j++)
                {

                    i++;
                    string password = HelperFunctions.YahooTupleToPassword(frequency, j);
                    errorMean += Math.Abs(frequency - t.EstimateMean(password));
                    errorMedian += Math.Abs(frequency - t.EstimateMedian(password));
                    // tuplelist.Add(Tuple.Create(frequency, password));
                    totalCount = totalCount + 1;
                    if (i == 1 || i == 10 || i == 100 || i == 1000 || i == 10000 || i == 100000)
                    {
                        Console.WriteLine("_________________ i=" + i + "____________");
                        Console.WriteLine("Total Count: " + totalCount);
                        Console.WriteLine("L1 Error (Mean): " + errorMean);
                        Console.WriteLine("L1 Error (Mean)/N: " + errorMean / total);
                        Console.WriteLine("L1 Error (Median): " + errorMedian);
                        Console.WriteLine("L1 Error (Median)/N: " + errorMedian / total);
                        //  Console.WriteLine("Score is " + Convert.ToString(Scoring(tuplelist, t)));
                        Console.WriteLine("Password is " + password);
                        Console.WriteLine("_________________________________");
                    }
                }
            }

            Console.WriteLine("L1 Error (Mean) is " + errorMean / total);
            Console.WriteLine("L1 Error (Median) is " + errorMedian / total);

            return errorMedian / total;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void L1ErrorStat(SketchCount c)
        {
            // Need to use both SketchCount and input file for ground truth
        }

        public static double Scoring(List<Tuple<int, string>> tuple, SketchCount sc)
        {
            double sj = 0;
            for (int i = 0; i < tuple.Count; i++)
            {
                sj = sj + Math.Abs((double)(tuple[i].Item1) - sc.EstimateMean(tuple[i].Item2));
            }
            return sj / tuple.Count;
        }
    }
}
