using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SYPHU.Data.PotencyAndConfidenceLimit.Result;
using SYPHU.Utilities;

namespace SYPHU.Data
{
    public class SigmoidCurveGradedPCResult : PotencyAndConfidenceLimitResult
    {
        #region 效价预计算变量

        public List<double> DoseList = new List<double>();
        public List<double> nList = new List<double>();
        public List<double> rList = new List<double>();

        public List<double> xList = new List<double>();
        public List<double> pList = new List<double>();
        public List<double> YList = new List<double>();
        public List<double> PhiList = new List<double>();
        public List<double> ZList = new List<double>();
        public List<double> yList = new List<double>();
        public List<double> wList = new List<double>();
        public List<double> wxList = new List<double>();
        public List<double> wyList = new List<double>();
        public List<double> wx2List = new List<double>();
        public List<double> wy2List = new List<double>();
        public List<double> wxyList = new List<double>();

        public List<double> wSumList = new List<double>();
        public List<double> wxSumList = new List<double>();
        public List<double> wySumList = new List<double>();
        public List<double> wx2SumList = new List<double>();
        public List<double> wy2SumList = new List<double>();
        public List<double> wxySumList = new List<double>();

        public List<double> SxxList = new List<double>();
        public List<double> SxyList = new List<double>();
        public List<double> SyyList = new List<double>();

        public List<double> xAveList = new List<double>();
        public List<double> yAveList = new List<double>();
        public List<double> aList = new List<double>();
        public double b = 0;

        private int _iterTime = 0;
        public int DataLen = 0;

        #endregion

        #region 效价预计算内容

        public List<double> MList = new List<double>();

        public double C;

        public double V;

        #endregion


        public void Do_Cpt(CalculationModel model)
        {
            Cpt_x();
            Cpt_p();
            YList = Cpt_Y();
            List<double> tmpY = new List<double>();
            bool che = true;
            do
            {
                _iterTime++;
                Cpt_Phi(model);
                Cpt_Z(model);
                Cpt_y();
                Cpt_w();
                Cpt_wx();
                Cpt_wy();
                Cpt_wx2();
                Cpt_wy2();
                Cpt_wxy();
                Cpt_Sum_xx(wSumList, wList);
                Cpt_Sum_xx(wxSumList, wxList);
                Cpt_Sum_xx(wySumList, wyList);
                Cpt_Sum_xx(wx2SumList, wx2List);
                Cpt_Sum_xx(wy2SumList, wy2List);
                Cpt_Sum_xx(wxySumList, wxyList);
                Cpt_Sxx();
                Cpt_Sxy();
                Cpt_Syy();
                Cpt_xAve();
                Cpt_yAve();
                Cpt_b();
                Cpt_a();
                tmpY = Cpt_Y();
                che = Check_End(tmpY);
                YList = tmpY;
                //DoDisplay();
            } while (!che);
        }

        #region 计算表格1、2的内容

        private void Cpt_x()
        {
            xList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                xList.Add(Math.Log(DoseList[i]));
            }
        }

        private void Cpt_p()
        {
            pList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                pList.Add(rList[i]/nList[i]);
            }
        }

        private List<double> Cpt_Y()
        {
            List<double> tmpY = new List<double>();
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
                    tmpY.Add(aList[i/(DataLen/2)] + b*xList[i]);
                }
            }
            return tmpY;
        }

        private void Cpt_Phi(CalculationModel model)
        {
            PhiList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                switch (model)
                {
                    case CalculationModel.Probit:
                        PhiList.Add(Distributions.Dist_Phi(YList[i]));
                        break;
                    case CalculationModel.Logit:
                        PhiList.Add(1.0/(1.0 + Math.Exp(-YList[i])));
                        break;
                    case CalculationModel.Gompit:
                        PhiList.Add(1 - Math.Exp(-Math.Exp(YList[i])));
                        break;
                    case CalculationModel.Angle:
                        PhiList.Add(0.5*Math.Sin(YList[i]) + 0.5);
                        break;
                }
            }
        }

        private void Cpt_Z(CalculationModel model)
        {
            ZList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                switch (model)
                {
                    case CalculationModel.Probit:
                        ZList.Add(Math.Exp(-YList[i]*YList[i]/2.0)/Math.Sqrt(2.0*ConstantsExt.Pi));
                        break;
                    case CalculationModel.Logit:
                        ZList.Add(Math.Exp(-YList[i])/Math.Pow((1.0 + Math.Exp(-YList[i])), 2.0));
                        break;
                    case CalculationModel.Gompit:
                        ZList.Add(Math.Exp(YList[i] - Math.Exp(YList[i])));
                        break;
                    case CalculationModel.Angle:
                        ZList.Add(0.5*Math.Cos(YList[i]));
                        break;
                }
            }
        }

        private void Cpt_y()
        {
            yList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                yList.Add(YList[i] + (pList[i] - PhiList[i])/ZList[i]);
            }
        }

        private void Cpt_w()
        {
            wList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wList.Add(nList[i]*ZList[i]*ZList[i]/(PhiList[i] - PhiList[i]*PhiList[i]));
            }
        }

        private void Cpt_wx()
        {
            wxList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wxList.Add(wList[i]*xList[i]);
            }
        }

        private void Cpt_wy()
        {
            wyList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wyList.Add(wList[i]*yList[i]);
            }
        }

        private void Cpt_wx2()
        {
            wx2List.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wx2List.Add(wList[i]*xList[i]*xList[i]);
            }
        }

        private void Cpt_wy2()
        {
            wy2List.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wy2List.Add(wList[i]*yList[i]*yList[i]);
            }
        }

        private void Cpt_wxy()
        {
            wxyList.Clear();
            for (int i = 0; i < DataLen; ++i)
            {
                wxyList.Add(wList[i]*xList[i]*yList[i]);
            }
        }

        private void Cpt_Sum_xx(List<double> ww, List<double> xx)
        {
            ww.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                ww.Add(xx.PartialSum(DataSizeInfo.Instance.DoseNum*i, DataSizeInfo.Instance.DoseNum*(i + 1) - 1));
            }

            //ww.Add(xx.PartialSum(0, len / 2 - 1));
            //ww.Add(xx.PartialSum(len / 2, len - 1));
        }

        private void Cpt_Sxx()
        {
            SxxList.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                SxxList.Add(wx2SumList[i] - wxSumList[i]*wxSumList[i]/wSumList[i]);
            }
        }

        private void Cpt_Sxy()
        {
            SxyList.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                SxyList.Add(wxySumList[i] - wxSumList[i]*wySumList[i]/wSumList[i]);
            }
        }

        private void Cpt_Syy()
        {
            SyyList.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                SyyList.Add(wy2SumList[i] - wySumList[i]*wySumList[i]/wSumList[i]);
            }
        }

        private void Cpt_xAve()
        {
            xAveList.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                xAveList.Add(wxSumList[i]/wSumList[i]);
            }
        }

        private void Cpt_yAve()
        {
            yAveList.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                yAveList.Add(wySumList[i]/wSumList[i]);
            }
        }

        private void Cpt_b()
        {
            b = SxyList.Sum()/SxxList.Sum();
        }

        private void Cpt_a()
        {
            aList.Clear();
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                aList.Add(yAveList[i] - b*xAveList[i]);
            }
        }

        private bool Check_End(List<double> tmpY)
        {
            for (int i = 0; i < DataLen; ++i)
            {
                YList[i] -= tmpY[i];
                YList[i] = Math.Abs(YList[i]);
            }
            if (YList.Max() < ConstantsExt.Eps(-8))
            {
                return true;
            }
            return false;
        }

        #endregion

    }
}
