using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {
        private ContentReader contentReader_;
        private FastFSM fastFSM_;

        public MonoBehaviour getMono()
        {
            return mono_;
        }


        public LibMVCS.Logger getLogger()
        {
            return logger_;
        }

        public MyConfig.Style getStyle()
        {
            return style_;
        }

        public MyCatalog getCatalog()
        {
            return catalog_;
        }

        public MyEntry getMyEntry()
        {
            return entry_ as MyEntry;
        }

        public ContentReader getContentReader()
        {
            return contentReader_;
        }


        public MyInstance(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _catalog, _logger, _settings, _entry, _mono, _rootAttachments)
        {
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <remarks>
        /// 可用于加载主题目录的数据
        /// </remarks>
        public void HandleCreated()
        {
            // 设置背景
            Color color = Color.white;
            RawImage instanceBackground = rootUI.transform.Find("bg").GetComponent<RawImage>();
            instanceBackground.gameObject.SetActive(false);
            if (UnityEngine.ColorUtility.TryParseHtmlString(style_.background.color, out color))
            {
                rootUI.transform.Find("bg").GetComponent<RawImage>().color = color;
                instanceBackground.gameObject.SetActive(true);
            }

            // 隐藏层的模板
            var goLayerTemplate = rootUI.transform.Find("LayerContainer/LayerTemplate").gameObject;
            goLayerTemplate.SetActive(false);

            // 隐藏工具栏
            var goToolBar = rootUI.transform.Find("ToolBar").gameObject;
            goToolBar.SetActive(false);
            // 加载工具栏logo
            if (!string.IsNullOrEmpty(style_.toolBar.logoImage))
            {
                loadTextureFromTheme("logo.png", (_texture) =>
                {
                    var imgLogo = goToolBar.transform.Find("Panel/logo").GetComponent<Image>();
                    imgLogo.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
                }, () => { });
            }
            // 应用工具栏样式
            var vlg = goToolBar.transform.Find("Panel").GetComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(style_.toolBar.paddingLeft, style_.toolBar.paddingRight, style_.toolBar.paddingTop, style_.toolBar.paddingBottom);
            vlg.spacing = style_.toolBar.spacing;
            var rtPanel = vlg.GetComponent<RectTransform>();
            rtPanel.sizeDelta = new Vector2(style_.toolBar.entryWidth + style_.toolBar.paddingLeft + style_.toolBar.paddingRight, 0);

            var imageTitle = rootUI.transform.Find("LayerContainer/LayerTemplate/imgTitle");
            alignByAncor(imageTitle, style_.title.anchor);

            var imageProfile = rootUI.transform.Find("LayerContainer/LayerTemplate/imgProfile");
            alignByAncor(imageProfile, style_.profile.anchor);
        }

        /// <summary>
        /// 当被删除时
        /// </summary>
        public void HandleDeleted()
        {
        }

        /// <summary>
        /// 当被打开时
        /// </summary>
        /// <remarks>
        /// 可用于加载内容目录的数据
        /// </remarks>
        public void HandleOpened(string _source, string _uri)
        {
            contentReader_ = new ContentReader(contentObjectsPool);
            contentReader_.AssetRootPath = settings_["path.assets"].AsString();

            fastFSM_ = new FastFSM();
            fastFSM_.myInstance = this;
            fastFSM_.virtualResolution = rootUI.GetComponent<RectTransform>().rect.size;
            fastFSM_.Initialize();
            rootUI.gameObject.SetActive(true);
            mono_.StartCoroutine(fastFSM_.updateFSM());
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            rootUI.gameObject.SetActive(false);
            fastFSM_.Release();
            fastFSM_ = null;
        }

        /// <see cref="MyInstanceBase.loadTextureFromTheme(string, System.Action{Texture2D}, System.Action)"/>
        public void LoadTextureFromTheme(string _file, System.Action<Texture2D> _onFinish, System.Action _onError)
        {
            loadTextureFromTheme(_file, _onFinish, _onError);
        }

        public void PopupToolBar()
        {
            fastFSM_.extendFeatures.toolbar.Popup();
        }
    }
}
