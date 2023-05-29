using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : Singleton<TitleManager>
{
    [SerializeField] private Image wallpaperImage;

    [Header("버튼")] [SerializeField] private Button gameStartButton;
    [SerializeField] private Button gameLoadButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button explorerButton;
    [SerializeField] private Button sideTextButton;
    [SerializeField] private UIExtraWindow extraWindow;

    [Header("경고")] [SerializeField] private Image warningWindow;
    [SerializeField] private Button warningOkayButton;
    [SerializeField] private Button warningCancelButton;

    [Header("연출")] [SerializeField] private Image wallpaperDark;
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private Image downWindow;
    private Sequence startSequence;

    protected override void OnCreated()
    {
        extraWindow.OnCreated();

        gameStartButton.onClick.RemoveAllListeners();
        gameStartButton.onClick.AddListener(GameStart);

        gameLoadButton.onClick.RemoveAllListeners();
        gameLoadButton.onClick.AddListener(() =>
            WindowManager.Instance.ClickWindow(WindowType.LOAD, gameLoadButton.transform.position));

        settingButton.onClick.RemoveAllListeners();
        settingButton.onClick.AddListener(() =>
            WindowManager.Instance.ClickWindow(WindowType.SETTING, settingButton.transform.position));

        explorerButton.onClick.RemoveAllListeners();
        explorerButton.onClick.AddListener(() => extraWindow.Init(explorerButton.transform.position));

        sideTextButton.onClick.RemoveAllListeners();
        sideTextButton.onClick.AddListener(WindowManager.Instance.WindowOpen);

        warningOkayButton.onClick.RemoveAllListeners();
        warningOkayButton.onClick.AddListener(() => GameManager.Instance.sceneManager.SceneLoad(Scene.INGAME));

        warningCancelButton.onClick.RemoveAllListeners();
        warningCancelButton.onClick.AddListener(() => warningWindow.gameObject.SetActive(false));

        GameManager.Instance.saveManager.nowGameData = null;

        if (string.IsNullOrEmpty(GameManager.Instance.saveManager.GameData.saigoCg)) return;

        wallpaperImage.color = new Color(0.8f, 0.8f, 0.8f);
        wallpaperImage.sprite = ResourcesManager.Instance.GetBackground(GameManager.Instance.saveManager.GameData.saigoCg);
    }

    private void Start()
    {
        startSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                wallpaperDark.gameObject.SetActive(true);
                wallpaperDark.color = Color.black;

                foreach (var rect in buttons)
                {
                    rect.anchoredPosition = new Vector2(-483, rect.anchoredPosition.y);
                }

                downWindow.rectTransform.anchoredPosition = new Vector2(0, -40);
            });


        startSequence.Append(wallpaperDark.DOFade(0, 1))
            .Append(downWindow.rectTransform.DOAnchorPosY(40, 0.25f).SetEase(Ease.InSine));

        for (int i = 0; i < buttons.Length; i++)
        {
            var rect = buttons[i];
            startSequence.Insert(1.25f + i * 0.25f, rect.DOAnchorPosX(75, 1).SetEase(Ease.OutBack));
        }

        startSequence.OnComplete(() =>
        {
            //TODO 타이틀 브금 시작
        });
    }

    private void Update()
    {
        CheckClick();
    }

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
            startSequence.Kill(true);
    }

    private void GameStart()
    {
        if (GameManager.Instance.saveManager.GameData.savedGameDatas.Count <= 0)
        {
            GameManager.Instance.sceneManager.SceneLoad(Scene.INGAME);
            return;
        }

        warningWindow.gameObject.SetActive(true);
    }
}