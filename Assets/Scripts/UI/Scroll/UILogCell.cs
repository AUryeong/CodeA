using GamesTan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogCell : MonoBehaviour, IScrollCell
    {

        private LogCellData data;
        [SerializeField] TextMeshProUGUI talkerText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image faceIcon;
        [SerializeField] Image faceSprite;

        public void SetData(LogCellData data)
        {
            bool flag = string.IsNullOrEmpty(data.name);
            talkerText.gameObject.SetActive(!flag);
            if (!flag)
                talkerText.text = data.name;

            descriptionText.color = flag ? new Color(0.7f, 0.7f, 0.7f) : Color.white;
            descriptionText.rectTransform.sizeDelta = new Vector2(descriptionText.rectTransform.sizeDelta.x, flag ? 190 : 140);
            descriptionText.text = data.text;
            if (string.IsNullOrEmpty(data.standingName)) return;

        }
    }
}