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
            string path = Path.Combine("D:\\", "MiningData");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, name);
            return path;
        } 
        /// <summary>
        /// 根据URL取出ID号
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetID(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;
            return Path.GetFileName(url).Replace(MiningHelper.LangExt, "");
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
            //using (var wc = new WebClient())
            //{
            //    wc.Headers.Add("Referer", "http://www.panoramio.com/");
            //    wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36");
            //    wc.Encoding = Encoding.UTF8;
            //    return wc.DownloadString(url);
            //}
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Referer = "http://www.panoramio.com/";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        string html = reader.ReadToEnd();
                        return html;
                    }
                }
                ShowMessageBox("下载网页错误!" + response.StatusCode);
                return string.Empty;
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
            request.Referer = "http://www.panoramio.com/photo/" + Path.GetFileNameWithoutExtension(fileName);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (!response.ContentType.ToLower().StartsWith("text/") && response.StatusCode == HttpStatusCode.OK)
                {
                    SaveBinaryFile(response, fileName);
                }
                else
                {
                    ShowMessageBox("下载错误!" + response.StatusCode);
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
