using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SYPHU.ViewModels
{
    public class OutputInfo
    {
        public int Index { get; set; }
        public String Content { get; set; }
    }
    public class OutputWinVM : VMBase
    {
        public ObservableCollection<OutputInfo> Items { get; set; }

        private int _itemIndex;

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyPropertyChanged("SelectedIndex");
            }
        }

        public OutputWinVM()
        {
            Items = new ObservableCollection<OutputInfo>();
        }

        public void Clear()
        {
            Items.Clear();
            _itemIndex = 0;
        }

        public void ShowInformation(String information)
        {
            if (information == null)
            {
                return;
            }
            OutputInfo item = new OutputInfo();
            item.Index = _itemIndex++;
            item.Content = information;
            Items.Add(item);
            if (Items.Count == 500/*SYPHU.Data.InterfaceDefinitions._maxInformationStringCount*/)
            {
                Items.RemoveAt(0);
            }
            SelectedIndex = Items.Count - 1;
        }

        public void ShowInformation(List<String> informations)
        {
            if (informations == null)
            {
                return;
            }
            foreach (var info in informations)
            {
                ShowInformation(info);
            }
        }

        public void ShowInformation(List<List<String>> informations)
        {
            if (informations == null)
            {
                return;
            }
            foreach (var infos in informations)
            {
                ShowInformation(infos);
            }
        }
    }
}
