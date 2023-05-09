using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITipCell : EnhancedScrollerCellView
    {
        [HideInInspector] public UIExtraWindow extraWindow;
        [SerializeField] private TextMeshProUGUI tipNameText;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void SetData(string data)
        {
            tipNameText.text = data + "에 대해";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                extraWindow.EventOpen(data);
            });
        }
    }
}