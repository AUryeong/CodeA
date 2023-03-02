using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    LOADING,
    TITLE,
    INGAME
}

public class GameManager : Singleton<GameManager>
{
    protected override bool IsDontDestroying => true;
    public Scene nowScene;
    [SerializeField] protected Camera uiCamera;

    public Camera UICamera
    {
        get
        {
            return uiCamera;
        }
    }
    public SubGameData nowGameData;
    
    public void SceneLoad(Scene scene)
    {
        nowScene = scene;
        SceneManager.LoadScene((int)scene);
    }

    protected override void OnCreated()
    {
        Application.targetFrameRate = Application.platform == RuntimePlatform.Android ? 30 : 120;
        SetResolution(uiCamera);
        OnReset();
    }

    public void ViewCG(string cgName)
    {
        SaveManager.Instance.GameData.saigoCg = cgName;

        if (SaveManager.Instance.GameData.getCg.Contains(cgName)) return;
        
        SaveManager.Instance.GameData.getCg.Add(cgName);
        SaveManager.Instance.GameData.getCg.Sort();
    }

    protected override void OnReset()
    {
        SetResolution(Camera.main);
        foreach (var canvas in FindObjectsOfType<Canvas>())
            canvas.worldCamera = uiCamera;
    }

    private void SetResolution(Camera changeCamera)
    {
        if(changeCamera == null) return;
        int setWidth = 1920;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

        float screenMultiplier = (float)setWidth / setHeight;
        float deviceMultiplier = (float)deviceWidth / deviceHeight;

        if (screenMultiplier < deviceMultiplier)
        {
            float newWidth = screenMultiplier / deviceMultiplier;
            changeCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceMultiplier / screenMultiplier;
            changeCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }

    public void UpdateGameData()
    {
        if (nowGameData == null) return;
        
        nowGameData.leftTalks = TalkManager.Instance.GetLeftTalks();
        nowGameData.saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        nowGameData.year = InGameManager.Instance.year;
        nowGameData.month = InGameManager.Instance.month;
        nowGameData.week = InGameManager.Instance.week;
        nowGameData.time = InGameManager.Instance.time;

        nowGameData.loveLevel = InGameManager.Instance.loveLevel;
        nowGameData.statLevels = InGameManager.Instance.statLevels;
        nowGameData.hasSkills = InGameManager.Instance.hasSkills;
        nowGameData.hasItems = InGameManager.Instance.hasItems;
    }
}