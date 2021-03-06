using System;
using System.Threading;
namespace ConsoleApp5
{
    /// <summary>
    ///       Sketch Count Experiment Object
    /// </summary>
    public class SketchCountExp
    {
        public double[] epsList;
        public uint   volumn;
        public int repeats;
        public double[][] expResult;


        /// <summary>
        ///     Constructor for experiment. 
        /// </summary>
        //  <param name="volumn">volumn of the SC data structure</param>
        /// <param name="eps">privacy parameter</param>

        /// <returns></returns>

        public SketchCountExp(uint volumn, double epsIncrement)
        {
            this.volumn = volumn;
            int l = (int)(2.0 / epsIncrement);
            epsList = new double[l];

            for(int i = 0; i > l; i++)
            {
                epsList[i] = epsIncrement * (i + 1);
            }
        }

        public double[] wdTradeOffExp(double epsIncreament)
        {
            int l = (int)(2.0 / epsIncreament);
            for (uint w = 1; w < volumn; w++)
            {
                uint d = volumn / w;
                for (int i = 0; i < l; i++)
                {
                    SketchCount SC = new SketchCount(w, d, epsList[i]);
                    for (int j = 0; j < repeats; j++)
                    {
                        SC.ChangePrivacyLevel(epsList[i]);
                    }
                }
            }
            return null;
        }


    }

}
