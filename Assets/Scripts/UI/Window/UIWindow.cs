using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum WindowType
    {
        NONE,
        TITLE,
        SETTING,
        LOAD,
        SAVE,
        LOG
    }
    public class UIWindow : MonoBehaviour
    {
        public WindowType type;
        private RectTransform rectTransform;
        private Image windowButton;

        public virtual void OnCreated()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void Init(Image button)
        {
            if (windowButton == null)
                windowButton = button;
            
            rectTransform.DOKill();
            gameObject.SetActive(true);

            var position = button.rectTransform.position;

            rectTransform.position = position;
            rectTransform.localScale = Vector3.zero;

            rectTransform.DOScale(Vector3.one, 0.2f).SetUpdate(true);
            rectTransform.DOAnchorPos(Vector2.zero, 0.2f).SetUpdate(true);
        }

        public virtual void Disable()
        {
            if (windowButton == null) return;
            
            rectTransform.DOKill();
            rectTransform.localScale = Vector3.one;

            rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => gameObject.SetActive(false)).SetUpdate(true);
            rectTransform.DOMove(windowButton.rectTransform.position, 0.2f).SetUpdate(true);
        }
    }
}