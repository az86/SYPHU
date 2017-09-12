using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.ConstTables;
using SYPHU.Assay.Data;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    public class DataProcess
    {
        private int _h;
        private int _d;
        private int _n;
        private InitCalculationInfo _calculationInfo;
        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="calculationInfo">计算信息</param>
        /// <param name="origData">原始数据</param>
        /// <param name="tranData">转换后数据</param>
        public String DataTransformation(InitCalculationInfo calculationInfo, AssayData origData, AssayData tranData)
        {
            _calculationInfo = calculationInfo;
            _h = origData.Data.Count;
            _d = origData.Data[0].Count;
            _n = origData.Data[0][0].Count;
            tranData.Precision = origData.Precision;
            return calculationInfo.CalcCase == CalcCases.Single
                       ? SingleCalcDataTransformation(origData, tranData)
                       : MergeCalcDataTransformation(origData, tranData);
        }

        private String SingleCalcDataTransformation(AssayData origData, AssayData tranData)
        {
            switch (_calculationInfo.DataTransFormula)
            {
                case DataTransformationFormula.Null:
                    for (int i = 0; i < _h; i++)
                    {
                        for (int j = 0; j < _d; j++)
                        {
                            for (int k = 0; k < _n; k++)
                            {
                                tranData.Data[i][j][k] = origData.Data[i][j][k];
                            }
                        }
                    }
                    return null;
                case DataTransformationFormula.Log10:
                    for (int i = 0; i < _h; i++)
                    {
                        for (int j = 0; j < _d; j++)
                        {
                            for (int k = 0; k < _n; k++)
                            {
                                if (origData.Data[i][j][k] < ConstantsExt.Eps())
                                {
                                    return "数据转换出现异常，对数值不能为0.";
                                }
                                tranData.Data[i][j][k] = Math.Log10(origData.Data[i][j][k]);
                            }
                        }
                    }
                    return null;
                case DataTransformationFormula.LogE:
                    for (int i = 0; i < _h; i++)
                    {
                        for (int j = 0; j < _d; j++)
                        {
                            for (int k = 0; k < _n; k++)
                            {
                                if (origData.Data[i][j][k] < ConstantsExt.Eps())
                                {
                                    return "数据转换出现异常，对数值不能为0.";
                                }
                                tranData.Data[i][j][k] = Math.Log(origData.Data[i][j][k], ConstantsExt.E);
                            }
                        }
                    }
                    return null;
                case DataTransformationFormula.Square:
                    for (int i = 0; i < _h; i++)
                    {
                        for (int j = 0; j < _d; j++)
                        {
                            for (int k = 0; k < _n; k++)
                            {
                                tranData.Data[i][j][k] = origData.Data[i][j][k] * origData.Data[i][j][k];
                            }
                        }
                    }
                    return null;
                case DataTransformationFormula.SquareRoot:
                    for (int i = 0; i < _h; i++)
                    {
                        for (int j = 0; j < _d; j++)
                        {
                            for (int k = 0; k < _n; k++)
                            {
                                if (origData.Data[i][j][k] < 0)
                                {
                                    return "数据转换出现异常，开方值不能为负.";
                                }
                                tranData.Data[i][j][k] = Math.Sqrt(origData.Data[i][j][k]);
                            }
                        }
                    }
                    return null;
                case DataTransformationFormula.Reciprocal:
                    for (int i = 0; i < _h; i++)
                    {
                        for (int j = 0; j < _d; j++)
                        {
                            for (int k = 0; k < _n; k++)
                            {
                                if (origData.Data[i][j][k] < ConstantsExt.Eps())
                                {
                                    return "数据转换出现异常，倒数值不能为0.";
                                }
                                tranData.Data[i][j][k] = 1 / origData.Data[i][j][k];
                            }
                        }
                    }
                    return null;
                case DataTransformationFormula.UserDefined:
                    var rpn = new ReversePolishNotation();
                    if (rpn.IsValid(_calculationInfo.UserDefinedFormula) && rpn.Parse())
                    {
                        for (int i = 0; i < _h; i++)
                        {
                            for (int j = 0; j < _d; j++)
                            {
                                for (int k = 0; k < _n; k++)
                                {
                                    try
                                    {
                                        tranData.Data[i][j][k] = rpn.Evaluate(origData.Data[i][j][k]);
                                    }
                                    catch (Exception e)
                                    {
                                        return "表达式不规范，无法计算.";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return "转换公式无法解析.";
                    }
                    return null;
                default:
                    return null;
            }
        }

        private String MergeCalcDataTransformation(AssayData origData, AssayData tranData)
        {
            for (int i = 0; i < _h; i++)
            {
                for (int j = 0; j < _d; j++)
                {
                    //d=5变换下限、效价、上限；d=4变换效价
                    if ((_d == 5 && (j >= 1 && j <= 3)) || (_d == 4 && j == 1))
                    {
                        for (int k = 0; k < _n; k++)
                        {
                            switch (_calculationInfo.DataTransFormula)
                            {
                                case DataTransformationFormula.Null:
                                    tranData.Data[i][j][k] = origData.Data[i][j][k];
                                    break;
                                case DataTransformationFormula.Log10:
                                    if (origData.Data[i][j][k] < ConstantsExt.Eps())
                                    {
                                        return "数据转换出现异常，对数值不能为0.";
                                    }
                                    tranData.Data[i][j][k] = Math.Log10(origData.Data[i][j][k]);
                                    break;
                                case DataTransformationFormula.LogE:
                                    if (origData.Data[i][j][k] < ConstantsExt.Eps())
                                    {
                                        return "数据转换出现异常，对数值不能为0.";
                                    }
                                    tranData.Data[i][j][k] = Math.Log(origData.Data[i][j][k], ConstantsExt.E);
                                    break;
                                default:
                                    goto case DataTransformationFormula.Null;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < _n; k++)
                        {
                            tranData.Data[i][j][k] = origData.Data[i][j][k];
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 拉丁方设计数据转换
        /// </summary>
        /// <param name="tranData">原始数据的转换数据</param>
        /// <param name="latinConvertData">拉丁方转换后的数据</param>
        /// <param name="convertTable">转换表</param>
        public String LatinDataConvert(AssayData tranData, AssayData latinConvertData, LatinConvertTable convertTable)
        {
            String errorMsg = convertTable.Parse();
            latinConvertData.Precision = tranData.Precision;
            if (errorMsg == null)
            {
                for (int i = 0; i < _h; i++)
                {
                    for (int j = 0; j < _d; j++)
                    {
                        for (int k = 0; k < _n; k++)
                        {
                            latinConvertData.Data[i][j][k] = tranData.Data[convertTable.ConvertMatrix[i][j][k][0]][
                                convertTable.ConvertMatrix[i][j][k][1]][k];
                        }
                    }
                }
            }
            return errorMsg;
        }

        /// <summary>
        /// 异常值检测
        /// </summary>
        /// <param name="tranData">转换后的数据</param>
        /// <param name="checkMethods">异常值检测方法列表</param>
        /// <returns></returns>
        public List<List<String>> AbnormalDataCheck(AssayData tranData,
                                                    IEnumerable<AbnormalDataCheckMethods> checkMethods)
        {
            
            ExtremeAbnormalDataLocationList = new List<List<int>>();
            //NonExtremeAbnormalDataLocationList = new List<List<int>>();
            return (checkMethods == null || checkMethods.Contains(AbnormalDataCheckMethods.Null)) ? new List<List<string>>() :
                checkMethods.Select(item => DoAbnCheck(item, tranData)).ToList();
        }

        private List<List<int>> _extremeAbnormalDataLocationList;

        private List<List<int>> _extremeAbnormalDataNum;

        /// <summary>
        /// 极端异常值总数
        /// </summary>
        public int TotalExtremeAbnormalDataNum
        {
            get { return ExtremeAbnormalDataLocationList.Count; }
        }

        public List<List<int>> ExtremeAbnormalDataLocationList
        {
            get { return _extremeAbnormalDataLocationList; }
            set { _extremeAbnormalDataLocationList = value; }
        }

        //public List<List<int>> NonExtremeAbnormalDataLocationList;

        /// <summary>
        /// 拉丁方设计B均值
        /// </summary>
        public List<List<double>> ListAveB;

        /// <summary>
        /// 异常值处理
        /// </summary>
        /// <param name="tranData"></param>
        /// <param name="latinData"></param>
        /// <param name="corrData"></param>
        /// <param name="calculationInfo"></param>
        public void AbnormalDataProcess(AssayData tranData, AssayData latinData, AssayData corrData, InitCalculationInfo calculationInfo)
        {
            corrData.Precision = tranData.Precision;
            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                SingleCalcAbnormalDataProcess(tranData, latinData, corrData, calculationInfo);
            }
            else if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                MergeCalcAbnormalDataProcess(tranData, corrData);
            }
        }

        private void SingleCalcAbnormalDataProcess(AssayData tranData, AssayData latinData, AssayData corrData,
                                                   InitCalculationInfo calculationInfo)
        {
            //存储每组、每列的异常值个数
            _extremeAbnormalDataNum = new List<List<int>>();
            for (int i = 0; i < _h; i++)
            {
                _extremeAbnormalDataNum.Add(new List<int>());
                for (int j = 0; j < _d; j++)
                {
                    _extremeAbnormalDataNum[i].Add(0);
                }
            }
            //转换数据赋给校正数据，直接写=会同步变化（指针问题）
            for (int i = 0; i < _h; i++)
            {
                for (int j = 0; j < _d; j++)
                {
                    for (int k = 0; k < _n; k++)
                    {
                        corrData.Data[i][j][k] = tranData.Data[i][j][k];
                    }
                }
            }
            //将异常值赋为0，统计每列异常值的个数
            var crossOverExtraLocation = new List<List<int>>();
            foreach (List<int> t in ExtremeAbnormalDataLocationList)
            {
                corrData.Data[t[0]][t[1]][t[2]] = 0;
                _extremeAbnormalDataNum[t[0]][t[1]]++;
                //双交叉法，同一只有问题，对应的也算异常值，需要剔除
                if (calculationInfo.Design == Designs.CrossOver)
                {
                    corrData.Data[3 - t[0]][1 - t[1]][t[2]] = 0;
                    crossOverExtraLocation.Add(new List<int> { 3 - t[0], 1 - t[1], t[2] });
                    _extremeAbnormalDataNum[3 - t[0]][1 - t[1]]++;
                }
            }
            ExtremeAbnormalDataLocationList.AddRange(crossOverExtraLocation);
            //根据方法填充异常值位置
            switch (calculationInfo.AbnDataProcessMethod)
            {
                case AbnormalDataProcessMethod.Average:
                    AverageMethod(corrData);
                    return;
                case AbnormalDataProcessMethod.EquationSet:
                    EquationSetMethod(latinData, corrData, calculationInfo.Design);
                    return;
                default:
                    return;
            }
        }

        private void MergeCalcAbnormalDataProcess(AssayData tranData, AssayData corrData)
        {
            if (corrData == null) throw new ArgumentNullException("corrData");
            for (int i = 0; i < _h; i++)
            {
                for (int j = 0; j < _d; j++)
                {
                    for (int k = 0; k < _n; k++)
                    {
                        corrData.Data[i][j][k] = tranData.Data[i][j][k];
                    }
                }
            }
            for (int i = _extremeAbnormalDataLocationList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < corrData.Data[0].Count; j++)
                {
                    corrData.Data[0][j].RemoveAt(_extremeAbnormalDataLocationList[i][2]);
                }
            }
        }

        /// <summary>
        /// 均值法：计算每列除去异常值后的均值，用均值补充该列所有的异常值
        /// </summary>
        /// <param name="corrData"></param>
        private void AverageMethod(AssayData corrData)
        {
            //按照异常值位置计算每个异常值的补充值
            var addValue =
                ExtremeAbnormalDataLocationList.Select(
                    t =>
                    corrData.Data[t[0]][t[1]].Sum() /
                    (_n - _extremeAbnormalDataNum[t[0]][t[1]])).ToList();
            //用补充值补充每个异常值
            for (int i = 0; i < ExtremeAbnormalDataLocationList.Count; i++)
            {
                corrData.Data[ExtremeAbnormalDataLocationList[i][0]][ExtremeAbnormalDataLocationList[i][1]][
                    ExtremeAbnormalDataLocationList[i][2]] = addValue[i];
            }
        }

        /// <summary>
        /// 高斯法：解线性方程组
        /// </summary>
        /// <param name="latinData"></param>
        /// <param name="corrData"></param>
        /// <param name="design"></param>
        private void EquationSetMethod(AssayData latinData, AssayData corrData, Designs design)
        {
            #region 计算R、C、G、B值

            double dG = 0;
            var listC = new List<List<double>>();
            var listR = new List<double>();
            for (int i = 0; i < _n; i++)
            {
                listR.Add(0.0);
            }
            for (int i = 0; i < _h; i++)
            {
                listC.Add(new List<double>());
                for (int j = 0; j < _d; j++)
                {
                    listC[i].Add(corrData.Data[i][j].Sum());
                    dG += listC[i][j];
                    for (int k = 0; k < _n; k++)
                    {
                        listR[k] += corrData.Data[i][j][k];
                    }
                }
            }

            ListAveB = new List<List<double>>();
            var bList = new List<List<double>>();
            if (design == Designs.LatinSquare)
            {
                for (int i = 0; i < _h; i++)
                {
                    bList.Add(new List<double>());
                    ListAveB.Add(new List<double>());
                    for (int j = 0; j < _d; j++)
                    {
                        bList[i].Add(latinData.Data[i][j].Sum());
                        ListAveB[i].Add(bList[i][j]/_n);
                    }
                }
            }

            #endregion

            double h = _h;
            double d = _d;
            double n = _n;

            double y = 0.0;
            if (TotalExtremeAbnormalDataNum == 1)
            {
                if (design == Designs.RandomisedBlock)
                {
                    y = (h*d*listC[ExtremeAbnormalDataLocationList[0][0]][ExtremeAbnormalDataLocationList[0][1]] +
                         n*listR[ExtremeAbnormalDataLocationList[0][2]] - dG)/((h*d - 1)*(n - 1));
                }
                if (design == Designs.LatinSquare)
                {
                    y = (n*(listC[ExtremeAbnormalDataLocationList[0][0]][ExtremeAbnormalDataLocationList[0][1]] +
                            listR[ExtremeAbnormalDataLocationList[0][2]] +
                            ListAveB[ExtremeAbnormalDataLocationList[0][0]][ExtremeAbnormalDataLocationList[0][1]]) - 2*dG)/
                        ((n - 1)*(n - 2));
                }
                corrData.Data[ExtremeAbnormalDataLocationList[0][0]][ExtremeAbnormalDataLocationList[0][1]][
                        ExtremeAbnormalDataLocationList[0][2]] = y;
                return;
            }

            //解方程组
            if (TotalExtremeAbnormalDataNum > 1)
            {
                //无同行、同列时方程组系数
                var coeffMatrix = new List<List<double>>();
                for (int i = 0; i < TotalExtremeAbnormalDataNum; i++)
                {
                    coeffMatrix.Add(new List<double>());
                    if (design == Designs.RandomisedBlock)
                    {
                        y = (h*d*listC[ExtremeAbnormalDataLocationList[i][0]][ExtremeAbnormalDataLocationList[i][1]] +
                             n*listR[ExtremeAbnormalDataLocationList[i][2]] - dG);
                        for (int j = 0; j < TotalExtremeAbnormalDataNum; j++)
                        {
                            coeffMatrix[i].Add(i == j ? (h*d - 1)*(n - 1) : 1.0);
                        }
                    }
                    if (design == Designs.LatinSquare)
                    {
                        y = (n*(listC[ExtremeAbnormalDataLocationList[i][0]][ExtremeAbnormalDataLocationList[i][1]] +
                                listR[ExtremeAbnormalDataLocationList[i][2]] +
                                ListAveB[ExtremeAbnormalDataLocationList[i][0]][ExtremeAbnormalDataLocationList[i][1]]) -
                             2*dG);
                        for (int j = 0; j < TotalExtremeAbnormalDataNum; j++)
                        {
                            coeffMatrix[i].Add(i == j ? (n - 1)*(n - 2) : 2.0);
                        }
                    }
                    coeffMatrix[i].Add(y);
                }

                //补充有同行或同列时方程组系数
                for (int i = 0; i < TotalExtremeAbnormalDataNum; i++)
                {
                    for (int j = i + 1; j < TotalExtremeAbnormalDataNum; j++)
                    {
                        //同表，同列
                        if (ExtremeAbnormalDataLocationList[i][0] == ExtremeAbnormalDataLocationList[j][0] && 
                            ExtremeAbnormalDataLocationList[i][1] == ExtremeAbnormalDataLocationList[j][1])
                        {
                            coeffMatrix[i][j] -= h*d;
                            coeffMatrix[j][i] -= h*d;
                        }
                        //同行
                        if (ExtremeAbnormalDataLocationList[i][2] == ExtremeAbnormalDataLocationList[j][2])
                        {
                            coeffMatrix[i][j] -= n;
                            coeffMatrix[j][i] -= n;
                        }
                    }
                }
                
                //解方程组
                var equationSet = new GaussEquationSet(TotalExtremeAbnormalDataNum);
                equationSet.Init(coeffMatrix);
                equationSet.Solve();

                //替换异常值
                for (int i = 0; i < TotalExtremeAbnormalDataNum; i++)
                {
                    corrData.Data[ExtremeAbnormalDataLocationList[i][0]][ExtremeAbnormalDataLocationList[i][1]][
                        ExtremeAbnormalDataLocationList[i][2]] = equationSet.Root[i];
                }
            }
        }

        #region 异常值检测

        private List<String> DoAbnCheck(AbnormalDataCheckMethods method, AssayData data)
        {
            if (!ValidCheck(method))
            {
                return ValidMethod(ConstStrings.CheckMethodsStringDict[_calculationInfo.Lang][method]);
            }
            var extremeLocationList = new List<List<int>>();
            for (int i = 0; i < _h; i++)
            {
                if (_calculationInfo.CalcCase == CalcCases.Single)
                {
                    for (int j = 0; j < _d; j++)
                    {
                        extremeLocationList.AddRange(DoAbnCheck(method, data.Data[i][j], i, j));
                    }
                }
                else
                {
                    extremeLocationList.AddRange(DoAbnCheck(method, data.Data[i][2], i, 3));
                }
            }
            ExtremeAbnormalDataLocationList.AddRange(extremeLocationList);
            return OutputCheckResult(ConstStrings.CheckMethodsStringDict[_calculationInfo.Lang][method], extremeLocationList);
        }

        private List<List<int>> DoAbnCheck(AbnormalDataCheckMethods method, ObservableCollection<double> colData,
                                                  int prepId, int doseId)
        {
            switch (method)
            {
                case AbnormalDataCheckMethods.Dixon:
                    return DoDixonCheck(colData, prepId, doseId);
                case AbnormalDataCheckMethods.Grubb:
                    return DoGrubbsCheck(colData, prepId, doseId);
                case AbnormalDataCheckMethods.Romanovsky:
                    return DoRomanovskyCheck(colData, prepId, doseId);
                case AbnormalDataCheckMethods.Hampel:
                    return DoHampelCheck(colData, prepId, doseId);
                case AbnormalDataCheckMethods.Quartile:
                    return DoQuartileCheck(colData, prepId, doseId);
                default:
                    return new List<List<int>>();
            }
        }

        /// <summary>
        /// 方法有效性检验
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool ValidCheck(AbnormalDataCheckMethods method)
        {
            switch (method)
            {
                case AbnormalDataCheckMethods.Dixon:
                    return _n >= DixonTable.GetMinRegNum &&
                           _n <= DixonTable.GetMaxRegNum;
                case AbnormalDataCheckMethods.Grubb:
                    return _n >= 2;
                case AbnormalDataCheckMethods.Romanovsky:
                    return _n >= 2;
                case AbnormalDataCheckMethods.Quartile:
                    return _n >= 4;
                case AbnormalDataCheckMethods.Hampel:
                    return true;
                default:
                    return false;
            }
        }

        #region Dixon

        private List<List<int>> DoDixonCheck(ObservableCollection<double> colData, int prepId, int doseId)
        {
            var abnormalDataLocations = new List<List<int>>();
            //原数据
            var tmpData = new List<double>();
            tmpData.AddRange(colData);
            //排序数据
            var tmpDataSorted = new List<double>();
            tmpDataSorted.AddRange(colData);
            tmpDataSorted.Sort();

            double maxJudgeValue = 0;
            double minJudgeValue = 0;
            int curDataNum = _n;
            bool hasAbnormalData;

            do
            {
                hasAbnormalData = false;
                if (curDataNum >= DixonTable.GetMinRegNum && curDataNum <= 7)
                {
                    minJudgeValue = (tmpDataSorted[1] - tmpDataSorted[0])/
                                    (tmpDataSorted.Last() - tmpDataSorted[0]);
                    maxJudgeValue = (tmpDataSorted.Last() - tmpDataSorted[curDataNum - 2])/
                                    (tmpDataSorted.Last() - tmpDataSorted[0]);
                }
                else if (curDataNum >= 8 && curDataNum <= 10)
                {
                    minJudgeValue = (tmpDataSorted[1] - tmpDataSorted[0])/
                                    (tmpDataSorted[curDataNum - 2] - tmpDataSorted[0]);
                    maxJudgeValue = (tmpDataSorted.Last() - tmpDataSorted[curDataNum - 2])/
                                    (tmpDataSorted.Last() - tmpDataSorted[1]);
                }
                else if (curDataNum >= 11 && curDataNum <= 13)
                {
                    minJudgeValue = (tmpDataSorted[2] - tmpDataSorted[0])/
                                    (tmpDataSorted[curDataNum - 2] - tmpDataSorted[0]);
                    maxJudgeValue = (tmpDataSorted.Last() - tmpDataSorted[curDataNum - 3])/
                                    (tmpDataSorted.Last() - tmpDataSorted[1]);
                }
                else if (curDataNum >= 14 && curDataNum <= DixonTable.GetMaxRegNum)
                {
                    minJudgeValue = (tmpDataSorted[2] - tmpDataSorted[0])/
                                    (tmpDataSorted[curDataNum - 3] - tmpDataSorted[0]);
                    maxJudgeValue = (tmpDataSorted.Last() - tmpDataSorted[curDataNum - 3])/
                                    (tmpDataSorted.Last() - tmpDataSorted[2]);
                }
                else
                {
                    return new List<List<int>>();
                }

                double dixonValue = DixonTable.GetValue(curDataNum);

                if (minJudgeValue > dixonValue)
                {
                    abnormalDataLocations.Add(new List<int>{ prepId, doseId, tmpData.IndexOf(tmpDataSorted[0])});
                    tmpData[tmpData.IndexOf(tmpDataSorted[0])] = -1;
                    tmpDataSorted.RemoveAt(0);
                    curDataNum--;
                    hasAbnormalData = true;
                }
                if (maxJudgeValue > dixonValue)
                {
                    abnormalDataLocations.Add(new List<int>{ prepId, doseId, tmpData.IndexOf(tmpDataSorted.Last()) });
                    tmpData[tmpData.IndexOf(tmpDataSorted.Last())] = -1;
                    tmpDataSorted.RemoveAt(curDataNum - 1);
                    curDataNum--;
                    hasAbnormalData = true;
                }
            } while (hasAbnormalData);

            return abnormalDataLocations;
        }

        #endregion

        #region Grubb

        private List<List<int>> DoGrubbsCheck(ObservableCollection<double> colData, int prepId, int doseId)
        {
            var abnormalDataLocations = new List<List<int>>();
            var tmpData = new List<double>();
            tmpData.AddRange(colData);
            var tmpDataRemoved = new List<double>();
            tmpDataRemoved.AddRange(colData);
            var adoubtData = new List<double>();
            int curDataNum = _n;
            bool hasAbnormalData;

            do
            {
                hasAbnormalData = false;
                int adoubtDataNum = curDataNum < 20 ? 1 : (curDataNum / 10 > 5 ? 5 : curDataNum / 10);
                adoubtData.Clear();

                var R = new List<double>();
                for (int i = 0; i < adoubtDataNum; i++)
                {
                    double sd = tmpDataRemoved.StandardDevivation();
                    double ave = tmpDataRemoved.Average();

                    if ((tmpDataRemoved.Max() - ave) > (ave - tmpDataRemoved.Min()))
                    {
                        adoubtData.Add(tmpDataRemoved.Max());
                        tmpDataRemoved.RemoveAt(tmpDataRemoved.MaxValPos());
                    }
                    else
                    {
                        adoubtData.Add(tmpDataRemoved.Min());
                        tmpDataRemoved.RemoveAt(tmpDataRemoved.MinValPos());
                    }
                    R.Add(Math.Abs(adoubtData[i] - ave) / sd);
                }
                
                if (adoubtDataNum == 1)
                {
                    if (R[0] > Distributions.GrubbsTValue(0.05, curDataNum, adoubtDataNum))
                    {
                        abnormalDataLocations.Add(new List<int> { prepId, doseId, tmpData.IndexOf(adoubtData[0]) });
                        tmpData[tmpData.IndexOf(adoubtData[0])] = -1;
                        curDataNum--;
                        hasAbnormalData = true;
                    }
                }
                else if (adoubtDataNum > 1)
                {
                    adoubtData.Reverse();
                    for (int i = 0; i < adoubtDataNum; i++)
                    {
                        if (R[i] > Distributions.GrubbsTValue(0.05, curDataNum, adoubtDataNum))
                        {
                            for (int j = i; j < adoubtDataNum; j++)
                            {
                                abnormalDataLocations.Add(new List<int> { prepId, doseId, tmpData.IndexOf(adoubtData[j]) });
                                tmpData[tmpData.IndexOf(adoubtData[j])] = -1;
                                curDataNum--;
                            }
                            hasAbnormalData = true;
                            break;
                        }
                        tmpDataRemoved.Add(adoubtData[i]);
                        adoubtDataNum--;
                    }
                }

            } while (hasAbnormalData);

            return abnormalDataLocations;
        }

        #endregion

        #region Romanovsky

        private List<List<int>> DoRomanovskyCheck(ObservableCollection<double> colData, int prepId, int doseId)
        {
            var abnormalDataLocations = new List<List<int>>();
            var tmpData = new List<double>();
            tmpData.AddRange(colData);
            var tmpDataRemoved = new List<double>();
            tmpDataRemoved.AddRange(colData);

            int curDataNum = _n;
            bool hasAbnormalData;

            do
            {
                hasAbnormalData = false;
                double ave = tmpDataRemoved.Average();
                double adoubtData;
                if ((tmpDataRemoved.Max() - ave) > (ave - tmpDataRemoved.Min()))
                {
                    adoubtData = tmpDataRemoved.Max();
                    tmpDataRemoved.RemoveAt(tmpDataRemoved.MaxValPos());
                }
                else
                {
                    adoubtData = tmpDataRemoved.Min();
                    tmpDataRemoved.RemoveAt(tmpDataRemoved.MinValPos());
                }

                double sd = tmpDataRemoved.StandardDevivation();

                if (Math.Abs(adoubtData - tmpDataRemoved.Average()) > sd * Distributions.RomanovskyTValue(0.05, curDataNum))
                {
                    abnormalDataLocations.Add(new List<int> { prepId, doseId, tmpData.IndexOf(adoubtData) });
                    tmpData[tmpData.IndexOf(adoubtData)] = -1;
                    curDataNum--;
                    hasAbnormalData = true;
                }
            } while (hasAbnormalData);

            return abnormalDataLocations;
        }

        #endregion

        #region Hampel

        private List<List<int>> DoHampelCheck(ObservableCollection<double> colData, int prepId, int doseId)
        {
            var abnormalDataLocations = new List<List<int>>();
            var tmpData = new List<double>();
            tmpData.AddRange(colData);

            var tmpDataRemoved = new List<double>();
            tmpDataRemoved.AddRange(colData);
            bool hasAbnormalData;

            do
            {
                hasAbnormalData = false;
                if (tmpDataRemoved.Count == 0)
                {
                    break;
                }
                double mOrig = tmpDataRemoved.Median();
                var tmpDataR = tmpDataRemoved.Select(t => Math.Abs(t - mOrig)).ToList();
                double mR = tmpDataR.Median();
                var removeItems = new List<int>();
                for (int i = 0; i < tmpDataR.Count; i++)
                {
                    if (tmpDataR[i] > 4.5*mR)
                    {
                        abnormalDataLocations.Add(new List<int> { prepId, doseId, tmpData.IndexOf(tmpDataRemoved[i]) });
                        tmpData[tmpData.IndexOf(tmpDataRemoved[i])] = -1;
                        removeItems.Add(i);
                        hasAbnormalData = true;
                    }
                }
                for (int i = removeItems.Count - 1; i >= 0; i--)
                {
                    tmpDataRemoved.RemoveAt(removeItems[i]);
                }
            } while (hasAbnormalData);

            return abnormalDataLocations;
        }

        #endregion

        #region Quartile

        private List<List<int>> DoQuartileCheck(ObservableCollection<double> colData, int prepId, int doseId)
        {
            var abnormalDataLocations = new List<List<int>>();
            var tmpData = new List<double>();
            tmpData.AddRange(colData);
            var tmpDataSorted = new List<double>();
            tmpDataSorted.AddRange(colData);
            tmpDataSorted.Sort();
            bool hasAbnormalData;

            do
            {
                hasAbnormalData = false;
                var removeItems = new List<int>();
                double Q1 = QuartileAlg(tmpDataSorted, 1);
                double Q3 = QuartileAlg(tmpDataSorted, 3);
                double H = Q3 - Q1;
                double lowerLimit = Q1 - 1.5*H;
                double upperLimit = Q3 + 1.5*H;
                for (int i = 0; i < tmpData.Count; i++)
                {
                    if (tmpData[i] < lowerLimit || tmpData[i] > upperLimit)
                    {
                        abnormalDataLocations.Add(new List<int>{prepId,doseId,i});
                        removeItems.Add(i);
                        hasAbnormalData = true;
                    }
                }
                for (int i = removeItems.Count - 1; i >= 0; i--)
                {
                    tmpData.RemoveAt(removeItems[i]);
                }
            } while (hasAbnormalData);

            return abnormalDataLocations;
        }

        private double QuartileAlg(List<double> ld, int n)
        {
            double location = (ld.Count + 1.0)*(n/4.0) - 1;
            int intLocation = (int) location;
            return ld[intLocation] + (location - intLocation)*(ld[intLocation + 1] - ld[intLocation]);
        }

        #endregion

        #region 私有方法

        private List<String> NoResult(String method)
        {
            var noResult = new List<String>();
            noResult.Add(method + "检测法没有检测到异常值.");
            return noResult;
        }

        private List<String> ValidMethod(String method)
        {
            var validString = new List<string>();
            validString.Add(method + "检测法不适用.");
            return validString;
        }

        private List<String> OutputCheckResult(String method, List<List<int>> location, bool isExtreme = true)
        {
            return
                location.Count == 0
                    ? NoResult(method)
                    : (from t in location
                       let prepString = " 第" + (t[0] + 1).ToString() + "个表 "
                       let doseString = " 第" + (t[1] + 1).ToString() + "列 "
                       let repString = " 第" + (t[2] + 1).ToString() + "行 "
                       select
                           method + "检测法检测到" + prepString + doseString + repString + "的值为" + (isExtreme ? "极端" : "稳定") +
                           "异常值.").ToList();
        }

        #endregion

        #endregion
    }
}
