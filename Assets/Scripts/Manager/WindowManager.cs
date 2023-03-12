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

    [Header("스프라이트")] [SerializeField] private Sprite buttonSelectSprite;
    [SerializeField] private Sprite buttonDeSelectSprite;
    [SerializeField] private Sprite scrollSelectSprite;
    [SerializeField] private Sprite scrollDeSelectSprite;

    [Space(20)] [SerializeField] private List<UIWindow> windows;

    protected override void OnReset()
    {
        CloseScrollAndWindow();
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
        scrollButton.onClick.RemoveAllListeners();
        scrollButton.onClick.AddListener(WindowScroll);

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

    private void CloseScrollAndWindow()
    {
        CloseAllWindow();

        isScrolling = false;

        scrollImage.rectTransform.DOAnchorPosX(-85.5f, 0.4f).SetUpdate(true);
        scrollImage.sprite = scrollDeSelectSprite;
    }

    public void WindowOpen()
    {
        if (isScrolling) return;

        scrollImage.rectTransform.DOAnchorPosX(-940.5f, 0.4f).SetUpdate(true);
        scrollImage.sprite = scrollSelectSprite;

        isScrolling = !isScrolling;
    }

    private void WindowScroll()
    {
        if (isScrolling)
        {
            scrollImage.rectTransform.DOAnchorPosX(-85.5f, 0.4f).SetUpdate(true);
            scrollImage.sprite = scrollDeSelectSprite;
        }
        else
        {
            scrollImage.rectTransform.DOAnchorPosX(-940.5f, 0.4f).SetUpdate(true);
            scrollImage.sprite = scrollSelectSprite;
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

    public void ClickWindow(WindowType type, Vector3 pos = default)
    {
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
        if (uiWindow == null)
        {
            Debug.Log("not found window");
            return;
        }

        uiWindow.Init(pos != default ? pos : button.transform.position);
    }
}