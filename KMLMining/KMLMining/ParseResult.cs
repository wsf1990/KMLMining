using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMLMining
{
    /// <summary>
    /// 解析结果处理类
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// 获取保存的文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            return CommonHelper.GetFilePath("result.txt");
        }

        /// <summary>
        /// 对解析结果进行保存
        /// </summary>
        /// <param name="imgName"></param>
        /// <param name="point"></param>
        public static void Save(string imgName, PointLatLng point)
        {
            string line = point.Name + "," + point.Lat + "," + point.Lng + "," + imgName;
            File.AppendAllLines(GetFileName(), new string[] {line});
        }
        /// <summary>
        /// 根据照片路径获取信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string[] GetByPhoto(string name)
        {
            string str = File.ReadAllLines(GetFileName()).FirstOrDefault(s=>s.Contains(name));
            if (string.IsNullOrWhiteSpace(str))
                return null;
            return str.Split(',');
        }
    }
}
