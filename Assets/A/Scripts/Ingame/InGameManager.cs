using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ingame
{
    public class InGameManager : Singleton<InGameManager>
    {
        [Header("이름 설정")] [SerializeField] private Canvas namingCanvas;
        [SerializeField] private TMP_InputField namingInput;
        [SerializeField] private Button enterButton;

        [SerializeField] private Image warningWindow;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Button warningOkay;
        [SerializeField] private Button warningCancel;

        [SerializeField] private Image fadeInBlack;

        [Header("인게임 요소")] public int year;
        public int month;
        public int week;
        public TimeType time;

        public List<int> loveLevel = new List<int>();
        public List<int> statLevels = new List<int>();
        public List<SkillType> hasSkills = new List<SkillType>();
        public List<Item> hasItems = new List<Item>();


        private void Start()
        {
            if (Instance != this) return;
            
            if (GameManager.Instance.saveManager.nowGameData != null && GameManager.Instance.saveManager.nowGameData.idx >= 0)
                ContinueSaveData();
            else
                NewSaveData();
        }

        #region 세이브 로드

        private void ApplySaveData()
        {
            var gameData = GameManager.Instance.saveManager.nowGameData;
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
            StartFadeOut();
            ApplySaveData();
            AddLeftTalks();
        }

        private void StartFadeOut()
        {
            fadeInBlack.gameObject.SetActive(true);
            fadeInBlack.color = Color.black;
            fadeInBlack.DOFade(0, 1).SetDelay(1).OnComplete(() => { fadeInBlack.gameObject.SetActive(false); });
        }

        private void AddLeftTalks()
        {
            if (GameManager.Instance.saveManager.nowGameData.leftTalks.Count > 0)
                TalkManager.Instance.AddTalk(GameManager.Instance.saveManager.nowGameData.leftTalks);

            TalkManager.Instance.talkSkipText = GameManager.Instance.saveManager.nowGameData.leftTalkSkipText;
        }

        private void NewSaveData()
        {
            GameManager.Instance.saveManager.nowGameData = new SubGameData();

            ApplySaveData();
            NamingSetting();
        }

        private void NamingSetting()
        {
            if (!string.IsNullOrEmpty(GameManager.Instance.saveManager.GameData.name))
            {
                StartFadeOut();
                TalkManager.Instance.AddTalk("new");
                return;
            }

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
            GameManager.Instance.saveManager.GameData.name = string.IsNullOrEmpty(namingInput.text) ? "김준우" : namingInput.text;
            fadeInBlack.gameObject.SetActive(true);
            fadeInBlack.color = Utility.GetFadeColor(Color.black, 0);
            fadeInBlack.DOFade(1, 1).OnComplete(() =>
            {
                namingCanvas.gameObject.SetActive(false);
                fadeInBlack.DOFade(0, 1).SetDelay(1).OnComplete(() => { fadeInBlack.gameObject.SetActive(false); });
            });

            TalkManager.Instance.AddTalk("new");
        }

        #endregion
    }
}