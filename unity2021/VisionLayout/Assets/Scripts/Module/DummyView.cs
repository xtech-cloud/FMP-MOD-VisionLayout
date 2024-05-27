
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
            addSubscriber(MySubject.DummyBoardOpen, handleDummyBoardOpen);
            addSubscriber(MySubject.DummyBoardClose, handleDummyBoardClose);
            addSubscriber(MySubject.BackgroundVisible, handleBackgroundVisible);
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

        private void handleDummyBoardOpen(LibMVCS.Model.Status _status, object _data)
        {
            getLogger().Debug("handle DummyBoardOpen with {0}", JsonConvert.SerializeObject(_data));
            string uid = "";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            try
            {
                parameters = _data as Dictionary<string, object>;
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
            instance.OpenDummyBoard(parameters);
        }

        private void handleDummyBoardClose(LibMVCS.Model.Status _status, object _data)
        {
            getLogger().Debug("handle DummyBoardClose with {0}", JsonConvert.SerializeObject(_data));
            string uid = "";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            try
            {
                parameters = _data as Dictionary<string, object>;
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
            instance.CloseDummyBoard(parameters);
        }

        private void handleBackgroundVisible(LibMVCS.Model.Status _status, object _data)
        {
            getLogger().Debug("handle BackgroundVisible with {0}", JsonConvert.SerializeObject(_data));
            string uid = "";
            bool visible = true;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            try
            {
                parameters = _data as Dictionary<string, object>;
                uid = parameters["uid"] as string;
                visible = (bool)parameters["flag"] ;
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
            instance.ChangeBackgroundVisible(visible);
        }

    }
}

