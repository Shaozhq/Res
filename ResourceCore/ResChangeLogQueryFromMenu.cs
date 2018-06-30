using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.PubQuery.SPI;

namespace ResourceCore
{
    public class ResInfo
    {
        /// <summary>
        /// 资源内码
        /// </summary>
        public string Resnm { get; set; }
        /// <summary>
        /// 资源编号
        /// </summary>
        public string Reszybh { get; set; }
        /// <summary>
        /// 资源名称
        /// </summary>
        public string Reszymc { get; set; }
    }

    /// <summary>
    /// 资源变更查询:非联查-来自菜单
    /// </summary>
    public class ResChangeLogQueryFromMenu: IBusinessQueryServer
    {

        /// <summary>
        /// 资源数据模型内码
        /// </summary>
        private readonly string resModelId = "2c8e808e-3474-45bd-9f90-3968ba5357ff";
        /// <summary>
        /// 满足task调用的格式；Func<object,T>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private DataTable GetEachChangeTable(object obj)
        {
            var resInfo = obj as ResInfo;
            if (resInfo == null)
                return new DataTable();
            return ResChangeLog.GetChangeDataTable(resModelId, resInfo.Resnm, resInfo.Reszybh, resInfo.Reszymc);
        }

        public DataSet GetDataSet(IQueryServerContext context)
        {
            //根据前台选择获取所有满足条件的合同信息
            var needResList = GetResInfo(context);
            //筛选不到资源直接返回空
            if (needResList == null || needResList.Count == 0)
            {
                return GetEmptySet();
            }
            //并行获取变更记录
            var taskList = new Task<DataTable>[needResList.Count];
            for (int i = 0; i < needResList.Count; ++i)
            {
                taskList[i] = Task<DataTable>.Factory.StartNew(GetEachChangeTable, needResList[i]);
            }
            //汇总dt
            var dtResult = new DataTable();
            foreach (var item in taskList)
            {
                //批量合并到汇总dt，获取result的时候会去等待当前任务执行完毕
                var dt = item.Result;
                if (dt == null || dt.Rows.Count == 0)
                    continue;
                if (dtResult.Rows.Count == 0)
                {
                    dtResult = dt.Copy();
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtResult.ImportRow(dr);
                    }
                }
            }
            //没有变更
            if (dtResult.Rows.Count == 0)
            {
                return GetEmptySet();
            }
            var ds = new DataSet();
            ds.Tables.Add(dtResult);
            return ds;
        }



        private List<ResInfo> GetResInfo(IQueryServerContext context)
        {
            var wherePart = string.Empty;
            var list = new List<string>();
            //资源类别名称
            if (!string.IsNullOrEmpty(context["zylbmc"]))
            {
                list.Add($"ResLbmc = '{context["zylbmc"]}'");
            }
            //资源编号
            if (!string.IsNullOrEmpty(context["zybh"]))
            {
                list.Add($"ResZybh = '{context["zybh"]}'");
            }
            //资源名称名称
            if (!string.IsNullOrEmpty(context["zymc"]))
            {
                list.Add($"ResZymc = '{context["zymc"]}'");
            }
            //资源核算单位
            if (!string.IsNullOrEmpty(context["hsdwbh"]))
            {
                list.Add($"ResSsdwId = '{context["hsdwbh"]}'");
            }
            //资源签订日期
            if (!string.IsNullOrEmpty(context["zytzrq"]))
            {
                var tzrqTime = Convert.ToDateTime(context["zytzrq"]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (tzrqTime.StartsWith("0001") == false)
                {
                    //2018-01-01:10位数只计算到天数
                    list.Add($"convert(varchar(10),ResTzrq,121) <= '{tzrqTime}'");
                }
            }
            //没有是全部
            if (list.Count != 0)
            {
                wherePart = string.Join(" and ", list);
            }
            wherePart = string.Join(" And ", list);
            var sql = "select resnm,ResZybh,ResZymc,reslbmc from fqres";
            if (!string.IsNullOrEmpty(wherePart))
            {
                sql = string.Concat(sql, " where ", wherePart);
            }

            var resList = new List<ResInfo>();
            var ds = Utility.CurDatabase.ExecuteDataSet(sql);
            if (!DataSetValidator.IsDatasetValid(ds))
            {
                return resList;
            }
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                var resInfo = new ResInfo();
                resInfo.Resnm = item["resnm"].ToString();
                resInfo.Reszybh = item["ResZybh"].ToString();
                resInfo.Reszymc = item["ResZymc"].ToString();
                resList.Add(resInfo);
            }
            return resList;
        }
        public DataSet GetDataSetByPage(IQueryServerContext context, int startRecNum, int endRecNum)
        {
            throw new NotImplementedException();
        }

        private DataSet GetEmptySet()
        {
            var ds = new DataSet();
            var dt = new DataTable();
            dt.Columns.Add("rescode");
            dt.Columns.Add("resname");
            dt.Columns.Add("id");
            dt.Columns.Add("tablename");
            dt.Columns.Add("dataid");
            dt.Columns.Add("bizid");
            dt.Columns.Add("fieldcode");
            dt.Columns.Add("fieldname");
            dt.Columns.Add("oldvalue");
            dt.Columns.Add("newvalue");
            dt.Columns.Add("note");
            dt.Columns.Add("operatetype");
            dt.Columns.Add("parentid");
            dt.Columns.Add("changename");
            dt.Columns.Add("changetime");
            ds.Tables.Add(dt);
            return ds;
        }
    }
}
