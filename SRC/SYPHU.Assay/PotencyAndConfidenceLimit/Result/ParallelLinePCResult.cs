using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SYPHU.Data.PotencyAndConfidenceLimit.Result;

namespace SYPHU.Data
{
    public class ParallelLinePCResult : PotencyAndConfidenceLimitResult
    {
        public double I;
        public double b;
        public List<double> MList = new List<double>();
        public double C;
        public double V;
        public List<double> rate = new List<double>();

        //public List<TreatPCValues> PCValues = new List<TreatPCValues>();
    }
}
