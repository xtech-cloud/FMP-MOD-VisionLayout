using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 图片标题
    /// </summary>
    public class ImageTitle
    {
        public MyInstance myInstance;

        /// <summary>
        /// 图标标题的组件的列表
        /// </summary>
        private Dictionary<string, RawImage> imageS = new Dictionary<string, RawImage>();

        public void HandleLayerCreated(GameObject _layerGameObject, MyCatalogBase.Section _catalogSection)
        {
            var imgTitle = _layerGameObject.transform.Find("imgTitle").GetComponent<RawImage>();
            imageS[_layerGameObject.name] = imgTitle;
            string valImageTitle;
            _catalogSection.kvS.TryGetValue("TitleImage", out valImageTitle);
            if (!string.IsNullOrEmpty(valImageTitle))
            {
                myInstance.LoadTextureFromTheme(valImageTitle, (_texture) =>
                {
                    imgTitle.texture = _texture;
                    int fitWidth = (int)(_texture.width * 1.0f / _texture.height * myInstance.getStyle().title.anchor.height);
                    imgTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(fitWidth, myInstance.getStyle().title.anchor.height);
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
        }

        /// <summary>
        /// 消隐
        /// </summary>
        /// <param name="_layer">层的名称</param>
        public void Disappear(string _layer)
        {
            imageS[_layer].gameObject.SetActive(false);
        }
    }
}
