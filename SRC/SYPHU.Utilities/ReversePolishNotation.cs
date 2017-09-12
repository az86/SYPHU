using System;
using System.Collections.Generic;
using System.Linq;

namespace SYPHU.Utilities
{
    /// <summary>
    /// 逆波兰式
    /// </summary>
    public class ReversePolishNotation
    {
        private readonly Stack<Object> _tokens = new Stack<object>();

        /// <summary>
        /// 最终的逆波兰式堆栈
        /// </summary>
        //public Stack<Object> Tokens
        //{
        //    get { return _tokens; }
        //}

        //private string _RPNExpression;

        //public string RPNExpression
        //{
        //    get
        //    {
        //        if (_RPNExpression == null)
        //        {
        //            foreach (object item in Tokens)
        //            {
        //                if (item is Operand)
        //                {
        //                    _RPNExpression += ((Operand)item).Value + ",";
        //                }
        //                if (item is Operator)
        //                {
        //                    _RPNExpression += ((Operator)item).Value + ",";
        //                }
        //            }
        //        }
        //        return _RPNExpression;
        //    }
        //}

        private string _formula = "";

        /// <summary>
        /// 判断字符串是否有效
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public bool IsValid(string formula)
        {
            return Regularization(formula) && !ValidInput.IsIncludeInvalidStr(_formula) && IsBracketsMatching();
        }

        private bool Regularization(string formula)
        {
            _formula = formula.Trim();
            if (_formula == "")
            {
                return false;
            }
            GetRidOfBlanks();
            ReplacePi();
            ConvertPositiveAndNegativeToPlusAndMinus();
            _formula += "@";
            return true;
        }

        private void ReplacePi()
        {
            if (_formula.Contains("pi"))
            {
                _formula = _formula.Replace("pi", "acos(-1)");
            }
        }

        private void GetRidOfBlanks()
        {
            for (int i = _formula.Length - 1; i >= 0; i--)
            {
                if (_formula.Substring(i, 1) == " ")
                {
                    _formula = _formula.Remove(i, 1);
                }
            }
        }

        private void ConvertPositiveAndNegativeToPlusAndMinus()
        {
            for (int i = _formula.Length - 1; i >= 0; i--)
            {
                string sub = _formula.Substring(i, 1);
                if (sub == "+" || sub == "-")
                {
                    if (i == 0)
                    {
                        _formula = _formula.Insert(i, "0");
                    }
                    else if (_formula.Substring(i - 1, 1) == "(")
                    {
                        _formula = _formula.Insert(i, "0");
                    }
                }
            }
        }

        private bool IsBracketsMatching()
        {
            int numLB = 0;
            int numRB = 0;
            for (int i = 0; i < _formula.Length; i++)
            {
                string tmp = _formula.Substring(i, 1);
                if (tmp == "(")
                {
                    numLB++;
                }
                else if (tmp == ")")
                {
                    numRB++;
                }
            }
            if (_formula.IndexOf(')') < _formula.IndexOf('('))
            {
                return false;
            }
            if (_formula.IndexOf(')') == _formula.IndexOf('(') + 1)
            {
                return false;
            }
            return numLB == numRB;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <returns></returns>
        public bool Parse()
        {
            _tokens.Clear();
            var operands = new Stack<object>(); //操作数堆栈
            var operators = new Stack<Operator>(); //运算符堆栈

            string exp = _formula;
            while (true)
            {
                int curPos = FindOperator(exp, ""); //当前位置
                string curOpd = exp.Substring(0, curPos).Trim(); //当前操作数
                string curOpt = exp.Substring(curPos, 1); //当前运算符
                //存储当前操作数到操作数堆栈
                if (curOpd != "")
                {
                    operands.Push(new Operand(curOpd, curOpd));
                }
                //若当前运算符为结束运算符，则停止循环
                if (curOpt == "@")
                {
                    break;
                }
                //若当前运算符为左括号,则直接存入堆栈。
                if (curOpt == "(")
                {
                    operators.Push(new Operator(OperatorType.LB, "("));
                    exp = exp.Substring(curPos + 1);
                    continue;
                }
                //若当前运算符为右括号,则依次弹出运算符堆栈中的运算符并存入到操作数堆栈,直到遇到左括号为止,此时抛弃该左括号.
                if (curOpt == ")")
                {
                    while (operators.Count > 0)
                    {
                        if (operators.Peek().Type != OperatorType.LB)
                        {
                            operands.Push(operators.Pop());
                        }
                        else
                        {
                            operators.Pop();
                            break;
                        }
                    }
                    exp = exp.Substring(curPos + 1).Trim();
                    continue;
                }


                //调整运算符
                curOpt = Operator.AdjustOperator(curOpt, exp, ref curPos);

                var optType = Operator.ConvertOperator(curOpt); //运算符类型

                //若运算符堆栈为空,或者若运算符堆栈栈顶为左括号,则将当前运算符直接存入运算符堆栈.
                if (operators.Count == 0 || operators.Peek().Type == OperatorType.LB)
                {
                    operators.Push(new Operator(optType, curOpt));
                    exp = exp.Substring(curPos + 1).Trim();
                    continue;
                }

                //若当前运算符优先级大于运算符栈顶的运算符,则将当前运算符直接存入运算符堆栈.
                if (Operator.ComparePriority(optType, operators.Peek().Type) > 0)
                {
                    operators.Push(new Operator(optType, curOpt));
                }
                else
                {
                    //若当前运算符若比运算符堆栈栈顶的运算符优先级低或相等，则输出栈顶运算符到操作数堆栈，直至运算符栈栈顶运算符低于（不包括等于）该运算符优先级，
                    //或运算符栈栈顶运算符为左括号
                    //并将当前运算符压入运算符堆栈。
                    while (operators.Count > 0)
                    {
                        if (Operator.ComparePriority(optType, operators.Peek().Type) <= 0 &&
                            operators.Peek().Type != OperatorType.LB)
                        {
                            operands.Push(operators.Pop());

                            if (operators.Count == 0)
                            {
                                operators.Push(new Operator(optType, curOpt));
                                break;
                            }
                        }
                        else
                        {
                            operators.Push(new Operator(optType, curOpt));
                            break;
                        }
                    }
                }
                exp = exp.Substring(curPos + 1).Trim();
            }
            //转换完成,若运算符堆栈中尚有运算符时,
            //则依序取出运算符到操作数堆栈,直到运算符堆栈为空
            while (operators.Count > 0)
            {
                operands.Push(operators.Pop());
            }
            //调整操作数栈中对象的顺序并输出到最终栈
            while (operands.Count > 0)
            {
                _tokens.Push(operands.Pop());
            }

            return true;
            
        }

        private int FindOperator(string exp, string findOpt)
        {
            for (int i = 0; i < exp.Length; i++)
            {
                string chr = exp.Substring(i, 1);
                if (findOpt != "")
                {
                    if (findOpt == chr)
                    {
                        return i;
                    }
                }
                else
                {
                    if (ValidInput.ValidOperators.Exists(x => x.Contains(chr)))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }



        /// <summary>
        /// 对逆波兰表达式求值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public double Evaluate(double val = 0.0)
        {
            /*
               逆波兰表达式求值算法：
               1、循环扫描语法单元的项目。
               2、如果扫描的项目是操作数，则将其压入操作数堆栈，并扫描下一个项目。
               3、如果扫描的项目是一个二元运算符，则对栈的顶上两个操作数执行该运算。
               4、如果扫描的项目是一个一元运算符，则对栈的最顶上操作数执行该运算。
               5、将运算结果重新压入堆栈。
               6、重复步骤2-5，堆栈中即为结果值。
             */

            if (_tokens.Count == 0) return ConstantsExt.NAN;
            //替换变量
            var tmpT = ReplaceVariable(val);
            double value = 0;
            var opds = new Stack<Operand>();
            Operand opdA, opdB;

            foreach (object item in tmpT)
            {
                if (item is Operand)
                {
                    //如果为操作数则压入操作数堆栈
                    opds.Push((Operand)item);
                }
                else
                {
                    switch (((Operator)item).Type)
                    {
                        #region 乘,*,multiplication

                        case OperatorType.MUL:
                            opdA = opds.Pop();
                            opdB = opds.Pop();
                            if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      double.Parse(opdB.Value.ToString()) *
                                                      double.Parse(opdA.Value.ToString())));
                            }
                            else
                            {
                                throw new Exception("乘运算的两个操作数必须均为数字.");
                            }
                            break;

                        #endregion

                        #region 除,/,division

                        case OperatorType.DIV:
                            opdA = opds.Pop();
                            opdB = opds.Pop();
                            if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                            {
                                if (Math.Abs(double.Parse(opdA.Value.ToString())) < ConstantsExt.Eps())
                                {
                                    throw new Exception("除数不能为0.");
                                }
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      double.Parse(opdB.Value.ToString()) /
                                                      double.Parse(opdA.Value.ToString())));
                            }
                            else
                            {
                                throw new Exception("除运算的两个操作数必须均为数字.");
                            }
                            break;

                        #endregion

                        #region 余,%,modulus

                        case OperatorType.MOD:
                            opdA = opds.Pop();
                            opdB = opds.Pop();
                            if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                            {
                                if (double.Parse(opdA.Value.ToString()) < ConstantsExt.Eps())
                                {
                                    throw new Exception("余数不能为0.");
                                }
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      double.Parse(opdB.Value.ToString()) %
                                                      double.Parse(opdA.Value.ToString())));
                            }
                            else
                            {
                                throw new Exception("余运算的两个操作数必须均为数字.");
                            }
                            break;

                        #endregion

                        #region 指数,^,power

                        case OperatorType.POW:
                            opdA = opds.Pop();
                            opdB = opds.Pop();
                            if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      Math.Pow(double.Parse(opdB.Value.ToString()),
                                                      double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("指数运算的两个操作数必须均为数字.");
                            }
                            break;

                        #endregion

                        #region 加,+,Addition

                        case OperatorType.ADD:
                            opdA = opds.Pop();
                            opdB = opds.Pop();
                            if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      double.Parse(opdB.Value.ToString()) +
                                                      double.Parse(opdA.Value.ToString())));
                            }
                            else
                            {
                                throw new Exception("加运算的两个操作数必须均为数字");
                            }
                            break;

                        #endregion

                        #region 减,-,subtraction

                        case OperatorType.SUB:
                            opdA = opds.Pop();
                            opdB = opds.Pop();
                            if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      double.Parse(opdB.Value.ToString()) -
                                                      double.Parse(opdA.Value.ToString())));
                            }
                            else
                            {
                                throw new Exception("减运算的两个操作数必须均为数字");
                            }
                            break;

                        #endregion

                        #region 正弦,sin

                        case OperatorType.SIN:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      Math.Sin(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                            }
                            else
                            {
                                throw new Exception("正弦运算的操作数必须为角度数字.");
                            }
                            break;

                        #endregion

                        #region 余弦,cos

                        case OperatorType.COS:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      Math.Cos(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                            }
                            else
                            {
                                throw new Exception("余弦运算的操作数必须为角度数字.");
                            }
                            break;

                        #endregion

                        #region 正切,tan

                        case OperatorType.TAN:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      Math.Tan(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                            }
                            else
                            {
                                throw new Exception("正切运算的操作数必须为角度数字.");
                            }
                            break;

                        #endregion

                        #region 余切,cot

                        case OperatorType.COT:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER,
                                                      1.0 / Math.Tan(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                            }
                            else
                            {
                                throw new Exception("余切运算的操作数必须为角度数字.");
                            }
                            break;

                        #endregion

                        #region 反正弦,arcsin

                        case OperatorType.ASIN:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, Math.Asin(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("反正弦运算的操作数必须为数字");
                            }
                            break;

                        #endregion

                        #region 反余弦,arccos

                        case OperatorType.ACOS:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, Math.Acos(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("反余弦运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region 反正切,atan

                        case OperatorType.ATAN:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, Math.Atan(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("反正切运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region 反余切, arccot

                        case OperatorType.ACOT:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, 1.0 / Math.Atan(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("反余切运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region 绝对值,abs

                        case OperatorType.ABS:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, Math.Abs(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("绝对值运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region e对数,ln

                        case OperatorType.LN:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                if (double.Parse(opdA.Value.ToString()) < ConstantsExt.Eps())
                                {
                                    throw new Exception("对数值必须大于0.");
                                }
                                opds.Push(new Operand(OperandType.NUMBER, Math.Log(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("e对数运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region 10对数,lg

                        case OperatorType.LG:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                if (double.Parse(opdA.Value.ToString()) < ConstantsExt.Eps())
                                {
                                    throw new Exception("对数值必须大于0.");
                                }
                                opds.Push(new Operand(OperandType.NUMBER, Math.Log10(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("10对数运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region e指数,exp

                        case OperatorType.EXP:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, Math.Exp(double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("e指数运算的操作数必须为数字.");
                            }
                            break;

                        #endregion

                        #region 10指数,antilg

                        case OperatorType.ANTILG:
                            opdA = opds.Pop();
                            if (Operand.IsNumber(opdA.Value))
                            {
                                opds.Push(new Operand(OperandType.NUMBER, Math.Pow(10.0, double.Parse(opdA.Value.ToString()))));
                            }
                            else
                            {
                                throw new Exception("10指数运算的操作数必须为数字.");
                            }
                            break;

                        #endregion
                    }
                }
            }

            if (opds.Count == 1)
            {
                value = Convert.ToDouble(opds.Pop().Value);
            }

            return value;
        }

        private Stack<Object> ReplaceVariable(double val)
        {
            var tmpS = new Stack<object>();
            for (int i = _tokens.Count - 1; i >= 0; i--)
            {
                if (_tokens.ElementAt(i) is Operand && ((Operand)_tokens.ElementAt(i)).Type == OperandType.VARIABLE)
                {
                    tmpS.Push(new Operand(OperandType.NUMBER, val));
                }
                else
                {
                    tmpS.Push(_tokens.ElementAt(i));
                }
            }
            return tmpS;
        }
    }
}
