using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class StackedLayoutAction : LayoutAction
    {
        public const string NAME = "StackedLayout";

        /// <summary>
        /// 视窗的左上角为(0,0)，右下角为(canvasWidth,canvasHeight)
        /// </summary>
        private class Viewport
        {
            public int index { get; set; }
            public int columnIndex { get; set; }
            public int[] columnBottomLines { get; set; }
            public int contentIndex { get; set; }
            public float totalOffset { get; set; }
            public UnityEngine.GameObject container { get; set; }
        }

        private int columnWidth_ { get; set; }
        private float speed_ { get; set; }
        private float alphaStep_ { get; set; }
        private int cellMinLength_ { get; set; }
        private int cellMaxLength_ { get; set; }
        private int minSpaceX_ { get; set; }
        private int maxSpaceX_ { get; set; }
        private int minSpaceY_ { get; set; }
        private int maxSpaceY_ { get; set; }
        private int columnCount_ { get; set; }

        private List<string> contentUriS_ = new List<string>();
        private List<Cell> needDeleteCells_ = new List<Cell>();

        private Viewport[] viewports_ { get; set; }

        /// <summary>
        /// 节点位于视窗（Viewport）中的锚点
        /// 节点锚点的左上角为(0,0)，右下角为(cellWidth,cellHeight)
        /// </summary>
        private class CellAnchor
        {
            public int posX;
            public int posY;
            public int width;
            public int height;
            public bool visible;
        }

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

            float offset = UnityEngine.Time.deltaTime * speed_;
            needDeleteCells_.Clear();
            UnityEngine.Vector2 pos = UnityEngine.Vector2.zero;
            int margin = 10;
            foreach (var cell in layoutCells_)
            {
                cell.dynamicY += offset;
                pos.x = cell.dynamicX;
                pos.y = cell.dynamicY;
                cell.target.anchoredPosition = pos;
                // 移除超出上边界的节点
                if (cell.dynamicY - cell.height / 2 - margin > canvasHeight_ / 2)
                    needDeleteCells_.Add(cell);
            }

            // 删除超出范围的节点
            foreach (var cell in needDeleteCells_)
            {
                layoutCells_.Remove(cell);
                UnityEngine.GameObject.DestroyImmediate(cell.target.gameObject);
            }

            // 补充节点
            foreach (var viewport in viewports_)
            {
                viewport.totalOffset += offset;
                while (true)
                {
                    // 最上方的列底线
                    float topPosY = viewport.columnBottomLines[0];
                    for (int j = 0; j < viewport.columnBottomLines.Length; j++)
                    {
                        topPosY = UnityEngine.Mathf.Min(topPosY, viewport.columnBottomLines[j]);
                    }
                    // 补充节点直到最上方的列底线超过视窗高度位置
                    if (topPosY - viewport.totalOffset > canvasHeight_)
                        break;
                    var cell = addCell(viewport);
                    layoutCells_.Add(cell);
                    cell.dynamicY += viewport.totalOffset;
                    pos.x = cell.dynamicX;
                    pos.y = cell.dynamicY;
                    cell.target.anchoredPosition = pos;
                }
            }
        }

        protected override void layout(List<string> _contentList)
        {
            contentUriS_ = _contentList;
            /*
            0  1  2  3  4  5  6
            | *|**|* | #|##|# |
            | *|**|* | #|##|# |
            | *|**|* | #|##|# |
            |  |  |  | #|##|# |
            | *|**|**|**|* |  |
            | *|**|**|**|* |  |
            | *|**|**|**|* |  |
            布局算法：
            1. 设视窗的左上角为(0,0)，右下角为(canvasWidth,canvasHeight)
            2. 将整个视窗划分为N列
            3. 将列游标移动到第0列
            4. 设备每个列的底线为0
            4.1 在列游标的所在列放入一个节点，此节点跨越K1至Kn个列
            4.2 节点的x坐标为K1列的坐标
            4.3 节点的y坐标为所跨越的k1-kn个列的最底部的列底线
            4.4 将k1-kn个列的列底线更新为节点的底部坐标
            4.5. 移动列游标到Kn
            5. 重复执行4.1-4.5
            */

            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;

            speed_ = parseFloatFromProperty("speed");
            alphaStep_ = parseFloatFromProperty("alphaStep");
            cellMinLength_ = parseIntFromProperty("cellMinLength");
            cellMaxLength_ = parseIntFromProperty("cellMaxLength");
            minSpaceX_ = parseIntFromProperty("minSpaceX");
            minSpaceY_ = parseIntFromProperty("minSpaceY");
            maxSpaceX_ = parseIntFromProperty("maxSpaceX");
            maxSpaceY_ = parseIntFromProperty("maxSpaceY");
            int span = parseIntFromProperty("span");
            columnCount_ = canvasWidth_ / span;

            columnWidth_ = canvasWidth_ / columnCount_;

            List<Cell> cells = new List<Cell>();
            // 上下两层视窗
            viewports_ = new Viewport[2];
            for (int i = 0; i < viewports_.Length; i++)
            {
                // 创建视窗
                UnityEngine.GameObject container = new UnityEngine.GameObject();
                container.name = string.Format("viewport#{0}", i);
                var rtContainer = container.AddComponent<UnityEngine.RectTransform>();
                // 视窗的父对象设置为CellContainer
                rtContainer.SetParent(runtimeClone.cellTemplateMap[layer].transform.parent);
                rtContainer.sizeDelta = UnityEngine.Vector2.zero;
                rtContainer.anchoredPosition = UnityEngine.Vector2.zero;
                rtContainer.anchorMin = UnityEngine.Vector2.zero;
                rtContainer.anchorMax = UnityEngine.Vector2.one;
                rtContainer.localPosition = UnityEngine.Vector3.zero;
                rtContainer.localScale = UnityEngine.Vector3.one;
                rtContainer.localRotation = UnityEngine.Quaternion.identity;

                viewports_[i] = new Viewport();
                var viewport = viewports_[i];
                viewport.container = container;
                viewport.index = i;
                viewport.columnIndex = 0;
                viewport.columnBottomLines = new int[columnCount_];
                viewport.contentIndex = 0;
                viewport.totalOffset = 0;
                while (true)
                {
                    // 最上端的列底线
                    float topPosY = viewport.columnBottomLines[0];
                    for (int j = 0; j < viewport.columnBottomLines.Length; j++)
                    {
                        topPosY = UnityEngine.Mathf.Min(topPosY, viewport.columnBottomLines[j]);
                    }
                    // 如果最上端的列底线已经超出了视窗高度，终止添加节点
                    if (topPosY > canvasHeight_)
                        break;
                    Cell cell = addCell(viewport);
                    cells.Add(cell);
                }
            }
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

        private Cell addCell(Viewport _viewport)
        {
            var contentUri = _viewport.contentIndex < contentUriS_.Count ? contentUriS_[_viewport.contentIndex] : "";
            UnityEngine.Texture2D coverTexture = loadContentCover(contentUri);
            Cell cell = new Cell();

            cell.contentUri = contentUri;
            cell.surround = false;

            cell.target = newCell(layer, contentUri).GetComponent<UnityEngine.RectTransform>();
            cell.target.SetParent(_viewport.container.transform);
            cell.target.Find("frame").gameObject.SetActive(_viewport.index == viewports_.Length - 1);
            cell.column = _viewport.columnIndex;
            // 赋值图片
            cell.pinAlpha = _viewport.index == viewports_.Length - 1 ? 1.0f : alphaStep_ * (_viewport.index + 1);
            cell.image = cell.target.gameObject.GetComponent<UnityEngine.UI.RawImage>();
            cell.canvasGroup = cell.target.gameObject.GetComponent<UnityEngine.CanvasGroup>();
            var color = cell.image.color;
            color.a = cell.pinAlpha;
            cell.image.color = color;
            if (null != coverTexture)
            {
                cell.image.texture = coverTexture;
            }

            // 根据图片计算大小
            int length = UnityEngine.Random.Range(cellMinLength_, cellMaxLength_);
            int width = length;
            int height = length;
            if (coverTexture.width > coverTexture.height)
            {
                width = length;
                height = fitHeight(coverTexture.width, coverTexture.height, length);
            }
            else
            {
                height = length;
                width = fitWidth(coverTexture.width, coverTexture.height, length);
            }

            // 设置位置和大小
            int spaceX = UnityEngine.Random.Range(minSpaceX_, maxSpaceX_);
            int spaceY = UnityEngine.Random.Range(minSpaceY_, maxSpaceY_);
            CellAnchor anchor = calculateAnchor(_viewport, spaceX, spaceY, width, height);
            // 需要将节点在视窗中的位置转换为在canvas中的位置
            // 视窗的左上角为（0，0），右下角为（canvasWidth, canvasHeight）
            // canvas的中心为(0,0)，左上角为（-canvasWidth/2，canvasWidth/2），右下角为（canvasWidth/2, -canvasHeight/2）
            // 节点锚点在视窗中，左上角为（0，0），右下角为（cellWidth，cellHeight）
            // 节点在canvas中，中心点为（0,0），左上角为（-cellWidth/2，cellHeight/2），右下角为（cellWidth/2，-cellHeight/2）
            cell.pinX = anchor.posX - canvasWidth_ / 2 + anchor.width / 2;
            cell.pinY = -anchor.posY + canvasHeight_ / 2 - anchor.height / 2;
            cell.dynamicX = cell.pinX;
            cell.dynamicY = cell.pinY;
            cell.animStartPos.x = cell.pinX;
            cell.animStartPos.y = cell.pinY;
            cell.animEndPos.x = cell.pinX;
            cell.animEndPos.y = cell.pinY;
            cell.width = anchor.width;
            cell.height = anchor.height;

            cell.target.anchoredPosition = new UnityEngine.Vector2(cell.pinX, cell.pinY);
            cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
            cell.pinVisible = anchor.visible;
            cell.target.gameObject.SetActive(cell.pinVisible);

            _viewport.contentIndex += 1;
            if (_viewport.contentIndex >= contentUriS_.Count)
            {
                _viewport.contentIndex = 0;
            }

            cell.target.anchoredPosition = new UnityEngine.Vector2(cell.pinX, cell.pinY);
            cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
            return cell;
        }

        private CellAnchor calculateAnchor(Viewport _viewport, int _spaceX, int _spaceY, int _cellWidth, int _cellHeight)
        {
            //Debug.LogFormat("space is ({0}, {1})", _spaceX, _spaceY);
            //Debug.LogFormat("size of cell is ({0}, {1})", _cellWidth, _cellHeight);

            CellAnchor anchor = new CellAnchor();
            anchor.width = _cellWidth;
            anchor.height = _cellHeight;
            anchor.visible = true;

            // 1: 当前节点的开始位置
            int startX = _viewport.columnIndex * columnWidth_ + _spaceX;
            // 2: 当前节点的结束位置
            int endX = startX + _cellWidth;
            // 3: 当前节点跨越的起始列
            int startColumn = _viewport.columnIndex;
            // 4: 当前节点跨越的结束列
            int endColumn = (int)endX / columnWidth_ + 1;
            if (endColumn > columnCount_ - 1)
                endColumn = columnCount_ - 1;
            _viewport.columnIndex = (endColumn + 1) % columnCount_;
            //Debug.LogFormat("range of column is ({0}, {1}), next column is {2}", startColumn, endColumn, columnIndex_);
            anchor.posX = startX;

            // 5: 缩小超出视窗外的节点
            if (anchor.posX + anchor.width + minSpaceX_ > canvasWidth_)
            {
                anchor.width = (int)canvasWidth_ - anchor.posX - minSpaceX_;
                anchor.height = fitHeight(_cellWidth, _cellHeight, anchor.width);
                anchor.visible = anchor.width >= cellMinLength_ && anchor.height >= cellMinLength_;
            }

            // 6: 获取节点跨越列的最下方的列底线位置
            int bottomY = _viewport.columnBottomLines[startColumn];
            for (int i = startColumn; i <= endColumn; ++i)
            {
                bottomY = UnityEngine.Mathf.Max(bottomY, _viewport.columnBottomLines[i]);
            }
            anchor.posY = bottomY + _spaceY;

            // 7：更新范围列的列底线位置
            for (int i = startColumn; i <= endColumn; ++i)
            {
                _viewport.columnBottomLines[i] = anchor.posY + _cellHeight;
            }


            //printColumnTopLines();

            return anchor;
        }

        private int fitHeight(int _originWidth, int _originHeight, int _width)
        {
            return (int)(_width / (_originWidth * 1.0f / _originHeight));
        }

        private int fitWidth(int _originWidth, int _originHeight, int _height)
        {
            return (int)(_height * (_originWidth * 1.0f / _originHeight));
        }

    }

}
