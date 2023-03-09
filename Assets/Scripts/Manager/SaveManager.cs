using System;
using System.Collections.Generic;
using UnityEngine;
using static TimeType;

public enum LoveLevelType
{
    A,
}

public enum StatType
{
    NOTMAJOR,
    PROGRAMMING,
    GRAPHIC,
    WRITING,
    ATTRACTION
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
    
    public string name = string.Empty; // 이름
    public float sfxSound = 1; // 효과음 배율
    public float bgmSound = 1; // BGM 배율
    public float textSpeed = 0.5f; // 대화 속도 배율

    public bool textAuto;

    public string saigoCg = string.Empty;

    public List<string> getCg = new List<string>(); // 얻은 CG

    public List<string> getVideo = new List<string>(); // 얻은 w


    public SubGameData GetSaveData(int idx)
    {
        return savedGameDatas.Find((data) => data.idx.Equals(idx));
    }

    public void ChangeSaveData(int idx)
    {
        if (GameManager.Instance.nowGameData == null) return;

        GameManager.Instance.UpdateGameData();

        var data = GameManager.Instance.nowGameData.Copy();
        GameManager.Instance.nowGameData = data;
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

    public int year = 1;
    public int month = 3;
    public int week = 1;
    public TimeType time = MORNING;

    public List<int> loveLevel = new List<int>();
    
    public List<int> statLevels = new List<int>();

    public List<SkillType> hasSkills = new List<SkillType>();

    public List<Item> hasItems = new List<Item>();

    public List<string> getTips = new List<string>();

    public List<Talk> leftTalks = new List<Talk>();

    public string saveTime;

    public SubGameData Copy()
    {
        return new SubGameData
        {
            idx = idx,
            year = year,
            month = month,
            week = week,
            time = time,
            loveLevel = loveLevel,
            statLevels = statLevels,
            hasSkills = hasSkills,
            hasItems = hasItems,
            leftTalks = leftTalks,
            saveTime = saveTime
        };
    }
}

public class SaveManager : Singleton<SaveManager>
{
    public string prefsName = "CodeA";

    private GameData gameData;

    public GameData GameData
    {
        get
        {
            if (gameData == null)
                LoadGameData();
            return gameData;
        }
    }

    protected override bool IsDontDestroying => true;

    protected override void OnCreated()
    {
        LoadGameData();

        //TODO talk.saveTime = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 tt");
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