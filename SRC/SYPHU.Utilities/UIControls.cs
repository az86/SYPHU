using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace SYPHU.Utilities
{
    public static class UIControls
    {
        /// <summary>
        ///     计算文字宽度
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="typeface">字体</param>
        /// <param name="emSize">字号</param>
        /// <returns></returns>
        public static double CalculateTextWidth(string text, Typeface typeface = null, double emSize = 0)
        {
            if (typeface == null)
            {
                typeface = new Typeface("Global User Interface");
            }
            if (emSize <= 0 )
            {
                emSize = 14;
            }
            var lineText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                                             emSize, Brushes.Black);
            double width = lineText.WidthIncludingTrailingWhitespace;
            return width;
        }
    }
}
