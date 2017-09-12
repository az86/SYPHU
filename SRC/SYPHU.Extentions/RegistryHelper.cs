using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SYPHU.Extentions
{
    public class RegistryHelper
    {
        public static bool FileTypeRegistered(String ExtendName)
        {
            RegistryKey SoftwareKey = Registry.ClassesRoot.OpenSubKey(ExtendName);
            if (SoftwareKey != null)
            {
                return true;
            }

            return false;
        }
        private string softwareKey = string.Empty;
        private RegistryKey rootRegistry = Registry.CurrentUser;

        /**/
        /// <summary>
        /// 使用注册表键构造，默认从Registry.CurrentUser开始。
        /// </summary>
        /// <param name="softwareKey">注册表键，格式如"SOFTWARE\\Huaweisoft\\EDNMS"的字符串</param>
        public RegistryHelper(string softwareKey)
            : this(softwareKey, Registry.CurrentUser)
        {
        }

        /**/
        /// <summary>
        /// 指定注册表键及开始的根节点查询
        /// </summary>
        /// <param name="softwareKey">注册表键</param>
        /// <param name="rootRegistry">开始的根节点（Registry.CurrentUser或者Registry.LocalMachine等）</param>
        public RegistryHelper(string softwareKey, RegistryKey rootRegistry)
        {
            this.softwareKey = softwareKey;
            this.rootRegistry = rootRegistry;
        }


        /**/
        /// <summary>
        /// 根据注册表的键获取键值。
        /// 如果注册表的键不存在，返回空字符串。
        /// </summary>
        /// <param name="key">注册表的键</param>
        /// <returns>键值</returns>
        public string GetValue(string key)
        {
            const string parameter = "key";
            if (null == key)
            {
                throw new ArgumentNullException(parameter);
            }

            string result = string.Empty;
            try
            {
                RegistryKey registryKey = rootRegistry.OpenSubKey(softwareKey);
                result = registryKey.GetValue(key).ToString();
            }
            catch
            {
                ;
            }
            return result;
        }

        /// <summary>
        /// 保存注册表的键值
        /// </summary>
        /// <param name="key">注册表的键</param>
        /// <param name="value">键值</param>
        /// <returns>成功返回true，否则返回false.</returns>
        public bool SaveValue(string key, string value)
        {
            const string parameter1 = "key";
            const string parameter2 = "value";
            if (null == key)
            {
                throw new ArgumentNullException(parameter1);
            }

            if (null == value)
            {
                throw new ArgumentNullException(parameter2);
            }

            RegistryKey registryKey = rootRegistry.OpenSubKey(softwareKey, true);

            if (null == registryKey)
            {
                registryKey = rootRegistry.CreateSubKey(softwareKey);
            }
            registryKey.SetValue(key, value);

            return true;
        }
    }
}
