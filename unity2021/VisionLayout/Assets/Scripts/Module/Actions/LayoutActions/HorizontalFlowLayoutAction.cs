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
        /// 行末尾元素的位置
        /// </summary>
        private Dictionary<int, int> rowEdgeS_ = new Dictionary<int, int>();
        /// <summary>
        /// 行需要移动的元素
        /// </summary>
        private Dictionary<int, Cell> rowMoveCellS_ = new Dictionary<int, Cell>();

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.Display);
            if (!active_)
                return;

            baseEnter(NAME);
            if(!filterLayoutCells(LayerCategory.Display))
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

            for (int i = 0; i < rowCount_; i++)
            {
                rowEdgeS_[i] = i % 2 == 0 ? int.MaxValue : -int.MaxValue;
            }
            rowMoveCellS_.Clear();
            float offset = UnityEngine.Time.deltaTime * speed_;

            //TODO dummyboards
            //var dummyBoards = model.DummyBoards;
            foreach (var cell in layoutCells_)
            {
                float x = cell.target.anchoredPosition.x + offset * cell.directionX;
                UnityEngine.Vector2 pos = new UnityEngine.Vector2(x, cell.pinY);
                cell.dynamicX = pos.x;
                cell.dynamicY = pos.y;
                //if (cell.surround)
                 //   pos.y = roundDummyBoardFitY(dummyBoards, layerPattern.dummyBoard.radius, pos);
                cell.target.anchoredPosition = pos;

                if (cell.directionX > 0)
                {
                    // 移动超出屏幕的节点到队尾
                    if (x - cell.width / 2 > canvasWidth_ / 2)
                        rowMoveCellS_[cell.row] = cell;
                    if (x < rowEdgeS_[cell.row])
                        rowEdgeS_[cell.row] = (int)x - cell.width / 2;
                }
                else if (cell.directionX < 0)
                {
                    // 移动超出屏幕的节点到队尾
                    if (x + cell.width / 2 < -canvasWidth_ / 2)
                        rowMoveCellS_[cell.row] = cell;
                    if (x > rowEdgeS_[cell.row])
                        rowEdgeS_[cell.row] = (int)x + cell.width / 2;
                }
            }

            foreach (var row in rowMoveCellS_.Keys)
            {
                var cell = rowMoveCellS_[row];
                if (null == cell)
                    continue;
                cell.target.anchoredPosition = new UnityEngine.Vector2(rowEdgeS_[cell.row] - space_ * cell.directionX - cell.width / 2 * cell.directionX, cell.target.anchoredPosition.y);
            }
        }

        protected override void layout(List<string> _contentList)
        {
            List<Cell> cells = new List<Cell>();

            speed_ = parseFloatFromProperty("speed");
            bool surround = parseBoolFromProperty("surround");

            rowCount_ = parseIntFromProperty("row");
            space_ = parseIntFromProperty("space");
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            int cellHeight = (canvasHeight_ - (rowCount_ + 1) * space_) / rowCount_;
            int cellWidth = cellHeight;

            int counter = 0;
            // 最下方为首行
            // 首行往右移动
            int direction = 1;
            // 首个节点在右下侧
            int lastPinX = canvasWidth_ / 2 * direction;
            int lastPinY = -canvasHeight_ / 2;
            int rowIndex = 0;
            // 在屏幕外预留节点
            int outOfEdgeCount = 0;
            // 先将节点按行从左往右排列
            while (true)
            {
                if (rowIndex >= rowCount_)
                    break;

                string contentUri = "";
                int fitWidth = cellWidth;
                int fitHeight = cellHeight;
                UnityEngine.Texture2D coverTexture = null;
                if (_contentList.Count > 0)
                {
                    contentUri = _contentList[counter % _contentList.Count];
                    object cover;
                    if (preloadsRepetition.TryGetValue(contentUri+"/cover.png", out cover))
                    {
                        coverTexture = cover as UnityEngine.Texture2D;
                        if (null != coverTexture)
                        {
                            fitWidth = (int)((float)coverTexture.width / coverTexture.height * fitHeight);
                        }
                    }
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
                if (outOfEdgeCount >= 3)
                {
                    rowIndex += 1;
                    direction *= -1;
                    lastPinX = canvasWidth_ / 2 * direction;
                    lastPinY = pinY + fitHeight / 2;
                    outOfEdgeCount = 0;
                    continue;
                }

                counter += 1;
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
            }

            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

    }//class
}//namespace
