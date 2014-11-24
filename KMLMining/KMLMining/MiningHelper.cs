using System.IO;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KMLMining
{
    /// <summary>
    /// 挖掘帮助类
    /// </summary>
    public class MiningHelper
    {
        /// <summary>
        /// 当前抓取的地名特征
        /// </summary>
        public static Character CurrentCharacter = new Character();
        /// <summary>
        /// 暂停操作
        /// </summary>
        public static ManualResetEvent manualReset = new ManualResetEvent(true);
        public static void DoMining(string url)
        {
            int count = 0;
            string nexturl = string.Empty;
            while (!string.IsNullOrWhiteSpace(nexturl = ParseURL(url)))
            {
                WorkerHelper.BgWorker.ReportProgress(++count, url);
                url = nexturl + "?hl=zh-CN";
                manualReset.WaitOne();
                if (WorkerHelper.BgWorker.CancellationPending)//取消操作
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 解析URL，获取其经纬度、地名信息、以及图片
        /// </summary>
        /// <param name="url"></param>
        static string ParseURL(string url)
        {
            string content = CommonHelper.GetUrlString(url);
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }
            bool isOk = true;//确认此次解析是否成功
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            string imgPath = ParseImg(doc);//解析图片
            var point = ParsePoint(doc);//解析经纬度信息
            CurrentCharacter.PhotoPath = imgPath;
            CurrentCharacter.Point = point;
            CurrentCharacter.Url = url.Replace("?hl=zh-CN", "");
            ParseResult.Save(imgPath, point);
            var nextUrl = ParseNextURL(doc);
            return nextUrl;
        }
        /// <summary>
        /// 解析下一个URL
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static string ParseNextURL(HtmlDocument doc)
        {
            var next = doc.GetElementbyId("photo-page-prev-next-container");
            if (next == null)
                return string.Empty;
            var ne = next.Descendants("a").FirstOrDefault();
            if (ne == null)
                return string.Empty;
            return "http://www.panoramio.com" + ne.Attributes["href"].Value.Trim();
        }

        /// <summary>
        /// 解析图片
        /// </summary>
        private static string ParseImg(HtmlDocument doc)
        {
            var a_main_phot = doc.GetElementbyId("main-photo");
            var img = a_main_phot.Descendants("img").FirstOrDefault(); //图片
            if (img == null)
                return string.Empty;
            string img_url = img.Attributes["src"].Value;
            if (string.IsNullOrWhiteSpace(img_url))
                return string.Empty;
            string name = Path.GetFileName(img_url);
            string path = CommonHelper.GetFilePath(name);
            CommonHelper.SaveFileFromUrl(path, img_url);
            return path;
        }

        /// <summary>
        /// 解析经纬度信息
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static PointLatLng ParsePoint(HtmlDocument doc)
        {
            var point = new PointLatLng();
            var name1 = doc.GetElementbyId("map_info_breadcrumbs");
            if (name1 != null)
            {
                name1.Descendants("a").ToList().ForEach(s => point.Name += s.InnerText.Trim() + "•");
            }
            var name2 = doc.GetElementbyId("map_info_name");
            if (name2 != null)
            {
                name2.Descendants("a").ToList().ForEach(s=>point.Name += s.InnerText.Trim());
            }
            var location = doc.GetElementbyId("location");
            if (location != null)
            {
                var abbrs = location.Descendants("abbr");
                if (abbrs.Count() >= 2)
                {
                    point.Lat = Convert.ToDouble(abbrs.ToArray()[0].Attributes["title"].Value.Trim());
                    point.Lng = Convert.ToDouble(abbrs.ToArray()[1].Attributes["title"].Value.Trim());
                }
            }
            return point;
        }
        
    }
}
