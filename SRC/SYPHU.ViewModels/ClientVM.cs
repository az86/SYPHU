using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Tables;

namespace SYPHU.ViewModels
{
    public class ClientVM:VMBase
    {
        public delegate void OnUpdateElements<T, S, A, A1>(T sender, S dest, A offset, A1 endIndex);

        public event OnUpdateElements<object, List<TableDesc>, int, int> OnUpdateTablesEvent;

        public event OnUpdateElements<object, List<PlotInfo>, int, int> OnUpdatePlotsEvent;

        public event OnUpdateElements<object, List<MergeCalcPlotInfo>, int, int> OnUpdateMergePlotsEvent;

        public event OnUpdateElements<object, Histogram, int, int> OnUpdateHistogramEvent;

        public event EventHandler OnUnDoEvent;

        public event EventHandler OnReDoEvent;

        public event EventHandler OnCopyTableEvent;

        public event EventHandler OnPasteTableEvent;

        public void UpdateElements(List<TableDesc> tableList, int offset = 0, int endIndex = int.MaxValue)
        {
            OnUpdateTablesEvent.Invoke(this, tableList, offset, endIndex);
        }

        public void UpdateElements(List<PlotInfo> plotList, int offset = 0, int endIndex = int.MaxValue)
        {
            OnUpdatePlotsEvent.Invoke(this, plotList, offset, endIndex);
        }

        public void UpdateElements(List<MergeCalcPlotInfo> plotList, int offset = 0, int endIndex = int.MaxValue)
        {
            OnUpdateMergePlotsEvent.Invoke(this, plotList, offset, endIndex);
        }

        public void UpdateElements(Histogram hist, int reserve = 0, int reserve1 = 0)
        {
            OnUpdateHistogramEvent.Invoke(this, hist, reserve, reserve);
        }

        public void UnDo()
        {
            OnUnDoEvent.Invoke(this, null);
        }

        public void ReDo()
        {
            OnReDoEvent.Invoke(this, null);
        }

        public void CopyTable()
        {
            OnCopyTableEvent.Invoke(this, null);
        }

        public void PasteTable()
        {
            OnPasteTableEvent.Invoke(this, null);
        }
    }
}
