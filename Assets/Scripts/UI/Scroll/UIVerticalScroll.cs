using GamesTan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogCellData
{
    public string name;
    public string text;
}

public class UIVerticalScroll : MonoBehaviour, ISuperScrollRectDataProvider
{
    [Header("Basic")] public SuperScrollRect ScrollRect;
    public int Count = 500;
    private List<LogCellData> Datas = new List<LogCellData>();
    private void Awake()
    {
        for (int i = 0; i < Count; i++)
        {
            Datas.Add(new LogCellData()
            {
                name = i.ToString(),
                text = (Count - i).ToString(),
            });
        }

        ScrollRect.DoAwake(this);
        DoAwake();
    }
    protected virtual void DoAwake()
    {
    }

    public int GetCellCount()
    {
        return Datas.Count;
    }

    public void SetCell(GameObject cell, int index)
    {
        var item = cell.GetComponent<UILogCell>();
        item.BindData(Datas[index]);
    }
}