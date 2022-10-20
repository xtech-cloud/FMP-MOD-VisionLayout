using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 选择布局的行为
    /// </summary>
    public class SelectLayoutAction : BaseAction
    {
        public const string NAME = "SelectLayout";

        /// <summary>
        /// 布局行为列表
        /// </summary>
        /// <remarks>
        /// key: 层的名称
        /// value: 布局行为的列表
        /// </remarks>
        public Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

        /// <summary>
        /// 层列表
        /// </summary>
        /// <remarks>
        /// item: 层的名称
        /// </remarks>
        public List<string> layers = new List<string>();

        /// <summary>
        /// 显示的层
        /// </summary>
        private string layerDisplay_ { get; set; }

        /// <summary>
        /// 消失的层
        /// </summary>
        private string layerDisappear_ { get; set; }

        /// <summary>
        /// 显示层的布局行为
        /// </summary>
        private string layoutActionDisplay_ { get; set; }

        /// <summary>
        /// 消失层的布局行为
        /// </summary>
        private string layoutActionDisappear_ { get; set; }

        /// <summary>
        /// 当行为进入时
        /// </summary>
        protected override void onEnter()
        {
            myInstance.getLogger().Trace(NAME + ".onEnter");

            var len = layers.Count;
            if (len < 2)
            {
                myInstance.getLogger().Error("only has {0} layers, the count must large than 2", layers.Count);
                return;
            }

            // 如果工具栏激活并且已经设置了层
            if (extendFeatures.toolbar.IsActive)
                selectFromToolBar();
            else
                selectFromQueue();

            // 设置选择后的结果
            setParameter(ParameterDefine.Layer_Display, Parameter.FromString(layerDisplay_));
            setParameter(ParameterDefine.LayoutAction_Display, Parameter.FromString(layoutActionDisplay_));
            setParameter(ParameterDefine.Layer_Disappear, Parameter.FromString(layerDisappear_));
            setParameter(ParameterDefine.LayoutAction_Disappear, Parameter.FromString(layoutActionDisappear_));

            myInstance.getLogger().Debug("select disappear layer:{0} layoutAction:{1}", layerDisappear_, layoutActionDisappear_);
            myInstance.getLogger().Debug("select display layer:{0} layoutAction:{1}", layerDisplay_, layoutActionDisplay_);

            finish();
        }

        /// <summary>
        /// 当行为更新时
        /// </summary>
        protected override void onUpdate()
        {
        }

        /// <summary>
        /// 当行为退出时
        /// </summary>
        protected override void onExit()
        {
            myInstance.getLogger().Trace(NAME + ".onExit");
        }

        /// <summary>
        /// 从队列中选择
        /// </summary>
        private void selectFromQueue()
        {
            // 列表第一个为选择后需要消隐的层
            layerDisappear_ = layers[0];
            layoutActionDisappear_ = getParameter(ParameterDefine.LayoutAction_Display).AsString;

            // 列表的第二个为选择后需要显示的层
            layerDisplay_ = layers[1];

            // 需要显示的层的行为列表
            var aryDisplay = actions[layerDisplay_];
            // 随机选择显示的层布局的行为
            int idxDisplay = new System.Random().Next(0, aryDisplay.Count);
            layoutActionDisplay_ = aryDisplay[idxDisplay];

            // 正向循环队列
            var firstLayer = layers[0];
            layers.RemoveAt(0);
            layers.Add(firstLayer);
        }

        /// <summary>
        /// 从工具栏选择
        /// </summary>
        private void selectFromToolBar()
        {
            // 列表第一个为需要消隐的层
            layerDisappear_ = layers[0];
            layoutActionDisappear_ = getParameter(ParameterDefine.LayoutAction_Display).AsString;

            // 需要显示的层为工具栏选择的层
            layerDisplay_ = extendFeatures.toolbar.activeLayer;
            // 需要显示的层的行为列表
            var aryDisplay = actions[layerDisplay_];
            // 随机选择显示布局的行为
            int idxDisplay = new System.Random().Next(0, aryDisplay.Count);
            layoutActionDisplay_ = aryDisplay[idxDisplay];

            // 以正向循环队列的形式将过时的层移到队尾
            var firstLayer = layers[0];
            layers.RemoveAt(0);
            layers.Add(firstLayer);
            // 将显示的层移到队首
            layers.Remove(layerDisplay_);
            layers.Insert(0, layerDisplay_);
        }


    }
}
