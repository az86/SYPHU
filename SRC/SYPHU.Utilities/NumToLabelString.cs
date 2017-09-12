using System;

namespace SYPHU.Utilities
{
    public static class NumToLabelString
    {
        public static String IntToLabelString(this int val)
        {
            if (val <= 26)
            {
                return Convert.ToString(Convert.ToChar(val + 'a'));
            }
            return IntToLabelString(val - 26);
        }
    }
}
