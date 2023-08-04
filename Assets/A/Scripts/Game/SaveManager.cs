using Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LoveLevelType
{
    A,
    Loli,
    Sister
}

public enum StatType
{
    NonMajor,
    Programming,
    Graphic,
    Director,
    Communication
}

public enum SkillType
{
}

public enum Item
{
}

public enum TimeType
{
    Morning,
    Afternoon,
    Night,
    Dawn
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
    public TimeType time = TimeType.Morning;

    public Dictionary<LoveLevelType, int> loveLevels = new Dictionary<LoveLevelType, int>()
    {
        { LoveLevelType.A, 0 },
        { LoveLevelType.Loli, 0 },
        { LoveLevelType.Sister, 0 }
    };

    public Dictionary<StatType, int> statLevels = new Dictionary<StatType, int>()
    {
        { StatType.NonMajor, 0 },
        { StatType.Programming, 0 },
        { StatType.Graphic, 0 },
        { StatType.Director, 0 },
        { StatType.Communication, 0 }
    };

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
            loveLevels = loveLevels.ToDictionary(enter => enter.Key, enter => enter.Value),
            statLevels = statLevels.ToDictionary(enter => enter.Key, enter => enter.Value),
            hasSkills = hasSkills.ToList(),
            hasItems = hasItems.ToList(),
            leftDialogList = leftDialogList.ToList(),
            leftDialogSkipText = leftDialogSkipText,
            saveTime = saveTime
        };
    }
}

public class SaveManager : Manager
{
    public string prefsName = "CodeA";

    [SerializeField] private GameData gameData; // 게임 데이터 확인용

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

        nowGameData.loveLevels = InGameManager.Instance.loveLevels;
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