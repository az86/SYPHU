using System;

namespace SYPHU.Utilities
{
    /// <summary>
    /// 分布
    /// </summary>
    public static class Distributions
    {
        private static double GammaCoff(int n)
        {
            if (n == 4)
            {
                return 0.75;
            }
            if (n == 3)
            {
                return 2.0/ConstantsExt.Pi;
            }
            return (n - 1.0)/(n - 2.0)*GammaCoff(n - 2);
        }

        //步长修改0.000001->0.0001
        public static double Dist_t(double alpha, int n, double step = 0.0001)
        {
            double sqrtV = Math.Sqrt(Convert.ToDouble(n));
            double sum = (0.5 - 0.5*alpha)/(GammaCoff(n)/sqrtV)/step;
            double y = 1;
            double mstep = 0;
            sum += 0.5;
            do
            {
                sum -= y;
                mstep += step;
                y = Math.Pow((1 + mstep * mstep / n), -(n + 1.0) * 0.5);
            } while (sum - y*0.5 > ConstantsExt.Eps(-20));
            return mstep;
        }

        public static double RomanovskyTValue(double p, int n)
        {
            if (n<5)
            {
                return ConstantsExt.INF;
            }
            return Dist_t(p, n-2)*Math.Sqrt(n/(n - 1.0));
        }

        public static double GrubbsTValue(double p, int n, int ridNum)
        {
            if (n < 5)
            {
                return ConstantsExt.INF;
            }
            double t = Dist_t( p/(n - ridNum + 1.0), n - ridNum - 1);
            return (n - ridNum)*Math.Sqrt(t*t/(n - ridNum - 1.0 + t*t))/Math.Sqrt(n - ridNum + 1.0);
        }

        public static double Dist_t(double f)
        {
            double f2 = f * f;
            double f4 = f2 * f2;
            return 1.959964 + 2.37228 / f + 2.82202 / f2 + 2.56449 / (f2 * f) + 1.51956 / f4 + 1.02579 / (f4 * f) + 0.44210 / (f4 * f2 * f);
        }

        public static double Dist_t(int f)
        {
            return Dist_t(Convert.ToDouble(f));
        }

        public static double Dist_F(double f1, double f2, double F)
        {
            if (F < ConstantsExt.Eps())
            {
                F = ConstantsExt.Eps();
            }
            double x, s, t, p;
            if ((int)(f1) % 2 == 0)
            {
                x = f1 / (f1 + f2 / F);
                s = 1.0;
                t = 1.0;
                for (int i = 2; i <= (int)(f1) - 2; i += 2)
                {
                    t *= x * (f2 + i - 2.0) / i;
                    s += t;
                }
                p = s * Math.Pow((1.0 - x), (f2 / 2.0));
            }
            else if ((int)(f2) % 2 == 0)
            {
                x = f2 / (f2 + f1 * F);
                s = 1.0;
                t = 1.0;
                for (int i = 2; i <= (int)(f2) - 2; i += 2)
                {
                    t *= x * (f1 + i - 2.0) / i;
                    s += t;
                }
                p = 1.0 - s * Math.Pow((1.0 - x), (f1 / 2.0));
            }
            else
            {
                x = Math.Atan(Math.Sqrt(f1 * F / f2));
                double cs = Math.Cos(x);
                double sn = Math.Sin(x);
                x /= 2.0;
                s = 0;
                t = sn * cs / 2.0;
                double v = 0;
                double w = 1.0;
                for (int i = 2; i <= (int)(f2) - 1; i += 2)
                {
                    s += t;
                    t *= cs * cs * i / (i + 1.0);
                }
                for (int i = 1; i <= (int)(f1) - 2; i += 2)
                {
                    v += w;
                    w *= (f2 + i) / (i + 2.0) * sn * sn;
                }
                p = 1.0 + (t * f2 * v - x - s) * 4.0 / ConstantsExt.Pi;
            }

            return p;
        }

        public static double Dist_F(int f1, int f2, double F)
        {
            return Dist_F(Convert.ToDouble(f1), Convert.ToDouble(f2), F);
        }

        public static double Dist_X2(double f, double X2)
        {
            if (X2 < ConstantsExt.Eps())
            {
                X2 = ConstantsExt.Eps();
            }
            double s, t, p;
            if ((int)(f) % 2 == 0)
            {
                s = 0;
                t = Math.Exp(-X2 / 2.0);
                for (int i = 2; i <= f; i += 2)
                {
                    s += t;
                    t *= X2 / i;
                }
                p = s;
            }
            else
            {
                s = 0;
                t = Math.Sqrt(X2) * Math.Exp(-X2 / 2.0) / Math.Sqrt(ConstantsExt.Pi / 2.0);
                for (int i = 3; i <= f; i += 2)
                {
                    s += t;
                    t *= X2 / i;
                }
                p = 2.0 + s - 2.0 * Dist_Phi(Math.Sqrt(X2));
            }
            return p;
        }

        public static double Dist_X2(int f, double X2)
        {
            return Dist_X2(Convert.ToDouble(f), X2);
        }

        /// <summary>
        /// x->phi
        /// </summary>
        /// <param name="x"></param>
        /// <returns>phi</returns>
        public static double Dist_Phi(double x)
        {
            if (x > 8.15)
            {
                return 1.0;
            }
            if (x < 0)
            {
                return 1 - Dist_Phi(-x);
            }
            double s = 0, t = x, i = 1.0;
            do
            {
                s += t;
                i += 2.0;
                t *= x * x / i;
            } while (t >= ConstantsExt.Eps());
            return 0.5 + s * Math.Exp(-x * x / 2.0) / Math.Sqrt(2.0 * ConstantsExt.Pi);
        }

        //步长修改0.000001->0.0001
        /// <summary>
        /// phi->x
        /// </summary>
        /// <param name="phi"></param>
        /// <param name="step"></param>
        /// <returns>x</returns>
        public static double Dist_Phi_Inv(double phi, double step = 0.0001)
        {
            if (Math.Abs(phi - 0.5) < ConstantsExt.Eps() || Math.Abs(phi - 0.5) < step/Math.Sqrt(2*ConstantsExt.Pi))
            {
                return 0.0;
            }
            double val;
            double intsum = 0;
            double nstep = 0;
            do
            {
                nstep += step;
                intsum += 0.5*Math.Exp(-0.5*(nstep-step)*(nstep-step)) + 0.5*Math.Exp(-0.5*nstep*nstep);
                val = Math.Abs(phi - 0.5) - step/Math.Sqrt(2*ConstantsExt.Pi)*intsum;
            } while (val > ConstantsExt.Eps());
            return phi > 0.5 ? nstep : -nstep;
        }
    }
}
