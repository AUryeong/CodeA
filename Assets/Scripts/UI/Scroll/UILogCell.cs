using GamesTan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UILogCell : MonoBehaviour, IScrollCell, IPointerClickHandler
    {
        [SerializeField] UILogWindow logWindow;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(descriptionText, Input.mousePosition, GameManager.Instance.UICamera);

            if (linkIndex != -1)
            {
                var linkInfo = descriptionText.textInfo.linkInfo[linkIndex];
                logWindow.EventOpen(linkInfo.GetLinkID(), eventData.position);
            }
        }
    }
}