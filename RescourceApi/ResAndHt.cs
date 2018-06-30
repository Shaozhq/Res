using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RescourceApi
{

    /// <summary>
    /// 合同和资源关系
    /// </summary>
    public class ResAndHt
    {
        /// <summary>
        /// 资源内码
        /// </summary>
        public string ResNm { get; set; }
        /// <summary>
        /// 合同内码
        /// </summary>
        public string HtNm { get; set; }
        /// <summary>
        /// 合同结束时间
        /// </summary>
        public DateTime HtEndTime { get; set; }
    }
}
