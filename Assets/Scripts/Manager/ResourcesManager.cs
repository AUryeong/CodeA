using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ResourcesManager : Singleton<ResourcesManager>
{
    protected override bool IsDontDestroying => true;

    public bool isLoading;
    private readonly Dictionary<string, Sprite> backgroundSprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, CharacterStanding> characters = new Dictionary<string, CharacterStanding>();
    private readonly Dictionary<string, Talks> talks = new Dictionary<string, Talks>();
    private readonly Dictionary<string, string> tips = new Dictionary<string, string>();

    [SerializeField] private List<SerializedCharacter> serializedCharacters;

    #region 서버

    [Header("어드레서블용")] [SerializeField] private AssetLabelReference backgroundLabel;
    [SerializeField] private AssetLabelReference talkLabel;
    [SerializeField] private AssetLabelReference tipLabel;
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

    protected override void OnCreated()
    {
        Time.timeScale = 0;
        isLoading = true;
        loadingWindow.gameObject.SetActive(true);

        LoadCharacter();
        StartCoroutine(WaitDownload());
    }

    IEnumerator WaitDownload()
    {
        CheckDownload();

        float loadingCooltime = 1f;
        float loadingDuration = 0;
        float loadingIdx = 0;
        while (isLoading)
        {
            loadingDuration += Time.unscaledDeltaTime;
            if (loadingDuration >= loadingCooltime)
            {
                loadingDuration -= loadingCooltime;
                loadingIdx = (loadingIdx + 1) % 4;

                if (loadingIdx == 0)
                    loadingText.text = "로딩 중";
                else
                    loadingText.text += ".";
            }

            yield return null;
        }

        loadingWindow.gameObject.SetActive(false);
        Time.timeScale = 1;

        if (GameManager.Instance.nowScene == Scene.LOADING)
            GameManager.Instance.SceneLoad(Scene.TITLE);
    }

    private void CheckDownload()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!SaveManager.Instance.GameData.isDownloadFile)
            {
                DisableDownloadWindow();
                return;
            }
        }

        Addressables.GetDownloadSizeAsync(backgroundLabel).Completed +=
            CheckDownloadSize;
    }

    private void ForcedUpdate()
    {
        downloadWindow.gameObject.SetActive(true);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Application.Quit);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            downloadingText.text = "업데이트 또는 버그가 발생했습니다.\n파일을 새로 다운로드 해주세요.";
            downloadButton.gameObject.SetActive(true);

            downloadButton.onClick.RemoveAllListeners();
            downloadButton.onClick.AddListener(CheckWifi);
        }
        else
        {
            downloadingText.text = "와이파이에 연결되어 있지 않습니다.\n최초 또는 업데이트 파일 다운로드를 위해 와이파이를 연결해주세요.";
        }
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
                downloadingText.text = string.Format("{0:##.##} {1}", sizeHandle.Result / (Math.Pow(1024, 2)), " MB") + "만큼의 파일을\n다운로드 해야합니다.";
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
            if (!SaveManager.Instance.GameData.isDownloadFile) SaveManager.Instance.GameData.isDownloadFile = true;
            LoadBackground();
        }

        Addressables.Release(sizeHandle);
    }

    private void DisableDownloadWindow()
    {
        downloadButton.gameObject.SetActive(false);
        downloadWarningWindow.gameObject.SetActive(false);
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
                SaveManager.Instance.GameData.isDownloadFile = true;

                LoadBackground();
            };
    }

    private void FixedUpdate()
    {
        if (downloadHandle.IsValid())
            downloadingText.text = string.Concat("다운로드 중 : ", downloadHandle.PercentComplete * 100, "% / 100%");
    }

    #endregion

    private void LoadBackground()
    {
        Addressables.LoadAssetsAsync<Sprite>(backgroundLabel, sprite => { backgroundSprites.Add(sprite.name, sprite); })
            .Completed += (list) => { LoadTalk(); };
    }

    private void LoadTalk()
    {
        Addressables.LoadAssetsAsync<Talks>(talkLabel, talk => { talks.Add(talk.name, talk); })
            .Completed += (list) => { LoadTips(); };
    }

    private void LoadTips()
    {
        Addressables.LoadAssetsAsync<Tips>(tipLabel, tipLists =>
        {
            foreach (var tip in tipLists.tips)
            {
                tips.Add(tip.id, tip.lore);
            }

            ;
        }).Completed += (list) => { isLoading = false; };
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
        return backgroundSprites.ContainsKey(backgroundName) ? backgroundSprites[backgroundName] : null;
    }

    public CharacterStanding GetCharacter(string characterName)
    {
        return characters[characterName];
    }

    public Talks GetTalk(string talkName)
    {
        return talks[talkName];
    }

    public string GetTip(string tipName)
    {
        return tips.ContainsKey(tipName) ? tips[tipName] : null;
    }
}