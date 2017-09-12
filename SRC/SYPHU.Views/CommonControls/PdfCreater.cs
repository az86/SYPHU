using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace SYPHU.Views.CommonControls
{
    class PdfCreater
    {
        private XpsDocument _xpsDocument;

        private XpsDocumentWriter GetSaveXpsDocumentWriter(string containerName)
        {
            File.Delete(containerName);
            _xpsDocument = new XpsDocument(containerName, FileAccess.ReadWrite);
            XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(_xpsDocument);
            return xpsdw;
        }

        /// <summary>
        /// 保存单个视觉对象
        /// </summary>
        /// <param name="willSavedVisual">将要保存的视觉逻辑</param>
        /// <param name="containerPath">将要保存的文件名称</param>
        private void SaveSingleVisual(Visual willSavedVisual, string containerPath)
        {
            XpsDocumentWriter xdwSave = GetSaveXpsDocumentWriter(containerPath);
            SaveVisual(xdwSave, willSavedVisual);
            _xpsDocument.Close();
        }

        private void SaveVisual(XpsDocumentWriter xpsdw, Visual v)
        {
            var element = v as System.Windows.FrameworkElement;
            element.Width = element.ActualWidth;
            element.Height = element.ActualHeight;
            var adjustedVisual = element as Visual;
            xpsdw.Write(adjustedVisual);
        }

        public void SavePDFFile(Visual visual, string pdfFilePath=@"D:\123.pdf")
        {
            string xpsPath = @"D:\tmp.xps";
            SaveSingleVisual(visual, xpsPath);
            if (File.Exists(xpsPath))
            {
                var excuteDll = Path.Combine(System.Environment.CurrentDirectory, "gxps.exe");
                ProcessStartInfo gxpsArguments = new ProcessStartInfo(excuteDll, String.Format("-sDEVICE=pdfwrite -sOutputFile={0} -dNOPAUSE {1}", pdfFilePath, xpsPath));
                gxpsArguments.WindowStyle = ProcessWindowStyle.Hidden;
                using (var gxps = Process.Start(gxpsArguments))
                {
                    gxps.WaitForExit();
                }
                File.Delete(xpsPath);//删除临时文件
            }

        }
    }
}
