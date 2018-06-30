using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.ChangeLog.Api;
using Genersoft.Platform.ChangeLog.Manager;
using Genersoft.Platform.Core.Common;
using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.PubQuery.SPI;

namespace ResourceCore
{
    /// <summary>
    /// 资源变更记录查询
    /// </summary>
    public class ResChangeLog: IBusinessQueryServer
    {

        public ResChangeLog()
        {
        }

        /// <summary>
        /// 资源数据模型内码
        /// </summary>
        private readonly string resModelId = "2c8e808e-3474-45bd-9f90-3968ba5357ff";

        private readonly GSPState session;

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

        /// <summary>
        /// 根据模型和数据id以及实体编号和名称查询实体
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="dataId"></param>
        /// <param name="dataCode"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static DataTable GetChangeDataTable(string modelId, string dataId, string dataCode, string dataName)
        {
            var startTime = "2017-01-01";
            var curYear = DateTime.Now.Year;
            var endTime = (curYear + 1).ToString() + "-01-01";
            var dt = new DataTable();
            var changeHeaders =
                ChangeLogDTLocalClient.Instance.GetLogHeaderByDataId(GSPState.Current, dataId, modelId, startTime, endTime);
            if (DataSetValidator.IsDatasetValid(changeHeaders) == false)
            {
                return dt;
            }
            foreach (DataRow header in changeHeaders.Tables[0].Rows)
            {
                var headId = header["id"].ToString();
                var changeUser = header["username"].ToString();
                var changeTime = header["ChangeTime"].ToString();
                var changeEntity = ChangeLogDTLocalClient.Instance.GetLogItemDataSet(GSPState.Current, modelId, headId, changeTime);
                changeEntity.Tables[0].CaseSensitive = false;
                var needEntiy = changeEntity.Tables[0].Select("FieldCode='resxm05'");
                //不含有变更记录的数据
                if (needEntiy == null || needEntiy.Length == 0)
                    continue;
                //只有变更记录的数据
                if (changeEntity.Tables[0].Rows.Count == 1)
                    continue;
                //需要的变更记录
                var colChanger = new DataColumn("changeName");
                colChanger.DefaultValue = changeUser;
                var colchangeTime = new DataColumn("changeTime");
                colchangeTime.DefaultValue = changeTime;
                changeEntity.Tables[0].Columns.Add(colChanger);
                changeEntity.Tables[0].Columns.Add(colchangeTime);
                if (dt.Rows.Count == 0)
                {
                    dt = changeEntity.Tables[0].Copy();
                }
                else
                {
                    foreach (DataRow dr in changeEntity.Tables[0].Rows)
                    {
                        dt.ImportRow(dr);
                    }
                }
            }
            //遍历所有变更记录后，没有满足条件的变更
            if (dt.Rows.Count == 0)
            {
                return dt;
            }
            //添加当前行资源编号和资源名称
            var colResCode = new DataColumn("rescode");
            colResCode.DefaultValue = dataCode;
            var colResName = new DataColumn("resName");
            colResName.DefaultValue = dataName;
            dt.Columns.Add(colResCode);
            dt.Columns.Add(colResName);
            //显示在最前边
            dt.Columns["rescode"].SetOrdinal(0);
            dt.Columns["resName"].SetOrdinal(1);
            //删除作为变更记录的变更行
            //不区分大小写
            dt.CaseSensitive = false;
            var changeFlag = dt.Select("FieldCode='resxm05'");
            foreach (DataRow row in changeFlag)
            {
                dt.Rows.Remove(row);
            }
            return dt;
        }

        /// <summary>
        /// 查询资源变更日志记录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public DataSet GetDataSet(IQueryServerContext context)
        {
            var dataid = context["resnm"];
            var resCode = context["ResZybh"];
            var resName = context["ResZymc"];

            var dt = GetChangeDataTable(resModelId, dataid, resCode, resName);
            if (dt == null || dt.Rows.Count == 0)
                return GetEmptySet();
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// 业务变更日志明细列表DataSet转
        /// </summary>
        /// <param name="dsItem"></param>
        /// <returns></returns>
        private List<LogItem> DataSetConvertToLogItemList(DataSet dsItem)
        {
            List<LogItem> logItemList = new List<LogItem>();
            foreach (DataRow dr in dsItem.Tables[0].Rows)
            {
                LogItem logItem = new LogItem();
                logItem.Id = Convert.ToString(dr["Id"]);
                logItem.TableName = Convert.ToString(dr["TableName"]);
                logItem.DataId = Convert.ToString(dr["DataId"]);
                logItem.BizId = Convert.ToString(dr["BizId"]);
                logItem.FieldCode = Convert.ToString(dr["FieldCode"]);
                logItem.FieldName = Convert.ToString(dr["FieldName"]);
                logItem.OldValue = Convert.ToString(dr["OldValue"]);
                logItem.NewValue = Convert.ToString(dr["NewValue"]);
                logItem.Note = Convert.ToString(dr["Note"]);
                logItem.ParentId = Convert.ToString(dr["ParentId"]);
                Convert.ToString(logItem.OperateType);
                logItemList.Add(logItem);
            }
            return logItemList;
        }


        public DataSet GetDataSetByPage(IQueryServerContext context, int startRecNum, int endRecNum)
        {
            throw new System.NotImplementedException();
        }
    }
}