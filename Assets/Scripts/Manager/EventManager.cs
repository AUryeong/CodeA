using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    protected override bool IsDontDestroying => true;
    [SerializeField] private Image eventWindow;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button okayButton;
    [SerializeField] private Button exitButton;
    private TextMeshProUGUI okayButtonText;

    private Vector2 nowPos;
    private Vector2 defaultPos;
    private string nowEventName;

    protected override void OnCreated()
    {
        okayButtonText = okayButton.GetComponent<TextMeshProUGUI>();
        defaultPos = eventWindow.rectTransform.anchoredPosition;
        eventWindow.gameObject.SetActive(false);
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Exit);
        
        okayButton.onClick.RemoveAllListeners();
        okayButton.onClick.AddListener(Okay);
    }

    protected override void OnReset()
    {
        eventWindow.gameObject.SetActive(false);
    }
    
    public void EventOpen(string eventName, Vector2 pos)
    {
        nowEventName = eventName;
        nowPos = pos;

        titleText.text = eventName;
        descriptionText.text = ResourcesManager.Instance.GetTip(eventName);
        
        eventWindow.gameObject.SetActive(true);
        eventWindow.rectTransform.DOKill();
        
        Debug.Log(pos);
        eventWindow.rectTransform.anchoredPosition = pos;
        eventWindow.rectTransform.localScale = Vector3.zero;

        eventWindow.rectTransform.DOScale(Vector3.one, 0.2f);
        eventWindow.rectTransform.DOAnchorPos(defaultPos, 0.2f);
        
        okayButtonText.gameObject.SetActive(true);
        okayButtonText.DOKill();
        okayButtonText.color = new Color(0, 221/255f, 1, 1);
        okayButtonText.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
    }

    private void Okay()
    {
        Debug.Log(nowEventName);
    }

    private void Exit()
    {
        eventWindow.rectTransform.DOKill();
        eventWindow.rectTransform.localScale = Vector3.one;

        eventWindow.rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => eventWindow.gameObject.SetActive(false));
        eventWindow.rectTransform.DOAnchorPos(nowPos, 0.2f);
    }
}