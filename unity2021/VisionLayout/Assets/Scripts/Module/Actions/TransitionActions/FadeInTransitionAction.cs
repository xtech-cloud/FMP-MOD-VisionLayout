using System.Numerics;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FadeInTransitionAction : TransitionAction
    {
        public const string NAME = "FadeInTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.In);
            if (!active_)
                return;

            baseEnter(NAME);
            showLayer();

            if (!filterLayoutCells(LayerCategory.Display))
                return;

            float blank = parseFloatFromProperty("blank");
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                cell.animDelay = blank + UnityEngine.Random.Range(0f, (duration - blank) / 2);
                cell.animDuration = (duration - blank) / 2;
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

            // 移动所有目标节点到动画结束的位置
            foreach (var cell in animCells)
            {
                cell.canvasGroup.alpha = 1;
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();
            if (null == animCells)
                return;

            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                var alpha = UnityEngine.Mathf.Lerp(0, 1, percent);
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}
