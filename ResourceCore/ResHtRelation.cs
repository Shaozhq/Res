using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.Core.Common;
using Genersoft.Platform.Core.DataAccess;
using ResourceApi;

namespace ResourceCore
{
    /// <summary>
    /// 资源和合同关系
    /// </summary>
    public class ResHtRelation
    {
        private static object locker = new object();
        public ResHtRelation() {

        }


        /// <summary>
        /// 获取资源类别分级级数
        /// </summary>
        /// <returns></returns>
        public string GetMaxResLbLayer()
        {
            //查询资源类别分级级数
            var resTypeDeepSql = "select  max(len(path))/4 from Zylb";
            var resTypeDeep = Utility.CurDatabase.ExecuteScalar(resTypeDeepSql).ToString();
            return resTypeDeep;
        }
        /// <summary>
        /// 检查当前资源编号是否有对应的合同信息（正常状态下的合同）
        /// </summary>
        /// <param name="resCode"></param>
        /// <returns></returns>
        public string GetHtByRescourceCode(string resCode)
        {
            //获取资源编号对应的合同信息，查询当前资源对应的合同且合同处于正常状态
            var sql = "select htnm from ResContract where Resbh ={0} and HtZsState in({1},{2})";
            var htNm = Utility.CurDatabase.ExecuteScalar(sql, resCode, (int) HtState.Editing, (int) HtState.HasConfirm);
            return htNm as string;
        }

        /// <summary>
        /// 获取当前资源类别最大的资源编号
        /// </summary>
        /// <param name="curTypeCode"></param>
        /// <param name="hsdwbh"></param>
        /// <param name="isSleep"></param>
        /// <returns></returns>
        public string GetMaxResCode(string curTypeCode,string hsdwbh,string isSleep)
        {
            lock (locker)
            {
                var sleepFlag = Serializer.Deserialize<bool>(isSleep);
                if (sleepFlag)
                {
                    Thread.Sleep(280);
                }
                var sql = $"select max(reszybh) from fqres where ResLbbh={{0}} and resssdwid={{1}} and reszybh like '{curTypeCode}%'";
                var maxValue = Utility.CurDatabase.ExecuteScalar(sql, curTypeCode,hsdwbh).ToString();
                var num = string.Empty;
                //默认为5位长度流水号
                if (string.IsNullOrEmpty(maxValue))
                {
                    num = "00001";
                }
                else
                {
                    var maxLength = 5;

                    var lastNum = maxValue.Remove(0, curTypeCode.Length);
                    if (string.IsNullOrEmpty(lastNum))
                        lastNum = "00000";
                    var nextNum = Convert.ToInt32(lastNum) + 1;
                    num = nextNum.ToString().PadLeft(maxLength, '0');

                }

                var result = curTypeCode + num;
                return result;
            }
        }
        /// <summary>
        /// 更新资源状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="resId"></param>
        public void SaveResState(string value,string resId)
        {
            var resStateCode = Convert.ToInt32(value);
            var resStateName = ResStateDictionary.ConvertToString((ResourceState)resStateCode);
            var sql = "update fqres set resstate={0},resstatename={1} where resnm={2}";
            Utility.CurDatabase.ExecSqlStatement(sql, resStateCode, resStateName, resId);
        }

        /// <summary>
        /// 更新合同状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="htId"></param>
        public void SaveHtState(string value, string htId)
        {
            var htStateCode = Convert.ToInt32(value);
            var htStateName = HtStateDictionary.ConvertToString((HtState)htStateCode);
            var htSql = "update ResContract set htzsstate={0},htzsstatemc={1} where htnm={2}";
            Utility.CurDatabase.ExecuteSqlStatement(
                htSql,
                htStateCode,
                htStateName,
                htId);
        }

        /// <summary>
        /// 根据资源的有效时间更新资源状态和合同状态
        /// </summary>
        public void UpdateResAndHtStateByEndTime()
        {
            var curTime = DateTime.Now.ToString("yyyy-MM-dd");
            //状态为正常状态且超期的合同
            var sql = "select htnm from ResContract where htzsstate in ({0},{1}) and htjsrq<{2}";
            var dt = Utility.CurDatabase.ExecuteDataSet(sql, HtState.Editing, HtState.HasConfirm, curTime);
            if (DataSetValidator.IsDatasetValid(dt) == false)
            {
                return;
            }
            //修改资源
            var resSql = @"update fqres set resstate={0},resstatename={1} where resnm in(
                           select resnm from ResContract where htzsstate in({2},{3}) and htjsrq<{4})";
            Utility.CurDatabase.ExecuteSqlStatement(
                resSql, 
                ResourceState.HasConfirm,
                ResStateDictionary.ConvertToString(ResourceState.HasConfirm),
                HtState.Editing, 
                HtState.HasConfirm,
                curTime);
            //修改合同
            var htSql = @"update ResContract set htzsstate={0},htzsstatemc={1} where htzsstate in({2},{3}) and htjsrq<{4}";
            Utility.CurDatabase.ExecuteSqlStatement(
                htSql, 
                HtState.OutDay,
                HtStateDictionary.ConvertToString(HtState.OutDay),
                HtState.Editing,
                HtState.HasConfirm,
                curTime);
        }

        /// <summary>
        /// 终止合同
        /// </summary>
        /// <param name="htNm"></param>
        public void StopHt(string htNm)
        {
            //1.先终止合同
            var htSql = "update ResContract set htzsstate={0},htzsstatemc={1} where htnm={2}";
            Utility.CurDatabase.ExecuteSqlStatement(
                htSql, 
                HtState.Stop,
                HtStateDictionary.ConvertToString(HtState.Stop), 
                htNm);
            //2.更新当前合同对应的资源为已确认状态
            var resSql = @"update fqres set resstate={0},resstatename={1} where resnm in(
            select resnm from ResContract where htnm={2})";
            Utility.CurDatabase.ExecuteSqlStatement(
                resSql,
                ResourceState.HasConfirm,
                ResStateDictionary.ConvertToString(ResourceState.HasConfirm),
                htNm);
        }
    }
}
