using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UILogCell : EnhancedScrollerCellView, IPointerClickHandler
    {
        [HideInInspector] public UILogWindow logWindow;
        [SerializeField] private TextMeshProUGUI talkerText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image faceIcon;
        [SerializeField] private Image faceSprite;

        public void SetData(LogCellData data)
        {
            talkerText.text = string.IsNullOrEmpty(data.name) ? " " : "> " + data.name;
            descriptionText.text = (string.IsNullOrEmpty(data.text)) ? " " : data.text;
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
            if (linkIndex == -1) return;
            
            var linkInfo = descriptionText.textInfo.linkInfo[linkIndex];
            logWindow.EventOpen(linkInfo.GetLinkID(), eventData.position);
        }
    }
}