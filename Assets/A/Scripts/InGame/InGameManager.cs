using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
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
        Communication,
        Play
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

    public struct InGameTime
    {
        public int year;
        public int month;
        public int week;
        public TimeType time;
    }
    public class InGameManager : Singleton<InGameManager>
    {
        [Header("이름 설정")] 
        [SerializeField] private Canvas namingCanvas;
        [SerializeField] private TMP_InputField lastNamingInput;
        [SerializeField] private TMP_InputField namingInput;
        [SerializeField] private Button enterButton;

        [SerializeField] private Image warningWindow;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Button warningOkay;
        [SerializeField] private Button warningCancel;

        [SerializeField] private Image fadeInBlack;

        public SubGameData nowGameData;


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

            nowGameData = gameData;
        }

        private void ContinueSaveData()
        {
            ApplySaveData();
            AddLeftTalks();
            StartFadeOut();
        }

        private void StartFadeOut()
        {
            fadeInBlack.gameObject.SetActive(true);
            fadeInBlack.color = Color.black;
            fadeInBlack.DOFade(0, 1).SetDelay(1).OnComplete(() => { fadeInBlack.gameObject.SetActive(false); });
        }

        private void AddLeftTalks()
        {
            if (nowGameData.leftDialogList.Count > 0)
                GameManager.Instance.dialogManager.AddDialog(nowGameData.leftDialogList);

            GameManager.Instance.dialogManager.dialogSkipText = nowGameData.leftDialogSkipText;
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
                GameManager.Instance.dialogManager.AddDialog("editor");
                return;
            }

            namingCanvas.gameObject.SetActive(true);

            enterButton.onClick.RemoveAllListeners();
            enterButton.onClick.AddListener(() =>
            {
                warningWindow.gameObject.SetActive(true);
                warningText.text = "당신의 이름을 " + (string.IsNullOrEmpty(lastNamingInput.text) ? "김" : lastNamingInput.text) + (string.IsNullOrEmpty(namingInput.text) ? "준우" : namingInput.text) + "(으)로 확정하시겠습니까?";
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
            GameManager.Instance.saveManager.GameData.lastName = string.IsNullOrEmpty(lastNamingInput.text) ? "김" : lastNamingInput.text;
            GameManager.Instance.saveManager.GameData.name = string.IsNullOrEmpty(namingInput.text) ? "준우" : namingInput.text;
            
            fadeInBlack.gameObject.SetActive(true);
            fadeInBlack.color = Utility.GetFadeColor(Color.black, 0);
            fadeInBlack.DOFade(1, 1).OnComplete(() =>
            {
                namingCanvas.gameObject.SetActive(false);
                fadeInBlack.DOFade(0, 1).SetDelay(1).OnComplete(() => { fadeInBlack.gameObject.SetActive(false); });
            });

            GameManager.Instance.dialogManager.AddDialog("new");
        }

        #endregion
    }
}