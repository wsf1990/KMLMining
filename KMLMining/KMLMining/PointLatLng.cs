using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMLMining
{
    /// <summary>
    /// 经纬度
    /// </summary>
    public class PointLatLng
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 地名信息
        /// </summary>
        public string Name { get; set; }
    }
}
