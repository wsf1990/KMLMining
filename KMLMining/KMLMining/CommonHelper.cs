using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMLMining
{
    public class CommonHelper
    {
        #region 0. Utility
        public static void ShowMessageBox(string msg)
        {
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFilePath(string name)
        {
            string path = Path.Combine(Application.StartupPath, "MiningData");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, name);
            return path;
        } 
        #endregion

        #region 1. 从Url获取源代码
        /// <summary>
        /// 获取url的源代码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlString(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                return wc.DownloadString(url);
            }
        }
        #endregion

        #region 2. 从Url下载文件

        /// 从URL地址下载文件到本地磁盘
        /// </summary>
        /// <param name="fileName">本地磁盘地址</param>
        /// <param name="url">URL网址</param>
        /// <returns></returns>
        public static void SaveFileFromUrl(string fileName, string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);

            using (WebResponse response = request.GetResponse())
            {
                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    SaveBinaryFile(response, fileName);
                }
            }
        }

        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string fileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024 * 1024];
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
                using (Stream outStream = System.IO.File.Create(fileName))
                {
                    using (Stream inStream = response.GetResponseStream())
                    {
                        int l;
                        do
                        {
                            l = inStream.Read(buffer, 0, buffer.Length);
                            if (l > 0)
                                outStream.Write(buffer, 0, l);
                        }
                        while (l > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Value = false;
            }
            return Value;
        }
        #endregion

        
    }
}
