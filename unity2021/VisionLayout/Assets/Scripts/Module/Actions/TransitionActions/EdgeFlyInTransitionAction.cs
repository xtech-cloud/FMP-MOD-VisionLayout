﻿namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 边缘飞入变换
    /// </summary>
    public class EdgeFlyInTransitionAction : TransitionAction
    {
        public const string NAME = "EdgeFlyInTransition";

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
            float margin = 10;

            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                var edge = UnityEngine.Random.Range(0, 4);
                //左边界
                if (edge == 0)
                {
                    cell.animStartPos.x = -canvasWidth_ / 2 - cell.width / 2 - margin;
                    cell.animStartPos.y = UnityEngine.Random.Range(-canvasHeight_ / 2 - cell.height / 2 - margin, canvasHeight_ / 2 + cell.height / 2 + margin);
                }
                //右边界
                else if (edge == 1)
                {
                    cell.animStartPos.x = canvasWidth_ / 2 + cell.width / 2 + margin;
                    cell.animStartPos.y = UnityEngine.Random.Range(-canvasHeight_ / 2 - cell.height / 2 - margin, canvasHeight_ / 2 + cell.height / 2 + margin);
                }
                //上边界
                else if (edge == 2)
                {
                    cell.animStartPos.x = UnityEngine.Random.Range(-canvasWidth_ / 2 - cell.width / 2 - margin, canvasWidth_ / 2 + cell.width / 2 + margin);
                    cell.animStartPos.y = canvasHeight_ / 2 + cell.height / 2 + margin;
                }
                //下边界
                else if (edge == 3)
                {
                    cell.animStartPos.x = UnityEngine.Random.Range(-canvasWidth_ / 2 - cell.width / 2 - margin, canvasWidth_ / 2 + cell.width / 2 + margin);
                    cell.animStartPos.y = -canvasHeight_ / 2 - cell.height / 2 - margin;
                }
                int currentX = (int)cell.dynamicX;
                int currentY = (int)cell.dynamicY;
                cell.animDelay = blank + UnityEngine.Random.Range(0f, (duration - blank) / 2);
                cell.animDuration = (duration - blank) / 2;
                cell.animEndPos.x = currentX;
                cell.animEndPos.y = currentY;
                cell.target.anchoredPosition = cell.animStartPos;
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
                cell.canvasGroup.alpha = 1;
            }

        }

        protected override void onUpdate()
        {
            baseUpdate();
            if (null == animCells)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                pos = UnityEngine.Vector2.Lerp(cell.animStartPos, cell.animEndPos, percent);
                var alpha = UnityEngine.Mathf.Lerp(0, 1, percent);
                cell.target.anchoredPosition = pos;
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}
