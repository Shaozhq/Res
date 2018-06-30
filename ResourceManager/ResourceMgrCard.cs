using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Genersoft.Platform.AppFramework.ClientService;
using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.AppFrameworkGui.Services;
using Genersoft.Platform.ChangeLog.Api;
using Genersoft.Platform.ChangeLog.RemoteClient;
using Genersoft.Platform.Controls.WinForms;
using Genersoft.Platform.Resource.Metadata.Component.Attributes;
using Genersoft.Platform.XForm.FormController.Basic;
using Genersoft.Platform.XForm.SPI;
using Genersoft.Platform.XFormController.FormController.Basic;
using Genersoft.Platform.XFormController.FormController.Utility;
using Genersoft.Platform.XFormEngine.MLFC.XFML.DOM;
using ResourceApi;

namespace ResourceManager
{
    /// <summary>
    /// 资源卡片界面扩展
    /// </summary>
    public class ResourceMgrCard:BasicSearchWizzardUtilityController //BasicFormController
    {

        private readonly GSPState curState;

        #region 菜单Id

        private readonly string ResAddFuncId = "3cb708c3-0207-4a89-bfa4-d23e4084bb3e";
        private readonly string ResChangeFuncId = "09bf53f1-89be-485d-bfdd-f59fe712affb";
        private readonly string ResTransFuncId = "66218d9f-fbbb-403b-a47d-c30f5cbf4b2b";
        /// <summary>
        /// 资源合列卡查询菜单
        /// </summary>
        private readonly string ResHtLcListCardFuncId = "0e57dd30-d4ed-4015-ba0e-562707219b0d";


        #endregion 

        #region 模型和控件字段
        /// <summary>
        /// 资源内码
        /// </summary>
        private readonly string ResNm = "ResNm";
        /// <summary>
        /// 资源卡片编号
        /// </summary>
        private readonly string ResKpbh = "ResKpbh";

        /// <summary>
        /// 资源编号
        /// </summary>
        private readonly string ResZybh = "ResZybh";
        /// <summary>
        /// 资源名称
        /// </summary>
        private readonly string ResZymc = "ResZymc";
        /// <summary>
        /// 资源类别编号
        /// </summary>
        private readonly string ResLbbh = "ResLbbh";
        /// <summary>
        /// 资源类别名称
        /// </summary>
        private readonly string ResLbmc = "ResLbmc";
        /// <summary>
        /// 资源状态编号
        /// </summary>
        private readonly string ResState = "ResState";
        /// <summary>
        /// 资源状态名称
        /// </summary>
        private readonly string ResStateName = "ResStateName";
        /// <summary>
        /// 资源类别Id
        /// </summary>
        private readonly string ResTypeId = "ResTypeId";
        /// <summary>
        /// 经营方式Id
        /// </summary>
        private readonly string ResJyfsId = "ResJyfsId";
        /// <summary>
        /// 使用状况Id
        /// </summary>
        private readonly string ResSyzkId = "ResSyzkId";
        /// <summary>
        /// 计量单位Id
        /// </summary>
        private readonly string ResJldwId = "ResJldwId";
        /// <summary>
        /// 所属部门Id
        /// </summary>
        private readonly string ResSsbmId = "ResSsbmId";

        #region 控件ID
        /// <summary>
        /// 表单上列表控件
        /// </summary>
        private readonly string gridControl = "XDataGrid1";

        /// <summary>
        /// 资源类别编号智能帮助
        /// </summary>
        private readonly string ctrlResTypeCodeHelp = "XSmartDictLookup1";
        /// <summary>
        /// 资源类别名称智能帮助
        /// </summary>
        private readonly string ctrlResTypeNameHelp = "XSmartDictLookup2";
        /// <summary>
        /// 资源表单所属部门帮助
        /// </summary>
        private readonly string ctrlResSsbmHelp = "XSmartDictLookup3";
        /// <summary>
        /// 计量单位
        /// </summary>
        private readonly string ctrlResJldwHelp = "XSmartDictLookup4";
        /// <summary>
        /// 经营方式
        /// </summary>
        private readonly string ctrlResJyfsHelp = "XSmartDictLookup5";
        /// <summary>
        /// 使用状况
        /// </summary>
        private readonly string ctrlResSyzkHelp = "XSmartDictLookup6";
        /// <summary>
        /// 编辑菜单
        /// </summary>
        private readonly string ctrlBtnEdit = "btnItemEdit";
        /// <summary>
        /// 删除菜单
        /// </summary>
        private readonly string ctrlBtnDelete = "btnItemDelete";
        /// <summary>
        /// 工具栏
        /// </summary>
        private readonly string menuBar = "Bar1";
        /// <summary>
        /// 合同变更菜单
        /// </summary>
        private readonly string ctrlBtnChange = "BarButtonItem3";

        /// <summary>
        /// 变更记录
        /// </summary>
        private readonly string ctrlBtnChangLog = "BarButtonItem4";
        #endregion

        #endregion

        #region 表单控件
        /// <summary>
        /// 类别编号单值帮助
        /// </summary>
        private DataDictLookUpElement ResTypeCodeHelp
        {
            get { return GetDataDictLookUp(ctrlResTypeCodeHelp); }
        }
        /// <summary>
        /// 类别名称单值帮助
        /// </summary>
        private DataDictLookUpElement ResTypeNameHelp
        {
            get { return GetDataDictLookUp(ctrlResTypeNameHelp); }
        }
        /// <summary>
        /// 所属部门帮助
        /// </summary>
        private DataDictLookUpElement ResSsbmHelp
        {
            get { return GetDataDictLookUp(ctrlResSsbmHelp); }
        }
        /// <summary>
        /// 计量单位
        /// </summary>
        private DataDictLookUpElement ResJldwHelp
        {
            get { return GetDataDictLookUp(ctrlResJldwHelp); }
        }
        /// <summary>
        /// 经营方式
        /// </summary>
        private DataDictLookUpElement ResJyfsHelp
        {
            get { return GetDataDictLookUp(ctrlResJyfsHelp); }
        }

        /// <summary>
        /// 使用状况
        /// </summary>
        private DataDictLookUpElement ResSyzkHelp
        {
            get { return GetDataDictLookUp(ctrlResSyzkHelp); }
        }

        /// <summary>
        /// 左列表
        /// </summary>
        private RepeatElement ListGrid
        {
            get { return GetDataGrid(gridControl); }
        }
        /// <summary>
        /// 删除按钮
        /// </summary>
        private BarButtonItemElement BarBtnDelete
        {
            get { return GetButton(ctrlBtnDelete); }
        }

        /// <summary>
        /// 编辑按钮
        /// </summary>
        private BarButtonItemElement BarBtnEdit
        {
            get { return GetButton(ctrlBtnEdit); }
        }
        /// <summary>
        /// 工具栏
        /// </summary>
        private BarElement MenuBar
        {
            get { return GetMenuBar(menuBar); }
        }
        /// <summary>
        /// 变更按钮
        /// </summary>
        private BarButtonItemElement BarBtnChange
        {
            get { return GetButton(ctrlBtnChange); }
        }

        /// <summary>
        /// 变更记录按钮
        /// </summary>
        private BarButtonItemElement BarBtnChangeLog
        {
            get { return GetButton(ctrlBtnChangLog); }
        }
        /// <summary>
        /// 获取界面帮助控件的Element
        /// </summary> 
        private DataDictLookUpElement GetDataDictLookUp(string elementId)
        {
            var ele = CompContext.GetElementByID<DataDictLookUpElement>(XFMLConstants.ELEMENT_DATA_DICT_LOOKUP, elementId);
            if (ele == null)
            {
                throw new InvalidFormDefException($"无法找到ID为{elementId}的值映射的DataDictLookUpElement控件");
            }
            return ele;

            
        }
        /// <summary>
        /// 获取列表空间
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        private RepeatElement GetDataGrid(string elementId)
        {

            var ele = this.CompContext.GetElementByID<RepeatElement>(XFMLConstants.ELEMENT_REPEAT, elementId);
            if (ele == null)
            {
                throw new InvalidFormDefException($"无法找到ID为{elementId}的值映射的RepeatElement控件");
            }
            return ele;
        }

        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        private BarButtonItemElement GetButton(string elementId)
        {
            var ele = this.CompContext.GetElementByID<BarButtonItemElement>(XFMLConstants.ELEMENT_BAR_BUTTON_ITEM, elementId);
            if (ele == null)
            {
                throw new InvalidFormDefException($"无法找到ID为{elementId}的值映射的BarButtonItemElement控件");
            }
            return ele;
        }

        private BarElement GetMenuBar(string elementId)
        {
            var ele = this.CompContext.GetElementByID<BarElement>(XFMLConstants.ELEMENT_BAR, elementId);
            if (ele == null)
            {
                throw new InvalidFormDefException($"无法找到ID为{elementId}的值映射的BarElement控件");
            }
            return ele;
        }


        private TextBoxElement GetTextBox(string elementId)
        {
            var ele = this.CompContext.GetElementByID<TextBoxElement>(XFMLConstants.ELEMENT_TEXTBOX, elementId);
            if (ele == null)
            {
                throw new InvalidFormDefException($"无法找到ID为{elementId}的值映射的TextBoxElement控件");
            }
            return ele;
        }

        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public ResourceMgrCard(IFormActionContext context) : base(context)
        {
            this.curState = context.ElementHandler.ADPUIState;
        }


        /// <summary>
        /// 获取表单上的DataModel
        /// </summary>
        /// <returns></returns>
        private DataRow GetCurRow()
        {
            return ListGrid.SelectedDataRow;
        }

        /// <summary>
        /// 卡片上的当前数据源
        /// </summary>
        /// <returns></returns>
        private DataRow GetRightCardDataTable()
        {
            return this.CompContext.DefaultInstanceData.DataSet.Tables[0].Rows[0];
        }

        /// <summary>
        /// 当前行数据状态
        /// </summary>
        /// <returns></returns>
        private ResourceState GetCurRowState()
        {
            var value = (ResourceState)Convert.ToInt32(GetCurRow()[ResState]);
            return value;
        }

        /// <summary>
        /// 1.产生符合框架格式的传入参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        private string GetArgumentStr(Dictionary<string,string> dic)
        {

            if (dic == null || dic.Count == 0)
                return string.Empty;
            var argStr = "argumentString=";
            foreach (var item in dic)
            {
                argStr = string.Concat(argStr, item.Key, "=", item.Value,"&");
            }

            //添加一个联查标识,到合同界面有单独的控制
            argStr = string.Concat(argStr,"isLc","=","1","&");
            //因为表单那里加了过滤条件，菜单加标识无用，必须在这里传入参数？？
            argStr = string.Concat(argStr,"HTSTATE","=","1");

            return argStr;
        }
        /// <summary>
        /// 2.检查当前资源是否有对应的合同信息
        /// </summary>
        /// <returns></returns>
        private List<string> GetActionIdStr()
        {
            var curRow = GetCurRow();
            var curRowResCode = curRow[ResZybh].ToString();
            //服务器端检查
            var htId = ResourceRestFul.GetClient().GetHtByRescourceCode(curRowResCode);
            if (string.IsNullOrEmpty(htId))
            {
                //新增状态，双数据源的是Add,有的是Create
                return new List<string> { "actionID=Add" };
            }
            //TODO 应该是只读状态，因为界面上过滤调了，所以只有新增的情况啦
            return new List<string> {"actionID=", $"dataID={htId}"};
        }



        #region 表单方法

        /// <summary>
        /// 刷新资源的状态-表单加载前
        /// </summary>
        [ControllerMethod]
        public void UpdateResAndHtStateByEndTime()
        {
            ResourceRestFul.GetClient().UpdateResAndHtStateByEndTime();
        }

        /// <summary>
        /// 事件初始根据不同的FuncId显示不同的菜单
        /// </summary>
        [ControllerMethod]
        public void FormInit()
        {

            UpdataUIByFuncId();
            //帮助后事件
            //资源类别编号
            ResTypeCodeHelp.DictEntryPicked += ResTypeCodeHelped;
            //资源类别名称
            ResTypeNameHelp.DictEntryPicked += ResTypeNameHelped;
            //所属部门
            ResSsbmHelp.DictEntryPicked += ResSsbmHelped;
            //计量单位
            ResJldwHelp.DictEntryPicked += ResJldwHelped;
            //经营方式
            ResJyfsHelp.DictEntryPicked += ResJyfsHelped;
            //使用状况
            ResSyzkHelp.DictEntryPicked += ResSyzkHelped;
            //焦点行切换后
            ListGrid.Repeat_SelectRowChanged += SelectRowChanged;
            SelectRowChanged(this, null);
        }
        /// <summary>
        /// 新增时资源默认状态
        /// </summary>
        [ControllerMethod]
        public void SetResStateWhenCreate()
        {
            //保存走的是卡片，所以要卡片
            GetCurRow()[ResState] = (int)ResourceState.Editing;
            GetCurRow()[ResStateName] = ResStateDictionary.ConvertToString(ResourceState.Editing);
            GetRightCardDataTable()[ResState] = (int)ResourceState.Editing;
            GetRightCardDataTable()[ResStateName] = ResStateDictionary.ConvertToString(ResourceState.Editing);
        }


        /// <summary>
        /// 资源流转--打开功能菜单(资源合同联查)
        /// </summary>
        [ControllerMethod]
        public void OpenResHtFunc()
        {
            //当前状态
            var curResState = GetCurRowState();
            if (curResState == ResourceState.HasBuildHt)
            {
                MessageBox.Show("该资源已经产生合同，不允许再次流转");
                return;
            }

            //只有确认状态和已经生成合同的资源才能打开
            //TODO 联查资源
            //1.判断有无相应的资源合同已经在这里处理掉了，包括判断合同的状态未使用
            var FuncArgs = GetActionIdStr();
            //无数据
            if (FuncArgs.Count == 1)
            {
                var curData = GetCurRow();
                var dic = new Dictionary<string, string>();
                //资源内码和资源编号和名称
                dic.Add(ResNm, curData[ResNm].ToString());
                dic.Add(ResZymc, curData[ResZymc].ToString());
                dic.Add(ResZybh, curData[ResZybh].ToString());
                var argStr = GetArgumentStr(dic);
                //需要传递过去的数据
                FuncArgs.Add(argStr);
            }

            //2.资源合同联查
            GSPFunctionService.Current.OpenFunc(ResHtLcListCardFuncId, curState, FuncArgs.ToArray());
        }

        private bool confirmRes =false;

        /// <summary>
        /// 资源确认
        /// </summary>
        [ControllerMethod]
        public void ResConfirm()
        {
            var index = ListGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择资源行数据!", "资源选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resState = GetCurRowState();
            if (resState != ResourceState.Editing)
            {
                MessageBox.Show("该资源已经被确认，无需再次确认", "资源选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //1.先判断该资源是否确认过
        var question = MessageBox.Show("资源确认后将不可修改，是否确认资源?", "资源确认提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            //取消
            if (question == DialogResult.Cancel)
                return;

            //进入提交状态，设置资源状态为不可编辑状态

            
            var state = (int) ResourceState.HasConfirm;
            var resId = GetCurRow()[ResNm].ToString();
            GetCurRow()[ResState] = state;
            ResourceRestFul.GetClient().SaveResState(state, resId);
            GetCurRow()[ResStateName] = ResStateDictionary.ConvertToString(ResourceState.HasConfirm);
            GetRightCardDataTable()[ResStateName] = ResStateDictionary.ConvertToString(ResourceState.HasConfirm);
            MessageBox.Show("资源确认成功！", "资源确认提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //成功后触发一下当前按钮状态
            confirmRes = true;
            SelectRowChanged(this, null);

        }

        /// <summary>
        /// 资源变更
        /// </summary>
        [ControllerMethod]
        public void ResChange()
        {
            var index = ListGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择有效数据行!", "资源提示");
                return;
            }
            var state = GetCurRowState();
            if (state != ResourceState.HasConfirm)
            {
                MessageBox.Show("只有已确认状态的资源允许变更!");
                return;
            }

            if (state == ResourceState.Editing)
            {
                MessageBox.Show("该资源未进行确认,请先确认后进行资源变更!");
                return;
            }
            //资源编号
            var curRowResCode = GetCurRow()[ResZybh].ToString();
            //服务器端检查资源有否有正常合同
            var htId = ResourceRestFul.GetClient().GetHtByRescourceCode(curRowResCode);
            if (string.IsNullOrEmpty(htId)==false)
            {
                MessageBox.Show("该资源有对应合同,合同未终止前无法进行资源变更!");
                return;
            }
            //先灰色显示
            this.BarBtnChange.VisualComponent.Enabled = false;
            var changeCol = "ResXM05";
            var curValue = GetCurRow()[changeCol].ToString();
            var differentValue = Guid.NewGuid();
            GetCurRow()[changeCol] = differentValue;
            GetRightCardDataTable()[changeCol] = differentValue;

            //进行编辑
            this.CompContext.ExcuteAction("Edit");
        }

        /// <summary>
        /// 保存后
        /// </summary>
        [ControllerMethod]
        public void AfterSave()
        {
            //把变更菜单打开
            this.BarBtnChange.VisualComponent.Enabled = true;

        }

        /// <summary>
        /// 资源流转
        /// </summary>
        [ControllerMethod]
        public void ResTrans()
        {
            var index = ListGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择有效数据行!", "资源提示");
                return;
            }
            var state = GetCurRowState();
            if (state == ResourceState.Editing)
            {
                MessageBox.Show("该资源未进行确认,请先确认后进行资源流转!");
                return;
            }
            //触发联查
            OpenResHtFunc();
        }
        /// <summary>
        /// 查看资源变更记录
        /// </summary>
        [ControllerMethod]
        public void ViewResChangeLog()
        {
            var resModel = "2c8e808e-3474-45bd-9f90-3968ba5357ff";
            var dataid = GetCurRow()[ResNm].ToString();
            var result = ChangeLogRemoteClient.Instance.GetLogHeaderByDataId(curState, dataid, resModel, "2018-03-05", "2018-07-01");
            var headId = result.Tables[0].Rows[0]["id"].ToString();
            var changeTime = result.Tables[0].Rows[0]["ChangeTime"].ToString();
            var entity = ChangeLogRemoteClient.Instance.GetLogItemDataSet(curState, resModel, headId, changeTime);
        }

        #endregion


        #region 私有方法
        /// <summary>
        /// 根据菜单Id刷新界面
        /// </summary>
        private void UpdataUIByFuncId()
        {
            var curFuncId = ClientContext.Current.FramworkState.FuncID;
            //新增
            if (string.Compare(curFuncId, ResAddFuncId, true) == 0)
            {
                ResAddCard();
            }
            //变更
            if (string.Compare(curFuncId, ResChangeFuncId, true) == 0)
            {
                ResChangeCard();
            }
            //流转
            if (string.Compare(curFuncId, ResTransFuncId, true) == 0)
            {
                ResTransCard();
            }
        }

        /// <summary>
        /// 资源增加,除了资源流转和资源增加都显示
        /// </summary>
        private void ResAddCard()
        {
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("资源变更")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("资源流转")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("变更记录"))
                {
                    MenuBar.VisualComponent.ItemLinks[i].Visible = false;
                }
            }

        }
        /// <summary>
        /// 资源变更,只显示资源变更按钮
        /// </summary>
        private void ResChangeCard()
        {
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("保存并"))
                {
                    MenuBar.VisualComponent.ItemLinks[i].Visible = false;
                }
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("资源变更")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("变更记录")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("关闭")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("保存"))
                {
                    continue;
                }
                MenuBar.VisualComponent.ItemLinks[i].Visible = false;

            }
        }
        /// <summary>
        /// 资源流转,只显示资源流转按钮
        /// </summary>
        private void ResTransCard()
        {
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("资源流转")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("关闭"))
                {
                    continue;
                }
                MenuBar.VisualComponent.ItemLinks[i].Visible = false;

            }

        }
        /// <summary>
        /// 根据选择的资源类别生成资源编号
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        private string GetResCodeByType(string typeCode)
        {
            //var curTimeStr = DateTime.Now.ToString("yyyyMMddHHmm");
            //TODO 找当前资源类别开头的末尾五位数的最大值+1
            var curMaxCode = ResourceRestFul.GetClient().GetMaxResCode(typeCode);
            var num = string.Empty;
            //默认为5位长度流水号
            if (string.IsNullOrEmpty(curMaxCode))
            {
                num = "00001";
            }
            else
            {
                var maxLength = 5;

                var lastNum = curMaxCode.Remove(0, typeCode.Length);
                if (string.IsNullOrEmpty(lastNum))
                    lastNum = "00000";
                var nextNum = Convert.ToInt32(lastNum) + 1;
                num = nextNum.ToString().PadLeft(maxLength, '0');

            }
            
            var result = typeCode + num;
            return result;
        }

        /// <summary>
        /// 资源类别编号帮助后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResTypeCodeHelped(object sender, DictEntryPickedEventArgs e)
        {
            DictEntry dic = e.DictEntry;  //获取多值帮助选择的数据

            if (dic == null)  //没有选中任何值 "确定"
                return;
            else if (dic.Row == null) //"取消"
            {
                return;
            }

            var id = dic.ID;
            var code = dic.Code;
            var name = dic.Name;
            //名称赋值
            GetCurRow()[ResLbmc] = name;
            var resCode = GetResCodeByType(code);
            //给列表模型赋值
            GetCurRow()[ResZybh] = resCode;
            GetCurRow()[ResLbmc] = name;

            //给卡片赋值
            GetRightCardDataTable()[ResZybh] = resCode;
            GetRightCardDataTable()[ResKpbh] = resCode;
            GetRightCardDataTable()[ResLbmc] = name;

        }

        /// <summary>
        /// 资源类别名称帮助后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResTypeNameHelped(object sender, DictEntryPickedEventArgs e)
        {
            DictEntry dic = e.DictEntry;  //获取多值帮助选择的数据

            if (dic == null)  //没有选中任何值 "确定"
                return;
            else if (dic.Row == null) //"取消"
            {
                return;
            }

            var id = dic.ID;
            var code = dic.Code;
            var name = dic.Name;
            //编号赋值
            GetCurRow()[ResLbbh] = code;
            var resCode = GetResCodeByType(code);
            //给列表模型赋值
            GetCurRow()[ResZybh] = resCode;
            GetCurRow()[ResLbmc] = name;

            //给卡片赋值
            GetRightCardDataTable()[ResZybh] = resCode;
            GetRightCardDataTable()[ResKpbh] = resCode;
            GetRightCardDataTable()[ResLbmc] = name;

        }
        /// <summary>
        /// 所属部门帮助后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResSsbmHelped(object sender, DictEntryPickedEventArgs e)
        {
            DictEntry dic = e.DictEntry;  //获取多值帮助选择的数据

            if (dic == null)  //没有选中任何值 "确定"
                return;
            else if (dic.Row == null) //"取消"
            {
                return;
            }

            var id = dic.ID;
            //id赋值
            GetCurRow()[ResSsbmId] = id;
            //TODO 单位信息赋值
        }

        /// <summary>
        /// 计量单位帮助后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResJldwHelped(object sender, DictEntryPickedEventArgs e)
        {
            DictEntry dic = e.DictEntry;  //获取多值帮助选择的数据

            if (dic == null)  //没有选中任何值 "确定"
                return;
            else if (dic.Row == null) //"取消"
            {
                return;
            }

            var id = dic.ID;
            //id赋值
            GetCurRow()[ResJldwId] = id;
        }

        /// <summary>
        /// 经营方式帮助后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResJyfsHelped(object sender, DictEntryPickedEventArgs e)
        {
            DictEntry dic = e.DictEntry;  //获取多值帮助选择的数据

            if (dic == null)  //没有选中任何值 "确定"
                return;
            else if (dic.Row == null) //"取消"
            {
                return;
            }

            var id = dic.ID;
            //id赋值
            GetCurRow()[ResJyfsId] = id;
        }

        /// <summary>
        /// 使用状况帮助后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResSyzkHelped(object sender, DictEntryPickedEventArgs e)
        {
            DictEntry dic = e.DictEntry;  //获取多值帮助选择的数据

            if (dic == null)  //没有选中任何值 "确定"
                return;
            else if (dic.Row == null) //"取消"
            {
                return;
            }

            var id = dic.ID;
            //id赋值
            GetCurRow()[ResSyzkId] = id;
        }

        /// <summary>
        /// 当焦点行切换时刷新当前按钮的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void SelectRowChanged(object sender, EventArgs arg)
        {
            var index = ListGrid.SelectedIndex;
            //过滤行字段
            if (index < 0 && confirmRes == false)
                return;

            //资源变更
            if (this.CompContext.IsEditing)
            {
                this.BarBtnChange.VisualComponent.Enabled = true;
            }

            var x = GetCurRow();
            //切换按钮状态
            //数据库为int 看看空的时候是不是为null,或者为新增
            var resState = (ResourceState)Convert.ToInt32(GetCurRow()[ResState]);


            if (resState == ResourceState.Editing)
            {
                //待确认的资源没有变更记录
                BarBtnChangeLog.VisualComponent.Enabled = false;
                BarBtnEdit.VisualComponent.Enabled = true;
                BarBtnDelete.VisualComponent.Enabled = true;
                return;
            }

            BarBtnChangeLog.VisualComponent.Enabled = true;
            //已经确认
            BarBtnEdit.VisualComponent.Enabled = false;
            BarBtnDelete.VisualComponent.Enabled = false;

            //先硬刷新一把,一定放在最后
            UpdataUIByFuncId();

        }



        #endregion


    }
}
