using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class FilmOutTransitionAction : TransitionAction
    {
        public const string NAME = "FilmOutTransition";
        public class TrackAnimation
        {
            public RectTransform rectTransform;
            public CanvasGroup canvasGroup;
            public Vector2 originalPos;
            public float originalAlpha;
            public Vector2 animEndPos;
            public float animDelay;
        }

        private Image bgImg_ { get; set; }
        private List<TrackAnimation> trackAnimations = new List<TrackAnimation>();
        private float blank { get; set; }

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);

            if (!filterLayoutCells(LayerCategory.Disappear))
                return;

            var tLayer = runtimeClone.layerMap[layer];
            bgImg_ = tLayer.Find("bg").GetComponent<Image>();
            bgImg_.color = new Color(1, 1, 1, 0);
            bgImg_.gameObject.SetActive(true);

            blank = parseFloatFromProperty("blank");
            trackAnimations.Clear();
            foreach (Transform child in tLayer.Find("CellContainer"))
            {
                if (!child.name.Contains("track"))
                    continue;
                var trackAnimation = new TrackAnimation();
                trackAnimations.Add(trackAnimation);
                trackAnimation.rectTransform = child.GetComponent<RectTransform>();
                trackAnimation.canvasGroup = child.GetComponent<CanvasGroup>();
                trackAnimation.animDelay = blank;
                trackAnimation.animEndPos = new Vector2(Random.Range(-canvasHeight_ / 2, canvasHeight_ / 2), Random.Range(-canvasHeight_ / 2, canvasHeight_ / 2));
                // 保存原始状态
                trackAnimation.originalPos = trackAnimation.rectTransform.anchoredPosition;
                trackAnimation.originalAlpha = trackAnimation.canvasGroup.alpha;
            }
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            hideLayer();

            bgImg_.color = new Color(1, 1, 1, 0);
            foreach (var trackAnimation in trackAnimations)
            {
                trackAnimation.rectTransform.gameObject.SetActive(false);
                // 动画结束时将状态设置为动画开始前的原始状态
                trackAnimation.rectTransform.anchoredPosition = trackAnimation.originalPos;
                trackAnimation.canvasGroup.alpha = trackAnimation.originalAlpha;
            }
        }

        protected override void onUpdate()
        {
            baseUpdate();

            float percent = timer_ / duration;
            var color = bgImg_.color;
            color.a = 1 - percent;
            bgImg_.color = color;

            foreach (var trackAnimation in trackAnimations)
            {
                if (timer_ < trackAnimation.animDelay)
                    continue;
                float percentTrack = (timer_ - trackAnimation.animDelay) / (duration - blank - trackAnimation.animDelay);
                Vector2 pos = Vector2.Lerp(trackAnimation.originalPos, trackAnimation.animEndPos, percentTrack);
                trackAnimation.rectTransform.anchoredPosition = pos;
                trackAnimation.canvasGroup.alpha = trackAnimation.originalAlpha * (1 - percent);
            }
        }
    }
}
