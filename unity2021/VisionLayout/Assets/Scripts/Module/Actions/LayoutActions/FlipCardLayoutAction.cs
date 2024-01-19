using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FlibCardCell : Cell
    {
        /// <summary>
        /// 原图的上半部
        /// </summary>
        public Transform topPartFrom;

        /// <summary>
        /// 原图的下半部分
        /// </summary>
        public Transform bottomPartFrom;

        /// <summary>
        /// 新图的下半部分
        /// </summary>
        public Transform bottomPartTo;
        public bool isFliping = false;
    }

    public class SpriteClip
    {
        public Sprite topFrom;
        public Sprite bottomFrom;
        public Sprite bottomTo;
    }

    public class FlipCardLayoutAction : LayoutAction
    {
        public const string NAME = "FlipCardLayout";
        private float animDelayMin_ = 0;
        private float animDelayMax_ = 0;
        private float splashTime_ = 0;
        private bool splashFinished_ = false;
        /// <summary>
        /// 内容使用权重表
        /// </summary>
        private Dictionary<string, int> contentUsedWeight_ = new Dictionary<string, int>();
        private Dictionary<string, SpriteClip> spriteClipS = new Dictionary<string, SpriteClip>();
        // 节点的封面图
        private Dictionary<Cell, Texture2D> cellCoverS = new Dictionary<Cell, Texture2D>();
        // 节点的过场图
        private Dictionary<Cell, Texture2D> cellSplashS = new Dictionary<Cell, Texture2D>();


        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.Display);
            if (!active_)
                return;

            baseEnter(NAME);
            if (!filterLayoutCells(LayerCategory.Display))
                return;

            restoreVisible();
            // 在过场结束前禁用交互
            disableInteractable();

            if (null == layoutCells_)
                return;

            foreach (var cell in layoutCells_)
            {
                resetSplash(cell);
            }

            splashFinished_ = false;
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
            disableInteractable();

            if (null == layoutCells_)
                return;

            foreach (var cell in layoutCells_)
            {
                finishFlip(cell);
            }

        }

        protected override void onUpdate()
        {
            baseUpdate();

            if (null == layoutCells_)
                return;

            if (timer_ < splashTime_)
                return;

            if (!splashFinished_)
            {
                splashFinished_ = true;
                restoreInteractable();
                return;
            }

            float deltaTime = Time.deltaTime;
            foreach (var cell in layoutCells_)
            {
                cell.animTimer += deltaTime;
                if (cell.animTimer >= cell.animDelay && cell.animTimer <= cell.animDelay + cell.animDuration)
                {
                    runFlip(cell);
                }
                else if (cell.animTimer > cell.animDelay + cell.animDuration)
                {
                    finishFlip(cell);
                }
            }
        }

        protected override void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            int row = parseIntFromProperty("row");
            int column = parseIntFromProperty("column");
            int spaceX = parseIntFromProperty("spaceX");
            int spaceY = parseIntFromProperty("spaceY");
            animDelayMin_ = parseFloatFromProperty("animDelayMin");
            animDelayMax_ = parseFloatFromProperty("animDelayMax");
            splashTime_ = parseFloatFromProperty("splashTime");
            float animDuration = parseFloatFromProperty("animDuration");

            int cellWidth = (canvasWidth_ - (column + 1) * spaceX) / column;
            int cellHeight = (canvasHeight_ - (row + 1) * spaceY) / row;


            // 初始化内容使用权重为0
            foreach (var contentUri in _contentList)
            {
                contentUsedWeight_[contentUri] = 0;
            }

            List<Cell> cells = new List<Cell>();
            // 按从左往右，从上到下的顺序排列
            for (int i = 0; i < row * column; i++)
            {
                int fitSize = Math.Min(cellWidth, cellHeight);
                string contentUri = _contentList.Count > 0 ? _contentList[i % _contentList.Count] : "";
                // 更新内容权重
                contentUsedWeight_[contentUri] += 1;
                var coverTexture = loadContentCover(contentUri);

                int columnIndex = i % column;
                int rowIndex = i / column;
                int pinX = (columnIndex + 1) * spaceX + columnIndex * cellWidth + cellWidth / 2 - canvasWidth_ / 2;
                int pinY = -(rowIndex + 1) * spaceY - rowIndex * cellHeight - cellHeight / 2 + canvasHeight_ / 2;

                FlibCardCell cell = new FlibCardCell();
                cells.Add(cell);
                cell.width = fitSize;
                cell.height = fitSize;
                cell.pinX = pinX;
                cell.pinY = pinY;
                cell.dynamicX = pinX;
                cell.dynamicY = pinY;
                cell.animStartPos.x = pinX;
                cell.animStartPos.y = pinY;
                cell.animEndPos.x = pinX;
                cell.animEndPos.y = pinY;
                cell.animDuration = animDuration;
                cell.animDelay = splashTime_;
                cell.row = rowIndex;
                cell.column = columnIndex;
                cell.directionX = 0;
                cell.contentUri = contentUri;
                cell.surround = false;
                cell.target = newCell(layer, contentUri).GetComponent<UnityEngine.RectTransform>();
                cell.image = cell.target.gameObject.GetComponent<UnityEngine.UI.RawImage>();
                cell.canvasGroup = cell.target.gameObject.GetComponent<UnityEngine.CanvasGroup>();
                addFlipCard(cell.target.transform, fitSize);
                cell.topPartFrom = cell.target.transform.Find("topFrom");
                cell.bottomPartFrom = cell.target.transform.Find("bottomFrom");
                cell.bottomPartTo = cell.target.transform.Find("bottomTo");

                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.pinX, cell.pinY);
                cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
                cell.pinVisible = true;
                cell.target.gameObject.SetActive(false);
                cellCoverS[cell] = coverTexture;
                cell.image.texture = coverTexture;

                string splash = $"flipcard.splash.{layer.Replace("/", "_")}-{i + 1}.png";
                myInstance.LoadTextureFromTheme(splash, (_texture) =>
                {
                    cellSplashS[cell] = _texture;
                }, () =>
                {
                });
            }
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

        private void addFlipCard(Transform _cell, int _fitSize)
        {
            Action<string, float> createPart = (_name, _pivotY) =>
            {
                var part = new GameObject(_name);
                part.transform.SetParent(_cell, false);
                var img = part.AddComponent<Image>();
                var rt = img.GetComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, _pivotY);
                rt.sizeDelta = new Vector2(_fitSize, _fitSize / 2);
                part.SetActive(false);
            };

            createPart("topFrom", 0);
            createPart("bottomFrom", 1f);
            createPart("bottomTo", 1f);
        }

        private void runFlip(Cell _cell)
        {
            var myCell = (_cell as FlibCardCell);
            if (!myCell.isFliping)
            {
                myCell.isFliping = true;

                myCell.bottomPartFrom.gameObject.SetActive(true);

                // 按内容使用权重排序，并从最低的使用权重中选择一个
                var contentS = (from entity in contentUsedWeight_ orderby entity.Value ascending select entity).ToList();
                string contentUri = contentS.First().Key;
                contentUsedWeight_[contentUri] += 1;
                contentUsedWeight_[myCell.contentUri] -= 1;

                Texture2D newTexture = loadContentCover(contentUri);
                Texture2D oldTexture = myCell.image.texture as Texture2D;

                // 将FlipCard设置为下层的图片
                SpriteClip spriteClip;
                if (!spriteClipS.TryGetValue(myCell.contentUri, out spriteClip))
                {
                    spriteClip = new SpriteClip();
                    spriteClip.topFrom = Sprite.Create(oldTexture, new Rect(0, oldTexture.height / 2, oldTexture.width, oldTexture.height / 2), new Vector2(0.5f, 0.5f));
                    spriteClip.bottomFrom = Sprite.Create(oldTexture, new Rect(0, 0, oldTexture.width, oldTexture.height / 2), new Vector2(0.5f, 0.5f));
                    spriteClip.bottomTo = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height / 2), new Vector2(0.5f, 0.5f));
                }
                myCell.topPartFrom.GetComponent<Image>().sprite = spriteClip.topFrom;
                myCell.bottomPartFrom.GetComponent<Image>().sprite = spriteClip.bottomFrom;
                myCell.bottomPartTo.GetComponent<Image>().sprite = spriteClip.bottomTo;

                myCell.image.texture = newTexture;
                myCell.contentUri = contentUri;
                myCell.target.name = contentUri;
            }
            var rotationTopX = Mathf.Lerp(0, -180f, (myCell.animTimer - myCell.animDelay) / myCell.animDuration);
            myCell.topPartFrom.gameObject.SetActive(rotationTopX > -90);
            myCell.topPartFrom.transform.rotation = Quaternion.Euler(rotationTopX, 0f, 0f);

            var rotationBottomX = Mathf.Lerp(180, 0, (myCell.animTimer - myCell.animDelay) / myCell.animDuration);
            myCell.bottomPartTo.gameObject.SetActive(rotationBottomX < 90);
            myCell.bottomPartTo.transform.rotation = Quaternion.Euler(rotationBottomX, 0f, 0f);
        }

        private void finishFlip(Cell _cell)
        {
            var myCell = (_cell as FlibCardCell);
            myCell.animTimer = 0;
            myCell.animDelay = UnityEngine.Random.Range(animDelayMin_, animDelayMax_);
            myCell.isFliping = false;
            myCell.topPartFrom.gameObject.SetActive(false);
            myCell.bottomPartFrom.gameObject.SetActive(false);
            myCell.bottomPartTo.gameObject.SetActive(false);
            myCell.topPartFrom.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            myCell.bottomPartTo.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        private void resetSplash(Cell _cell)
        {
            finishFlip(_cell);
            _cell.animDelay = splashTime_;
            Texture2D texture;
            if (!cellSplashS.TryGetValue(_cell, out texture))
            {
                return;
            }
            _cell.image.texture = texture;
        }

    }
}
