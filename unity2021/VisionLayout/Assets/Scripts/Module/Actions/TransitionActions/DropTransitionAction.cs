﻿namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class DropTransitionAction : TransitionAction
    {
        public const string NAME = "DropTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);

            if (!filterLayoutCells(LayerCategory.Disappear))
                return;

            // 计算动画结束的位置
            int canvasHeight = getParameter("virtual_resolution_height").AsInt;
            int margin = 10;
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                int currentX = (int)cell.dynamicX;
                int currentY = (int)cell.dynamicY;
                cell.animDelay = UnityEngine.Random.Range(0f, duration / 2);
                cell.animDuration = duration / 2;
                cell.animStartPos.x = currentX;
                cell.animStartPos.y = currentY;
                cell.animEndPos.x = currentX;
                cell.animEndPos.y = -canvasHeight / 2 - cell.height / 2 - margin;
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
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();
            if (null == layoutCells_)
                return;

            if (null == animCells)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                pos = UnityEngine.Vector2.Lerp(cell.animStartPos, cell.animEndPos, percent);
                cell.target.anchoredPosition = pos;
            }
        }
    }
}
