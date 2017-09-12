using System;

namespace SYPHU.Extentions
{
    public class FileTypeRegInfo
    {
        /// <Summary>
        /// 目标类型文件的扩展名
        /// </Summary>
        public String ExtendName; //".linkin"

        /// <Summary>
        /// 目标文件类型说明
        /// </Summary>
        public String Description; //"XCodeFactory项目文件"

        /// <Summary>
        /// 目标类型文件关联的图标
        /// </Summary>
        public String IcoPath;

        /// <Summary>
        /// 打开目标类型文件的应用程序
        /// </Summary>
        public String ExePath;

        public FileTypeRegInfo()
        {
        }

        public FileTypeRegInfo(String ExtendName)
        {
            this.ExtendName = ExtendName;
        }
    }
}
