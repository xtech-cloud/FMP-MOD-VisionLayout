
using Newtonsoft.Json;
using System.Collections.Generic;
using XTC.FMP.MOD.VisionLayout.LIB.Unity;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class MySubject : MySubjectBase
    {
        /// <summary>
        /// �򿪹�����
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// model.Publish(/XTC/VisionLayout/ToolBar/Popup, data);
        /// </example>
        public const string ToolBarPopup = "/XTC/VisionLayout/ToolBar/Popup";

        /// <summary>
        /// ���ñ����ɼ���
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["flag"] = true;
        /// model.Publish(/XTC/VisionLayout/Background/Visible, data);
        /// </example>
        public const string BackgroundVisible = "/XTC/VisionLayout/Background/Visible";

        /// <summary>
        /// ���������
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// model.Publish(/XTC/VisionLayout/DummyBoard/Open, data);
        /// </example>
        public const string DummyBoardOpen = "/XTC/VisionLayout/DummyBoard/Open";

        /// <summary>
        /// ���������
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// model.Publish(/XTC/VisionLayout/DummyBoard/Close, data);
        /// </example>
        public const string DummyBoardClose = "/XTC/VisionLayout/DummyBoard/Close";

        /// <summary>
        /// �����Ⲽ��Ƕ��ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        ///     var width = _data["virtual_resolution_width"] as int;
        ///     var height = _data["virtual_resolution_height"] as int;
        ///     var uiSlot = _data["uiSlot"] as Transform;
        /// }
        /// </example>
        public const string OnDummyLayoutInlay = "/XTC/VisionLayout/DummyLayout/OnInlay";
        
        /// <summary>
        /// �����Ⲽ��״̬����ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        ///     var duration = _data["duration"] as float;
        /// }
        /// </example>
        public const string OnDummyLayoutEnter= "/XTC/VisionLayout/DummyLayout/OnEnter";

        /// <summary>
        /// �����Ⲽ��״̬�˳�ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        /// }
        /// </example>
        public const string OnDummyLayoutExit = "/XTC/VisionLayout/DummyLayout/OnExit";

        /// <summary>
        /// �����Ⲽ����任״̬����ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        ///     var duration = _data["duration"] as float;
        /// }
        /// </example>
        public const string OnDummyInTransitionEnter = "/XTC/VisionLayout/DummyInTransition/OnEnter";

        /// <summary>
        /// �����Ⲽ����任�˳�ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        /// }
        /// </example>
        public const string OnDummyInTransitionExit = "/XTC/VisionLayout/DummyInTransition/OnExit";

        /// <summary>
        /// �����Ⲽ�ֳ��任״̬����ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        ///     var duration = _data["duration"] as float;
        /// }
        /// </example>
        public const string OnDummyOutTransitionEnter = "/XTC/VisionLayout/DummyOutTransition/OnEnter";

        /// <summary>
        /// �����Ⲽ�ֳ��任״̬�˳�ʱ
        /// </summary>
        /// <example>
        /// private void handle(LibMVCS.Model.Status _status, object _data)
        /// {
        ///     var layer = _data["layer"] as string;
        /// }
        /// </example>
        public const string OnDummyOutTransitionExit = "/XTC/VisionLayout/DummyOutTransition/OnExit";

    }
}
