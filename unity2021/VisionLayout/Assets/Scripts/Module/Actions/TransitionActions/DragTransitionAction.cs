
namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class DragTransitionAction : TransitionAction
    {
        public const string NAME = "DragTransition";

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
            int margin = 10;
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                int currentX = (int)cell.dynamicX;
                int currentY = (int)cell.dynamicY;
                cell.animDelay = blank + UnityEngine.Random.Range(0f, (duration - blank) / 2);
                cell.animDuration = (duration - blank) / 2;
                cell.animStartPos.x = currentX;
                cell.animStartPos.y = canvasHeight_ / 2 + cell.height / 2 + margin;
                cell.animEndPos.x = currentX;
                cell.animEndPos.y = currentY;
                cell.target.anchoredPosition = cell.animStartPos;
                cell.target.gameObject.SetActive(cell.pinVisible && isCellInCanvasRect(cell, canvasWidth_, canvasHeight_));
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
            // 移动所有目标节点到动画结束位置
            foreach (var cell in animCells)
            {
                pos.x = cell.animEndPos.x;
                pos.y = cell.animEndPos.y;
                cell.target.anchoredPosition = pos;
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
                cell.target.anchoredPosition = pos;
            }
        }
    }
}