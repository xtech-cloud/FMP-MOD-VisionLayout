using System.Collections.Generic;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class DummyOutTransitionAction : TransitionAction
    {
        public const string NAME = "DummyOutTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);

            var data = new Dictionary<string, object>();
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            data["duration"] = duration;
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyOutTransitionEnter, data);
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);

            hideLayer();

            var data = new Dictionary<string, object>();
            data["layer"] = layer;
            data["pattern"] = layerPattern.name;
            myInstance.getMyEntry().getDummyModel().Publish(MySubject.OnDummyOutTransitionExit, data);
        }

        protected override void onUpdate()
        {
            baseUpdate();
        }
    }
}
