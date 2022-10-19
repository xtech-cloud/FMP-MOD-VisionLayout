using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// ����ʱʵ������UIԪ��
    /// </summary>
    public class RuntimeClone
    {
        /// <summary>
        /// ���ʵ�����б�
        /// </summary>
        /// <remarks>
        /// key: �������
        /// </remarks>
        public Dictionary<string, Transform> layerMap = new Dictionary<string, Transform>();

        /// <summary>
        /// �ڵ�ģ����б�
        /// </summary>
        /// <remarks>
        /// key: �������
        /// </remarks>
        public Dictionary<string, GameObject> cellTemplateMap = new Dictionary<string, GameObject>();

        public Dictionary<string, RectTransform> titleMap = new Dictionary<string, RectTransform>();
        public Dictionary<string, RectTransform> profileMap = new Dictionary<string, RectTransform>();
        public Dictionary<string, Image> profileTickMap = new Dictionary<string, Image>();
        public Dictionary<string, CanvasGroup> profileCanvasGroupMap = new Dictionary<string, CanvasGroup>();
        public Dictionary<string, Toggle> layerEntryMap = new Dictionary<string, Toggle>();

    }

}
