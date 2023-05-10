using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace UI
{
    public class UITipVerticalScroll : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private EnhancedScrollerCellView tipCellPrefab;
        private List<string> datas;
        private float tipCellSizeY;

        private void Awake()
        {
            scroller.Delegate = this;
            
            tipCellSizeY = tipCellPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        public void SetLog(List<string> cellDatas)
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
            return tipCellSizeY;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller enhancedScroller, int dataIndex, int cellIndex)
        {
            var tipCell = scroller.GetCellView(tipCellPrefab) as UITipCell;
            tipCell.SetData(datas[dataIndex]);
            return tipCell;
        }
    }
}