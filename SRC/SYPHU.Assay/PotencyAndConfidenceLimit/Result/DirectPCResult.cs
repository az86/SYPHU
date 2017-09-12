using System.Collections.Generic;
using SYPHU.Data.PotencyAndConfidenceLimit.Result;

namespace SYPHU.Data
{
    public class DirectPCResult : PotencyAndConfidenceLimitResult
    {
        public List<double> MList = new List<double>();
        public List<double> RList = new List<double>();
        public List<double> PList = new List<double>();
        public List<double> SMList = new List<double>();
        public List<double> RflLowerList = new List<double>();
        public List<double> RflUpperList = new List<double>();
        public List<double> PflLowerList = new List<double>();
        public List<double> PflUpperList = new List<double>();
        public List<double> FLPercentList = new List<double>();
    }
}