using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using SYPHU.Exceptions;

namespace SYPHU.ViewModels.RegisterMgr
{
    class CopyRightMgr
    {
        [DllImport("SYPHU.CDKeyCheck.dll", EntryPoint = "CheckCDKey", CallingConvention = CallingConvention.StdCall)]
        private static extern int CheckCDKey(uint[] usrCode, uint[] cdk);

        public CodeMgr CodeMgr;

        public CopyRightInfo CopyRightInfo;

        private String _copyRightPath = Path.Combine(System.Environment.SystemDirectory, "syphu.cpr");

        public int FreeDays
        {
            get
            {
                var tryTo = CopyRightInfo.InstallDate.AddDays(730);
                var ts = tryTo - DateTime.Now;
                return ts.Days;
            }
        }

        public CopyRightMgr()
        {
            Load();
        }

        ~CopyRightMgr()
        {
            Save();
        }

        public CopyRight GetCopyRightInfo()
        {
            CopyRightInfo.CopyRight = GetCopyRight();
            Save();
            return CopyRightInfo.CopyRight;
        }

        private void Load()
        {
            try
            {
                using (var file = new FileStream(_copyRightPath, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    CodeMgr = bf.Deserialize(file) as CodeMgr;
                    CopyRightInfo = bf.Deserialize(file) as CopyRightInfo;
                }
            }
            catch (System.Exception )
            {
                CodeMgr = new CodeMgr();
                CopyRightInfo = new CopyRightInfo();
                Save();
            }
        }

        private void Save()
        {
            using (var file = new FileStream(_copyRightPath, FileMode.Create))
            {
                if (CodeMgr.LastAccessTime < DateTime.Now)
                {
                    CodeMgr.LastAccessTime = DateTime.Now;
                }
                var bf = new BinaryFormatter();
                bf.Serialize(file, CodeMgr);
                bf.Serialize(file, CopyRightInfo);
            }
        }

        private CopyRight GetCopyRight()
        {
            if (CodeMgr.CDKey == CodeMgr.TryCDKey)
            {
                if (CodeMgr.LastAccessTime > DateTime.Now)
                {
                    return CopyRight.Pirate;
                }
                return CopyRight.Try;
            }
            var userCodeMd5STr = CodeMgr.GetUserCode();
            var userCodeMd5 = new uint[4];
            var cdk = new uint[4];
            if (CodeMgr.CDKey.Length != 32)
            {
                throw new ErrorRegCodeException();
            }
            for (int count = 0; count != 4; ++count)
            {
                userCodeMd5[count] = uint.Parse(userCodeMd5STr.Substring(count * 8, 8), NumberStyles.HexNumber);
                cdk[count] = uint.Parse(CodeMgr.CDKey.Substring(count * 8, 8), NumberStyles.HexNumber);
            }
            if (CheckCDKey(userCodeMd5, cdk) == 0)
            {
                return CopyRight.GenuineSoftware;
            }
            return CopyRight.Pirate;
        }
    }
}
