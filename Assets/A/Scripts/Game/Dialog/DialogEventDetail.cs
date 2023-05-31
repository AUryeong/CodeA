using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogEventDetail : MonoBehaviour
{
    [SerializeField] private Image eventWindow;
    [SerializeField] private TextMeshProUGUI eventTitleText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;
    [SerializeField] private Button eventOkayButton;
    [SerializeField] private Button eventExitButton;
    [SerializeField] private TextMeshProUGUI eventContinueText;
    private TextMeshProUGUI eventOkayButtonText;

    public bool IsHasEvent { get; private set; }
    private DialogTipEvent selectEvent;

    private Vector2 nowPos;
    private Vector2 defaultPos;

    private DialogText dialogText;

    public void OnCreated()
    {
        if (eventOkayButton != null)
        {
            eventOkayButtonText = eventOkayButton.GetComponent<TextMeshProUGUI>();

            eventOkayButton.onClick.RemoveAllListeners();
            eventOkayButton.onClick.AddListener(EventOkay);
        }

        defaultPos = eventWindow.rectTransform.anchoredPosition;
        eventWindow.gameObject.SetActive(false);

        eventExitButton.onClick.RemoveAllListeners();
        eventExitButton.onClick.AddListener(EventExit);
    }

    public void OnReset()
    {
        eventWindow.gameObject.SetActive(false);
    }

    public void NewDialog(DialogText setDialogText)
    {
        dialogText = setDialogText;

        IsHasEvent = false;
        if (dialogText != null)
        {
            IsHasEvent = dialogText.tipEvent != null && dialogText.tipEvent.Exists((tipEvent) =>
                GameManager.Instance.saveManager.GameData.getTips.Contains(tipEvent.eventName));
        }
    }

    public void EventOpen(string eventName, Vector2 pos, bool isCanContinue)
    {
        nowPos = pos;

        bool isHaveTips = GameManager.Instance.saveManager.GameData.getTips.Contains(eventName);
        if (dialogText != null)
        {
            selectEvent = dialogText.tipEvent.Find((tip) => tip.eventName == eventName);
            isCanContinue = !string.IsNullOrEmpty(selectEvent.dialogName) && !isCanContinue;
        }

        eventTitleText.text = eventName + "에 대해";
        eventDescriptionText.text = isHaveTips ? GameManager.Instance.resourcesManager.GetTip(eventName) : "정보가 없다...";

        eventWindow.gameObject.SetActive(true);
        eventWindow.rectTransform.DOKill();

        eventWindow.rectTransform.anchoredPosition = pos;
        eventWindow.rectTransform.localScale = Vector3.zero;

        eventWindow.rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        eventWindow.rectTransform.DOAnchorPos(defaultPos, 0.3f).SetEase(Ease.OutBack);

        if (eventContinueText != null && eventOkayButton != null)
        {
            bool isContinue = isHaveTips && isCanContinue;
            
            eventContinueText.gameObject.SetActive(isContinue);
            eventOkayButtonText.gameObject.SetActive(isContinue);
            if (isContinue)
            {
                eventOkayButtonText.DOKill();
                eventOkayButtonText.color = new Color(105 / 255f, 1, 126 / 255f, 1);
                eventOkayButtonText.DOFade(0, 0.8f).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    private void EventOkay()
    {
        EventExit();
        var tipEvent = dialogText.tipEvent.Find((tip) => tip == selectEvent);
        var dialogs = GameManager.Instance.resourcesManager.GetDialog(tipEvent.dialogName).dialogs;

        GameManager.Instance.dialogManager.DialogAdd(tipEvent.dialogEventType, dialogs);
    }

    public void EventExit()
    {
        if (!eventWindow.gameObject.activeSelf) return;

        eventWindow.rectTransform.DOKill();
        eventWindow.rectTransform.localScale = Vector3.one;

        eventWindow.rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => eventWindow.gameObject.SetActive(false));
        eventWindow.rectTransform.DOAnchorPos(nowPos, 0.2f);
    }

    public bool IsActivating()
    {
        return eventWindow.gameObject.activeSelf;
    }
}