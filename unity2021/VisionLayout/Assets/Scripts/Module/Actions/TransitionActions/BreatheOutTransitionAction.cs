﻿namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class BreatheOutTransitionAction : TransitionAction
    {
        public const string NAME = "BreatheOutTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);

            if (!filterLayoutCells(LayerCategory.Disappear))
                return;


            // 计算动画开始的位置
            // 并移动所有目标节点到动画开始位置
            float blank = parseFloatFromProperty("blank");
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                cell.animDelay = UnityEngine.Random.Range(0f, duration / 2);
                cell.animDuration = duration / 2;
                cell.animStartPos.x = cell.dynamicX;
                cell.animStartPos.y = cell.dynamicY;
                cell.animEndPos.x = 0;
                cell.animEndPos.y = 0;
                cell.target.anchoredPosition = cell.animStartPos;
                cell.target.localScale = UnityEngine.Vector3.one;
            }
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            hideLayer();
            if (null == animCells)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                cell.target.gameObject.SetActive(false);
                // 动画结束时将状态设置为动画开始前的原始状态
                pos.x = cell.dynamicX;
                pos.y = cell.dynamicY;
                cell.target.anchoredPosition = pos;
                cell.target.localScale = UnityEngine.Vector3.one;
                cell.canvasGroup.alpha = 1;
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();
            if (null == layoutCells_)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            UnityEngine.Vector2 scale = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                pos = UnityEngine.Vector2.Lerp(cell.animStartPos, cell.animEndPos, percent);
                scale = UnityEngine.Vector2.Lerp(UnityEngine.Vector3.one, UnityEngine.Vector3.zero, percent);
                var alpha = UnityEngine.Mathf.Lerp(cell.pinAlpha, 0, percent);
                cell.target.anchoredPosition = pos;
                cell.target.localScale = scale;
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}
