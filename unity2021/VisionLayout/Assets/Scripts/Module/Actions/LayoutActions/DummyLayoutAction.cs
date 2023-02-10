using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class DummyLayoutAction : LayoutAction
    {
        public const string NAME = "DummyLayout";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.Display);
            if (!active_)
                return;

            baseEnter(NAME);

            var data = new Dictionary<string, object>();
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            data["duration"] = duration;
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyLayoutEnter, data);
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            var data = new Dictionary<string, object>();
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyLayoutExit, data);
        }

        protected override void onUpdate()
        {
            baseUpdate();
        }

        protected override void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            List<Cell> cells = new List<Cell>();
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));

            var data = new Dictionary<string, object>();
            data["virtual_resolution_width"] = canvasWidth_;
            data["virtual_resolution_height"] = canvasHeight_;
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            // 不使用字符型绝对路径，避免在Find时找不到隐藏对象
            data["uiSlot"] = runtimeClone.layerMap[layer];
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyLayoutInlay, data);
        }
    }
}
