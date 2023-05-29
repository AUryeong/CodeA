using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct LogCellData
{
    public string standingName;
    public string clothesName;
    public string name;
    public string text;
}

public class LogDetail : MonoBehaviour
{
    private readonly Queue<LogCellData> datas = new Queue<LogCellData>();

    public void OnReset()
    {
        datas.Clear();
    }

    public void AddLog(Dialog talk)
    {
        if (talk.dialogText == null) return;

        if (datas.Count >= 50)
            datas.Dequeue();
        var logCellData = new LogCellData
        {
            name = Utility.GetDialogName(talk.dialogText.name),
            text = Utility.GetDialogName(talk.dialogText.text)
        };
        if (!string.IsNullOrEmpty(talk.dialogText.owner))
        {
            logCellData.standingName = talk.dialogText.owner;
            logCellData.clothesName = talk.characters.Find((x) => x.name == talk.dialogText.owner).clothes;
        }
        datas.Enqueue(logCellData);
    }

    public List<LogCellData> GetDatas()
    {
        return datas.ToList();
    }
}
