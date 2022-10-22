using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 横向流动布局
    /// </summary>
    public class HorizontalFlowLayoutAction : LayoutAction
    {
        public const string NAME = "HorizontalFlowLayout";

        private float speed_ { get; set; }
        private int space_ { get; set; }
        private int rowCount_ { get; set; }

        /// <summary>
        /// 每行的最后一个节点
        /// </summary>
        private Dictionary<int, Cell> rowTailS_ = new Dictionary<int, Cell>();

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

            if (null == layoutCells_)
                return;

            // 当前帧的节点的相对偏移位置
            float offset = UnityEngine.Time.deltaTime * speed_;

            //TODO dummyboards
            //var dummyBoards = model.DummyBoards;
            foreach (var cell in layoutCells_)
            {
                // 计算新的坐标位置
                cell.dynamicX += offset * cell.directionX;

                // 移动超出屏幕的节点到队尾
                // 正向为屏幕右方，负向为屏幕左方
                if (cell.directionX > 0)
                {
                    if (cell.dynamicX - cell.width / 2 > canvasWidth_ / 2)
                    {
                        cell.dynamicX = rowTailS_[cell.row].dynamicX - rowTailS_[cell.row].width / 2 - space_ - cell.width / 2 + offset;
                        rowTailS_[cell.row] = cell;
                    }
                }
                else if (cell.directionX < 0)
                {
                    if (cell.dynamicX + cell.width / 2 < -canvasWidth_ / 2)
                    {
                        cell.dynamicX = rowTailS_[cell.row].dynamicX + rowTailS_[cell.row].width / 2 + space_ + cell.width / 2 - offset;
                        rowTailS_[cell.row] = cell;
                    }
                }


                //if (cell.surround)
                //   pos.y = roundDummyBoardFitY(dummyBoards, layerPattern.dummyBoard.radius, pos);
                var pos = cell.target.anchoredPosition;
                pos.x = cell.dynamicX;
                cell.target.anchoredPosition = pos;
            }
        }

        protected override void layout(List<string> _contentList)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            speed_ = parseFloatFromProperty("speed");
            bool surround = parseBoolFromProperty("surround");
            rowCount_ = parseIntFromProperty("row");
            space_ = parseIntFromProperty("space");

            int cellHeight = (canvasHeight_ - (rowCount_ + 1) * space_) / rowCount_;

            // 正向为屏幕右方，负向为屏幕左方
            int direction = 1;
            // 首个节点的位置在屏幕右下角
            int lastPinX = canvasWidth_ / 2 * direction;
            int lastPinY = -canvasHeight_ / 2;
            int rowIndex = 0;
            // 屏幕外节点的数量
            int outOfEdgeCount = 0;
            // 屏幕外节点的换行数量
            const int outOfEdgeWrapCount = 3;

            List<Cell> cells = new List<Cell>();
            // 最下方为首行
            // 先将节点按行从左往右排列
            while (true)
            {
                if (rowIndex >= rowCount_)
                    break;

                int fitWidth = cellHeight;
                int fitHeight = cellHeight;
                string contentUri = _contentList.Count > 0 ? _contentList[cells.Count % _contentList.Count] : "";
                var coverTexture = loadContentCover(contentUri);
                if (null != coverTexture)
                {
                    fitWidth = (int)((float)coverTexture.width / coverTexture.height * fitHeight);
                }

                int pinX = lastPinX - space_ * direction - fitWidth / 2 * direction;
                lastPinX = pinX - fitWidth / 2 * direction;
                int pinY = lastPinY + space_ + fitHeight / 2;

                // 超出屏幕
                if (pinX > canvasWidth_ / 2 || pinX < -canvasWidth_ / 2)
                {
                    outOfEdgeCount += 1;
                }

                // 换行
                if (outOfEdgeCount >= outOfEdgeWrapCount)
                {
                    rowIndex += 1;
                    direction *= -1;
                    lastPinX = canvasWidth_ / 2 * direction;
                    lastPinY = pinY + fitHeight / 2;
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
                cell.row = rowIndex;
                cell.column = 0;
                cell.directionX = direction;
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

                rowTailS_[rowIndex] = cell;
            }

            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

    }//class
}//namespace
