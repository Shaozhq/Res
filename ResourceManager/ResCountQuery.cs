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
    public class ResCountQuery: BasicSearchWizzardUtilityController
    {
        private readonly GSPState curState;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public ResCountQuery(IFormActionContext context) : base(context)
        {
            this.curState = context.ElementHandler.ADPUIState;
        }

        /// <summary>
        /// 资源类别单选框控件ID
        /// </summary>
        private readonly string ResLbXSelector = "ResLbXSelector";
        private ComboBoxElement ResLbLayer
        {
            get { return GetComBox(ResLbXSelector); }
        }

        /// <summary>
        /// 获取界面帮助控件的Element
        /// </summary> 
        private ComboBoxElement GetComBox(string elementId)
        {
            var ele = CompContext.GetElementByID<ComboBoxElement>(XFMLConstants.ELEMENT_COMBOBOX, elementId);
            if (ele == null)
            {
                throw new InvalidFormDefException($"无法找到ID为{elementId}的值映射的ComboBoxElement控件");
            }
            return ele;
        }
        [ControllerMethod]
        public void SetResLayer()
        {
            //todo 根据资源类别获取最大级数替换成明细,默认是3级
            int maxLayer = 3;
            ResLbLayer.VisualComponent.DataSource = null;
            for (int i = 1; i < maxLayer; i++)
            {
                var curLayer = i.ToString();
                ResLbLayer.AppendItem(curLayer, curLayer);
            }
            //最后一级用明细表示
            ResLbLayer.AppendItem("明细", maxLayer.ToString());
        }
    }
}

