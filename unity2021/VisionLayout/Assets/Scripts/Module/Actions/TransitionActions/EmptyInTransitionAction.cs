using System.Collections.Generic;
using System.Numerics;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class EmptyInTransitionAction : TransitionAction
    {
        public const string NAME = "EmptyInTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.In);
            if (!active_)
                return;

            baseEnter(NAME);
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
            showLayer();
        }

        protected override void onUpdate()
        {
            baseUpdate();
        }
    }
}
