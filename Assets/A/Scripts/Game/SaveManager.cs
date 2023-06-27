using Ingame;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TimeType;

public enum LoveLevelType
{
    A,
    LOLI,
    SISTER
}

public enum StatType
{
    NON_MAJOR,
    PROGRAMMING,
    GRAPHIC,
    DIRECTOR,
    COMMUNICATION
}

public enum SkillType
{
}

public enum Item
{
}

public enum TimeType
{
    MORNING,
    AFTERNOON,
    NIGHT,
    DAWN
}

[Serializable]
public class GameData
{
    public bool isDownloadFile;

    public string lastName = string.Empty;
    public string name = string.Empty;
    
    public float sfxSoundMultiplier = 1;
    public float bgmSoundMultiplier = 1;

    public float textSpeed = 0.5f;
    public bool textAuto;

    public string lastCg = string.Empty;

    public List<string> getCg = new List<string>();
    public List<string> getScenes = new List<string>();
    public List<string> getTips = new List<string>();


    public SubGameData GetSaveData(int idx)
    {
        return savedGameDatas.Find((data) => data.idx.Equals(idx));
    }

    public void ChangeSaveData(int idx)
    {
        if (GameManager.Instance.saveManager.nowGameData == null) return;

        GameManager.Instance.saveManager.UpdateGameData();

        var data = GameManager.Instance.saveManager.nowGameData.Copy();
        GameManager.Instance.saveManager.nowGameData = data;
        data.idx = idx;

        if (savedGameDatas != null && savedGameDatas.Count > 0)
        {
            int changedDataIdx = savedGameDatas.FindIndex(x => x.idx.Equals(idx));
            if (changedDataIdx >= 0)
            {
                savedGameDatas[changedDataIdx] = data;
                return;
            }
        }

        savedGameDatas.Add(data);
    }

    public List<SubGameData> savedGameDatas = new List<SubGameData>(); // 따로 저장한 데이터
}

[Serializable]
public class SubGameData
{
    public int idx = -1;

    public string saveTime;

    public int year = 1;
    public int month = 3;
    public int week = 1;
    public TimeType time = MORNING;

    public List<int> loveLevels = new List<int>();
    public List<int> statLevels = new List<int>();

    public List<SkillType> hasSkills = new List<SkillType>();
    public List<Item> hasItems = new List<Item>();

    public string leftDialogSkipText = string.Empty;
    public List<Dialog> leftDialogList = new List<Dialog>();

    public SubGameData Copy()
    {
        return new SubGameData
        {
            idx = idx,
            year = year,
            month = month,
            week = week,
            time = time,
            loveLevels = loveLevels,
            statLevels = statLevels,
            hasSkills = hasSkills,
            hasItems = hasItems,
            leftDialogList = leftDialogList,
            leftDialogSkipText = leftDialogSkipText,
            saveTime = saveTime
        };
    }
}

public class SaveManager : Manager
{
    public string prefsName = "CodeA";

    [SerializeField] private GameData gameData;

    public GameData GameData
    {
        get
        {
            if (gameData == null)
                LoadGameData();
            return gameData;
        }
    }

    public SubGameData nowGameData;

    public override void OnCreated()
    {
        LoadGameData();
    }

    public void AddTip(string tipName)
    {
        if (GameData.getTips.Contains(tipName)) return;

        GameData.getTips.Add(tipName);
        GameData.getTips.Sort();
    }

    public void AddCgData(string cgName)
    {
       GameData.lastCg = cgName;

        if (GameData.getCg.Contains(cgName)) return;

        GameData.getCg.Add(cgName);
        GameData.getCg.Sort();
    }

    public void UpdateGameData()
    {
        if (nowGameData == null) return;

        nowGameData.leftDialogList = GameManager.Instance.dialogManager.GetLeftDialogs();
        nowGameData.leftDialogSkipText = GameManager.Instance.dialogManager.dialogSkipText;
        nowGameData.saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        nowGameData.year = InGameManager.Instance.year;
        nowGameData.month = InGameManager.Instance.month;
        nowGameData.week = InGameManager.Instance.week;
        nowGameData.time = InGameManager.Instance.time;

        nowGameData.loveLevels = InGameManager.Instance.loveLevel;
        nowGameData.statLevels = InGameManager.Instance.statLevels;
        nowGameData.hasSkills = InGameManager.Instance.hasSkills;
        nowGameData.hasItems = InGameManager.Instance.hasItems;
    }

    public void ResetSaveFile()
    {
        gameData = new GameData();
        SaveGameData();
        LoadGameData();
    }

    private void LoadGameData()
    {
        var s = PlayerPrefs.GetString(prefsName, "null");
        gameData = s.Equals("null") || string.IsNullOrEmpty(s) ? new GameData() : JsonUtility.FromJson<GameData>(s);
    }


    private void SaveGameData()
    {
        PlayerPrefs.SetString(prefsName, JsonUtility.ToJson(gameData));
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGameData();
    }
}