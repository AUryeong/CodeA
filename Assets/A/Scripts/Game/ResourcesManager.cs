using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ResourcesManager : Manager
{
    public bool IsLoading { get; private set; }
    private readonly Dictionary<string, Sprite> backgroundSprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, CharacterStanding> characters = new Dictionary<string, CharacterStanding>();
    private readonly Dictionary<string, Dialogs> dialogs = new Dictionary<string, Dialogs>();
    private readonly Dictionary<string, Sprite> popupSprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, string> tips = new Dictionary<string, string>();

    [SerializeField] private List<SerializedCharacter> serializedCharacters;

    #region 서버

    [Header("어드레서블용")] 
    [SerializeField] private AssetLabelReference backgroundLabel;
    [SerializeField] private AssetLabelReference dialogLabel;
    [SerializeField] private AssetLabelReference tipLabel;
    [SerializeField] private AssetLabelReference popupLabel;


    [Space(20)] [SerializeField] private Canvas downloadWindow;
    [SerializeField] private TextMeshProUGUI downloadingText;

    [SerializeField] private Image downloadWarningWindow;
    [SerializeField] private Button downloadWarningButton;
    [SerializeField] private Button exitWarningButton;

    [SerializeField] private Button downloadButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Canvas loadingWindow;
    [SerializeField] private TextMeshProUGUI loadingText;
    private AsyncOperationHandle downloadHandle;

    public override void OnCreated()
    {
        Time.timeScale = 0;
        IsLoading = true;
        loadingWindow.gameObject.SetActive(true);

        StartCoroutine(WaitDownload());
    }

    private IEnumerator WaitDownload()
    {
        CheckDownload();

        const float loadingCoolTime = 1f;
        
        float loadingDuration = 0;
        float loadingIdx = 0;
        
        while (IsLoading)
        {
            loadingDuration += Time.unscaledDeltaTime;
            if (loadingDuration >= loadingCoolTime)
            {
                loadingDuration -= loadingCoolTime;
                loadingIdx = (loadingIdx + 1) % 3;

                if (loadingIdx == 0)
                    loadingText.text = "로딩 중.";
                else
                    loadingText.text += ".";
            }

            yield return null;
        }

        loadingWindow.gameObject.SetActive(false);
        Time.timeScale = 1;

        if (GameManager.Instance.sceneManager.NowScene == SceneType.Loading)
            GameManager.Instance.sceneManager.SceneLoad(SceneType.Title);
    }

    private void CheckDownload()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!GameManager.Instance.saveManager.GameData.isDownloadFile)
            {
                DisableDownloadWindow();
                return;
            }
        }

        Addressables.GetDownloadSizeAsync(backgroundLabel).Completed +=
            CheckDownloadSize;
    }

    private void CheckDownloadSize(AsyncOperationHandle<long> sizeHandle)
    {
        if (sizeHandle.Status == AsyncOperationStatus.Failed)
        {
            DisableDownloadWindow();
            return;
        }

        if (sizeHandle.Result > 0)
        {
            downloadWindow.gameObject.SetActive(true);

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(Application.Quit);

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                downloadingText.text = $"{sizeHandle.Result / Math.Pow(1024, 2):##.##} MB만큼의 파일을\n다운로드 해야합니다.";
                downloadButton.gameObject.SetActive(true);

                downloadButton.onClick.RemoveAllListeners();
                downloadButton.onClick.AddListener(CheckWifi);
            }
            else
            {
                downloadingText.text = "와이파이에 연결되어 있지 않습니다.\n최초 또는 업데이트 파일 다운로드를 위해 와이파이를 연결해주세요.";
            }
        }
        else
        {
            LoadAddressable();
        }

        Addressables.Release(sizeHandle);
    }

    private void DisableDownloadWindow()
    {
        downloadButton.gameObject.SetActive(false);
        downloadWindow.gameObject.SetActive(true);
        downloadingText.gameObject.SetActive(true);
        downloadWarningWindow.gameObject.SetActive(true);
        loadingWindow.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);

        downloadingText.text = "와이파이에 연결되어 있지 않습니다.\n최초 또는 업데이트 파일 다운로드를 위해 와이파이를 연결해주세요.";
    }

    private void CheckWifi()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                DisableDownloadWindow();
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                downloadWarningWindow.gameObject.SetActive(true);

                downloadWarningButton.onClick.RemoveAllListeners();
                downloadWarningButton.onClick.AddListener(DownloadBundle);

                exitWarningButton.onClick.RemoveAllListeners();
                exitWarningButton.onClick.AddListener(Application.Quit);
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                DownloadBundle();
                break;
            default:
                DisableDownloadWindow();
                break;
        }
    }

    private void DownloadBundle()
    {
        downloadWindow.gameObject.SetActive(true);
        downloadWarningWindow.gameObject.SetActive(true);
        downloadWarningButton.gameObject.SetActive(false);
        downloadingText.gameObject.SetActive(true);
        downloadButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        exitWarningButton.gameObject.SetActive(false);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            DisableDownloadWindow();
            return;
        }

        downloadHandle = Addressables.DownloadDependenciesAsync(backgroundLabel);
        downloadHandle.Completed +=
            handle =>
            {
                Addressables.Release(handle);
                downloadingText.text = string.Concat("다운로드 완료!");
                downloadWindow.gameObject.SetActive(false);
                GameManager.Instance.saveManager.GameData.isDownloadFile = true;

                LoadAddressable();
            };
    }

    private void FixedUpdate()
    {
        if (downloadHandle.IsValid())
            downloadingText.text = string.Concat("다운로드 중 : ", downloadHandle.PercentComplete * 100, "% / 100%");
    }

    #endregion

    private void LoadAddressable()
    {
        StartCoroutine(LoadAddressableCoroutine());
    }

    private IEnumerator LoadAddressableCoroutine()
    {
        LoadCharacter();

        var backgroundAsync = Addressables.LoadAssetsAsync<Sprite>(backgroundLabel, sprite => { backgroundSprites.Add(sprite.name, sprite); });
        if (!backgroundAsync.IsDone)
            yield return null;

        var dialogAsync = Addressables.LoadAssetsAsync<Dialogs>(dialogLabel, dialog => { dialogs.Add(dialog.name, dialog); });
        if (!dialogAsync.IsDone)
            yield return null;

        var tipAsync = Addressables.LoadAssetsAsync<Tips>(tipLabel, tipLists =>
        {
            foreach (var tip in tipLists.tips)
                tips.Add(tip.id, tip.lore);
        });
        if (!tipAsync.IsDone)
            yield return null;

        var popupAsync = Addressables.LoadAssetsAsync<Sprite>(popupLabel, sprite => { popupSprites.Add(sprite.name, sprite); });
        if (!popupAsync.IsDone)
            yield return null;

        IsLoading = false;
        GameManager.Instance.saveManager.GameData.isDownloadFile = true;
        Debug.Log("로딩 완료!");
    }

    private void LoadCharacter()
    {
        foreach (var serializedCharacter in serializedCharacters)
        {
            var character = new CharacterStanding();
            foreach (var serializedStanding in serializedCharacter.standings)
            {
                var standing = new Standing();
                var faceDictionary = new Dictionary<string, Sprite>();
                foreach (var faceSprite in serializedStanding.face)
                {
                    faceDictionary.Add(faceSprite.name, faceSprite);
                }

                standing.baseStanding = serializedStanding.baseStanding;
                standing.logFace = serializedStanding.logFace;
                standing.faces = faceDictionary;
                character.standings.Add(serializedStanding.name, standing);
            }

            characters.Add(serializedCharacter.name, character);
        }
    }

    public Sprite GetBackground(string backgroundName)
    {
        return backgroundSprites.TryGetValue(backgroundName, out var sprite) ? sprite : null;
    }

    public CharacterStanding GetCharacter(string characterName)
    {
        return characters[characterName];
    }

    public Dialogs GetDialog(string dialogName)
    {
        return dialogs.TryGetValue(dialogName, out var dialog) ? dialog : null;
    }

    public Sprite GetPopup(string popupName)
    {
        return popupSprites.TryGetValue(popupName, out var sprite) ? sprite : null;
    }

    public string GetTip(string tipName)
    {
        return tips.TryGetValue(tipName, out var tip) ? tip : null;
    }
}