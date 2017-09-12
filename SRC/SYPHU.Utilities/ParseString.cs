using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYPHU.Utilities
{
    public static class ParseString
    {
        public static List<int> ParseEDString(String str)
        {
            String[] edSplit = str.Trim().Split(',');
            var list = new List<int>();
            foreach (var s in edSplit)
            {
                int val;
                if (int.TryParse(s.Trim(), out val))
                {
                    list.Add(val);
                    if (val <= 0 || val >= 100)
                    {
                        throw new Exception("计算值应在0~100之间.");
                    }
                }
                else
                {
                    throw new Exception("请输入整数值，多个值用“,”隔开.");
                }
            }
            return list;
        }
    }
}
