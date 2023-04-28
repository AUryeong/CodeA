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

        private float quitPosY;

        public void SetLog(List<LogCellData> cellDatas)
        {
            datas = cellDatas;
            cellDictionaries.Clear();

            scrollRect.DoAwake(this);
            DoAwake();
        }
        private void DoAwake()
        {
            float startPosY = Mathf.Max(0, scrollRect.content.sizeDelta.y - 930);
            quitPosY = startPosY + 300;
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, startPosY);
        }

        private void Update()
        {
            if(scrollRect.content.anchoredPosition.y >= quitPosY)
            {
                WindowManager.Instance.CloseAllWindow();
            }
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
