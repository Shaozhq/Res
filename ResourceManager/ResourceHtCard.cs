using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Genersoft.Platform.AppFramework.ClientService;
using Genersoft.Platform.ChangeLog.RemoteClient;
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
    /// 资源合同卡片扩展
    /// </summary>
    public class ResourceHtCard: BasicSearchWizzardUtilityController
    {
        #region 字段名
        /// <summary>
        /// 合同状态编号
        /// </summary>
        private readonly string HtZsState = "HtZsState";
        /// <summary>
        /// 合同状态名称
        /// </summary>
        private readonly string HtZsStateMc = "HtZsStateMc";
        /// <summary>
        /// 合同内码
        /// </summary>
        private readonly string Htnm = "htnm";
        #endregion

        #region 控件ID
        /// <summary>
        /// 资源编号帮助
        /// </summary>
        private readonly string ctrlResCode ="XSmartDictLookup2";
        /// <summary>
        /// 资源名称帮助
        /// </summary>
        private readonly string ctrlResName ="XSmartDictLookup3";
        /// <summary>
        /// 左列表控件
        /// </summary>
        private readonly string ctrlgridViewl = "XDataGrid1";
        /// <summary>
        /// 工具栏
        /// </summary>
        private readonly string menuBar = "Bar1";
        /// <summary>
        /// 编辑菜单
        /// </summary>
        private readonly string ctrlBtnEdit = "btnItemEdit";
        /// <summary>
        /// 合同变更菜单
        /// </summary>
        private readonly string ctrlBtnChange = "BarButtonItem3";

        /// <summary>
        /// 变更记录
        /// </summary>
        private readonly string ctrlBtnChangLog = "BarButtonItem4";
        #endregion


        #region 控件实体

        /// <summary>
        /// 资源编号
        /// </summary>
        private DataDictLookUpElement ResCodeHelp
        {
            get { return GetDataDictLookUp(ctrlResCode); }
        }

        /// <summary>
        /// 资源名称
        /// </summary>
        private DataDictLookUpElement ResNameHelp
        {
            get { return GetDataDictLookUp(ctrlResName); }
        }

        /// <summary>
        /// 左列表
        /// </summary>
        private RepeatElement ListGrid
        {
            get { return GetDataGrid(ctrlgridViewl); }
        }
        /// <summary>
        /// 工具栏
        /// </summary>
        private BarElement MenuBar
        {
            get { return GetMenuBar(menuBar); }
        }
        /// <summary>
        /// 编辑按钮
        /// </summary>
        private BarButtonItemElement BarBtnEdit
        {
            get { return GetButton(ctrlBtnEdit); }
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
        #endregion

        #region 获取控件方法
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

        #endregion
        /// <summary>
        /// 必须要写的构造函数
        /// </summary>
        /// <param name="context"></param>
        public ResourceHtCard(IFormActionContext context) : base(context)
        {
            
        }

        #region 私有方法

        /// <summary>
        /// 对应资源内码
        /// </summary>
        /// <returns></returns>
        private string GetCurCardResId()
        {
            return CompContext.DefaultInstanceData.DataSet.Tables[0].Rows[0]["Resnm"].ToString();
        }

        /// <summary>
        /// 获取表单上选定的行
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
        private HtState GetCurRowState()
        {
            var value = (HtState)Convert.ToInt32(GetCurRow()[HtZsState]);
            return value;
        }

        #endregion


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
        /// 初始表单状态
        /// </summary>
        [ControllerMethod]
        public void InitFormState()
        {
            //焦点行切换后
            ListGrid.Repeat_SelectRowChanged += SelectRowChanged;
            var actionId = this.CompContext.FormUri.InitialActionID;
            //通过是否新增来判断是否有相应的合同,不同的表单可能不一定是Create
            //非新增会传递过来DataId显示合同并进入编辑动作
            //判断是否联查，有问题，框架上打开了会直接调到框架
            if (CompContext.FormUri.Parameters.AllKeys.Contains("isLc"))
            {
                //联查进来只有新增的界面或者后期在处理
                HtAddEditFromLc();
                //联查新增
                if (string.Compare(actionId.Trim(), "Add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var rescourceCode = CompContext.FormUri.Parameters["ResZybh"];
                    var rescourceId = CompContext.FormUri.Parameters["ResNm"];
                    var rescourceName = CompContext.FormUri.Parameters["ResZymc"];
                    //默认带过来编号和名称
                    CompContext.DefaultInstanceData.DataSet.Tables[0].Rows[0]["Resnm"] = rescourceId;
                    CompContext.DefaultInstanceData.DataSet.Tables[0].Rows[0]["ResBh"] = rescourceCode;
                    CompContext.DefaultInstanceData.DataSet.Tables[0].Rows[0]["ResMc"] = rescourceName;

                    //保存走的是卡片，所以要卡片,合同状态
                    GetCurRow()[HtZsState] = (int)HtState.Editing;
                    GetCurRow()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Editing);
                    //右边同步显示
                    GetRightCardDataTable()[HtZsState] = (int)HtState.Editing;
                    GetRightCardDataTable()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Editing);
                }
                else//联查有对应合同
                {
                    
                }
                //联查编辑
            }
            else //框架菜单进入
            {
                UpdataUIByFuncId();

            }

            SelectRowChanged(this, null);
        }

        /// <summary>
        /// 当前合同保存后，立即更新对应资源的状态信息
        /// </summary>
        [ControllerMethod]
        public void AfterSave()
        {
            //保存后必须设定对应的资源状态为已生成合同状态
            //可能会有问题，比如再次编辑呢。。。。。应该也没事
            //要判断是否联查进来的
            if (CompContext.FormUri.Parameters.AllKeys.Contains("isLc"))
            {
                //保存走的是卡片，所以要卡片
                GetCurRow()[HtZsState] = (int)HtState.Editing;
                GetCurRow()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Editing);
                //右边同步显示
                GetRightCardDataTable()[HtZsState] = (int)HtState.Editing;
                GetRightCardDataTable()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Editing);
                //联查进来的要单独保存一下
                var htnm = GetCurRow()["htnm"].ToString();
                ResourceRestFul.GetClient().SaveHtState((int) HtState.Editing, htnm);

            }

            ResourceRestFul.GetClient().SaveResState((int)ResourceState.HasBuildHt, GetCurCardResId());
            this.BarBtnChange.VisualComponent.Enabled = true;
        }

        /// <summary>
        /// 编辑合同的时候不允许编辑这两个字段，因为这个是有相应的资源已经有了状态啦，不能更改，包括变更，只有新增的时候可以选择
        /// </summary>
        [ControllerMethod]
        public void BeforeEdit()
        {
            //无论是不是联查，编辑前都是只读
            ResCodeHelp.ReadOnly = true;
            ResNameHelp.ReadOnly = true;
        }

        /// <summary>
        /// 新增前要保证字段允许编辑但是要排除联查过来的为只读
        /// </summary>
        [ControllerMethod]
        public void BeforeAdd()
        {
            ////如果是联查，不允许修改,实际发现联查过来的不可以
            //if (CompContext.FormUri.Parameters.AllKeys.Contains("isLc"))
            //{
            //    ResCodeHelp.ReadOnly = true;
            //    ResNameHelp.ReadOnly = true;
            //}
            //else//如果是界面新增
            //{
                ResCodeHelp.ReadOnly = false;
                ResNameHelp.ReadOnly = false;
            //}
        }

        /// <summary>
        /// 初始合同状态
        /// </summary>
        [ControllerMethod]
        public void SetHtStateWhenAdd()
        {
            //保存走的是卡片，所以要卡片
            GetCurRow()[HtZsState] = (int)HtState.Editing;
            GetCurRow()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Editing);
            //右边同步显示
            GetRightCardDataTable()[HtZsState] = (int)HtState.Editing;
            GetRightCardDataTable()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Editing);
        }

        /// <summary>
        /// 合同确认后要同步状态
        /// </summary>
        [ControllerMethod]
        public void HtConfirm()
        {
            var index = ListGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择合同行数据!", "合同选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var curHtState = GetCurRowState();
            if (curHtState == HtState.HasConfirm)
            {
                MessageBox.Show("该合同已经被确认，无需再次确认!", "合同选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (curHtState == HtState.Stop)
            {
                MessageBox.Show("该合同已经被终止，无法确认!", "合同选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (curHtState == HtState.OutDay)
            {
                MessageBox.Show("该合同已经结束，无法确认!", "合同选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var question = MessageBox.Show("合同确认后将不可修改，是否确认合同?", "合同确认提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            //取消
            if (question == DialogResult.Cancel)
                return;

            //合同确认
            var state = (int)HtState.HasConfirm;
            var htId = GetCurRow()[Htnm].ToString();
            GetCurRow()[HtZsState] = state;
            ResourceRestFul.GetClient().SaveHtState(state, htId);
            GetCurRow()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.HasConfirm);
            GetRightCardDataTable()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.HasConfirm);
            MessageBox.Show("合同确认成功！", "合同确认提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //成功后触发一下当前按钮状态
            SelectRowChanged(this, null);
        }

        /// <summary>
        /// 合同变更
        /// </summary>
        [ControllerMethod]
        public void HtChange()
        {

            var index = ListGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择有效数据行!", "合同提示");
                return;
            }

            var curHtState = GetCurRowState();

        
            if (curHtState != HtState.HasConfirm)
            {
                MessageBox.Show("只有已确认状态的合同允许变更!", "合同提示");
                return;

            }

            var changeCol = "htchangeflag";
            var curValue = GetCurRow()[changeCol].ToString();
            var differentValue = Guid.NewGuid();
            GetCurRow()[changeCol] = differentValue;
            GetRightCardDataTable()[changeCol] = differentValue;
            this.BarBtnChange.VisualComponent.Enabled = false;
            this.CompContext.ExcuteAction("Edit");
  
            //合同变更逻辑
            ResCodeHelp.ReadOnly = true;
            ResNameHelp.ReadOnly = true;
        }

        /// <summary>
        /// 查看合同变更记录
        /// </summary>
        [ControllerMethod]
        public void ViewResChangeLog()
        {
            var state = ClientContext.Current.FramworkState;
            var resModel = "14f45b51-4b9a-418c-8b44-86ed3d144767";
            var dataid = GetCurRow()[Htnm].ToString();
            var result = ChangeLogRemoteClient.Instance.GetLogHeaderByDataId(state, dataid, resModel, "2018-03-05", "2018-07-01");
            var headId = result.Tables[0].Rows[0]["id"].ToString();
            var changeTime = result.Tables[0].Rows[0]["ChangeTime"].ToString();
            var entity = ChangeLogRemoteClient.Instance.GetLogItemDataSet(state, resModel, headId, changeTime);
        }

        /// <summary>
        /// 合同终止
        /// </summary>
        [ControllerMethod]
        public void HtStop()
        {
            var index = ListGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择合同行数据!", "合同选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var curHtState = GetCurRowState();
            if (curHtState != HtState.HasConfirm)
            {
                MessageBox.Show("只有已确认状态的合同允许终止！", "合同选择提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            var question = MessageBox.Show("合同终止后将不可恢复，是否终止合同?", "合同终止提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            //取消
            if (question == DialogResult.Cancel)
                return;
            var htId = GetCurRow()[Htnm].ToString();
            ResourceRestFul.GetClient().StopHt(htId);

            var stopState = (int)HtState.Stop;
            GetCurRow()[HtZsState] = stopState;
            GetCurRow()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Stop);
            GetRightCardDataTable()[HtZsStateMc] = HtStateDictionary.ConvertToString(HtState.Stop);
            //没有刷新,可能会有问题
            MessageBox.Show("合同终止成功");
            
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 合同增加
        /// </summary>
        private readonly string htAddFuncId = "0e57dd30-d4ed-4015-ba0e-562707219b0d";
        /// <summary>
        /// 合同变更
        /// </summary>
        private readonly string htChangeFuncId = "91d5b2cf-587b-484a-a329-5caff94698d4";
        /// <summary>
        /// 合同终止
        /// </summary>
        private readonly string htStopFuncId = "275c1561-c9f0-40bc-8a58-e86d6376e578";

        /// <summary>
        /// 根据菜单Id刷新界面
        /// </summary>
        private void UpdataUIByFuncId()
        {
            var curFuncId = ClientContext.Current.FramworkState.FuncID;
            //合同新增
            if (string.Compare(curFuncId, htAddFuncId, true) == 0)
            {
                HtAddForm();
            }
            //合同变更
            if (string.Compare(curFuncId, htChangeFuncId, true) == 0)
            {
                HtChangeForm();
            }
            //合同终止
            if (string.Compare(curFuncId, htStopFuncId, true) == 0)
            {
                HtStopForm();
            }

        }

        /// <summary>
        /// 合同新增
        /// </summary>
        private void HtAddForm()
        {
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同终止")
                || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同变更")
                || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("变更记录"))
                {
                    MenuBar.VisualComponent.ItemLinks[i].Visible = false;
                }
            }
        }

        /// <summary>
        /// 联查过来的新增或者编辑
        /// </summary>
        private void HtAddEditFromLc()
        {
            //资源编号都是新增
            ResCodeHelp.ReadOnly = true;
            ResNameHelp.ReadOnly = true;
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同终止")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同变更")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("新增")
                    || MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("变更记录"))
                {
                    MenuBar.VisualComponent.ItemLinks[i].Visible = false;
                }
            }
        }
        /// <summary>
        /// 合同变更
        /// </summary>
        private void HtChangeForm()
        {
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同确认")||
                    MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("新增")||
                    MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("编辑")||
                    MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同终止"))

                {
                    MenuBar.VisualComponent.ItemLinks[i].Visible = false;
                }
            }

        }

        private void HtStopForm()
        {
            for (var i = 0; i < MenuBar.VisualComponent.ItemLinks.Count; ++i)
            {
                if (MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("合同终止") ||
                    MenuBar.VisualComponent.ItemLinks[i].Caption.StartsWith("关闭"))
                {
                    continue;
                }
                MenuBar.VisualComponent.ItemLinks[i].Visible = false;
            }
            

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
            if (index < 0 )
                return;

            //合同变更
            if (this.CompContext.IsEditing)
            {
                this.BarBtnChange.VisualComponent.Enabled = true;
            }

            var x = GetCurRow();
            //切换按钮状态
            //数据库为int 看看空的时候是不是为null,或者为新增
            var curHtState = (HtState)Convert.ToInt32(GetCurRow()[HtZsState]);
            if (curHtState == HtState.Editing)
            {
                BarBtnChangeLog.VisualComponent.Enabled = false;
                BarBtnEdit.VisualComponent.Enabled = true;
                return;
            }

            BarBtnChangeLog.VisualComponent.Enabled = true;
            //已经确认
            BarBtnEdit.VisualComponent.Enabled = false;

            //先硬刷新一把,一定放在最后,因为事件触发没有区分菜单
            UpdataUIByFuncId();

        }

        #endregion



    }
}
