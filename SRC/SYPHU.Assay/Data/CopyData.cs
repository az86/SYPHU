using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SYPHU.Assay.Data
{
    public class CopyData
    {
        public String Ass { get; set; }

        public String Dil { get; set; }

        public List<String> DoseList { get; set; }

        public List<List<String>> DataListList { get; set; }

        public void CopyToClipBoard()
        {
            var clipList = new List<List<String>>();
            if (Ass != null)
            {
                clipList.Add(new List<String> { Ass });
            }
            if (Dil != null)
            {
                clipList.Add(new List<String> { Dil });
            }
            if (DoseList != null)
            {
                clipList.Add(DoseList);
            }
            clipList.AddRange(DataListList);
            var tmpStr = String.Empty;
            foreach (var strs in clipList)
            {
                tmpStr = strs.Aggregate(tmpStr, (current, str) => current + (str + '\t'));
                tmpStr += "\r\n";
            }
            Clipboard.SetText(tmpStr);
        }
    }
}
