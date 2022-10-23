namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class Cell
    {
        /// <summary>
        /// �ڵ�Ŀ��
        /// </summary>
        public int width = 0;

        /// <summary>
        /// �ڵ�ĸ߶�
        /// </summary>
        public int height = 0;

        /// <summary>
        /// �ڵ��ڲ����еĹ̶�͸��ֵ, ��ֵ�ڲ��ֺ󲻻�仯
        /// </summary>
        public float pinAlpha = 1.0f;

        /// <summary>
        /// �ڵ��ڲ����еĹ̶��ɼ���, ��ֵ�ڲ��ֺ󲻻�仯
        /// </summary>
        public bool pinVisible = true;

        /// <summary>
        /// �ڵ��ڲ����еĹ̶�X����, ��ֵ�ڲ��ֺ󲻻�仯
        /// </summary>
        public int pinX = 0;

        /// <summary>
        /// �ڵ��ڲ����еĹ̶�Y����, ��ֵ�ڲ��ֺ󲻻�仯
        /// </summary>
        public int pinY = 0;

        /// <summary>
        /// �ڵ��ڲ����еĵ�ǰX���꣬��ֵ������������Ӱ��
        /// </summary>
        public float dynamicX = 0;

        /// <summary>
        /// �ڵ��ڲ����еĵ�ǰY���꣬��ֵ������������Ӱ��
        /// </summary>
        public float dynamicY = 0;

        /// <summary>
        /// �ڵ㶯����ʼ��λ��
        /// </summary>
        public UnityEngine.Vector2 animStartPos = UnityEngine.Vector2.zero;

        /// <summary>
        /// �ڵ㶯��������λ��
        /// </summary>
        public UnityEngine.Vector2 animEndPos = UnityEngine.Vector2.zero;

        /// <summary>
        /// �ڵ��ˮƽ�ƶ�����-1Ϊ��1Ϊ��
        /// </summary>
        public int directionX = 0;

        /// <summary>
        /// �ڵ�Ĵ�ֱ �ƶ�����-1Ϊ�£�1Ϊ��
        /// </summary>
        public int directionY = 0;

        /// <summary>
        /// �ڵ�Ķ�����ʼ����ʱ
        /// </summary>
        public float animDelay = 0f;

        /// <summary>
        /// �ڵ�Ķ����ĳ���ʱ��
        /// </summary>
        public float animDuration = 0f;

        /// <summary>
        /// �ڵ����ڵ�����
        /// </summary>
        public int row = 0;

        /// <summary>
        /// �ڵ����ڵ�����
        /// </summary>
        public int column = 0;

        /// <summary>
        /// �ڵ��Ƿ���Ҫ�ƿ��������
        /// </summary>
        public bool surround = false;


        /// <summary>
        /// �ڵ��Ӧ����Դ����·��
        /// </summary>
        public string contentUri;

        /// <summary>
        /// �ڵ��Ŀ��
        /// </summary>
        public UnityEngine.RectTransform target;

        /// <summary>
        /// �ڵ��ͼ�����
        /// </summary>
        public UnityEngine.UI.RawImage image;

        /// <summary>
        /// �ڵ�Ļ��������
        /// </summary>
        public UnityEngine.CanvasGroup canvasGroup;
    }

}
