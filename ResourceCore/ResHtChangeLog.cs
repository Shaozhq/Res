using System;
using System.Collections.Generic;
using System.Data;
using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.ChangeLog.Api;
using Genersoft.Platform.ChangeLog.Manager;
using Genersoft.Platform.Core.Common;
using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.PubQuery.SPI;

namespace ResourceCore
{
    /// <summary>
    /// 资源合同变更记录查询
    /// </summary>
    public class ResHtChangeLog : IBusinessQueryServer
    {

        public ResHtChangeLog()
        {
        }

        /// <summary>
        /// 资源合同数据模型内码
        /// </summary>
        private readonly string resHtModelId = "14f45b51-4b9a-418c-8b44-86ed3d144767";

        private readonly GSPState session;

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

        /// <summary>
        /// 根据模型和数据id以及实体编号和名称查询实体
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="dataId"></param>
        /// <param name="dataCode"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static DataTable GetChangeDataTable(string modelId, string dataId,string dataCode,string dataName)
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
                var needEntiy = changeEntity.Tables[0].Select("FieldCode='htchangeflag'");
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
            var colResCode = new DataColumn("reshtcode");
            colResCode.DefaultValue = dataCode;
            var colResName = new DataColumn("reshtname");
            colResName.DefaultValue = dataName;
            dt.Columns.Add(colResCode);
            dt.Columns.Add(colResName);
            //显示在最前边
            dt.Columns["reshtcode"].SetOrdinal(0);
            dt.Columns["reshtname"].SetOrdinal(1);
            //删除作为变更记录的变更行
            //不区分大小写
            dt.CaseSensitive = false;
            var changeFlag = dt.Select("FieldCode='htchangeflag'");
            foreach (DataRow row in changeFlag)
            {
                dt.Rows.Remove(row);
            }
            return dt;
        }

        /// <summary>
        /// 查询资源合同变更日志记录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public DataSet GetDataSet(IQueryServerContext context)
        {
            var dataid = context["htnm"];
            var reshtCode = context["htbh"];
            var reshtName = context["htmc"];
            var dt = GetChangeDataTable(resHtModelId, dataid, reshtCode, reshtName);
            if (dt == null || dt.Rows.Count == 0)
                return GetEmptySet();
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;

        }

        /// <summary>
        /// 业务变更日志明细列表DataSet转List<LogItem>
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