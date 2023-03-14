using GamesTan.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
    public class UITipVerticalScroll : MonoBehaviour, ISuperScrollRectDataProvider
    {
        [SerializeField] SuperScrollRect scrollRect;
        private List<string> getTips;
        private readonly Dictionary<GameObject, UITipCell> cellDictionaries = new Dictionary<GameObject, UITipCell>();

        public void SetLog(List<string> cellDatas)
        {
            getTips = cellDatas;
            cellDictionaries.Clear();

            scrollRect.DoAwake(this);
        }
        public int GetCellCount()
        {
            return getTips.Count;
        }

        public void SetCell(GameObject cell, int index)
        {
            UITipCell tipCell;
            if (cellDictionaries.ContainsKey(cell))
            {
                tipCell = cellDictionaries[cell];
            }
            else
            {
                tipCell = cell.GetComponent<UITipCell>();
                cellDictionaries.Add(cell, tipCell);
            }
            tipCell.SetData(getTips[index]);
        }
    }
}