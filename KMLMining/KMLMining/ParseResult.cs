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
        /// 保存爬取信息
        /// </summary>
        /// <param name="character"></param>
        public static void Save(Character character)
        {
            string line = character.ID + "," + character.Url + "," + character.Point.Name + "," + character.Point.Lat +
                          "," + character.Point.Lng + "," + character.PhotoPath;
            File.AppendAllLines(GetFileName(), new string[] { line });
        }
        /// <summary>
        /// 根据id获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string[] GetByID(string id)
        {
            if (!File.Exists(GetFileName()))
                return null;
            string str = File.ReadAllLines(GetFileName()).FirstOrDefault(s => s.Contains(id));
            if (string.IsNullOrWhiteSpace(str))
                return null;
            return str.Split(',');
        }
        /// <summary>
        /// 判断是否已经爬取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exist(string id)
        {
            return GetByID(id) != null;
        }
    }
}
