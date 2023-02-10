using System.Collections.Generic;
using System.Numerics;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class DummyInTransitionAction : TransitionAction
    {
        public const string NAME = "DummyInTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.In);
            if (!active_)
                return;

            baseEnter(NAME);
            showLayer();

            var data = new Dictionary<string, object>();
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            data["duration"] = duration;
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyInTransitionEnter, data);
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            var data = new Dictionary<string, object>();
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyInTransitionExit, data);
        }

        protected override void onUpdate()
        {
            baseUpdate();
        }
    }
}
