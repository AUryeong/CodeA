using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Coffee.UIEffects;

namespace UI
{
    public class UIOption : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
    {
        [SerializeField] TextMeshProUGUI scriptText;
        [SerializeField] TextMeshProUGUI effectText;

        public Option option { get; private set; }

        public RectTransform rectTransform { get; private set; }
        private Image image;

        private Color defaultTextColor = Color.white;
        private Color specialTextColor = new Color(105f / 255f, 1, 126 / 255f);


        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        public void Disable()
        {
            if (option == null)
            {
                gameObject.SetActive(false);
            }

            option = null;
            rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                image.DOFade(0, 0.5f);
                scriptText.DOFade(0, 0.5f);
                effectText.DOFade(0, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
            });
        }

        public void SetOption(Option setOption)
        {
            option = setOption;

            scriptText.DOKill();
            scriptText.text = setOption.script;

            var color = defaultTextColor;
            if ((setOption.eventList != null && setOption.eventList.Count > 0) || setOption.special)
                color = specialTextColor;
            
            scriptText.color = Utility.ChangeColorFade(color, 0);
            scriptText.DOFade(1, 0.5f);

            rectTransform.DOKill();
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

            image.DOKill();
            image.color = Utility.ChangeColorFade(image.color, 0);
            image.DOFade(1, 0.3f);

            effectText.DOKill();
            effectText.color = Utility.ChangeColorFade(color, 0);
            effectText.DOFade(1, 0.3f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rectTransform.DOScale(Vector3.one, 0.2f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            rectTransform.DOScale(Vector3.one * 1.15f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                image.DOFade(0, 0.5f);
                scriptText.DOFade(0, 0.5f);
                effectText.DOFade(0, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
            });
            TalkManager.Instance.SelectOption(this);
            option = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rectTransform.DOScale(Vector3.one * 0.95f, 0.2f);
        }
    }
}