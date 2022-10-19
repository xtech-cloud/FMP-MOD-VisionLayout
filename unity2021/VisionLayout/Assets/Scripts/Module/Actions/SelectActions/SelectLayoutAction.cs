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
        private string actionDisplay_ { get; set; }

        /// <summary>
        /// 消失层的布局行为
        /// </summary>
        private string actionDisappear_ { get; set; }

        /// <summary>
        /// 当行为进入时
        /// </summary>
        protected override void onEnter()
        {
            logger.Trace(NAME + ".onEnter");

            var len = layers.Count;
            if (len < 2)
            {
                logger.Error("only has {0} layers, the count must large than 2", layers.Count);
                return;
            }

            // 如果控制台激活并且已经设置了层
            bool consoleActive = getParameter(ParameterDefine.Console_IsActive).AsBool;
            if (consoleActive)
                selectFromConsole();
            else
                selectFromQueue();

            setParameter(ParameterDefine.Layer_Display, Parameter.FromString(layerDisplay_));
            setParameter(ParameterDefine.Action_Display, Parameter.FromString(actionDisplay_));
            setParameter(ParameterDefine.Layer_Disappear, Parameter.FromString(layerDisappear_));
            setParameter(ParameterDefine.Action_Disappear, Parameter.FromString(actionDisappear_));

            logger.Debug("ready to disappear layer:{0} layoutAction:{1}", layerDisappear_, actionDisappear_);
            logger.Debug("ready to display layer:{0} layoutAction:{1}", layerDisplay_, actionDisplay_);

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
            logger.Trace(NAME + ".onExit");
        }

        /// <summary>
        /// 从队列中选择
        /// </summary>
        private void selectFromQueue()
        {
            // 列表第一个为需要消隐的层
            layerDisappear_ = layers[0];
            actionDisappear_ = getParameter(ParameterDefine.Action_Display).AsString;

            // 列表第二个为需要显示的层
            layerDisplay_ = layers[1];
            // 需要显示的层的行为列表
            var aryDisplay = actions[layerDisplay_];
            // 随机选择显示布局的行为
            int idxDisplay = new System.Random().Next(0, aryDisplay.Count);
            actionDisplay_ = aryDisplay[idxDisplay];

            // 以正向循环队列的形式将过时的层移到队尾
            var firstLayer = layers[0];
            layers.RemoveAt(0);
            layers.Add(firstLayer);
        }

        /// <summary>
        /// 从控制台选择
        /// </summary>
        private void selectFromConsole()
        {
            // 列表第一个为需要消隐的层
            layerDisappear_ = layers[0];
            actionDisappear_ = getParameter(ParameterDefine.Action_Display).AsString;

            // 需要显示的层为控制台选择的层
            layerDisplay_ = getParameter(ParameterDefine.Console_Layer).AsString;
            // 需要显示的层的行为列表
            var aryDisplay = actions[layerDisplay_];
            // 随机选择显示布局的行为
            int idxDisplay = new System.Random().Next(0, aryDisplay.Count);
            actionDisplay_ = aryDisplay[idxDisplay];

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
