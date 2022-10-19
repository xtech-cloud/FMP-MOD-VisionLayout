
namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 边缘飞出变换
    /// </summary>
    public class EdgeFlyOutTransitionAction : TransitionAction
    {
        public const string NAME = "EdgeFlyOutTransition";

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
            float margin = 10;
            animCells = filterInCanvasRectCells();
            foreach (var cell in animCells)
            {
                var edge = UnityEngine.Random.Range(0, 4);
                //左边界
                if (edge == 0)
                {
                    cell.animEndPos.x = -canvasWidth_ / 2 - cell.width / 2 - margin;
                    cell.animEndPos.y = UnityEngine.Random.Range(-canvasHeight_ / 2 - cell.height / 2 - margin, canvasHeight_ / 2 + cell.height / 2 + margin);
                }
                //右边界
                else if (edge == 1)
                {
                    cell.animEndPos.x = canvasWidth_ / 2 + cell.width / 2 + margin;
                    cell.animEndPos.y = UnityEngine.Random.Range(-canvasHeight_ / 2 - cell.height / 2 - margin, canvasHeight_ / 2 + cell.height / 2 + margin);
                }
                //上边界
                else if (edge == 2)
                {
                    cell.animEndPos.x = UnityEngine.Random.Range(-canvasWidth_ / 2 - cell.width / 2 - margin, canvasWidth_ / 2 + cell.width / 2 + margin);
                    cell.animEndPos.y = canvasHeight_ / 2 + cell.height / 2 + margin;
                }
                //下边界
                else if (edge == 3)
                {
                    cell.animEndPos.x = UnityEngine.Random.Range(-canvasWidth_ / 2 - cell.width / 2 - margin, canvasWidth_ / 2 + cell.width / 2 + margin);
                    cell.animEndPos.y = -canvasHeight_ / 2 - cell.height / 2 - margin;
                }
                int currentX = (int)cell.dynamicX;
                int currentY = (int)cell.dynamicY;
                cell.animDelay = UnityEngine.Random.Range(0f, duration / 2);
                cell.animDuration = duration / 2;
                cell.animStartPos.x = currentX;
                cell.animStartPos.y = currentY;
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

            //TODO DummyBoards
            //var dummyBoards = model.DummyBoards;
            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            // 移动所有目标节点到动画结束位置
            foreach (var cell in animCells)
            {
                pos.x = cell.animEndPos.x;
                pos.y = cell.animEndPos.y;
                //if(cell.surround)
                //   pos.x = roundWorckbenchFitX(dummyBoards, config.dummyBoard.radius, pos);
                cell.target.anchoredPosition = pos;
                cell.image.color = new UnityEngine.Color(1, 1, 1, cell.pinAlpha);
                cell.target.gameObject.SetActive(false);
            }

        }

        protected override void onUpdate()
        {
            baseUpdate();
            updateTitleOut();
            updateProfileOut();
            if (null == layoutCells_)
                return;

            //TODO DummyBoards
            //var dummyBoards = model.DummyBoards;
            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            UnityEngine.Color color;
            foreach (var cell in animCells)
            {
                if (timer_ < cell.animDelay)
                    continue;
                float percent = (timer_ - cell.animDelay) / cell.animDuration;
                pos = UnityEngine.Vector2.Lerp(cell.animStartPos, cell.animEndPos, percent);
                var alpha = UnityEngine.Mathf.Lerp(cell.pinAlpha, 0, percent);
                //if (cell.surround)
                //    pos.x = roundWorckbenchFitX(dummyBoards, config.dummyBoard.radius, pos);
                cell.target.anchoredPosition = pos;
                if (null == cell.image)
                    throw new System.Exception(cell.target.name);
                color = cell.image.color;
                color.a = alpha;
                cell.image.color = color;
            }
        }
    }
}
