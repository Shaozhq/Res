using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using Genersoft.Platform.PubQuery.SPI;

namespace ResourceCore
{
    /// <summary>
    /// 资源明细查询
    /// </summary>
    public class ResMxQuery : IBusinessQueryServer
    {
        /// <summary>
        /// 查询表字段
        /// </summary>
        private readonly string resQueryFileds = @"resnm,reskpbh,reslbbh,reslbmc,resjyfs,reszybh,reszymc,
                                           ResSyzk,resbmmc,ressjnms,resslormj,restzrq,resstatename";
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
        /// 资源单位名称
        /// </summary>
        private readonly string resdwmc = "resdwmc";
        /// <summary>
        /// 填制日期
        /// </summary>
        private readonly string tzrq = "tzrq";
        #endregion



        /// <summary>
        /// 返回数据结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public DataSet GetDataSet(IQueryServerContext context)
        {
            //把选中的核算单位名称加入到副标题
            var hsdwmc = context[resdwmc].ToString();
            context.AddMacroVal("hsdwmc", hsdwmc);
            var sql = $"select {resQueryFileds} from fqres";
            var wherePart = GetResFilter(context);
            if (!string.IsNullOrEmpty(wherePart))
            {
                sql = String.Concat(sql, " where ", wherePart);
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
            //是包含下级还是只看本级？
            if (!string.IsNullOrEmpty(context[resdwbh]))
            {
                list.Add($"ResSsdwId = '{context[resdwbh]}'");
            }
            //注意日期的处理
            if (!string.IsNullOrEmpty(context[tzrq]))
            {
                var tzrqTime = Convert.ToDateTime(context[tzrq]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (tzrqTime.StartsWith("0001")==false)
                {
                    list.Add($"convert(varchar(10),restzrq,121)<='{tzrqTime}%'");
                }
            }
            //没有是全部
            if (list.Count == 0)
                return string.Empty;
            return string.Join(" and ", list);
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