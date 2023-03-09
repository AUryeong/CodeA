using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : Singleton<WindowManager>
{
    protected override bool IsDontDestroying => true;

    [SerializeField] private Button scrollButton;
    private bool isScrolling;
    private Image scrollImage;

    [SerializeField] private List<Button> windowButtons;
    private WindowType selectType;

    [SerializeField] private Image windowBlock;
    private Button windowBlockButton;
    private bool windowBlockDisabling = false;

    [Header("스프라이트")] [SerializeField] private Sprite buttonSelectSprite;
    [SerializeField] private Sprite buttonDeSelectSprite;
    [SerializeField] private Sprite scrollSelectSprite;
    [SerializeField] private Sprite scrollDeSelectSprite;

    [Space(20)] [SerializeField] private List<UIWindow> windows;
    
    protected override void OnReset()
    {
        CloseScrollAndWindow();
    }

    public void OpenWindow(WindowType type)
    {
        if (isScrolling) return;

        selectType = WindowType.NONE;
        WindowScroll();
        ClickWindow(type);
    }

    private void Update()
    {
        EscapeCheck();
    }

    private void EscapeCheck()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (!isScrolling)
        {
            WindowScroll();
            return;
        }

        if (selectType == WindowType.NONE)
            WindowScroll();
        else
            CloseAllWindow();
    }

    protected override void OnCreated()
    {
        windowBlock.gameObject.SetActive(false);
        windowBlockButton = windowBlock.GetComponent<Button>();

        scrollButton.onClick.RemoveAllListeners();
        scrollButton.onClick.AddListener(WindowScroll);

        windowBlockButton.onClick.RemoveAllListeners();
        windowBlockButton.onClick.AddListener(CloseScrollAndWindow);

        scrollImage = scrollButton.GetComponent<Image>();
        for (int i = 0; i < windowButtons.Count; i++)
        {
            var type = (WindowType)(i + 1);
            windowButtons[i].onClick.AddListener(() => ClickWindow(type));
        }

        isScrolling = false;
        foreach (UIWindow window in windows)
            window.OnCreated();
    }

    public void CloseScrollAndWindow()
    {
        if (windowBlockDisabling) return;

        CloseAllWindow();

        if (isScrolling) return;

        Time.timeScale = 1;
        isScrolling = false;
        windowBlockDisabling = true;

        scrollImage.rectTransform.DOAnchorPosX(-85.5f, 0.4f).SetUpdate(true);
        scrollImage.sprite = scrollDeSelectSprite;

        windowBlock.DOFade(0f, 0.4f).SetUpdate(true).OnComplete(() =>
        {
            windowBlock.gameObject.SetActive(false);
            windowBlockDisabling = false;
        });
    }

    public void WindowOpen()
    {
        if (isScrolling) return;

        Time.timeScale = 0;

        scrollImage.rectTransform.DOAnchorPosX(-940.5f, 0.4f).SetUpdate(true);
        scrollImage.sprite = scrollSelectSprite;

        windowBlock.gameObject.SetActive(true);
        windowBlock.DOFade(0.4f, 0.4f).SetUpdate(true);

        isScrolling = !isScrolling;
    }

    private void WindowScroll()
    {
        if (isScrolling)
        {
            if (!windowBlockDisabling)
            {
                scrollImage.rectTransform.DOAnchorPosX(-85.5f, 0.4f).SetUpdate(true);
                scrollImage.sprite = scrollDeSelectSprite;

                if (selectType == WindowType.NONE)
                {
                    Time.timeScale = 1;
                    windowBlockDisabling = true;

                    windowBlock.DOFade(0f, 0.4f).SetUpdate(true).OnComplete(() =>
                    {
                        windowBlock.gameObject.SetActive(false);
                        windowBlockDisabling = false;
                    });
                }
            }
        }
        else
        {
            Time.timeScale = 0;

            scrollImage.rectTransform.DOAnchorPosX(-940.5f, 0.4f).SetUpdate(true);
            scrollImage.sprite = scrollSelectSprite;

            windowBlock.gameObject.SetActive(true);
            windowBlock.DOFade(0.4f, 0.4f).SetUpdate(true);
        }

        isScrolling = !isScrolling;
    }

    public void CloseAllWindow()
    {
        if (selectType.Equals(WindowType.NONE)) return;

        var prevButton = windowButtons[(int)selectType - 1];
        if (prevButton != null)
            prevButton.image.sprite = buttonDeSelectSprite;

        var prevWindow = windows.Find((window => window.type == selectType));
        if (prevWindow != null)
            prevWindow.Disable();

        selectType = WindowType.NONE;
    }

    public void ClickWindow(WindowType type)
    {
        if (!isScrolling) return;
        if (type.Equals(selectType))
        {
            CloseAllWindow();
            return;
        }

        var button = windowButtons[(int)type - 1];

        if (!selectType.Equals(WindowType.NONE))
        {
            var prevButton = windowButtons[(int)selectType - 1];
            if (prevButton != null)
                prevButton.image.sprite = buttonDeSelectSprite;

            var prevWindow = windows.Find((window => window.type == selectType));
            if (prevWindow != null)
                prevWindow.Disable();
        }

        button.image.sprite = buttonSelectSprite;
        selectType = type;

        var uiWindow = windows.Find((window => window.type == selectType));
        if (uiWindow != null)
            uiWindow.Init(button.image);
    }
}