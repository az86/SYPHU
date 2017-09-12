using System.Management;
using System;

namespace SYPHU.ViewModels.RegisterMgr
{
    [Serializable]
    class CodeMgr
    {
        /// <summary>
        /// 取得设备硬盘的卷标号
        /// </summary>
        /// <returns></returns>
        private string GetDiskVolumeSerialNumber()
        {
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        /// <summary>
        /// 获得CPU的序列号
        /// </summary>
        /// <returns></returns>
        private string getCpu()
        {
            string strCpu = null;
            var myCpu = new ManagementClass("win32_Processor");
            var myCpuConnection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuConnection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
                break;
            }
            return strCpu;
        }

        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <returns></returns>
        private string getMNum()
        {
            string strNum = getCpu() + GetDiskVolumeSerialNumber();//获得24位Cpu和硬盘序列号
            string strMNum = strNum.Substring(0, 24);//从生成的字符串中取出前24个字符做为机器码
            return strMNum;
        }

        /// <summary>
        /// 生成UserCode
        /// </summary>
        /// <returns></returns>
        public string GetUserCode()
        {
            return MD5.MDString(getMNum());
        }

        public String CDKey = TryCDKey;
        
        public const String TryCDKey = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

        public DateTime LastAccessTime = DateTime.Now;
    }
}
