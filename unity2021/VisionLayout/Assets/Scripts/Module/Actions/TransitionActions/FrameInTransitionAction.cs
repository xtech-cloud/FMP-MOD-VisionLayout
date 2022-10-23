using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FrameInTransitionAction : TransitionAction
    {
        public const string NAME = "FrameInTransition";

        private Image bgImg_ { get; set; }

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.In);
            if (!active_)
                return;

            baseEnter(NAME);
            showLayer();

            if (!filterLayoutCells(LayerCategory.Display))
                return;

            var tLayer = runtimeClone.layerMap[layer];
            bgImg_ = tLayer.Find("bg").GetComponent<Image>();
            bgImg_.color = new Color(1, 1, 1, 0);
            bgImg_.gameObject.SetActive(true);

            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                cell.canvasGroup.alpha = 0;
                cell.target.gameObject.SetActive(cell.pinVisible);
            }
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            bgImg_.color = new Color(1, 1, 1, 1);
            foreach (var cell in animCells)
            {
                // 动画结束时将状态设置为动画开始前的原始状态
                cell.canvasGroup.alpha = 1;
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();

            float percent = timer_ / duration;
            float alpha = UnityEngine.Mathf.Lerp(0, 1, percent);
            var color = bgImg_.color;
            color.a = alpha;
            bgImg_.color = color;

            foreach (var cell in animCells)
            {
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}