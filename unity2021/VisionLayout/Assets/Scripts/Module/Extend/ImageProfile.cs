using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 图片简介
    /// </summary>
    public class ImageProfile
    {
        public MyInstance myInstance;
        public MonoBehaviour mono;

        private Coroutine coroutinePopup_;
        private float timer_ = 0;
        private float duration_ = 0;

        /// <summary>
        /// 图标简介的组件的列表
        /// </summary>
        private Dictionary<string, RawImage> imageS = new Dictionary<string, RawImage>();

        public void HandleLayerCreated(GameObject _layerGameObject, MyCatalogBase.Section _catalogSection)
        {
            string layer = _layerGameObject.name;
            var imgTitle = _layerGameObject.transform.Find("imgProfile").GetComponent<RawImage>();
            imageS[layer] = imgTitle;

            var btnTimer = imgTitle.transform.Find("btnTimer").GetComponent<Button>();
            btnTimer.onClick.AddListener(() =>
            {
                if (null == coroutinePopup_)
                {
                    Display(layer);
                }
                else
                {
                    mono.StopCoroutine(coroutinePopup_);
                    coroutinePopup_ = null;
                }
            });
            string valImageTitle;
            _catalogSection.kvS.TryGetValue("ProfileImage", out valImageTitle);
            if (!string.IsNullOrEmpty(valImageTitle))
            {
                myInstance.LoadTextureFromTheme(valImageTitle, (_texture) =>
                {
                    imgTitle.texture = _texture;
                }, () =>
                {
                });
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="_layer">层的名称</param>
        public void Display(string _layer)
        {
            imageS[_layer].gameObject.SetActive(true);
            if (null != coroutinePopup_)
                mono.StopCoroutine(coroutinePopup_);
            coroutinePopup_ = mono.StartCoroutine(update(_layer));
        }

        public void Disappear(string _layer)
        {
            imageS[_layer].gameObject.SetActive(false);
            if (null != coroutinePopup_)
            {
                mono.StopCoroutine(coroutinePopup_);
                coroutinePopup_ = null;
            }
        }

        private IEnumerator update(string _layer)
        {
            timer_ = 0;
            duration_ = myInstance.getStyle().profile.duration;
            if (duration_ < 1)
                duration_ = 1;
            var imgTick = imageS[_layer].transform.Find("btnTimer/tick").GetComponent<Image>();
            while (timer_ < duration_)
            {
                yield return new WaitForEndOfFrame();
                timer_ += Time.deltaTime;
                imgTick.fillAmount = timer_ / duration_;
            }
            Disappear(_layer);
        }
    }
}
