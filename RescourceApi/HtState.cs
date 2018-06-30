using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourceApi
{
    /// <summary>
    /// 资源状态
    /// </summary>
    public enum HtState:int
    {
        /// <summary>
        /// 正常编辑
        /// </summary>
        Editing=10,
        /// <summary>
        /// 确认
        /// </summary>
        HasConfirm=11,
        /// <summary>
        /// 合同被终止
        /// </summary>
        Stop=12,
        /// <summary>
        /// 合同到期
        /// </summary>
        OutDay=13,
    }

    public class HtStateDictionary
    {
        public static string ConvertToString(HtState st)
        {
            switch (st)
            {
                case HtState.Editing:
                    return "待确认";
                case HtState.HasConfirm:
                    return "已确认";
                case HtState.Stop:
                    return "已终止";
                case HtState.OutDay:
                    return "已结束";
            }

            return "未知";
        }
    }
}
