
using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 选择变换
    /// </summary>
    public class SelectTransitionAction : BaseAction
    {
        public const string NAME = "SelectTransition";

        /// <summary>
        /// 进入变换的行为列表
        /// </summary>
        /// <remarks>
        /// {
        ///     top: [HorizontalFlow, VerticalFlow],
        ///     bottom: [VerticalSplit, Stacked],
        /// }
        /// </remarks>
        public Dictionary<string, List<string>> inActions = new Dictionary<string, List<string>>();

        /// <summary>
        /// 退出变换的行为列表
        /// </summary>
        /// <remarks>
        /// {
        ///     top: [HorizontalFlow, VerticalFlow],
        ///     bottom: [VerticalSplit, Stacked],
        /// }
        /// </remarks>
        public Dictionary<string, List<string>> outActions = new Dictionary<string, List<string>>();

        /// <summary>
        /// 当行为进入时
        /// </summary>
        /// <remarks>
        /// 从一个待消失的层切换到待显示的层，同时随机选择一个进入的变换行为和一个退出的变换行为设置到参数中
        /// </remarks>
        protected override void onEnter()
        {
            logger.Trace(NAME+".onEnter");

            var layerDisplay = getParameter(ParameterDefine.Layer_Display).AsString;
            var layerDisappear = getParameter(ParameterDefine.Layer_Disappear).AsString;
            logger.Debug("ready to transition layer: {0}->{1}", layerDisappear, layerDisplay);

            // 从进入变换的行为列表中随机选择下一个进入的行为
            var inAry = inActions[layerDisplay];
            var inIdx = new System.Random().Next(0, inAry.Count);
            var inAction = inAry[inIdx];
            logger.Debug("select transition in-action: {0}", inAction);
            setParameter(ParameterDefine.Action_In, Parameter.FromString(inAction));

            // 从退出变换的行为列表中随机选择下一个退出的行为
            var outAry = outActions[layerDisappear];
            var outIdx = new System.Random().Next(0, outAry.Count);
            var outAction = outAry[outIdx];
            logger.Debug("select transition out-action: {0}", outAction);
            setParameter(ParameterDefine.Action_Out, Parameter.FromString(outAction));
            // 完成行为
            finish();
        }

        /// <summary>
        /// 当行为退出时
        /// </summary>
        protected override void onExit()
        {
            logger.Trace(NAME+".onExit");
        }

        /// <summary>
        /// 当行为更新时
        /// </summary>
        protected override void onUpdate()
        {
        }
    }

}
