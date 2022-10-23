namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class Cell
    {
        /// <summary>
        /// 节点的宽度
        /// </summary>
        public int width = 0;

        /// <summary>
        /// 节点的高度
        /// </summary>
        public int height = 0;

        /// <summary>
        /// 节点在布局中的固定透明值, 此值在布局后不会变化
        /// </summary>
        public float pinAlpha = 1.0f;

        /// <summary>
        /// 节点在布局中的固定可见性, 此值在布局后不会变化
        /// </summary>
        public bool pinVisible = true;

        /// <summary>
        /// 节点在布局中的固定X坐标, 此值在布局后不会变化
        /// </summary>
        public int pinX = 0;

        /// <summary>
        /// 节点在布局中的固定Y坐标, 此值在布局后不会变化
        /// </summary>
        public int pinY = 0;

        /// <summary>
        /// 节点在布局中的当前X坐标，此值不受虚拟面板的影响
        /// </summary>
        public float dynamicX = 0;

        /// <summary>
        /// 节点在布局中的当前Y坐标，此值不受虚拟面板的影响
        /// </summary>
        public float dynamicY = 0;

        /// <summary>
        /// 节点动画开始的位置
        /// </summary>
        public UnityEngine.Vector2 animStartPos = UnityEngine.Vector2.zero;

        /// <summary>
        /// 节点动画结束的位置
        /// </summary>
        public UnityEngine.Vector2 animEndPos = UnityEngine.Vector2.zero;

        /// <summary>
        /// 节点的水平移动方向，-1为左，1为右
        /// </summary>
        public int directionX = 0;

        /// <summary>
        /// 节点的垂直 移动方向，-1为下，1为上
        /// </summary>
        public int directionY = 0;

        /// <summary>
        /// 节点的动画开始的延时
        /// </summary>
        public float animDelay = 0f;

        /// <summary>
        /// 节点的动画的持续时间
        /// </summary>
        public float animDuration = 0f;

        /// <summary>
        /// 节点所在的行数
        /// </summary>
        public int row = 0;

        /// <summary>
        /// 节点所在的列数
        /// </summary>
        public int column = 0;

        /// <summary>
        /// 节点是否需要绕开虚拟面板
        /// </summary>
        public bool surround = false;


        /// <summary>
        /// 节点对应的资源内容路径
        /// </summary>
        public string contentUri;

        /// <summary>
        /// 节点的目标
        /// </summary>
        public UnityEngine.RectTransform target;

        /// <summary>
        /// 节点的图像组件
        /// </summary>
        public UnityEngine.UI.RawImage image;

        /// <summary>
        /// 节点的画布组组件
        /// </summary>
        public UnityEngine.CanvasGroup canvasGroup;
    }

}
