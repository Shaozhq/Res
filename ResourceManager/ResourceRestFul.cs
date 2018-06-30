using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Genersoft.Platform.AppFramework.ClientService;
using Genersoft.Platform.AppFramework.Service;

namespace ResourceManager
{
    /// <summary>
    /// 客户端调用方法类
    /// </summary>
    internal class ResourceRestFul
    {

        private const string assembly = "ResourceCore";
        private const string className = "ResourceCore.ResHtRelation";
        internal GSPState CurState
        {
            get { return ClientContext.Current.FramworkState; }
        }

        public ResourceRestFul()
        {
        }

        public static ResourceRestFul GetClient()
        {
            return new ResourceRestFul();
        }

        /// <summary>
        /// 资源编号是否有合同信息
        /// </summary>
        /// <param name="resCode"></param>
        /// <returns></returns>
        internal string GetHtByRescourceCode(string resCode)
        {
            string[] parameters=new string[1];
            parameters[0] = resCode;
            var result = RESTFulService.Invoke(CurState, assembly, className, "GetHtByRescourceCode", true, parameters);
            return result;
        }
        /// <summary>
        /// 获取当前资源类别最大的资源编号
        /// </summary>
        /// <param name="resTypeCode"></param>
        /// <returns></returns>
        internal string GetMaxResCode(string resTypeCode)
        {
            string[] parameters = new string[1];
            parameters[0] = resTypeCode;
            var result = RESTFulService.Invoke(CurState, assembly, className, "GetMaxResCode", true, parameters);
            return result;
        }

        /// <summary>
        /// 保存资源状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="resId"></param>
        internal void SaveResState(int value, string resId)
        {
            string[] parameters = new string[2];
            parameters[0] = value.ToString();
            parameters[1] = resId;
            var result = RESTFulService.Invoke(CurState, assembly, className, "SaveResState", true, parameters);
        }

        /// <summary>
        /// 保存合同状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="htId"></param>
        internal void SaveHtState(int value, string htId)
        {
            string[] parameters = new string[2];
            parameters[0] = value.ToString();
            parameters[1] = htId;
            var result = RESTFulService.Invoke(CurState, assembly, className, "SaveHtState", true, parameters);

        }

        /// <summary>
        /// 根据资源的有效时间更新资源状态和合同状态
        /// </summary>
        internal void UpdateResAndHtStateByEndTime()
        {
            string[] parameters = new string[0];
            var result = RESTFulService.Invoke(CurState, assembly, className, "UpdateResAndHtStateByEndTime", true,
                parameters);
        }

        /// <summary>
        /// 终止合同
        /// </summary>
        /// <param name="htNm"></param>
        internal void StopHt(string htNm)
        {
            string[] parameters = new string[1];
            parameters[0] = htNm;
            var result = RESTFulService.Invoke(CurState, assembly, className, "StopHt", true,
                parameters);

        }
    }
}
