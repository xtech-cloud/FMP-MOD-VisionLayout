using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
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

            var goLayerTemplate = rootUI.transform.Find("LayerContainer/LayerTemplate").gameObject;
            goLayerTemplate.SetActive(false);

            fastFSM_ = new FastFSM();
            fastFSM_.logger = logger_;
            fastFSM_.style = style_;
            fastFSM_.catalog = catalog_;
            fastFSM_.layerTemplateGameObject = goLayerTemplate;
            fastFSM_.virtualResolution = rootUI.GetComponent<RectTransform>().rect.size;
            fastFSM_.preloadsRepetition = preloadsRepetition;
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
    }
}
