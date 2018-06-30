using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using Genersoft.Platform.PubQuery.SPI;

namespace ResourceCore
{
    /// <summary>
    /// 资源统计查询
    /// </summary>
    public class ResCountQuery : IBusinessQueryServer
    {
        /// <summary>
        /// 查询表字段
        /// </summary>
        private readonly string resQueryFileds = @"fqres.resnm,fqres.reskpbh,fqres.reslbbh,fqres.reslbmc,fqres.resjyfs,
                                                    fqres.reszybh,fqres.reszymc,fqres.ResSyzk,fqres.resbmmc,fqres.ressjnms,
                                                    fqres.resslormj,fqres.restzrq,fqres.resstatename";
        #region 资源通用查询过滤模型字段
        /// <summary>
        /// 资源类别编号
        /// </summary>
        private readonly string reslbbh = "reslbbh";
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
        /// <summary>
        /// 资源单位名称
        /// </summary>
        private readonly string resdwmc = "resdwmc";
        #endregion



        /// <summary>
        /// 返回数据结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public DataSet GetDataSet(IQueryServerContext context)
        {
            //核算单位
            var hsdwmc = context[resdwmc].ToString();
            context.AddMacroVal("hsdwmc", hsdwmc);
            var sql = $"select {resQueryFileds} from fqres";
            var wherePart = GetResFilter(context);
            if (!string.IsNullOrEmpty(wherePart))
            {
                sql = String.Concat(sql, " where ", wherePart);
            }
            //根据资源编号排序汇总，前台默认是类别必须必填
            sql = string.Concat(sql, " order by reslbbh ");
            var result = Utility.CurDatabase.ExecuteDataSet(sql);
            var colreslb = new DataColumn("queryreslbmc");
            colreslb.DefaultValue = context[reslbbh];
            result.Tables[0].Columns.Add(colreslb);

            return result;
        }

        private string GetResFilter(IQueryServerContext context)
        {
            //不能这么干，所有类别的可能不行
            var list = new List<string>();
            //如果资源类别编号有筛选条件，查询所有类别的合同
            if (!string.IsNullOrEmpty(context[reslbbh]))
            {
                //先找到分级码
                var getPathSql = "select path from zylb where zylbbh={0}";
                var path = Utility.CurDatabase.ExecuteScalar(getPathSql, context[reslbbh]);
                //查询资源类别编号是所有下级的信息
                list.Add($"reslbbh in (select zylbbh from zylb where path like '{path}%') ");
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