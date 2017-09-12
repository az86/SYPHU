using System.Collections.Generic;

namespace SYPHU.Assay.Results.SigmoidCurveIterResult
{
    //定性
    public class SCQuantalIterResult : SCIterResult
    {
        #region 效价预计算变量

        public List<double> nList = new List<double>();
        public List<double> rList = new List<double>();

        #endregion

        #region 计算表格1、2的内容

        protected override void Cpt_p()
        {
            pList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                pList.Add(rList[i]/nList[i]);
            }
        }

        protected override List<double> Cpt_Y()
        {
            var tmpY = new List<double>();
            if (_iterTime == 0)
            {
                //tmpY.Clear();
                for (int i = 0; i < DataLen; ++i)
                {
                    tmpY.Add(0.0);
                }
            }
            else
            {
                //tmpY.Clear();
                for (int i = 0; i < DataLen; ++i)
                {
                    tmpY.Add(aList[i/(DataLen/_dataSize.PreparationNum)] + b*xList[i]);
                }
            }
            return tmpY;
        }

        protected override void Cpt_w()
        {
            wList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wList.Add(nList[i]*ZList[i]*ZList[i]/(PhiList[i] - PhiList[i]*PhiList[i]));
            }
        }

        protected override void Cpt_a()
        {
            aList.Clear();
            for (int i = 0; i < _dataSize.PreparationNum; i++)
            {
                aList.Add(yAveList[i] - b*xAveList[i]);
            }
        }

        #endregion
    }
}
