using System.Collections.Generic;

namespace SYPHU.Utilities
{
    #region 定义操作数枚举类型

    public enum OperandType
    {
        NUMBER = 1,

        VARIABLE = 2,

        ERROR = 3
    }

    #endregion

    #region 操作数类

    public class Operand
    {
        public Operand(OperandType type, object value)
        {
            Type = type;
            Value = value;
        }

        public Operand(string opd, object value)
        {
            Type = ConvertOperand(opd);
            Value = value;
        }

        public OperandType Type { get; set; }

        public object Value { get; set; }

        public static OperandType ConvertOperand(string opd)
        {
            if (IsNumber(opd))
            {
                return OperandType.NUMBER;
            }
            if (IsVariable(opd))
            {
                return OperandType.VARIABLE;
            }
            return OperandType.ERROR;
        }


        public static bool IsNumber(object value)
        {
            double val;
            return double.TryParse(value.ToString(), out val);
        }

        private static bool IsVariable(object value)
        {
            string val = value.ToString();
            return val.Equals("y");
        }
    }

    #endregion

    #region 运算符枚举类型

    public enum OperatorType
    {
        LB = 0, RB = 1,

        ABS = 10,

        SIN = 20, COS = 21, TAN = 22, COT = 23, /*SEC = 24, CSC = 25,*/ ASIN = 26, ACOS = 27, ATAN = 28, ACOT = 29,

        LN = 30, LG = 31, EXP = 32, ANTILG = 33, 

        MUL = 40, DIV = 41, MOD = 42, POW = 43,

        ADD = 50, SUB = 51, 

        ERR = 255
    }

    public class Operator
    {
        public Operator(OperatorType type, string value)
        {
            Type = type;
            Value = value;
        }

        public OperatorType Type { get; set; }

        public string Value { get; set; }

        public static bool IsBinaryOperator(OperatorType inOperatorType)
        {
            return inOperatorType == OperatorType.ADD || inOperatorType == OperatorType.SUB
                || inOperatorType == OperatorType.MUL || inOperatorType == OperatorType.DIV
                || inOperatorType == OperatorType.MOD || inOperatorType == OperatorType.POW;
        }

        public static OperatorType ConvertOperator(string ope)
        {
            switch (ope)
            {
                case "(":
                    return OperatorType.LB;
                case ")":
                    return OperatorType.RB;
                case "abs":
                    return OperatorType.ABS;
                case "sin":
                    return OperatorType.SIN;
                case "cos":
                    return OperatorType.COS;
                case "tan":
                    return OperatorType.TAN;
                case "cot":
                    return OperatorType.COT;
                case "asin":
                    return OperatorType.ASIN;
                case "acos":
                    return OperatorType.ACOS;
                case "atan":
                    return OperatorType.ATAN;
                case "acot":
                    return OperatorType.ACOT;
                case "ln":
                    return OperatorType.LN;
                case "lg":
                    return OperatorType.LG;
                case "exp":
                    return OperatorType.EXP;
                case "antilg":
                    return OperatorType.ANTILG;
                case "*":
                    return OperatorType.MUL;
                case "/":
                    return OperatorType.DIV;
                case "%":
                    return OperatorType.MOD;
                case "^":
                    return OperatorType.POW;
                case "+":
                    return OperatorType.ADD;
                case "-":
                    return OperatorType.SUB;
                default:
                    return OperatorType.ERR;
            }
        }

        public static string AdjustOperator(string currentOpt, string currentExp, ref int currentOptPos)
        {
            switch (currentOpt)
            {
                case "a":
                    if (currentExp.Substring(currentOptPos, 6) == "antilg")
                    {
                        currentOptPos += 5;
                        return "antilg";
                    }
                    else if (currentExp.Substring(currentOptPos, 4) == "atan")
                    {
                        currentOptPos += 3;
                        return "atan";
                    }
                    else if (currentExp.Substring(currentOptPos, 4) == "asin")
                    {
                        currentOptPos += 3;
                        return "asin";
                    }
                    else if (currentExp.Substring(currentOptPos, 4) == "acos")
                    {
                        currentOptPos += 3;
                        return "acos";
                    }
                    else if (currentExp.Substring(currentOptPos, 4) == "acot")
                    {
                        currentOptPos += 3;
                        return "acot";
                    }
                    else if (currentExp.Substring(currentOptPos, 3) == "abs")
                    {
                        currentOptPos += 2;
                        return "abs";
                    }
                    return "error";
                case "s":
                    if (currentExp.Substring(currentOptPos, 3) == "sin")
                    {
                        currentOptPos += 2;
                        return "sin";
                    }
                    return "error";
                case "c":
                    if (currentExp.Substring(currentOptPos, 3) == "cos")
                    {
                        currentOptPos += 2;
                        return "cos";
                    }
                    else if (currentExp.Substring(currentOptPos, 3) == "cot")
                    {
                        currentOptPos += 2;
                        return "cot";
                    }
                    return "error";
                case "t":
                    if (currentExp.Substring(currentOptPos, 3) == "tan")
                    {
                        currentOptPos += 2;
                        return "tan";
                    }
                    return "error";
                case "e":
                    if (currentExp.Substring(currentOptPos, 3) == "exp")
                    {
                        currentOptPos += 2;
                        return "exp";
                    }
                    return "error";
                case "l":
                    if (currentExp.Substring(currentOptPos, 2) == "ln")
                    {
                        currentOptPos += 1;
                        return "ln";
                    }
                    else if (currentExp.Substring(currentOptPos, 2) == "lg")
                    {
                        currentOptPos += 1;
                        return "lg";
                    }
                    return "error";
                default:
                    return currentOpt;
            }
        }

        public static int ComparePriority(OperatorType optA, OperatorType optB)
        {
            if (optA == optB)
            {
                //A、B优先级相等
                return 0;
            }

            //乘,除,余(*,/,%)
            if ((optA >= OperatorType.MUL && optA <= OperatorType.POW) &&
                (optB >= OperatorType.MUL && optB <= OperatorType.POW))
            {
                return 0;
            }
            //加,减(+,-)
            if ((optA >= OperatorType.ADD && optA <= OperatorType.SUB) &&
                (optB >= OperatorType.ADD && optB <= OperatorType.SUB))
            {
                return 0;
            }
            //三角函数
            if ((optA >= OperatorType.ABS && optA <= OperatorType.ANTILG) &&
                (optB >= OperatorType.ABS && optB <= OperatorType.ANTILG))
            {
                return 0;
            }

            if (optA < optB)
            {
                //A优先级高于B
                return 1;
            }

            //A优先级低于B
            return -1;
        }

    }

    #endregion

    #region 有效输入
    public static class ValidInput
    {
        public static readonly List<string> ValidOperands = new List<string>(new[]
            {
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ".", "y"
            });

        public static readonly List<string> ValidOperators = new List<string>(new[]
            {
                "abs(","asin(", "acos(", "atan(", "acot(",
                "sin(", "cos(", "tan(", "cot(", 
                "ln(", "lg(", "exp(", "antilg(",
                "*", "/", "%", "^",
                "+", "-",
                "(", ")",
                "@"
            });

        public static bool IsIncludeInvalidStr(string str)
        {
            string tmp = str;
            foreach (string item in ValidOperands)
            {
                while (tmp.Contains(item))
                {
                    tmp = tmp.Remove(tmp.IndexOf(item), item.Length);
                }
            }
            foreach (string item in ValidOperators)
            {
                while (tmp.Contains(item))
                {
                    tmp = tmp.Remove(tmp.IndexOf(item), item.Length);
                }
            }
            return tmp.Trim() != "";
        }
    }
    #endregion
}
