using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYPHU.Data.PotencyAndConfidenceLimit.Result
{
    public class SlopeRatioPCResult : PotencyAndConfidenceLimitResult
    {
        public double V1, V2;

        public List<double> bList = new List<double>();

        public List<double> RList = new List<double>();

        public double C;

        public double K;
    }
}
