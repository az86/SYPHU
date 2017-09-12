using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SYPHU.Views.CommonControls
{
    class UserOperationMgr
    {
        private readonly List<List<TextBox>> _usrOpRec = new List<List<TextBox>>();
        
        private readonly List<List<TextBox>> _usrOpBackRec = new List<List<TextBox>>();

        public bool BeAutoRecord = true;

        private TextBox _frontTextBox;

        private TextBox _currentTextBox;

        public void AddOpTextBox(TextBox tb)
        {
            if (BeAutoRecord)
            {
                _frontTextBox = _currentTextBox;
                _currentTextBox = tb;
                if (_frontTextBox != null && _frontTextBox.CanUndo)
                {
                    var list = new List<TextBox> { _frontTextBox };
                    AddOpTextBox(list);
                }
            }
        }

        public void AddOpTextBox(List<TextBox> tbs)
        {
            _usrOpRec.Add(tbs);
        }

        public void UnDo()
        {
            
            if (_currentTextBox != null && _currentTextBox.CanUndo)
            {
                AddOpTextBox(new List<TextBox> { _currentTextBox });
            }
            if (_usrOpRec.Count <= 0)
                return;
            BeAutoRecord = false;
            var lastItem = _usrOpRec[_usrOpRec.Count - 1];
            CoreUnDo(lastItem);
            _usrOpBackRec.Add(lastItem);
            _usrOpRec.Remove(lastItem);
            BeAutoRecord = true;
        }

        private void CoreUnDo(List<TextBox> tbs)
        {
            foreach (var tb in tbs.Where(tb => tb.CanUndo))
            {
                tb.Undo();
            }
        }

        public void ReDo()
        {
            if (_usrOpBackRec.Count > 0)
            {
                var lastItem = _usrOpBackRec[_usrOpBackRec.Count - 1];
                CoreReDo(lastItem);
                _usrOpBackRec.Remove(lastItem);
                _usrOpRec.Add(lastItem);               
            }
        }

        private void CoreReDo(List<TextBox> tbs)
        {
            foreach (var tb in tbs.Where(tb => tb.CanRedo))
            {
                tb.Redo();
            }
        }

        public void PasteTable()
        {
            var border = _currentTextBox.Parent as Border;
            var grid = border.Parent as Grid;
            var table = grid.Parent as Table;
            table.PasteTable();
        }

        public void CopyTable()
        {
            var border = _currentTextBox.Parent as Border;
            var grid = border.Parent as Grid;
            var table = grid.Parent as Table;
            table.CopyTable();
        }
    }
}
