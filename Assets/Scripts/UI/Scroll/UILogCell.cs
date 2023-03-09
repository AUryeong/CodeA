using GamesTan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogCell : MonoBehaviour, IScrollCell
    {
        [SerializeField] TextMeshProUGUI talkerText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image faceIcon;
        [SerializeField] Image faceSprite;

        public void SetData(LogCellData data)
        {
            talkerText.text = "> " + data.name;
            descriptionText.text = data.text;
            if (string.IsNullOrEmpty(data.standingName))
            {
                faceIcon.gameObject.SetActive(false);
                return;
            }

            faceIcon.gameObject.SetActive(true);
            faceSprite.sprite = ResourcesManager.Instance.GetCharacter(data.standingName).standings[data.clothesName].logFace;
        }
    }
}