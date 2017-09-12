using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYPHU.ViewModels.RegisterMgr
{
    [Serializable]
    enum CopyRight
    {
        GenuineSoftware,
        Try,
        Pirate
    }

    [Serializable]
    class CopyRightInfo
    {
        public CopyRight CopyRight = CopyRight.Try;

        public DateTime InstallDate = DateTime.Now;
    }
}
