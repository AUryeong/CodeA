using GamesTan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITipCell : MonoBehaviour, IScrollCell
    {
        [HideInInspector] public UIExtraWindow extraWindow;
        [SerializeField] TextMeshProUGUI tipNameText;
        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
        }

        public void SetData(string data)
        {
            tipNameText.text = data;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                extraWindow.EventOpen(data);
            });
        }
    }
}