using System.Collections.Generic;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class EmptyOutTransitionAction : TransitionAction
    {
        public const string NAME = "EmptyOutTransition";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Disappear, ActionCategory.Out);
            if (!active_)
                return;

            baseEnter(NAME);
            hideLayer();
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
        }

        protected override void onUpdate()
        {
            baseUpdate();
        }
    }
}
