using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FenchLayoutAction : LayoutAction
    {
        public const string NAME = "FenchLayout";

        private float moveDuration_ { get; set; }
        private float moveInterval_ { get; set; }
        private int columnCount_ { get; set; }
        private int columnWidth_ { get; set; }
        private float intervalTimer_ { get; set; }
        private bool isMoving_ { get; set; }

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.Display);
            if (!active_)
                return;

            baseEnter(NAME);
            if (!filterLayoutCells(LayerCategory.Display))
                return;

            restoreVisible();
            restoreInteractable();
            intervalTimer_ = 0;
            isMoving_ = false;
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
            disableInteractable();
            // 重新计算X坐标，避免扩展功能在移动动画执行的过程中，强制结束了行为状态，造成下次进入行为时，节点坐标不正确
            float offset = -canvasWidth_ / 2;
            foreach (var cell in layoutCells_)
            {
                cell.dynamicX = offset + columnWidth_ / 2;
                offset += columnWidth_;
                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.dynamicX, cell.dynamicY);
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();

            if (null == layoutCells_)
                return;

            // 如果布局在切换前的剩余时间不够执行移动效果，就不执行
            if (timer_ + moveDuration_ >= duration)
                return;

            intervalTimer_ += UnityEngine.Time.deltaTime;
            if (intervalTimer_ < moveInterval_)
                return;


            // 运行切换效果

            // 在切换开始时计算一次动画开始和结束的位置
            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            if (!isMoving_)
            {
                isMoving_ = true;
                foreach (var cell in layoutCells_)
                {
                    cell.animStartPos.x = cell.dynamicX;
                    cell.animStartPos.y = cell.dynamicY;
                    cell.animEndPos.x = cell.dynamicX - columnWidth_;
                    cell.animEndPos.y = cell.dynamicY;
                    cell.target.gameObject.SetActive(true);
                }
            }

            float precentage = (intervalTimer_ - moveInterval_) / moveDuration_;

            // 在切换结束后计算一次动画开始和结束的位置
            if (precentage >= 1.0)
            {
                intervalTimer_ = 0f;
                precentage = 1.0f;
                isMoving_ = false;
                // 将队首的元素移到队尾，并将第一个节点的位置、动画开始位置、动画结束位置均设置为最后一个节点后一个节点的位置
                var first = layoutCells_[0];
                var last = layoutCells_[layoutCells_.Count - 1];
                first.dynamicX = getColumnPositionX(layoutCells_.Count-1);
                first.animStartPos.x = first.dynamicX;
                first.animEndPos.x = first.dynamicX;
                layoutCells_.RemoveAt(0);
                layoutCells_.Add(first);
                first.target.gameObject.SetActive(false);
            }

            foreach (var cell in layoutCells_)
            {
                cell.dynamicX = UnityEngine.Mathf.Lerp(cell.animStartPos.x, cell.animEndPos.x, precentage);
                pos.x = cell.dynamicX;
                pos.y = cell.dynamicY;
                cell.target.anchoredPosition = pos;
            }
        }
        protected override void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            moveDuration_ = parseFloatFromProperty("moveDuration");
            moveInterval_ = parseFloatFromProperty("moveInterval");
            columnCount_ = parseIntFromProperty("column");
            columnWidth_ = canvasWidth_ / columnCount_;
            int cellWidth = columnWidth_;
            int cellHeight = canvasHeight_;

            int columnIndex = 0;

            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < _contentList.Count || i < columnCount_ + 1; i++)
            {
                string contentUri = _contentList.Count > 0 ? _contentList[cells.Count % _contentList.Count] : "";
                UnityEngine.Texture2D coverTexture = loadContentCover(contentUri);

                Cell cell = new Cell();
                cells.Add(cell);
                cell.width = cellWidth;
                cell.height = cellHeight;
                cell.pinX = (int)getColumnPositionX(i);
                cell.pinY = 0;
                cell.dynamicX = cell.pinX;
                cell.dynamicY = cell.pinY;
                cell.animStartPos.x = cell.pinX;
                cell.animStartPos.y = cell.pinY;
                cell.animEndPos.x = cell.pinX;
                cell.animEndPos.y = cell.pinY;
                cell.row = 0;
                cell.column = columnIndex;
                cell.directionX = 0;
                cell.directionY = 0;
                cell.contentUri = contentUri;
                cell.surround = false;
                cell.target = newCell(layer, contentUri).GetComponent<UnityEngine.RectTransform>();
                cell.image = cell.target.gameObject.GetComponent<UnityEngine.UI.RawImage>();
                cell.canvasGroup = cell.target.gameObject.GetComponent<UnityEngine.CanvasGroup>();
                cell.pinVisible = true;
                cell.target.gameObject.SetActive(false);

                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.pinX, cell.pinY);
                cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
                if (null != coverTexture)
                    cell.image.texture = coverTexture;
            }
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

        private float getColumnPositionX(int _index)
        {
            int offset = canvasWidth_ / 2 - columnWidth_ / 2;
            return _index * columnWidth_ - offset;
        }
    }
}
