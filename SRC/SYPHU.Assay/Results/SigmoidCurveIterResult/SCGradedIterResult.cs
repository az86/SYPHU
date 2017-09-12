using System.Collections.Generic;

namespace SYPHU.Assay.Results.SigmoidCurveIterResult
{
    //定量
    public class SCGradedIterResult : SCIterResult
    {
        #region 效价预计算变量

        private double _alpha = 3.196;
        private double _b = 1.125;
        private double _delta = 0.145;
        private List<double> _gammaList = new List<double>{-4.307, -4.684};
        private double _s2 = 0.001429;

        public List<double> uList = new List<double>();

        //public double beta;

        #endregion

        #region 计算表格1、2的内容

        protected override void Cpt_p()
        {
            pList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                pList.Add((uList[i] - _delta)/(_alpha - _delta));
            }
        }

        protected override List<double> Cpt_Y()
        {
            var tmpY = new List<double>();
            if (_iterTime == 0)
            {
                for (int i = 0; i < DataLen; ++i)
                {
                    tmpY.Add(_b * (xList[i] - _gammaList[i / (DataLen / _dataSize.PreparationNum)]));
                }
            }
            else
            {
                for (int i = 0; i < DataLen; ++i)
                {
                    tmpY.Add(aList[i / (DataLen / _dataSize.PreparationNum)] + _b * xList[i]);
                }
            }
            return tmpY;
        }

        protected override void Cpt_w()
        {
            wList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wList.Add(ZList[i] * ZList[i] * (_alpha - _delta) * (_alpha - _delta) / _s2);
            }
        }

        protected override void Cpt_a()
        {
            aList.Clear();
            for (int i = 0; i < _dataSize.PreparationNum; i++)
            {
                aList.Add(yAveList[i] - _b * xAveList[i]);
                _gammaList[i] = -aList[i] / _b;
            }
        }

        #endregion
    }
}
