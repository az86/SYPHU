using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using SYPHU.ViewModels.RegisterMgr;

namespace SYPHU.ViewModels
{
    public class RegisterVM
    {
        public static readonly RegisterVM Instance = new RegisterVM();

        public event EventHandler ShowRegisterWnd;

        private CopyRightMgr _copyRightMgr = new CopyRightMgr();

        public String UserCode { get { return _copyRightMgr.CodeMgr.GetUserCode(); } }

        public String CDKey
        {
            get { return _copyRightMgr.CodeMgr.CDKey; }
            set { _copyRightMgr.CodeMgr.CDKey = value; }
        }

        private int FreeDays { get { return _copyRightMgr.CopyRightInfo.CopyRight == CopyRight.GenuineSoftware ? 9999 : _copyRightMgr.FreeDays; } }

        public String RegisterInfo { get { return FormatRegInfo(); } }

        private String FormatRegInfo()
        {
            if (_copyRightMgr.CopyRightInfo.CopyRight == CopyRight.GenuineSoftware)
            {
                return "正版软件";
            }
            else
            if (FreeDays <= 0)
            {
                return "试用已到期";
            }
            else
            {
                return "试用剩余" + FreeDays + "天";
            }
        }

        public bool OnClickOkBtn()
        {
            _copyRightMgr.GetCopyRightInfo();
            return _copyRightMgr.CopyRightInfo.CopyRight == CopyRight.GenuineSoftware;
        }

        public bool Check()
        {
            while (FreeDays < 0 || _copyRightMgr.GetCopyRightInfo() == CopyRight.Pirate)
            {
                ShowRegisterWnd.Invoke(this, null);
                return false;
            }
            return true;
        }
    }
}
