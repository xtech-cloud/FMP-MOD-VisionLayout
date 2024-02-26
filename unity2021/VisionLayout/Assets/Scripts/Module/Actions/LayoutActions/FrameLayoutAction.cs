using XTC.oelFSM;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FrameLayoutAction : LayoutAction
    {
        public const string NAME = "FrameLayout";

        private float changeInterval_ { get; set; }
        private int columnCount_ { get; set; }
        private int columnWidth_ { get; set; }

        private float intervalTimer_ { get; set; }
        private bool changed_ { get; set; }

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
            changed_ = false;
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

            // 如果布局在切换前的剩余时间不够执行切换效果，就不执行
            if (timer_ + 3 >= duration)
                return;

            intervalTimer_ += UnityEngine.Time.deltaTime;
            if (intervalTimer_ < changeInterval_)
                return;


            float precentage = (intervalTimer_ - changeInterval_) / 2;

            //在翻转到90度时切换节点
            if (precentage >= 0.5f && !changed_)
            {
                int offset = canvasWidth_ / 2;
                // 将末尾的元素放到列首
                for (int i = 0; i < columnCount_; i++)
                {
                    var cell = layoutCells_[layoutCells_.Count - 1];
                    layoutCells_.RemoveAt(layoutCells_.Count - 1);
                    layoutCells_.Insert(0, cell);
                }
                //重新计算位置
                for (int i = 0; i < layoutCells_.Count; ++i)
                {
                    var cell = layoutCells_[i];
                    cell.dynamicX = i * columnWidth_ - offset + columnWidth_ / 2;
                    cell.target.anchoredPosition = new UnityEngine.Vector2(cell.dynamicX, cell.dynamicY);
                    cell.target.gameObject.SetActive(cell.pinVisible && isCellInCanvasRect(cell, canvasWidth_, canvasHeight_));
                }
                changed_ = true;
            }

            if (precentage >= 1.0)
            {
                precentage = 1.0f;
                intervalTimer_ = 0f;
                changed_ = false;
            }

            var angles = UnityEngine.Vector3.zero;
            foreach (var cell in layoutCells_)
            {
                if (precentage <= 0.5f)
                    angles.y = 90 * (precentage * 2);
                else
                    angles.y = 90 * (1 - precentage) * 2;
                cell.target.rotation = UnityEngine.Quaternion.Euler(angles);
            }
        }

        protected override void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            changeInterval_ = parseFloatFromProperty("changeInterval");
            columnCount_ = parseIntFromProperty("column");
            string bgImage = parseStringFromProperty("bgImage");
            string frameImage = parseStringFromProperty("frameImage");
            int frameBorderTop = parseIntFromProperty("frameBorderTop");
            int frameBorderBottom = parseIntFromProperty("frameBorderBottom");
            int frameBorderLeft = parseIntFromProperty("frameBorderLeft");
            int frameBorderRight = parseIntFromProperty("frameBorderRight");
            int frameMarginTop = parseIntFromProperty("frameMarginTop");
            int frameMarginBottom = parseIntFromProperty("frameMarginBottom");
            int frameMarginLeft = parseIntFromProperty("frameMarginLeft");
            int frameMarginRight = parseIntFromProperty("frameMarginRight");
            int maxWidth = parseIntFromProperty("maxWidth");
            int maxHeight = parseIntFromProperty("maxHeight");
            columnWidth_ = canvasWidth_ / columnCount_;

            int columnIndex = 0;
            int offset = canvasWidth_ / 2;


            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < _contentList.Count || i < columnCount_; i++)
            {
                var contentUri = _contentList.Count > 0 ? _contentList[i % _contentList.Count] : "";
                var coverTexture = loadCellThumb(contentUri);
                int cellWidth = maxWidth;
                int cellHeight = maxHeight;
                if (null != coverTexture)
                {
                    fitCellSize(maxWidth, maxHeight, coverTexture.width, coverTexture.height, out cellWidth, out cellHeight);
                }

                Cell cell = new Cell();
                cells.Add(cell);
                cell.width = cellWidth;
                cell.height = cellHeight;
                cell.pinX = i * columnWidth_ - offset + columnWidth_ / 2;
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

                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.dynamicX, cell.dynamicY);
                cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
                if (null != coverTexture)
                    cell.image.texture = coverTexture;
            }

            // 加载背景图
            myInstance.LoadTextureFromTheme(bgImage, (_texture) =>
            {
                var rtLayer = runtimeClone.layerMap[layer];
                var imgBg = rtLayer.Find("bg").GetComponent<Image>();
                imgBg.type = Image.Type.Tiled;
                imgBg.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.one * 0.5f);
                imgBg.gameObject.SetActive(true);
            }, () => { });
            // 加载画框图
            myInstance.LoadTextureFromTheme(frameImage, (_texture) =>
            {
                foreach (var cell in cells)
                {
                    var imgFrame = cell.target.Find("frame").GetComponent<Image>();
                    var rtFrame = imgFrame.GetComponent<RectTransform>();
                    imgFrame.type = Image.Type.Sliced;
                    imgFrame.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.one * 0.5f, 100, 0, SpriteMeshType.Tight, new Vector4(frameBorderLeft, frameBorderTop, frameBorderRight, frameBorderBottom));
                    imgFrame.gameObject.SetActive(true);
                    rtFrame.anchoredPosition = new UnityEngine.Vector2(0, 0);
                    rtFrame.sizeDelta = new UnityEngine.Vector2(-(frameMarginLeft + frameMarginRight), -(frameMarginTop + frameMarginBottom));
                }
            }, () => { });
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

        private void fitCellSize(int _maxWidth, int _maxHeight, int _textureWidth, int _textureHeight, out int _realWidth, out int _realHeight)
        {
            _realHeight = _textureHeight;
            _realWidth = _textureWidth;
            if (_realHeight > _maxHeight)
            {
                _realHeight = _maxHeight;
                _realWidth = _realHeight * _textureWidth / _textureHeight;
            }
            if (_realWidth > _maxWidth)
            {
                _realWidth = _maxWidth;
                _realHeight = _realWidth * _textureHeight / _textureWidth;
            }
        }
    }
}
