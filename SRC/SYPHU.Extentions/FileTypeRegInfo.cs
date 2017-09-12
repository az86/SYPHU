using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SYPHU.Extentions
{
    public class FileTypeRegister
    {
        /// <Summary>
        /// RegisterFileType 使文件类型与对应的图标及应用程序关联起来。
        /// </Summary>        
        public static void RegisterFileType(FileTypeRegInfo RegInfo) //在注册表注册特定类型的文件
        {
            if (RegistryHelper.FileTypeRegistered(RegInfo.ExtendName))
            {
                return;
            }
            String RelationName = RegInfo.ExtendName.Substring(1, RegInfo.ExtendName.Length - 1).ToUpper() + "_FileType";
            RegistryKey FileTypeKey = Registry.ClassesRoot.CreateSubKey(RegInfo.ExtendName);
            FileTypeKey.SetValue("", RelationName);
            FileTypeKey.Close();

            RegistryKey RelationKey = Registry.ClassesRoot.CreateSubKey(RelationName);
            RelationKey.SetValue("", RegInfo.Description);
            if(!String.IsNullOrEmpty(RegInfo.IcoPath))
            {
                RegistryKey IconKey = RelationKey.CreateSubKey("DefaultIcon");
                IconKey.SetValue("", RegInfo.IcoPath);
            }

            RegistryKey ShellKey = RelationKey.CreateSubKey("Shell");
            RegistryKey OpenKey = ShellKey.CreateSubKey("Open");
            RegistryKey CommandKey = OpenKey.CreateSubKey("Command");
            CommandKey.SetValue("", RegInfo.ExePath + " %1");
            RelationKey.Close();
        }

        /// <Summary>
        /// GetFileTypeRegInfo 得到指定文件类型关联信息
        /// </Summary>        
        public static FileTypeRegInfo GetFileTypeRegInfo(String ExtendName) //得到文件类型在注册表中的所有信息
        {
            if (!RegistryHelper.FileTypeRegistered(ExtendName))
            {
                return null;
            }

            FileTypeRegInfo RegInfo = new FileTypeRegInfo(ExtendName);

            String RelationName = ExtendName.Substring(1, ExtendName.Length - 1).ToUpper() + "_FileType";
            RegistryKey RelationKey = Registry.ClassesRoot.OpenSubKey(RelationName);
            RegInfo.Description = RelationKey.GetValue("").ToString();

            RegistryKey IconKey = RelationKey.OpenSubKey("DefaultIcon");
            RegInfo.IcoPath = IconKey.GetValue("").ToString();

            RegistryKey ShellKey = RelationKey.OpenSubKey("Shell");
            RegistryKey OpenKey = ShellKey.OpenSubKey("Open");
            RegistryKey CommandKey = OpenKey.OpenSubKey("Command");
            String Temp = CommandKey.GetValue("").ToString();
            RegInfo.ExePath = Temp.Substring(0, Temp.Length - 3);

            return RegInfo;
        }

        /// <Summary>
        /// UpdateFileTypeRegInfo 更新指定文件类型关联信息
        /// </Summary>    
        public static bool UpdateFileTypeRegInfo(FileTypeRegInfo RegInfo) //更新一个文件注册表信息
        {
            if (!RegistryHelper.FileTypeRegistered(RegInfo.ExtendName))
            {
                return false;
            }
            String ExtendName = RegInfo.ExtendName;
            String RelationName = ExtendName.Substring(1, ExtendName.Length - 1).ToUpper() + "_FileType";
            RegistryKey RelationKey = Registry.ClassesRoot.OpenSubKey(RelationName, true);
            RelationKey.SetValue("", RegInfo.Description);

            if (!String.IsNullOrEmpty(RegInfo.IcoPath))
            {
                RegistryKey IconKey = RelationKey.OpenSubKey("DefaultIcon", true);
                IconKey.SetValue("", RegInfo.IcoPath);
            }

            RegistryKey ShellKey = RelationKey.OpenSubKey("Shell");
            RegistryKey OpenKey = ShellKey.OpenSubKey("Open");
            RegistryKey CommandKey = OpenKey.OpenSubKey("Command", true);
            CommandKey.SetValue("", RegInfo.ExePath + " %1");

            RelationKey.Close();
            return true;
        }
    }
}