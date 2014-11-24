using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMLMining
{
    /// <summary>
    /// 地名特征类
    /// </summary>
    public class Character
    {
        /// <summary>
        /// 对应的URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片保存地址
        /// </summary>
        public string PhotoPath { get; set; }

        /// <summary>
        /// 位置信息
        /// </summary>
        public PointLatLng Point { get; set; }
    }
}
