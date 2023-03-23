using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogWindow : UIWindow
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private UILogVerticalScroll logScroll;

        [Header("이벤트")][SerializeField] private Image eventWindow;
        [SerializeField] private TextMeshProUGUI eventTitleText;
        [SerializeField] private TextMeshProUGUI eventDescriptionText;
        [SerializeField] private Button eventExitButton;

        Vector3 nowPos;
        Vector3 defaultPos;

        public override void OnCreated()
        {
            base.OnCreated();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(WindowManager.Instance.CloseAllWindow);

            defaultPos = eventWindow.rectTransform.anchoredPosition;
            eventWindow.gameObject.SetActive(false);

            eventExitButton.onClick.RemoveAllListeners();
            eventExitButton.onClick.AddListener(EventExit);

            defaultPos = eventWindow.rectTransform.anchoredPosition;
        }

        public override void Init(Vector3 pos)
        {
            base.Init(pos);
            logScroll.SetLog(LogManager.Instance.GetDatas());
        }

        public void EventOpen(string eventName, Vector2 pos)
        {
            nowPos = pos;

            bool isHaveTips = SaveManager.Instance.GameData.getTips.Contains(eventName);

            eventTitleText.text = eventName + "에 대해";
            eventDescriptionText.text = isHaveTips ? ResourcesManager.Instance.GetTip(eventName) : "정보가 없다...";

            eventWindow.gameObject.SetActive(true);
            eventWindow.rectTransform.DOKill();

            eventWindow.rectTransform.anchoredPosition = pos;
            eventWindow.rectTransform.localScale = Vector3.zero;

            eventWindow.rectTransform.DOScale(Vector3.one, 0.2f);
            eventWindow.rectTransform.DOAnchorPos(defaultPos, 0.2f);
        }

        private void EventExit()
        {
            eventWindow.rectTransform.DOKill();
            eventWindow.rectTransform.localScale = Vector3.one;

            eventWindow.rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => eventWindow.gameObject.SetActive(false));
            eventWindow.rectTransform.DOAnchorPos(nowPos, 0.2f);
        }
    }
}