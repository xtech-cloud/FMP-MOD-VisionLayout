
using System;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.VisionLayout.LIB.Bridge;
using XTC.FMP.MOD.VisionLayout.LIB.MVCS;
using static XTC.FMP.MOD.VisionLayout.LIB.Unity.MyConfigBase;
using UnityEngine;
using XTC.FMP.LIB.MVCS;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 虚拟视图，用于处理消息订阅
    /// </summary>
    public class DummyView : DummyViewBase
    {
        public DummyView(string _uid) : base(_uid)
        {
        }

        protected override void setup()
        {
            base.setup();
            addSubscriber(MySubject.ToolBarPopup, handleToolBarPopup);
        }

        private void handleToolBarPopup(LibMVCS.Model.Status _status, object _data)
        {
            getLogger().Debug("handle ToolBarPopup with {0}", JsonConvert.SerializeObject(_data));
            string uid = "";
            try
            {
                var parameters = _data as Dictionary<string, object>;
                uid = parameters["uid"] as string;
            }
            catch (Exception ex)
            {
                getLogger().Exception(ex);
                return;
            }
            MyInstance instance;
            runtime.instances.TryGetValue(uid, out instance);
            if (null == instance)
            {
                getLogger().Error("instance {0} not found", uid);
                return;
            }
            instance.PopupToolBar();

        }

    }
}

