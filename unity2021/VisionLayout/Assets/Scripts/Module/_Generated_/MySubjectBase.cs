
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.70.0.  DO NOT EDIT!
//*************************************************************************************

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class MySubjectBase
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["style"] = "default";
        /// data["uiSlot"] = "";
        /// data["worldSlot"] = "";
        /// model.Publish(/XTC/VisionLayout/Create, data);
        /// </example>
        public const string Create = "/XTC/VisionLayout/Create";

        /// <summary>
        /// 打开
        /// </summary>
        /// <remarks>
        /// 先加载资源，然后显示
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["source"] = "file";
        /// data["uri"] = "";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/VisionLayout/Open, data);
        /// </example>
        public const string Open = "/XTC/VisionLayout/Open";

        /// <summary>
        /// 显示
        /// </summary>
        /// <remarks>
        /// 仅显示，不执行其他任何操作
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/VisionLayout/Show, data);
        /// </example>
        public const string Show = "/XTC/VisionLayout/Show";

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <remarks>
        /// 仅隐藏，不执行其他任何操作
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/VisionLayout/Hide, data);
        /// </example>
        public const string Hide = "/XTC/VisionLayout/Hide";

        /// <summary>
        /// 关闭
        /// </summary>
        /// <remarks>
        /// 先隐藏，然后释放资源
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/VisionLayout/Close, data);
        /// </example>
        public const string Close = "/XTC/VisionLayout/Close";

        /// <summary>
        /// 销毁
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// model.Publish(/XTC/VisionLayout/Close, data);
        /// </example>
        public const string Delete = "/XTC/VisionLayout/Delete";
    }
}
