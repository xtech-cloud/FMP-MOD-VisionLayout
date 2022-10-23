namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FadeOutTransitionAction : TransitionAction
    {
        public const string NAME = "FadeOutTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);

            if (!filterLayoutCells(LayerCategory.Disappear))
                return;

            float blank = parseFloatFromProperty("blank");
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                cell.animDelay = UnityEngine.Random.Range(0f, duration / 2);
                cell.animDuration = duration / 2;
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
            if (null == layoutCells_)
                return;

            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                var alpha = UnityEngine.Mathf.Lerp(1, 0, percent);
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}
