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
        /// URL加上本后缀用于获取地名中文
        /// </summary>
        public static string LangExt = "?hl=zh-CN";
        /// <summary>
        /// 爬取数量
        /// </summary>
        public static int Count = 0;
        /// <summary>
        /// 暂停操作
        /// </summary>
        public static ManualResetEvent manualReset = new ManualResetEvent(true);

        public static void DoMining(string url)
        {
            var random = new Random();
            string lastUrl = string.Empty;
            while (true)
            {
                try
                {
                    lastUrl = url;
                    url += LangExt;
                    url = ParseURL(url);
                    manualReset.WaitOne();
                    if (string.IsNullOrWhiteSpace(url)) //url为空的时候结束工作
                        break;
                    if (WorkerHelper.BgWorker.CancellationPending) //取消操作
                    {
                        break;
                    }
                    Thread.Sleep(random.Next(1000, 2000));
                }
                catch (Exception e)
                {
                    //CommonHelper.ShowMessageBox("系统异常：" + ex.Message + " 请等待片刻重试！");
                    //出现异常之后，重启翻墙软件
                    Thread.Sleep(1000 * 10);//休息一阵
                    ProcessHelper.OverWall();//重新翻墙
                    Thread.Sleep(1000 * 10);//休息一阵
                    url = lastUrl;
                }
            }
        }

        /// <summary>
        /// 随机爬取
        /// </summary>
        public static void RandomMining()
        {
            var random = new Random();
            while (true)
            {
                try
                {
                    string url = "http://www.panoramio.com/photo/" + random.Next(1000000000);
                    url += LangExt;
                    url = ParseURL(url);
                    if (!string.IsNullOrWhiteSpace(url))//如果找到了一个正确的URL，循环爬取之
                    {
                        DoMining(url);//当DoMining都找完了之后继续随机寻找
                    }
                }
                catch (Exception e)
                {
                    //CommonHelper.ShowMessageBox("系统异常：" + ex.Message + " 请等待片刻重试！");
                    //出现异常之后，重启翻墙软件
                    Thread.Sleep(1000 * 10);//休息一阵
                    ProcessHelper.OverWall();//重新翻墙
                    Thread.Sleep(1000 * 10);//休息一阵
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
            var doc = new HtmlDocument(); 
            doc.LoadHtml(content);
            //如果已经爬取过就不在爬取，只取下一条URL
            if (!ParseResult.Exist(CommonHelper.GetID(url)))
            {
                string imgPath = ParseImg(doc); //解析图片
                var point = ParsePoint(doc); //解析经纬度信息
                CurrentCharacter.PhotoPath = imgPath;
                CurrentCharacter.Point = point;
                CurrentCharacter.Url = url.Replace(LangExt, "");
                CurrentCharacter.ID = CommonHelper.GetID(CurrentCharacter.Url);
                ParseResult.Save(CurrentCharacter);
                WorkerHelper.BgWorker.ReportProgress(++Count, url);
            }
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
            if (next == null || next.Descendants("a").Count() < 2)
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
