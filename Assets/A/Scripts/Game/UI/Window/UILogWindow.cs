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

        [SerializeField] private UILogCell logCell;

        [Header("�̺�Ʈ")][SerializeField] private Image eventWindow;
        [SerializeField] private TextMeshProUGUI eventTitleText;
        [SerializeField] private TextMeshProUGUI eventDescriptionText;
        [SerializeField] private Button eventExitButton;

        private Vector3 nowPos;
        private Vector3 defaultPos;

        public override void OnCreated()
        {
            base.OnCreated();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(WindowManager.Instance.CloseAllWindow);

            eventWindow.gameObject.SetActive(false);

            eventExitButton.onClick.RemoveAllListeners();
            eventExitButton.onClick.AddListener(EventExit);

            logCell.logWindow = this;
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

            bool isHaveTips = GameManager.Instance.saveManager.GameData.getTips.Contains(eventName);

            eventTitleText.text = eventName + "�� ����";
            eventDescriptionText.text = isHaveTips ? ResourcesManager.Instance.GetTip(eventName) : "������ ����...";

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