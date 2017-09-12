using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SYPHU.ViewModels;
using SYPHU.Views.CommonControls;

namespace SYPHU.Views
{
    /// <summary>
    /// Client.xaml 的交互逻辑
    /// </summary>
    public partial class Client : UserControl
    {
        internal readonly ElementsMgr ElementMgr;

        public Client()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(Client_DataContextChanged);
            ElementMgr = new ElementsMgr(visualElements, this);
        }

        void Client_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as ClientVM;
            if (vm != null)
            {
                vm.OnUpdateTablesEvent += new ClientVM.OnUpdateElements<object, List<Assay.Tables.TableDesc>, int, int>(ViewModel_OnUpdateTablesEvent);
                vm.OnUpdatePlotsEvent += new ClientVM.OnUpdateElements<object, List<Assay.Plots.PlotInfo>, int, int>(ViewModel_OnUpdatePlotsEvent);
                vm.OnUpdateMergePlotsEvent += new ClientVM.OnUpdateElements<object, List<Assay.Plots.MergeCalcPlotInfo>, int, int>(ViewModel_OnUpdateMergePlotsEvent);
                vm.OnUpdateHistogramEvent += new ClientVM.OnUpdateElements<object, Assay.Plots.Histogram, int, int>(vm_OnUpdateHistogramEvent);
                vm.OnUnDoEvent += vm_OnUnDoEvent;
                vm.OnReDoEvent += new System.EventHandler(vm_OnReDoEvent);
                vm.OnCopyTableEvent += new System.EventHandler(vm_OnCopyTableEvent);
                vm.OnPasteTableEvent += new System.EventHandler(vm_OnPasteTableEvent);
            }
        }

        void vm_OnUpdateHistogramEvent(object sender, Assay.Plots.Histogram dest, int offset, int endIndex)
        {
            ElementMgr.UpdateCharts(dest);
        }

        void vm_OnPasteTableEvent(object sender, System.EventArgs e)
        {
            ElementMgr.PasteTable();
        }

        void vm_OnCopyTableEvent(object sender, System.EventArgs e)
        {
            ElementMgr.CopyTable();
        }

        void vm_OnReDoEvent(object sender, System.EventArgs e)
        {
            ElementMgr.ReDo();
        }

        void vm_OnUnDoEvent(object sender, System.EventArgs e)
        {
            ElementMgr.UnDo();
        }

        void ViewModel_OnUpdateTablesEvent(object sender, List<Assay.Tables.TableDesc> dest, int offset, int endIndex)
        {
            ElementMgr.Clear();
            ElementMgr.UpdateTables(dest, offset, endIndex);
        }

        void ViewModel_OnUpdatePlotsEvent(object sender, List<Assay.Plots.PlotInfo> dest, int offset, int endIndex)
        {
            ElementMgr.UpdateCharts(dest, offset, endIndex);
        }

        void ViewModel_OnUpdateMergePlotsEvent(object sender, List<Assay.Plots.MergeCalcPlotInfo> dest, int offset, int endIndex)
        {
            ElementMgr.UpdateCharts(dest, offset, endIndex);
        }
    }
}
