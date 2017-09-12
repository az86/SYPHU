using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYPHU.ViewModels
{
    public class MainWindowVM:VMBase
    {
        public static readonly MainWindowVM Instance = new MainWindowVM();

        String _currentFilePath;

        public String CurrentFilePath
        {
            get { return _currentFilePath; }
            set 
            { 
                _currentFilePath = value;
                NotifyPropertyChanged("CurrentFilePath");
                FileChangedEvent.Invoke(this, _currentFilePath);
            }
        }

        public delegate void FileChangedEventHandler<T, S>(T sender, S path);

        public event FileChangedEventHandler<object, String> FileChangedEvent;

        private MainWindowVM()
        {

        }
    }
}
