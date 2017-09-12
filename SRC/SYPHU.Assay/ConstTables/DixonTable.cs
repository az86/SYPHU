﻿
namespace SYPHU.Assay.ConstTables
{
    /// <summary>
    /// Dixon异常值检测法比较表格
    /// </summary>
    public static class DixonTable
    {
        /// <summary>
        /// a = 0.01
        /// </summary>
        private static readonly double[] Table =
            {
                0.988, 0.889, 0.782, 0.698, 0.637, 0.681, 0.635, 0.597, 0.674, 0.642, 0.617, 0.640, 0.618, 0.597, 0.580,
                0.564, 0.550, 0.538, 0.526, 0.516, 0.507, 0.497, 0.489, 0.482, 0.474, 0.468, 0.462, 0.456, 0.450, 0.445,
                0.441, 0.436, 0.432, 0.427, 0.423, 0.419, 0.416, 0.413, 0.409, 0.406, 0.403, 0.400, 0.397, 0.394, 0.391,
                0.389, 0.386, 0.384, 0.382, 0.379, 0.377, 0.375, 0.373, 0.371, 0.369, 0.367, 0.366, 0.363, 0.362, 0.361,
                0.359, 0.357, 0.355, 0.354, 0.353, 0.351, 0.350, 0.348, 0.347, 0.346, 0.344, 0.343, 0.342, 0.341, 0.340,
                0.338, 0.337, 0.336, 0.335, 0.334, 0.333, 0.332, 0.331, 0.330, 0.329, 0.328, 0.327, 0.326, 0.325, 0.324,
                0.323, 0.323, 0.322, 0.321, 0.320, 0.320, 0.319, 0.318
            };

        public static int GetMinRegNum
        {
            get { return 3; }
        }

        public static int GetMaxRegNum
        {
            get { return GetMinRegNum + Table.Length - 1; }
        }

        public static double GetValue(int regNum)
        {
            return Table[regNum - GetMinRegNum];
        }
    }
}
