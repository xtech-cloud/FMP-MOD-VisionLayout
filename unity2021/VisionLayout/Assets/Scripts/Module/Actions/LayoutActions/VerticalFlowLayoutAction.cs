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
        /// 每列的最后一个节点
        /// </summary>
        private Dictionary<int, Cell> columnTailS_ = new Dictionary<int, Cell>();

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

            // 当前帧的节点的相对偏移位置
            float offset = UnityEngine.Time.deltaTime * speed_;

            //TODO DummyBoards
            //var dummyBoards = model.DummyBoards;
            foreach (var cell in layoutCells_)
            {
                cell.dynamicY += offset * cell.directionY;

                // 移动超出屏幕的节点到队尾
                // 正向为屏幕上方，负向为屏幕下方
                if (cell.directionY > 0)
                {
                    if (cell.dynamicY - cell.height / 2 > canvasHeight_ / 2)
                    {
                        cell.dynamicY = columnTailS_[cell.column].dynamicY - columnTailS_[cell.column].height / 2 - space_ - cell.height / 2 + offset;
                        columnTailS_[cell.column] = cell;
                    }
                }
                else if (cell.directionY < 0)
                {
                    if (cell.dynamicY + cell.height / 2 < -canvasHeight_ / 2)
                    {
                        cell.dynamicY = columnTailS_[cell.column].dynamicY + columnTailS_[cell.column].height / 2 + space_ + cell.height / 2 - offset;
                        columnTailS_[cell.column] = cell;
                    }
                }
                //if (cell.surround)
                //    pos.x = roundWorckbenchFitX(dummyBoards, config.dummyBoard.radius, pos);
                var pos = cell.target.anchoredPosition;
                pos.y = cell.dynamicY;
                cell.target.anchoredPosition = pos;
            }
        }
        protected override void layout(List<string> _contentList)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            speed_ = parseFloatFromProperty("speed");
            bool surround = parseBoolFromProperty("surround");
            columnCount_ = parseIntFromProperty("column");
            space_ = parseIntFromProperty("space");

            int cellWidth = (canvasWidth_ - (columnCount_ + 1) * space_) / columnCount_;

            // 最左方为首列
            // 首列往上移动
            int direction = 1;
            // 首个节点的位置在屏幕左上角
            int lastPinX = -canvasWidth_ / 2;
            int lastPinY = canvasHeight_ / 2 * direction;
            int columnIndex = 0;
            // 屏幕外节点的数量
            int outOfEdgeCount = 0;
            // 屏幕外节点的换列数量
            const int outOfEdgeWrapCount = 3;

            List<Cell> cells = new List<Cell>();
            // 先将节点按行从上往下排列
            while (true)
            {
                if (columnIndex >= columnCount_)
                    break;

                string contentUri = "";
                int fitWidth = cellWidth;
                int fitHeight = cellWidth;
                UnityEngine.Texture2D coverTexture = null;
                if (_contentList.Count > 0)
                {
                    contentUri = _contentList[cells.Count % _contentList.Count];
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

                int pinY = lastPinY - space_ * direction - fitHeight / 2 * direction;
                lastPinY = pinY - fitHeight / 2 * direction;
                int pinX = lastPinX + space_ + fitWidth / 2;

                // 超出屏幕
                if (pinY > canvasHeight_ / 2 || pinY < -canvasHeight_ / 2)
                {
                    outOfEdgeCount += 1;
                }

                // 换行
                if (outOfEdgeCount >= outOfEdgeWrapCount)
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
                columnTailS_[columnIndex] = cell;
            }

            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

    }
}
