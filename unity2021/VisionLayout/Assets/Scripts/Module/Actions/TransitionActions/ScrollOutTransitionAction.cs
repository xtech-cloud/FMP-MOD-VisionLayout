using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class ScrollOutTransitionAction : TransitionAction
    {
        public const string NAME = "ScrollOutTransition";

        private Image bgImg_ { get; set; }
        private float blank { get; set; }

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);

            if (!filterLayoutCells(LayerCategory.Disappear))
                return;

            var tLayer = runtimeClone.layerMap[layer];
            bgImg_ = tLayer.Find("bg").GetComponent<Image>();
            bgImg_.color = new Color(1, 1, 1, 0);
            bgImg_.gameObject.SetActive(true);

            blank = parseFloatFromProperty("blank");
            animCells = filterInCanvasRectCells();
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            hideLayer();

            bgImg_.color = new Color(1, 1, 1, 0);
            foreach (var cell in animCells)
            {
                cell.target.gameObject.SetActive(false);
                // 动画结束时将状态设置为动画开始前的原始状态
                cell.canvasGroup.alpha = 1;
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();

            if (timer_ < blank)
                return;

            float percent = (timer_ - blank) / (duration - blank);
            float alpha = UnityEngine.Mathf.Lerp(1, 0, percent);
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
