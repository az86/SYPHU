using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYPHU.Data.PotencyAndConfidenceLimit.Result
{
    public class BasicPCValues
    {
        public double Lower;
        public double Est;
        public double Upper;
    }

    public class TreatPCValues
    {
        public BasicPCValues Potency = new BasicPCValues();
        public BasicPCValues RelToAss = new BasicPCValues();
        public BasicPCValues RelToEst = new BasicPCValues();
        public double CLPercent
        {
            get { return (RelToEst.Upper - RelToEst.Lower)*0.5; }
        }
    }

    public class PotencyAndConfidenceLimitResult
    {
        public List<TreatPCValues> PCValues = new List<TreatPCValues>();
    }
}
