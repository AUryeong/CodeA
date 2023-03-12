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
    
    [Header("경고")]
    [SerializeField] private Image warningWindow;
    [SerializeField] private Button warningOkayButton;
    [SerializeField] private Button warningCancelButton;

    protected override void OnCreated()
    {
        extraWindow.OnCreated();
        
        gameStartButton.onClick.RemoveAllListeners();
        gameStartButton.onClick.AddListener(GameStart);

        gameLoadButton.onClick.RemoveAllListeners();
        gameLoadButton.onClick.AddListener(() => WindowManager.Instance.ClickWindow(WindowType.LOAD, gameLoadButton.transform.position));

        settingButton.onClick.RemoveAllListeners();
        settingButton.onClick.AddListener(() => WindowManager.Instance.ClickWindow(WindowType.SETTING, settingButton.transform.position));

        explorerButton.onClick.RemoveAllListeners();
        explorerButton.onClick.AddListener(() => extraWindow.Init(explorerButton.transform.position));
        
        sideTextButton.onClick.RemoveAllListeners();
        sideTextButton.onClick.AddListener(WindowManager.Instance.WindowOpen);
        
        warningOkayButton.onClick.RemoveAllListeners();
        warningOkayButton.onClick.AddListener(() =>GameManager.Instance.SceneLoad(Scene.INGAME));
        
        warningCancelButton.onClick.RemoveAllListeners();
        warningCancelButton.onClick.AddListener(() =>warningWindow.gameObject.SetActive(false));
        
        GameManager.Instance.nowGameData = null;

        if (string.IsNullOrEmpty(SaveManager.Instance.GameData.saigoCg)) return;
        
        

        wallpaperImage.color = new Color(0.8f,0.8f,0.8f);
        wallpaperImage.sprite = ResourcesManager.Instance.GetBackground(SaveManager.Instance.GameData.saigoCg);
    }

    private void GameStart()
    {
        if (SaveManager.Instance.GameData.savedGameDatas.Count <= 0)
        {
            GameManager.Instance.SceneLoad(Scene.INGAME);
            return;
        }
        
        warningWindow.gameObject.SetActive(true);
    }
}