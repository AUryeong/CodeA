using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LogCellData
{
    public string standingName;
    public string clothesName;
    public string name;
    public string text;
}

public class LogManager : Singleton<LogManager>
{
    protected override bool IsDontDestroying => true;
    private Queue<LogCellData> datas = new Queue<LogCellData>();

    protected override void OnReset()
    {
        base.OnReset();
        datas.Clear();
    }

    public void AddLog(Talk talk)
    {
        if (talk.dialogue == null) return;

        if (datas.Count >= 50)
            datas.Dequeue();
        datas.Enqueue(new LogCellData
        {
            name = talk.dialogue.talker,
            text = talk.dialogue.text,
            standingName = talk.dialogue.owner,
            clothesName = talk.characters.Find((x) => x.name == talk.dialogue.owner).clothes
        });
    }

    public List<LogCellData> GetDatas()
    {
        return datas.ToList();
    }
}
