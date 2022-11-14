using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 工具栏
    /// </summary>
    public class ToolBar
    {
        public int clickTrigger = 20;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive
        {
            get { return rootGameObject_.activeSelf; }
        }

        /// <summary>
        /// 激活的层
        /// </summary>
        public string activeLayer { get; private set; } = "";

        public Action OnSwitch;

        private GameObject rootGameObject_;
        private int clickCellID_ = 0;
        private int clickCounter_ = 0;
        private float clickTimer_ = 0;
        private GameObject entryTemplate_;

        /// <summary>
        /// 入口列表
        /// </summary>
        /// <remarks>
        /// key: 层的名称
        /// </remarks>
        private Dictionary<string, Toggle> toggles_ = new Dictionary<string, Toggle>();

        /// <summary>
        /// 绑定工具栏的Ui根对象
        /// </summary>
        /// <param name="_rootGameObject"></param>
        public void BindRootGameObject(GameObject _rootGameObject)
        {
            rootGameObject_ = _rootGameObject;
            rootGameObject_.transform.Find("Panel/btnClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                rootGameObject_.SetActive(false);
            });
            entryTemplate_ = rootGameObject_.transform.Find("Panel/tgEntry").gameObject;
            entryTemplate_.SetActive(false);
        }

        public void Popup()
        {
            rootGameObject_.SetActive(true);
            toggles_[activeLayer].isOn = true;
        }

        /// <summary>
        /// 处理层创建后
        /// </summary>
        /// <param name="_layer">层的名称</param>
        /// <param name="_alias">显示的别名</param>
        public void HandleLayerCreated(string _layer, string _alias)
        {
            var clone = GameObject.Instantiate(entryTemplate_.gameObject, entryTemplate_.transform.parent);
            clone.name = _layer;
            clone.transform.Find("Label").GetComponent<Text>().text = _alias;
            clone.SetActive(true);
            clone.GetComponent<Toggle>().onValueChanged.AddListener((_toggled) =>
            {
                if (!IsActive)
                    return;
                if (!_toggled)
                    return;
                if (activeLayer.Equals(clone.name))
                    return;
                activeLayer = clone.name;
                OnSwitch();
            });
            toggles_[_layer] = clone.GetComponent<Toggle>();
        }

        // 点击隐藏区域
        public void ClickHiddenArea(UnityEngine.GameObject _cell)
        {
            if (_cell.GetInstanceID() != clickCellID_)
            {
                clickCounter_ = 0;
                clickTimer_ = 0;
            }

            // 超过间隔时间，重置计数器
            if (UnityEngine.Time.realtimeSinceStartup - clickTimer_ > 0.5f)
            {
                clickCounter_ = 0;
            }

            clickCounter_ += 1;
            clickCellID_ = _cell.GetInstanceID();
            clickTimer_ = UnityEngine.Time.realtimeSinceStartup;

            if (clickCounter_ >= clickTrigger)
            {
                clickCounter_ = 0;
                clickTimer_ = 0f;
                clickCellID_ = 0;
                rootGameObject_.SetActive(true);
                toggles_[activeLayer].isOn = true;
            }
        }

        public void SwitchLayer(string _layer)
        {
            activeLayer = _layer;
        }

        public void SwitchInteractable(bool _toggled)
        {
            foreach (var toggle in toggles_.Values)
            {
                toggle.interactable = _toggled;
            }
        }

    }
}
