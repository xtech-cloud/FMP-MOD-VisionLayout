using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 运行时实例化的UI元素
    /// </summary>
    public class RuntimeClone
    {
        /// <summary>
        /// 层的实例的列表
        /// </summary>
        /// <remarks>
        /// key: 层的名称
        /// </remarks>
        public Dictionary<string, Transform> layerMap = new Dictionary<string, Transform>();

        /// <summary>
        /// 节点模板的列表
        /// </summary>
        /// <remarks>
        /// key: 层的名称
        /// </remarks>
        public Dictionary<string, GameObject> cellTemplateMap = new Dictionary<string, GameObject>();

        public Dictionary<string, RectTransform> titleMap = new Dictionary<string, RectTransform>();
        public Dictionary<string, RectTransform> profileMap = new Dictionary<string, RectTransform>();
        public Dictionary<string, Image> profileTickMap = new Dictionary<string, Image>();
        public Dictionary<string, CanvasGroup> profileCanvasGroupMap = new Dictionary<string, CanvasGroup>();
        public Dictionary<string, Toggle> layerEntryMap = new Dictionary<string, Toggle>();

    }

}
