using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 竖直流动的布局
    /// </summary>
    public class VerticalFlowLayoutAction : LayoutAction
    {
        public const string NAME = "VerticalFlowLayout";

        private float speed_ { get; set; }
        private int space_ { get; set; }
        private int columnCount_ { get; set; }

        /// <summary>
        /// 行末尾元素的位置
        /// </summary>
        private Dictionary<int, int> columnEdgeS_ = new Dictionary<int, int>();
        /// <summary>
        /// 行需要移动的元素
        /// </summary>
        private Dictionary<int, Cell> columnMoveCellS_ = new Dictionary<int, Cell>();

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
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
            disableInteractable();
        }

        protected override void onUpdate()
        {
            baseUpdate();
            updateProfile();

            if (null == layoutCells_)
                return;

            for (int i = 0; i < columnCount_; i++)
            {
                columnEdgeS_[i] = i % 2 == 0 ? int.MaxValue : -int.MaxValue;
            }
            columnMoveCellS_.Clear();
            float offset = UnityEngine.Time.deltaTime * speed_;

            //TODO DummyBoards
            //var dummyBoards = model.DummyBoards;
            foreach (var cell in layoutCells_)
            {
                float y = cell.target.anchoredPosition.y + offset * cell.directionY;
                UnityEngine.Vector2 pos = new UnityEngine.Vector2(cell.pinX, y);
                cell.dynamicX = pos.x;
                cell.dynamicY = pos.y;
                //if (cell.surround)
                //    pos.x = roundWorckbenchFitX(dummyBoards, config.dummyBoard.radius, pos);
                cell.target.anchoredPosition = pos;

                if (cell.directionY > 0)
                {
                    // 移动超出屏幕的节点到队尾
                    if (y - cell.height / 2 > canvasHeight_ / 2)
                        columnMoveCellS_[cell.column] = cell;
                    if (y < columnEdgeS_[cell.column])
                        columnEdgeS_[cell.column] = (int)y - cell.height / 2;
                }
                else if (cell.directionY < 0)
                {
                    // 移动超出屏幕的节点到队尾
                    if (y + cell.height / 2 < -canvasHeight_ / 2)
                        columnMoveCellS_[cell.column] = cell;
                    if (y > columnEdgeS_[cell.column])
                        columnEdgeS_[cell.column] = (int)y + cell.height / 2;
                }
            }

            foreach (var column in columnMoveCellS_.Keys)
            {
                var cell = columnMoveCellS_[column];
                if (null == cell)
                    continue;
                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.target.anchoredPosition.x, columnEdgeS_[cell.column] - space_ * cell.directionY - cell.height / 2 * cell.directionY);
            }

        }
        protected override void layout(List<string> _contentList)
        {
            List<Cell> cells = new List<Cell>();

            speed_ = parseFloatFromProperty("speed");
            bool surround = parseBoolFromProperty("surround");

            columnCount_ = parseIntFromProperty("column");
            space_ = parseIntFromProperty("space");
            int canvasWidth = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            int cellWidth = (canvasWidth - (columnCount_ + 1) * space_) / columnCount_;
            int cellHeight = cellWidth;

            // 最下方为首列
            // 首列往上移动
            int direction = 1;
            // 首个节点在左上侧
            int lastPinX = -canvasWidth / 2;
            int lastPinY = canvasHeight_ / 2 * direction;
            int columnIndex = 0;
            // 在屏幕外预留节点
            int outOfEdgeCount = 0;
            // 避免BUG引起的死循环
            int counter = 0;
            // 先将节点按行从左往右排列
            while (true)
            {
                if (columnIndex >= columnCount_)
                    break;

                string contentUri = "";
                int fitWidth = cellWidth;
                int fitHeight = cellHeight;
                UnityEngine.Texture2D coverTexture = null;
                if (_contentList.Count > 0)
                {
                    contentUri = _contentList[counter % _contentList.Count];
                    object cover;
                    if (preloadsRepetition.TryGetValue(contentUri + "/cover.png", out cover))
                    {
                        coverTexture = cover as UnityEngine.Texture2D;
                        if (null != coverTexture)
                        {
                            fitHeight = (int)((float)coverTexture.height / coverTexture.width * fitWidth);
                        }
                    }
                }

                counter += 1;
                int pinY = lastPinY - space_ * direction - fitHeight / 2 * direction;
                lastPinY = pinY - fitHeight / 2 * direction;
                int pinX = lastPinX + space_ + fitWidth / 2;

                // 超出屏幕
                if (pinY > canvasHeight_ / 2 || pinY < -canvasHeight_ / 2)
                {
                    outOfEdgeCount += 1;
                }

                // 换行
                if (outOfEdgeCount >= 3)
                {
                    columnIndex += 1;
                    direction *= -1;
                    lastPinY = canvasHeight_ / 2 * direction;
                    lastPinX = pinX + fitWidth / 2;
                    outOfEdgeCount = 0;
                    continue;
                }

                Cell cell = new Cell();
                cells.Add(cell);
                cell.width = fitWidth;
                cell.height = fitHeight;
                cell.pinX = pinX;
                cell.pinY = pinY;
                cell.dynamicX = pinX;
                cell.dynamicY = pinY;
                cell.animStartPos.x = pinX;
                cell.animStartPos.y = pinY;
                cell.animEndPos.x = pinX;
                cell.animEndPos.y = pinY;
                cell.row = 0;
                cell.column = columnIndex;
                cell.directionY = direction;
                cell.contentUri = contentUri;
                cell.surround = surround;
                cell.target = newCell(layer, contentUri).GetComponent<UnityEngine.RectTransform>();
                cell.image = cell.target.gameObject.GetComponent<UnityEngine.UI.RawImage>();

                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.pinX, cell.pinY);
                cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
                cell.pinVisible = true;
                cell.target.gameObject.SetActive(false);
                if (null != coverTexture)
                    cell.image.texture = coverTexture;
            }

            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

    }
}
