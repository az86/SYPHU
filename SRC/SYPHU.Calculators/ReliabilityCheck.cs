using System;
using System.Collections.Generic;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Results.ReliabilityCheck;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 可靠性检测
    /// </summary>
    public class ReliabilityCheck
    {
        private OutLang _lang;

        private List<bool> _bList;

        private List<String> _conclusionList;

        /// <summary>
        /// 变异项可靠性结论列表
        /// </summary>
        public List<String> ConclusionList
        {
            get { return _conclusionList; }
        }

        /// <summary>
        /// 最终结论
        /// </summary>
        public String FinalConclusion
        {
            get { return _finalConclusion; }
        }

        private String _finalConclusion;

        /// <summary>
        /// 检测
        /// </summary>
        /// <param name="variationList">变异项列表：字符串、概率值</param>
        public void DoCheck(Dictionary<String, double> variationList, OutLang lang)
        {
            InitDict();
            _lang = lang;
            _bList = new List<bool>();
            _conclusionList = variationList.Select(d => SingleSourceChecker(d.Key, d.Value)).ToList();
            _finalConclusion = ConstStrings.SingleCalcReliabilityCheckConclution[_lang][_bList.All(t => t)];
        }

        /// <summary>
        /// 单个变异项检测
        /// </summary>
        /// <param name="sourceString">变异项</param>
        /// <param name="pValue">概率值</param>
        /// <returns></returns>
        private String SingleSourceChecker(String sourceString, double pValue)
        {
            //判断
            bool b = DoCompare(pValue, _compareStandardDict[VariationSourceParse(sourceString)],
                               _compareMethodDict[VariationSourceParse(sourceString)]);

            //试品间判断但不作为最终判断标准
            if (VariationSourceParse(sourceString) != VariationSources.Prep)
            {
                _bList.Add(b);
            }
            
            return sourceString +
                   ConstStrings.VariationSourceCheckResult[_lang][VariationSourceParse(sourceString)][b] + "\n";
        }

        /// <summary>
        /// 解析变异项字符串
        /// </summary>
        /// <param name="variationSourceString"></param>
        /// <returns></returns>
        private VariationSources VariationSourceParse(String variationSourceString)
        {
            if (variationSourceString.Contains(ConstStrings.VariationSourcesDict[_lang][VariationSources.LinT]))
            {
                return VariationSources.LinT;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.Prep])
            {
                return VariationSources.Prep;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.Reg])
            {
                return VariationSources.Reg;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.Par])
            {
                return VariationSources.Par;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.Lin])
            {
                return VariationSources.Lin;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.LinS])
            {
                return VariationSources.LinS;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.Inters])
            {
                return VariationSources.Inters;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.DaysPrep])
            {
                return VariationSources.DaysPrep;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.DaysReg])
            {
                return VariationSources.DaysReg;
            }
            if (variationSourceString == ConstStrings.VariationSourcesDict[_lang][VariationSources.DaysPar])
            {
                return VariationSources.DaysPar;
            }
            return VariationSources.Prep;
        }

        /// <summary>
        /// 判断方法： ">"、"<"
        /// </summary>
        private readonly Dictionary<VariationSources, CompareMethods> _compareMethodDict = new Dictionary<VariationSources, CompareMethods>();

        /// <summary>
        /// 标准值
        /// </summary>
        private readonly Dictionary<VariationSources, double> _compareStandardDict = new Dictionary<VariationSources, double>();

        /// <summary>
        /// 初始化判断方法、标准值
        /// </summary>
        private void InitDict()
        {
            _compareMethodDict.Add(VariationSources.Prep, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.Prep, 0.05);

            _compareMethodDict.Add(VariationSources.Reg, CompareMethods.LessThan);
            _compareStandardDict.Add(VariationSources.Reg, 0.01);

            _compareMethodDict.Add(VariationSources.Par, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.Par, 0.05);

            _compareMethodDict.Add(VariationSources.Lin, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.Lin, 0.05);

            _compareMethodDict.Add(VariationSources.LinS, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.LinS, 0.05);

            _compareMethodDict.Add(VariationSources.LinT, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.LinT, 0.05);

            _compareMethodDict.Add(VariationSources.Inters, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.Inters, 0.05);

            _compareMethodDict.Add(VariationSources.DaysPar, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.DaysPar, 0.05);

            _compareMethodDict.Add(VariationSources.DaysPrep, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.DaysPrep, 0.05);

            _compareMethodDict.Add(VariationSources.DaysReg, CompareMethods.MoreThan);
            _compareStandardDict.Add(VariationSources.DaysReg, 0.05);
        }

        /// <summary>
        /// 根据方法判断两个数值大小
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        private bool DoCompare(double v1, double v2, CompareMethods compare)
        {
            switch (compare)
            {
                case CompareMethods.EqualTo:
                    return Math.Abs(v1 - v2) < ConstantsExt.Eps();
                case CompareMethods.LessThan:
                    return v1 < v2;
                case CompareMethods.MoreThan:
                    return v1 > v2;
                case CompareMethods.NoLessThan:
                    return v1 >= v2;
                case CompareMethods.NoMoreThan:
                    return v1 <= v2;
                default:
                    return false;
            }
        }
    }
}
