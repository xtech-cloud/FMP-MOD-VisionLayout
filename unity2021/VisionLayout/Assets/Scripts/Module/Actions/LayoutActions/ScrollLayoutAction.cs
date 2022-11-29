using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class ScrollLayoutAction : LayoutAction
    {
        public const string NAME = "ScrollLayout";
        public class HotspotAnimation
        {
            public Animator animator;
            public float timer;
            public float interval;

        }

        private List<HotspotAnimation> hotspotS_ = new List<HotspotAnimation>();
        private int animationIntervalMin_ = 0;
        private int animationIntervalMax_ = 0;

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

            foreach (var hotspot in hotspotS_)
            {
                hotspot.interval = Random.Range(animationIntervalMin_, animationIntervalMax_);
            }
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

            foreach (var hotspot in hotspotS_)
            {
                hotspot.timer += Time.deltaTime;
                if (hotspot.timer >= hotspot.interval)
                {
                    hotspot.timer = 0;
                    hotspot.animator.SetTrigger("do");
                    hotspot.interval = Random.Range(animationIntervalMin_, animationIntervalMax_);
                }
            }
        }

        protected override void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;

            int cellSize = parseIntFromProperty("hotspot_size");
            animationIntervalMin_ = parseIntFromProperty("hotspot_animation_interval_min");
            animationIntervalMax_ = parseIntFromProperty("hotspot_animation_interval_max");

            string scrollImage = "";
            _kvS.TryGetValue("Scroll_Image", out scrollImage);
            if (string.IsNullOrEmpty(scrollImage))
            {
                myInstance.getLogger().Error("Scroll_Image is empty in catalog");
            }
            else
            {
                myInstance.getContentReader().ContentUri = "";
                myInstance.getContentReader().LoadTexture(scrollImage, (_texture) =>
                {
                    var rtLayer = runtimeClone.layerMap[layer];
                    var imgBg = rtLayer.Find("bg").GetComponent<Image>();
                    imgBg.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.one * 0.5f);
                    imgBg.gameObject.SetActive(true);
                }, () =>
                {

                });
            }

            hotspotS_.Clear();
            int lastX = -canvasWidth_ / 2;
            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < _contentList.Count; i++)
            {
                string contentUri = _contentList[i];
                var meta = loadContentMeta(contentUri);
                string x;
                string y;
                meta.kvS.TryGetValue("XTC_VisionLayout_Hotspot_x", out x);
                meta.kvS.TryGetValue("XTC_VisionLayout_Hotspot_y", out y);

                Cell cell = new Cell();
                cells.Add(cell);
                cell.width = cellSize;
                cell.height = cellSize;
                int.TryParse(x, out cell.pinX);
                int.TryParse(y, out cell.pinY);
                cell.dynamicX = cell.pinX;
                cell.dynamicY = cell.pinY;
                cell.animStartPos.x = cell.pinX;
                cell.animStartPos.y = cell.pinY;
                cell.animEndPos.x = cell.pinX;
                cell.animEndPos.y = cell.pinY;
                cell.row = 0;
                cell.column = i;
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
                //if (null != coverTexture)
                cell.image.color = Color.clear;
                var rtHotspot = cell.target.transform.Find("hotspot").GetComponent<RectTransform>();
                rtHotspot.gameObject.SetActive(true);
                rtHotspot.sizeDelta = new Vector2(cell.width, cell.height);
                cell.target.GetComponent<Button>().targetGraphic = rtHotspot.GetComponent<Image>();
                var hotspot = new HotspotAnimation();
                hotspot.animator = rtHotspot.GetComponent<Animator>();
                hotspotS_.Add(hotspot);
                lastX += cell.width;
            }
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }
    }
}
