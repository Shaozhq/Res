using Genersoft.Platform.PubQuery.SPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ResourceCore
{
    /// <summary>
    /// 合同收款日期查询
    /// </summary>
    public class HtskQuery : IBusinessQueryServer
    {
        /// <summary>
        /// 合同查询表字段
        /// </summary>
        private readonly string resQueryFileds2 = @"ResContract.htnm,ResContract.Htbh,ResContract.Htmc,ResContract.Htlb,ResContract.Htjkfs,ResContract.HtMcskje,ResContract.HtYskze,
                                                    ResContract.HtXcjksj,ResContract.HtXcjkje,ResContract.HtKsrq,ResContract.HtJsrq,rescontract.CreatedDate";
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
        /// 合同类别
        /// </summary>
        private readonly string HTLBMC = "HTLBMC";
        /// <summary>
        /// 合同编号
        /// </summary>
        private readonly string HTBH = "HTBH";
        /// <summary>
        /// 收款时间
        /// </summary>
        private readonly string SKRI = "SKRI";
        /// <summary>
        /// 合同开始时间
        /// </summary>
        private readonly string HTKSSJ = "HTKSSJ";
        /// <summary>
        /// 合同结束时间
        /// </summary>
        private readonly string HTJSSJ = "HTJSSJ";
        /// <summary>
        /// 填制日期
        /// </summary>
        private readonly string tzrq = "tzrq";
        #endregion

        public DataSet GetDataSet(IQueryServerContext context)
        {
            var sql = $"select {resQueryFileds2} from ResContract ";
            var wherePart = GetResFilter(context);
            if (!string.IsNullOrEmpty(wherePart))
            {
                sql = String.Concat(sql, " WHERE ", wherePart);
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
            if (!string.IsNullOrEmpty(context[HTLBMC]))
            {
                list.Add($"HtLb = '{context[HTLBMC]}'");
            }
            if (!string.IsNullOrEmpty(context[HTBH]))
            {
                list.Add($"HtBh = '{context[HTBH]}'");
            }
            if (!string.IsNullOrEmpty(context[SKRI]))
            {
                //缴费时间
                var skri = Convert.ToDateTime(context[SKRI]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (skri.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),HtXcjksj,121)<='{skri}'");
                }
            }
            //注意日期的处理
            if (!string.IsNullOrEmpty(context[tzrq]))
            {
                var tzrqTime = Convert.ToDateTime(context[tzrq]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (tzrqTime.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),CreatedDate,121)<='{tzrqTime}'");
                }
            }
            //合同开始时间
            if (!string.IsNullOrEmpty(context[HTKSSJ]))
            {
                var HTKSSJTime = Convert.ToDateTime(context[HTKSSJ]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (HTKSSJTime.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),HtKsrq,121) like '{HTKSSJTime}%'");
                }
            }
            //合同结束时间
            if (!string.IsNullOrEmpty(context[HTJSSJ]))
            {
                var HTJSSJTime = Convert.ToDateTime(context[HTJSSJ]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (HTJSSJTime.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),HtJsrq,121) like '{HTJSSJTime}%'");
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
