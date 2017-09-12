using System;
using SYPHU.Utilities;

namespace SYPHU.Assay.Data
{
    [Serializable]
    public class UnitData
    {
        public UnitData(bool canBeEmpty = false, double defaultValue = -1.0)
        {
            _canBeEmpty = canBeEmpty;
            Val = defaultValue;
        }

        public double Val { get; private set; }

        public String Unit { get; private set; }

        private String _val;

        private bool _canBeEmpty = false;

        public bool isEmpty;

        public String Parse(String content)
        {
            if (content.Trim() == "")
            {
                isEmpty = true;
            }
            if (Split(content))
            {
                if (!_canBeEmpty && _val == "")
                {
                    return "不能为空.";
                }
                if (_val != "")
                {
                    var rpn = new ReversePolishNotation();
                    if (rpn.IsValid(_val) && rpn.Parse())
                    {
                        Val = rpn.Evaluate();
                    }
                    else
                    {
                        return _val + "无法解析.";
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 将字符串拆分成数值部分和单位部分
        /// 拆分原则：
        ///     无空格->全是数值部分，无单位部分(字符串为"")
        ///     有空格->无论几个空格，第一个空格前的部分为数值部分，空格后的部分为单位部分
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private bool Split(String content)
        {
            if (content == "")
            {
                _val = "";
                Unit = "";
                return true;
            }
            if (content.Contains(" "))
            {
                if (content.Length >= 3)
                {
                    int blankPos = content.IndexOf(" ", StringComparison.Ordinal);
                    if (blankPos > 0 && blankPos < content.Length)
                    {
                        _val = content.Substring(0, blankPos);
                        Unit = content.Substring(blankPos + 1);
                        return true;
                    }
                }
            }
            else
            {
                _val = content;
                Unit = "";
                return true;
            }
            return false;
        }
    }
}
