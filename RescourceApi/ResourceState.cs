using System.Collections.Generic;

namespace ResourceApi
{
    public enum ResourceState:int
    {
        /// <summary>
        /// 可以进行编辑,资源新增界面看见所有,
        /// </summary>
        Editing=1,
        /// <summary>
        /// 已经确认，不允许编辑，可以进行流转，也可以进行变更，资源变更和资源流转界面，
        /// </summary>
        HasConfirm=10,
        /// <summary>
        /// 已经产生了合同，需等待合同到期或者终止合同后资源归位待确认状态
        /// </summary>
        HasBuildHt=11
    }

    public class ResStateDictionary
    {
        public static string ConvertToString(ResourceState st)
        {
            switch (st)
            {
                case ResourceState.Editing:
                    return "待确认";
                case ResourceState.HasConfirm:
                    return "已确认";
                case ResourceState.HasBuildHt:
                    return "已签约";
            }

            return "未知";
        }
    }
}