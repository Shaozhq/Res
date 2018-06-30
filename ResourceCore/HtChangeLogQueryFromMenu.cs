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

    /// <summary>
    /// 合同信息类
    /// </summary>
    public class HtInfo
    {
        public string Htnm { get; set; }
        public string Htbh { get; set; }
        public string Htmc { get; set; }
    }

    /// <summary>
    /// 合同变更记录查询：非联查-单独的菜单
    /// </summary>
    public class HtChangeLogQueryFromMenu: IBusinessQueryServer
    {

        /// <summary>
        /// 资源合同数据模型内码
        /// </summary>
        private readonly string resHtModelId = "14f45b51-4b9a-418c-8b44-86ed3d144767";
        /// <summary>
        /// 满足task调用的格式；Func<object,T>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private DataTable GetEachChangeTable(object obj)
        {
            var htInfo = obj as HtInfo;
            if (htInfo == null)
                return new DataTable();
            return ResHtChangeLog.GetChangeDataTable(resHtModelId, htInfo.Htnm, htInfo.Htbh, htInfo.Htmc);
        }

        public DataSet GetDataSet(IQueryServerContext context)
        {
            //根据前台选择获取所有满足条件的合同信息
            var needHtList = GetHtInfo(context);
            //筛选不到合同直接返回空
            if (needHtList == null || needHtList.Count == 0)
            {
                return GetEmptySet();
            }
            //并行获取变更记录
            var taskList = new Task<DataTable>[needHtList.Count];
            for (int i = 0; i < needHtList.Count; ++i)
            {
                taskList[i] = Task<DataTable>.Factory.StartNew(GetEachChangeTable, needHtList[i]);
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

        private List<HtInfo> GetHtInfo(IQueryServerContext context)
        {
            var wherePart = string.Empty;
            var list = new List<string>();
            //合同类别名称
            if (!string.IsNullOrEmpty(context["htlbmc"]))
            {
                list.Add($"htlb = '{context["htlbmc"]}'");
            }
            //合同编号
            if (!string.IsNullOrEmpty(context["htbh"]))
            {
                list.Add($"htbh = '{context["htbh"]}'");
            }
            //合同名称
            if (!string.IsNullOrEmpty(context["htmc"]))
            {
                list.Add($"htmc = '{context["htmc"]}'");
            }
            //合同核算单位
            if (!string.IsNullOrEmpty(context["hsdwbh"]))
            {
                list.Add($"hthsdwbh = '{context["hsdwbh"]}'");
            }
            //合同签订日期
            if (!string.IsNullOrEmpty(context["tzrq"]))
            {
                var tzrqTime = Convert.ToDateTime(context["tzrq"]).ToString("yyyy-MM-dd");
                //前台是空的时候传过来的是"0001-01-01"
                if (tzrqTime.StartsWith("0001") == false)
                {
                    list.Add($"convert(varchar(10),htqdrq,121) <= '{tzrqTime}'");
                }
            }
            //没有是全部
            if (list.Count != 0)
            {
                wherePart = string.Join(" and ", list);
            }
            wherePart = string.Join(" And ", list);
            var sql = "select htnm,htbh,htmc,htlb from rescontract";
            if (!string.IsNullOrEmpty(wherePart))
            {
                sql = string.Concat(sql, " where ", wherePart);
            }

            var htList = new List<HtInfo>();
            var ds = Utility.CurDatabase.ExecuteDataSet(sql);
            if (!DataSetValidator.IsDatasetValid(ds))
            {
                return htList;
            }
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                var htInfo = new HtInfo();
                htInfo.Htnm = item["htnm"].ToString();
                htInfo.Htbh = item["htbh"].ToString();
                htInfo.Htmc = item["htmc"].ToString();
                htList.Add(htInfo);
            }

            return htList;
        }

        public DataSet GetDataSetByPage(IQueryServerContext context, int startRecNum, int endRecNum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 无数据的时候返回一个空数据
        /// </summary>
        /// <returns></returns>
        private DataSet GetEmptySet()
        {
            var ds = new DataSet();
            var dt = new DataTable();
            dt.Columns.Add("reshtcode");
            dt.Columns.Add("reshtname");
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
