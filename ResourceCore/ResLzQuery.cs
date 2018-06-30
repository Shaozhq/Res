using Genersoft.Platform.PubQuery.SPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceCore
{
    /// <summary>
    /// 资源流转查询
    /// </summary>
    public class ResLzQuery : IBusinessQueryServer
    {
        /// <summary>
        /// 资源查询表字段
        /// </summary>
        private readonly string resQueryFileds1 = @"fqres.resnm,fqres.reskpbh,fqres.reslbbh,fqres.reslbmc,fqres.resjyfs,fqres.reszybh,fqres.reszymc,
                                           fqres.ResSyzk,fqres.resbmmc,fqres.ressjnms,fqres.resslormj,fqres.restzrq";

        /// <summary>
        /// 合同查询表字段
        /// </summary>
        private readonly string resQueryFileds2 = @"ResContract.htnm,ResContract.Htbh,ResContract.Htmc,ResContract.Htlb,ResContract.Htjkfs,ResContract.HtMcskje,ResContract.HtYskze,
                                                    ResContract.HtXcjksj,ResContract.HtXcjkje,ResContract.HtKsrq,ResContract.HtJsrq";
        #region 资源通用查询过滤模型字段
        /// <summary>
        /// 资源类别编号
        /// </summary>
        private readonly string reslbbn = "reslbbh";
        /// <summary>
        /// 资源编号
        /// </summary>
        private readonly string resbh = "resbh";
        /// <summary>
        /// 资源使用状况
        /// </summary>
        private readonly string ressyzk = "ressyzk";
        /// <summary>
        /// 资源单位编号
        /// </summary>
        private readonly string resdwbh = "resdwbh";
        /// <summary>
        /// 填制日期
        /// </summary>
        private readonly string tzrq = "tzrq";
        #endregion



        public DataSet GetDataSet(IQueryServerContext context)
        {
            var sql = $"select {resQueryFileds1},{resQueryFileds2} from fqres inner join ResContract on fqres.ResNm = ResContract.ResNm where fqres.resState<>1";
            //where fqres.resState=11去掉了这个条件，只要有合同就查询，就是能查询之前的合同
            var wherePart = GetResFilter(context);
            if (!string.IsNullOrEmpty(wherePart))
            {
                sql = String.Concat(sql, " and ", wherePart);
            }
            var result = Utility.CurDatabase.ExecuteDataSet(sql);
            return result;
        }

        private string GetResFilter(IQueryServerContext context)
        {

            var list = new List<string>();
            if (!string.IsNullOrEmpty(context[reslbbn]))
            {
                list.Add($"reslbbh='{context[reslbbn]}'");
            }

            if (!string.IsNullOrEmpty(context[resbh]))
            {
                list.Add($"reszybh = '{context[resbh]}'");
            }
            if (!string.IsNullOrEmpty(context[ressyzk]))
            {
                list.Add($"ResSyzkId = '{context[ressyzk]}'");
            }
            if (!string.IsNullOrEmpty(context[resdwbh]))
            {
                list.Add($"ResSsdwId = '{context[resdwbh]}'");
            }
            //注意日期的处理
            if (!string.IsNullOrEmpty(context[tzrq]))
            {
                var tzrqTime = Convert.ToDateTime(context[tzrq]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (tzrqTime.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),restzrq,121)<='{tzrqTime}%'");
                }
            }
            //没有是全部
            if (list.Count == 0)
                return string.Empty;
            return string.Join(" And ", list);
        }

        /// <summary>
        /// 分页情况
        /// </summary>
        /// <param name="context"></param>
        /// <param name="startRecNum"></param>
        /// <param name="endRecNum"></param>
        /// <returns></returns>
        public DataSet GetDataSetByPage(IQueryServerContext context, int startRecNum, int endRecNum)
        {
            throw new System.NotImplementedException();
        }
    }
}
