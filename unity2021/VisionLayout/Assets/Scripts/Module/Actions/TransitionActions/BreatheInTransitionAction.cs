﻿using System.Numerics;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class BreatheInTransitionAction : TransitionAction
    {
        public const string NAME = "BreatheInTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.In);
            if (!active_)
                return;

            baseEnter(NAME);
            showLayer();

            if (!filterLayoutCells(LayerCategory.Display))
                return;


            // 计算动画开始的位置
            // 并移动所有目标节点到动画开始位置
            float blank = parseFloatFromProperty("blank");
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {

                cell.animDelay = blank + UnityEngine.Random.Range(0f, (duration - blank) / 2);
                cell.animDuration = (duration - blank) / 2;
                cell.animStartPos.x = 0;
                cell.animStartPos.y = 0;
                cell.animEndPos.x = cell.dynamicX;
                cell.animEndPos.y = cell.dynamicY;
                cell.target.anchoredPosition = cell.animStartPos;
                cell.target.localScale = UnityEngine.Vector3.zero;
                cell.canvasGroup.alpha = 0;
                cell.target.gameObject.SetActive(cell.pinVisible);
            }

        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
            if (null == animCells)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
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
            if (null == animCells)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            UnityEngine.Vector2 scale = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                pos = UnityEngine.Vector2.Lerp(cell.animStartPos, cell.animEndPos, percent);
                scale = UnityEngine.Vector3.Lerp(UnityEngine.Vector3.zero, UnityEngine.Vector3.one, percent);
                var alpha = UnityEngine.Mathf.Lerp(0, 1, percent);
                cell.target.anchoredPosition = pos;
                cell.target.localScale = UnityEngine.Vector3.one * scale;
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}
