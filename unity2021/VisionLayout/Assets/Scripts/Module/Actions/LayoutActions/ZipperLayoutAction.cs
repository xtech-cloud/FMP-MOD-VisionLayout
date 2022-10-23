using System.Collections.Generic;
using XTC.oelFSM;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    public class ZipperLayoutAction : LayoutAction
    {
        public const string NAME = "ZipperLayout";

        protected override void onEnter()
        {
            checkActive(NAME, LayerCategory.Display, ActionCategory.Display);
            if (!active_)
                return;

            baseEnter(NAME);
            if (!filterLayoutCells(LayerCategory.Display))
                return;

            restoreVisible();
            restoreInteractable();
        }

        protected override void onExit()
        {
            if (!active_)
                return;

            baseExit(NAME);
            disableInteractable();
        }

        protected override void onUpdate()
        {
            baseUpdate();

            if (null == layoutCells_)
                return;
        }

        protected override void layout(List<string> _contentList)
        {
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            int columnCount = parseIntFromProperty("column");
            int space = parseIntFromProperty("space");

            int cellWidth = (canvasWidth_ - (columnCount + 1) * space) / columnCount;

            List<Cell> cells = new List<Cell>();
            // 按从左往右，从上到下的顺序排列
            int direction = 1;
            for (int i = 0; i < _contentList.Count || i < columnCount * 2; i++)
            {
                string contentUri = _contentList.Count > 0 ? _contentList[cells.Count % _contentList.Count] : "";
                int fitWidth = cellWidth;
                int fitHeight = cellWidth;
                var coverTexture = loadContentCover(contentUri);
                if (null != coverTexture)
                {
                    fitHeight = (int)((float)coverTexture.height / coverTexture.width * fitWidth);
                }

                int columnIndex = i / 2;
                int pinX = (columnIndex + 1) * space + columnIndex * cellWidth + cellWidth / 2 - canvasWidth_ / 2;
                int pinY = (fitHeight / 2 + space / 2) * direction;
                direction *= -1;

                Cell cell = new Cell();
                cells.Add(cell);
                cell.width = fitWidth;
                cell.height = fitHeight;
                cell.pinX = pinX;
                cell.pinY = pinY;
                cell.dynamicX = pinX;
                cell.dynamicY = pinY;
                cell.animStartPos.x = pinX;
                cell.animStartPos.y = pinY;
                cell.animEndPos.x = pinX;
                cell.animEndPos.y = pinY;
                cell.row = i % 2;
                cell.column = columnIndex;
                cell.directionX = direction;
                cell.contentUri = contentUri;
                cell.surround = false;
                cell.target = newCell(layer, contentUri).GetComponent<UnityEngine.RectTransform>();
                cell.image = cell.target.gameObject.GetComponent<UnityEngine.UI.RawImage>();
                cell.canvasGroup = cell.target.gameObject.GetComponent<UnityEngine.CanvasGroup>();

                cell.target.anchoredPosition = new UnityEngine.Vector2(cell.pinX, cell.pinY);
                cell.target.sizeDelta = new UnityEngine.Vector2(cell.width, cell.height);
                cell.pinVisible = true;
                cell.target.gameObject.SetActive(false);
                if (null != coverTexture)
                    cell.image.texture = coverTexture;
            }
            setParameter(string.Format("layer.{0}.{1}.cells", layer, NAME), Parameter.FromCustom(cells));
        }
    }
}
