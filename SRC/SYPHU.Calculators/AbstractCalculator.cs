using System;
using System.Collections.Generic;
using System.Globalization;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 计算器虚基类
    /// </summary>
    public abstract class AbstractCalculator
    {
        /// <summary>
        /// 试验数据（校正后）
        /// </summary>
        protected AssayData AssData = new AssayData();

        /// <summary>
        /// 计算信息
        /// </summary>
        protected InitCalculationInfo CalcInfo = new InitCalculationInfo();

        /// <summary>
        /// 极端异常值个数
        /// </summary>
        protected int ExtremeAbnormalDataNum;

        /// <summary>
        /// 方差分析表
        /// </summary>
        public readonly VarianceAnalysisTable VATable = new VarianceAnalysisTable();

        /// <summary>
        /// 可靠性检验
        /// </summary>
        protected readonly ReliabilityCheck ReliabilityChecker = new ReliabilityCheck();

        /// <summary>
        /// 可靠性检验结论表格
        /// </summary>
        public readonly ReliabilityConclusionTable ConclusionTable = new ReliabilityConclusionTable();

        /// <summary>
        /// 变异项列表
        /// </summary>
        protected readonly Dictionary<String, double> VariationList = new Dictionary<String, double>();

        /// <summary>
        /// 效价估计列表
        /// </summary>
        public readonly List<PotencyEstimateTable> PETableList = new List<PotencyEstimateTable>();

        /// <summary>
        /// 作图信息
        /// </summary>
        public List<PlotInfo> PlotsInfo;

        /// <summary>
        /// 读取计算数据
        /// </summary>
        /// <param name="assData"></param>
        public void LoadCalcData(AssayData assData)
        {
            AssData = assData;
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="calculationInfo">计算信息</param>
        /// <param name="extremeAbnormalDataNum">极端异常值个数</param>
        /// <param name="bList">拉丁方b值</param>
        public abstract void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum, List<List<double>> bList);

        /// <summary>
        /// 创建表格
        /// </summary>
        public abstract void CreateTable();

        /// <summary>
        /// 创建作图信息
        /// </summary>
        /// <param name="h"></param>
        public abstract void CreatePlotInfo(int h);

        /// <summary>
        /// 填充方差分析值
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="val">值</param>
        /// <param name="validNum">填充有效值个数</param>
        protected void FillVAValues(int row, BasicVarianceAnalysisValues val, int validNum = 5)
        {
            switch (validNum)
            {
                case 5:
                    VATable.Table.Cells[row][5].Content = PValueCheck(val.PValue);
                    VATable.Table.Cells[row][4].Content = val.FValue.ToString("F5");
                    goto case 3;
                case 3:
                    VATable.Table.Cells[row][3].Content = val.MeanSquare.ToString("F5");
                    VATable.Table.Cells[row][2].Content = val.SquareSum.ToString("F5");
                    VATable.Table.Cells[row][1].Content = val.FreedomDegree.ToString(CultureInfo.InvariantCulture);
                    break;
            }
        }

        /// <summary>
        /// p值加*判断
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        private String PValueCheck(double pValue)
        {
            if (0.05>pValue && pValue>=0.01)
            {
                return pValue.ToString("F5") + " (*)";
            }
            if (0.01 > pValue && pValue >= 0.001)
            {
                return pValue.ToString("F5") + " (**)";
            }
            if (0.001 > pValue/* && pValue >= 0*/)
            {
                return pValue.ToString("F5") + " (***)";
            }
            return pValue.ToString("F5");
        }

        /// <summary>
        /// 填充效价估计值
        /// </summary>
        /// <param name="groupId">组号</param>
        /// <param name="row">行号</param>
        /// <param name="val">值</param>
        /// <param name="precision">精度</param>
        protected void FillPEValues(int groupId, int row, BasicPEValues val, String precision = "P2")
        {
            PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol].Content =
                val.Lower.ToString(precision);
            PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol + 1].Content =
                val.Est.ToString(precision);
            PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol + 2].Content =
                val.Upper.ToString(precision);
        }

        protected abstract void CalcSM(int i, double secItem);
    }
}
