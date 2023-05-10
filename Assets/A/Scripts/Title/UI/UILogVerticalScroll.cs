using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace UI
{
    public class UILogVerticalScroll : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private EnhancedScrollerCellView logCellPrefab;
        private List<LogCellData> datas;

        private float logCellSizeY;

        private void Awake()
        {
            scroller.Delegate = this;

            logCellSizeY = logCellPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        public void SetLog(List<LogCellData> cellDatas)
        {
            datas = cellDatas;
            scroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller enhancedScroller)
        {
            return datas.Count;
        }

        public float GetCellViewSize(EnhancedScroller enhancedScroller, int dataIndex)
        {
            return logCellSizeY;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller enhancedScroller, int dataIndex, int cellIndex)
        {
            var logCell = scroller.GetCellView(logCellPrefab) as UILogCell;
            logCell.SetData(datas[dataIndex]);
            return logCell;
        }
    }
}