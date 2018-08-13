using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using Genersoft.Platform.Core.Common;
using Genersoft.Platform.Core.DataAccess;
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
            //核算单位,好像不管用
            var hsdwmc = context[resdwmc].ToString();
            context.AddMacroVal("hsdwmc", hsdwmc);
            //资源类别和资源表联查
            var resCountSql = @"SELECT
            lsbzdw.lsbzdw_dwmc hsdwmc,
            Zylb.path,
            Zylb.Zylbbh,
            Zylb.Zylbmc,
            Zylb.Layer,
            fqres.resslormj,
            fqres.resnm,
            fqres.reskpbh,
            fqres.reslbbh,
            fqres.reslbmc,
            fqres.resjyfs,
            fqres.reszybh,
            fqres.reszymc,
            fqres.ResSyzk,
            fqres.resbmmc,
            fqres.ressjnms,
            fqres.restzrq,
            fqres.resstatename
                from
            Zylb
                left join fqres on fqres.ResTypeId = Zylb.Zylbnm
                left join lsbzdw on fqres.resssdwid = lsbzdw.lsbzdw_dwbh";
            
            var joinPart = GetResFilter(context);
            
            if (!string.IsNullOrEmpty(joinPart))
            {
                resCountSql = String.Concat(resCountSql, " and ", joinPart);
            }
            var orderByPart = " order by Zylb.Path";
            //根据资源编号排序汇总，前台默认是类别必须必填
            resCountSql = string.Concat(resCountSql, orderByPart);
            var result = Utility.CurDatabase.ExecuteDataSet(resCountSql);
            //统计结果
            //获取查询级数
            int resLbQueryLayer = Convert.ToInt32(context["reslblayer"]);
            //int resLbQueryLayer = 4;
            GetNotDetailCount(result, resLbQueryLayer);
            return result;
        }


        private void GetNotDetailCount(DataSet ds,int layer)
        {
            if (DataSetValidator.IsDatasetValid(ds) == false)
            {
                return;
            }
            //首先根据级次得到本级次以及上级的分级码
            var resTypeDeepSql = "select  max(len(path))/4 from Zylb";
            var resTypeDeep = Utility.CurDatabase.ExecuteScalar(resTypeDeepSql).ToString();
            var resTypeLayer = Convert.ToInt32(resTypeDeep);
            var sumField = "resslormj";
            //1~layer-1
            for (int i = 1; i < resTypeLayer; i++)
            {
                var rowsEachLayer = ds.Tables[0].Select($"layer='{i}'");
                //针对每一级别的分级码
                foreach (var pathRow in rowsEachLayer)
                {
                    var curPath = pathRow["path"].ToString();
                    var curPathSubRows = ds.Tables[0].Select($"path like '{curPath}%' and path<>'{curPath}'");
                    //最末级节点要清楚
                    if (curPathSubRows == null || curPathSubRows.Length == 0)
                        continue;
                    decimal countNum = 0;
                    //当前行的数据要同步
                    if (pathRow[sumField] == DBNull.Value)
                        countNum = 0;
                    else
                    {
                        countNum= Convert.ToDecimal(pathRow[sumField]);
                    }
                    foreach (DataRow eachRow in curPathSubRows)
                    {
                        if (eachRow[sumField] == DBNull.Value)
                            continue;
                        countNum += Convert.ToDecimal(eachRow[sumField]);
                    }
                    //合计当前层级的下级数量和面积之和
                    pathRow[sumField] = countNum.ToString();
                }

            }

            if (layer == resTypeLayer)
                return;
            var deleteRows = ds.Tables[0].Select($"layer>{layer}");
            foreach (var item in deleteRows)
            {
                ds.Tables[0].Rows.Remove(item);
            }
        }


        private string GetResFilter(IQueryServerContext context)
        {
            var list = new List<string>();
            //如果资源类别编号有筛选条件，查询所有类别的合同,查询条件变化后是不是需要调整
            //if (!string.IsNullOrEmpty(context[reslbbh]))
            //{
            //    //先找到分级码
            //    var getPathSql = "select path from zylb where zylbbh={0}";
            //    var path = Utility.CurDatabase.ExecuteScalar(getPathSql, context[reslbbh]);
            //    //查询资源类别编号是所有下级的信息
            //    list.Add($"reslbbh in (select zylbbh from zylb where path like '{path}%') ");
            //}

            if (!string.IsNullOrEmpty(context[resbh]))
            {
                list.Add($"fqres.reszybh = '{context[resbh]}'");
            }
            if (!string.IsNullOrEmpty(context[ressyzk]))
            {
                list.Add($"fqres.ResSyzkId = '{context[ressyzk]}'");
            }
            if (!string.IsNullOrEmpty(context[resdwbh]))
            {
                var selectDwbh = context[resdwbh].ToString();
                var selectDwnmSql = $"select lsbzdw_dwnm from lsbzdw where lsbzdw_dwbh='{selectDwbh}'";
                var selectDwnm = Utility.CurDatabase.ExecuteScalar(selectDwnmSql).ToString();
                //找到核算单位的所有下级
                var hsdwSql = $"select lsbzdw_dwbh from LSBZDW where lsbzdw_dwnm like '{selectDwnm}%'";
                list.Add($"fqres.ResSsdwId in ({hsdwSql})");
            }
            //注意日期的处理
            if (!string.IsNullOrEmpty(context[tzrq]))
            {
                var tzrqTime = Convert.ToDateTime(context[tzrq]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (tzrqTime.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),fqres.restzrq,121)<='{tzrqTime}%'");
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