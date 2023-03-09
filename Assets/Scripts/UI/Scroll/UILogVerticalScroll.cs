using GamesTan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogVerticalScroll : MonoBehaviour, ISuperScrollRectDataProvider
    {
        [SerializeField] SuperScrollRect scrollRect;
        private List<LogCellData> datas;
        private readonly Dictionary<GameObject, UILogCell> cellDictionaries = new Dictionary<GameObject, UILogCell>();

        public void SetLog( List<LogCellData> cellDatas)
        {
            datas = cellDatas;
            cellDictionaries.Clear();

            scrollRect.DoAwake(this);
            DoAwake();
        }
        protected virtual void DoAwake()
        {
        }

        public int GetCellCount()
        {
            return datas.Count;
        }

        public void SetCell(GameObject cell, int index)
        {
            UILogCell logCell;
            if (cellDictionaries.ContainsKey(cell))
            {
                logCell = cellDictionaries[cell];
            }
            else
            {
                logCell = cell.GetComponent<UILogCell>();
                cellDictionaries.Add(cell, logCell);
            }
            logCell.SetData(datas[index]);
        }
    }
}
