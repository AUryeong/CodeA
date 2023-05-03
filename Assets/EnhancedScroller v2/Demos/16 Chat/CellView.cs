using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.Chat
{
    /// <summary>
    /// Super simple data class to hold information for each cell.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// The potential types of a cell
        /// </summary>
        public enum CellType
        {
            Spacer,
            MyText,
            OtherText
        }

        /// <summary>
        /// The type of the cell
        /// </summary>
        public CellType cellType;

        /// <summary>
        /// The text to display (only used on chat cells, not the spacer)
        /// </summary>
        public string someText;

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it.
        /// Only used on chat cells, not the spacer. Spacer always pulls the size of the scroll rect in the controller.
        /// </summary>
        public float cellSize;
    }
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// A reference to the UI Text element to display the cell data
        /// </summary>
        public Text someTextText;

        /// <summary>
        /// A reference to the rect transform which will be
        /// updated by the content size fitter
        /// </summary>
        public RectTransform textRectTransform;

        /// <summary>
        /// The space around the text label so that we
        /// aren't up against the edges of the cell
        /// </summary>
        public RectOffset textBuffer;

        public void SetData(Data data)
        {
            someTextText.text = data.someText;
        }
    }
}
