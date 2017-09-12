using System;
using System.Collections.Generic;
using SYPHU.Assay.Data;
using SYPHU.Utilities;

namespace SYPHU.Assay.CalculationInfo
{
    #region 从向导上确定的基本计算信息 + 数据表格计算参数

    /// <summary>
    /// 计算信息集合
    /// </summary>
    [Serializable]
    public class InitCalculationInfo
    {
        /// <summary>
        /// 初始化计算信息集合
        /// </summary>
        /// <param name="lang">语言</param>
        /// <param name="size">数据大小</param>
        /// <param name="calcCase">计算类型</param>
        /// <param name="method">计算方法</param>
        /// <param name="design">试验设计</param>
        /// <param name="model">计算方法</param>
        /// <param name="type">反应类型</param>
        /// <param name="formula">转换公式</param>
        /// <param name="checkMethods">异常值检测方法列表</param>
        /// <param name="edString">ED法字符串</param>
        /// <param name="percent">EDxx法百分比值</param>
        /// <param name="userDefinedFormula">用户自定义的转换公式，非自定义时为空</param>
        /// <param name="product">药品名称</param>
        public void Init(String product,  OutLang lang, DataSize size, CalcCases calcCase, Methods method, Designs design, Models model, Types type, DataTransformationFormula formula, List<AbnormalDataCheckMethods> checkMethods, String edString, String percent, String userDefinedFormula = "")
        {
            ProductName = product;
            Lang = lang;
            DataSize = size;
            CalcCase = calcCase;
            Method = method;
            Design = design;
            Model = model;
            Type = type;
            DataTransFormula = formula;
            AbnDataCheckMethods = checkMethods;
            EDString = edString;
            EDPercent = percent;
            UserDefinedFormula = userDefinedFormula;
        }

        public String ProductName;

        /// <summary>
        /// 语言
        /// </summary>
        public OutLang Lang = OutLang.Chinese;

        /// <summary>
        /// 数据表格大小
        /// </summary>
        public DataSize DataSize = new DataSize();

        /// <summary>
        /// 计算类型
        /// </summary>
        public CalcCases CalcCase = CalcCases.Single;

        /// <summary>
        /// 统计模型
        /// </summary>
        public Methods Method;

        /// <summary>
        /// ED未解析
        /// </summary>
        public String EDPercent = "50";

        /// <summary>
        /// ED类方法实际名称
        /// </summary>
        public String EDString = "ED";

        /// <summary>
        /// ED统计模型百分比：默认50
        /// </summary>
        public List<int> EDPercentList
        {
            get { return ParseString.ParseEDString(EDPercent); }
        }

        /// <summary>
        /// 试验设计
        /// </summary>
        public Designs Design;

        /// <summary>
        /// 计算方法
        /// </summary>
        public Models Model = Models.Null;

        /// <summary>
        /// 反应类型：默认为定量反应Graded，S型曲线和ED法通常是定性反应Quantal
        /// </summary>
        public Types Type = Types.Graded;

        /// <summary>
        /// 数据转换方法：不转换、内嵌（常用）方法、用户自定义方法
        /// </summary>
        public DataTransformationFormula DataTransFormula = DataTransformationFormula.Null;

        /// <summary>
        /// 数据转换公式：数据转换方法选择为“自定义”方法时，读取字符串公式
        /// </summary>
        public string UserDefinedFormula = "";

        /// <summary>
        /// 异常值预选检测方法：可多选，用于预览比较，需要用户确定选哪种检测方法
        /// </summary>
        private List<AbnormalDataCheckMethods> _abnDataCheckMethods = new List<AbnormalDataCheckMethods>();

        public List<AbnormalDataCheckMethods> AbnDataCheckMethods
        {
            get
            {
                if (_abnDataCheckMethods.Count == 0)
                {
                    _abnDataCheckMethods.Add(AbnormalDataCheckMethods.Null);
                }
                if (_abnDataCheckMethods.Contains(AbnormalDataCheckMethods.Null) && _abnDataCheckMethods.Count != 1)
                {
                    _abnDataCheckMethods.Remove(AbnormalDataCheckMethods.Null);
                }
                return _abnDataCheckMethods;
            }
            set { _abnDataCheckMethods = value; }
        }

        /// <summary>
        /// 异常值处理方法：根据统计模型自动选项，用户不可选，不需要在界面显示
        /// </summary>
        public AbnormalDataProcessMethod AbnDataProcessMethod
        {
            get
            {
                switch (Design)
                {
                    case Designs.RandomisedBlock:
                        return AbnormalDataProcessMethod.EquationSet;
                    case Designs.LatinSquare:
                        return AbnormalDataProcessMethod.EquationSet;
                    default:
                        return AbnormalDataProcessMethod.Average;
                }
            }
        }

        /// <summary>
        /// 效价值、每表1个
        /// </summary>
        public List<UnitData> UnitA;

        /// <summary>
        /// dose值，每列1个
        /// </summary>
        public List<List<UnitData>> Unitd;

        /// <summary>
        /// 稀释倍数值、每表1个
        /// </summary>
        public List<UnitData> UnitDil; 

        /// <summary>
        /// 是否使用稀释倍数值
        /// </summary>
        public bool UseDilutionsTimes
        {
            get
            {
                if (UnitDil == null || UnitDil.Count == 0)
                {
                    return false;
                }
                return UnitDil.TrueForAll(t => t.Val > 0);
            }
        }
    }
    
    #endregion 从向导上确定的基本计算信息
}