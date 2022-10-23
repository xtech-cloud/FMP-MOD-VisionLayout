namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
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
                cell.animDelay = blank;
                cell.animDuration = (duration - blank);
                cell.animStartPos.x = cell.dynamicX;
                cell.animStartPos.y = cell.dynamicY;
                cell.animEndPos.x = 0;
                cell.animEndPos.y = 0;
                cell.target.anchoredPosition = cell.animStartPos;
                cell.canvasGroup.alpha = 1;
                cell.target.gameObject.SetActive(cell.pinVisible);
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

            // 移动所有目标节点到动画结束的位置
            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                pos.x = cell.animEndPos.x;
                pos.y = cell.animEndPos.y;
                cell.target.anchoredPosition = pos;
                cell.canvasGroup.alpha = 0;
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();
            if (null == layoutCells_)
                return;

            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                pos = UnityEngine.Vector2.Lerp(cell.animStartPos, cell.animEndPos, percent);
                var alpha = UnityEngine.Mathf.Lerp(1, 0, percent);
                cell.target.anchoredPosition = pos;
                cell.canvasGroup.alpha = alpha;
            }
        }
    }
}
