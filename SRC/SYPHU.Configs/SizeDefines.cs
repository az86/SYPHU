using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SYPHU.Configs
{
    /// <summary>
    /// 页面尺寸定义
    /// </summary>
    public class SizeDefines
    {
        /// <summary>
        /// 默认页面尺寸，A4
        /// </summary>
        public static readonly Size DefaultPageSize = new Size(793, 1122);

        /// <summary>
        /// 默认页边距，A4
        /// </summary>
        public static readonly Thickness PageMargin = new Thickness(46, 50, 46, 46);

        /// <summary>
        /// 元素间的间距
        /// </summary>
        public const int ElementGap = 5;
    }
}
