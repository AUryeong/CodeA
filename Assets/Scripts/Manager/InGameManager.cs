using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : Singleton<InGameManager>
{
    [Header("이름 설정")] [SerializeField] private Canvas namingCanvas;
    [SerializeField] private TMP_InputField namingInput;
    [SerializeField] private Button enterButton;

    [SerializeField] private Image warningWindow;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button warningOkay;
    [SerializeField] private Button warningCancel;

    [Header("리얼리 인게임 요소")]

    public int year;
    public int month;
    public int week;
    public TimeType time;
    
    public List<int> loveLevel = new List<int>();
    public List<int> statLevels = new List<int>();
    public List<SkillType> hasSkills = new List<SkillType>();
    public List<Item> hasItems = new List<Item>();


    private void Start()
    {
        if (Instance == this)
        {
            if (GameManager.Instance.nowGameData != null && GameManager.Instance.nowGameData.idx >= 0)
                ContinueSaveData();
            else
                NewSaveData();
        }
    }

    #region 세이브 로드

    private void ApplySaveData()
    {
        var gameData = GameManager.Instance.nowGameData;
        if (gameData == null) return;
        
        year = gameData.year;
        month = gameData.month;
        week = gameData.week;
        time = gameData.time;
        
        loveLevel = gameData.loveLevel;
        statLevels = gameData.statLevels;
        hasSkills = gameData.hasSkills;
        hasItems = gameData.hasItems;
    }

    private void ContinueSaveData()
    {
        ApplySaveData();
        AddLeftTalks();
    }

    private void AddLeftTalks()
    {
        if (GameManager.Instance.nowGameData.leftTalks.Count > 0)
            TalkManager.Instance.AddTalk(GameManager.Instance.nowGameData.leftTalks);
        
        TalkManager.Instance.talkSkipText = GameManager.Instance.nowGameData.leftTalkSkipText;
    }

    private void NewSaveData()
    {
        GameManager.Instance.nowGameData = new SubGameData();
        
        ApplySaveData();
        NamingSetting();
    }

    private void NamingSetting()
    {
        if (!string.IsNullOrEmpty(SaveManager.Instance.GameData.name))
        {
            TalkManager.Instance.AddTalk("new");
            return;
        }

        GameManager.Instance.nowGameData = new SubGameData();
        namingCanvas.gameObject.SetActive(true);

        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(() =>
        {
            warningWindow.gameObject.SetActive(true);
            warningText.text = "당신의 이름을 " + (string.IsNullOrEmpty(namingInput.text) ? "김준우" : namingInput.text) + "(으)로 확정하시겠습니까?";
            //TODO SOUND
        });

        warningOkay.onClick.RemoveAllListeners();
        warningOkay.onClick.AddListener(EnterName);

        warningCancel.onClick.RemoveAllListeners();
        warningCancel.onClick.AddListener(() =>
        {
            warningWindow.gameObject.SetActive(false);
            //TODO SOUND
        });
    }

    private void EnterName()
    {
        //TODO SOUND
        SaveManager.Instance.GameData.name = string.IsNullOrEmpty(namingInput.text) ? "김준우" : namingInput.text;
        namingCanvas.gameObject.SetActive(false);

        TalkManager.Instance.AddTalk("new");
    }
    #endregion
}