using DG.Tweening;
using UnityEngine;

namespace UI
{
    public enum WindowType
    {
        None,
        Title,
        Setting,
        Load,
        Save,
        Log
    }

    public class UIWindow : MonoBehaviour
    {
        public WindowType type;
        private RectTransform rectTransform;
        private Vector3 pos = Vector3.zero;

        public virtual void OnCreated()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void Init(Vector3 buttonPos)
        {
            pos = buttonPos;

            rectTransform.DOKill();
            gameObject.SetActive(true);

            rectTransform.position = buttonPos;
            rectTransform.localScale = Vector3.zero;

            rectTransform.DOScale(Vector3.one, 0.2f).SetUpdate(true);
            rectTransform.DOAnchorPos(Vector2.zero, 0.2f).SetUpdate(true);
        }

        public void Disable()
        {
            rectTransform.DOKill();
            rectTransform.localScale = Vector3.one;

            rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => gameObject.SetActive(false)).SetUpdate(true);
            rectTransform.DOMove(pos, 0.2f).SetUpdate(true);
        }
    }
}