using XTC.oelFSM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FilmLayoutAction : LayoutAction
    {
        public const string NAME = "FilmLayout";

        private float[] speeds_ { get; set; }
        private float[] scales_ { get; set; }
        private int[] directions_ { get; set; }
        private int space_ { get; set; }
        private int trackCount_ { get; set; }

        /// <summary>
        /// 轨道在X轴上的投影长度
        /// </summary>
        private int[] trackLengthX_ { get; set; }

        /// <summary>
        /// 轨道在Y轴上的投影长度
        /// </summary>
        private int[] trackLengthY_ { get; set; }

        private UnityEngine.RectTransform[] tracks_ { get; set; }

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
            baseExit(NAME);
            if (!active_)
                return;

            disableInteractable();
        }

        protected override void onUpdate()
        {
            baseUpdate();

            if (null == layoutCells_)
                return;

            for (int i = 0; i < trackCount_; i++)
            {
                tracks_[i].transform.Translate(0, speeds_[i] * Time.deltaTime * directions_[i], 0);
                bool isTopIn = tracks_[i].anchoredPosition.y + trackLengthY_[i] / 2 * scales_[i] < canvasHeight_ / 2;
                bool isBottomIn = tracks_[i].anchoredPosition.y - trackLengthY_[i] / 2 * scales_[i] > -canvasHeight_ / 2;
                bool isRightIn = tracks_[i].anchoredPosition.x + trackLengthX_[i] / 2 * scales_[i] < canvasWidth_ / 2;
                bool isLeftIn = tracks_[i].anchoredPosition.x - trackLengthX_[i] / 2 * scales_[i] > -canvasWidth_ / 2;
                //判断以轨道为对边的矩形的四个角是否在屏幕内
                if ((isLeftIn && isTopIn) || (isRightIn && isTopIn) || (isLeftIn && isBottomIn) || (isRightIn && isBottomIn))
                {
                    directions_[i] = -directions_[i];
                }
            }

        }
        protected override void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            trackCount_ = parseIntFromProperty("trackCount");
            space_ = parseIntFromProperty("space");
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            string bgImage = parseStringFromProperty("bgImage");
            string trackImage = parseStringFromProperty("trackImage");
            string maskImage = parseStringFromProperty("maskImage");
            int cellWidth = parseIntFromProperty("pictureWidth");
            int trackWidth = parseIntFromProperty("trackWidth");

            speeds_ = new float[trackCount_];
            scales_ = new float[trackCount_];
            tracks_ = new RectTransform[trackCount_];
            directions_ = new int[trackCount_];
            trackLengthX_ = new int[trackCount_];
            trackLengthY_ = new int[trackCount_];
            int contentIndex = 0;
            List<Cell> cells = new List<Cell>();
            List<GameObject> trackGameObjecS = new List<GameObject>();
            // value：轨道序号
            Dictionary<GameObject, int> maskGameObjecS = new Dictionary<GameObject, int>();
            // 创建轨道
            for (int i = 0; i < trackCount_; i++)
            {
                // 获取轨道参数
                float x = parseFloatFromProperty(string.Format("track_{0}_x", i)) * canvasWidth_ / 2;
                int length = parseIntFromProperty(string.Format("track_{0}_length", i));
                float angleX = parseFloatFromProperty(string.Format("track_{0}_angle_x", i));
                float angleY = parseFloatFromProperty(string.Format("track_{0}_angle_y", i));
                float angleZ = parseFloatFromProperty(string.Format("track_{0}_angle_z", i));
                float alpha = parseFloatFromProperty(string.Format("track_{0}_alpha", i));
                speeds_[i] = parseFloatFromProperty(string.Format("track_{0}_speed", i));
                scales_[i] = parseFloatFromProperty(string.Format("track_{0}_scale", i));
                directions_[i] = parseIntFromProperty(string.Format("track_{0}_direction", i));

                //勾股定理计算轨道在X轴上的投影的长度
                trackLengthX_[i] = (int)(Mathf.Abs(Mathf.Sin(angleZ * Mathf.Deg2Rad) * length));
                //勾股定理计算轨道在Y轴上的投影的长度
                trackLengthY_[i] = (int)(Mathf.Abs(Mathf.Cos(angleZ * Mathf.Deg2Rad) * length));

                // 创建对象
                var goTrack = newBlankObject(layer);
                trackGameObjecS.Add(goTrack);
                goTrack.name = string.Format("track_{0}", i);
                var rtTrack = goTrack.GetComponent<RectTransform>();
                tracks_[i] = rtTrack;
                rtTrack.anchoredPosition = new Vector2(x, 0);
                rtTrack.sizeDelta = new Vector2(trackWidth, length);
                goTrack.transform.localRotation = Quaternion.Euler(angleX, angleY, angleZ);
                goTrack.transform.localScale = Vector3.one * scales_[i];
                var canvasGroup = goTrack.AddComponent<CanvasGroup>();
                canvasGroup.alpha = alpha;


                int offset = -length / 2;
                while (_contentList.Count > 0 && offset < length / 2)
                {
                    var contentUri = _contentList.Count > 0 ? _contentList[contentIndex % _contentList.Count] : "";
                    var coverTexture = loadCellThumb(contentUri);
                    contentIndex += 1;
                    // 适配高度
                    int fitWidth = cellWidth;
                    int fitHeight = cellWidth;


                    if (null != coverTexture)
                    {
                        fitHeight = (int)((float)coverTexture.height / coverTexture.width * fitWidth);
                    }

                    int pinY = offset + (fitHeight / 2 + space_);
                    offset += (fitHeight + space_);

                    Cell cell = new Cell();
                    cells.Add(cell);
                    cell.width = fitWidth;
                    cell.height = fitHeight;
                    cell.pinX = 0;
                    cell.pinY = pinY;
                    cell.dynamicX = cell.pinX;
                    cell.dynamicY = cell.pinY;
                    cell.animStartPos.x = cell.pinX;
                    cell.animStartPos.y = cell.pinY;
                    cell.animEndPos.x = cell.pinX;
                    cell.animEndPos.y = cell.pinY;
                    cell.row = 0; // 此布局无行的概念
                    cell.column = 0; // 此布局无列的概念
                    cell.directionX = 0; // 此布局无行的方向的概念
                    cell.directionY = 0; // 此布局无列的方向的概念
                    cell.contentUri = contentUri;
                    cell.surround = false;
                    cell.target = newCell(layer, contentUri).GetComponent<RectTransform>();
                    cell.image = cell.target.gameObject.GetComponent<RawImage>();
                    cell.canvasGroup = cell.target.gameObject.GetComponent<UnityEngine.CanvasGroup>();

                    if (null != coverTexture)
                        cell.image.texture = coverTexture;
                    cell.pinVisible = true;

                    cell.target.SetParent(goTrack.transform);
                    cell.target.transform.localPosition = Vector3.zero;
                    cell.target.transform.localRotation = Quaternion.identity;
                    cell.target.transform.localScale = Vector3.one;
                    cell.target.anchoredPosition = new Vector2(cell.pinX, cell.pinY);
                    cell.target.sizeDelta = new Vector2(cell.width, cell.height);
                    cell.target.gameObject.SetActive(true);
                }

                // 设置轨道遮罩
                var goMask = new GameObject("mask");
                maskGameObjecS[goMask] = i;
                goMask.transform.SetParent(goTrack.transform);
                goMask.transform.localPosition = Vector3.zero;
                goMask.transform.localScale = Vector3.one;
                goMask.transform.localRotation = Quaternion.identity;
                RectTransform rtMask = goMask.GetComponent<RectTransform>();
                if (null == rtMask)
                    rtMask = goMask.AddComponent<RectTransform>();
                rtMask.anchorMin = Vector2.zero;
                rtMask.anchorMax = Vector2.one;
                rtMask.sizeDelta = Vector2.zero;
                rtMask.anchoredPosition = Vector2.zero;
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
            // 加载轨道图
            myInstance.LoadTextureFromTheme(trackImage, (_texture) =>
            {
                foreach (var go in trackGameObjecS)
                {
                    var imgTrack = go.AddComponent<Image>();
                    imgTrack.type = Image.Type.Tiled;
                    imgTrack.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.one * 0.5f);
                }
            }, () => { });
            // 加载遮罩图
            myInstance.LoadTextureFromTheme(trackImage, (_texture) =>
            {
                foreach (var pair in maskGameObjecS)
                {
                    float mask = parseFloatFromProperty(string.Format("track_{0}_mask", pair.Value));
                    var imgMask = pair.Key.AddComponent<Image>();
                    imgMask.type = Image.Type.Tiled;
                    imgMask.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.one * 0.5f);
                    imgMask.color = new Color(1, 1, 1, mask);
                    imgMask.raycastTarget = false;
                }
            }, () => { });

            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }

    }
}
