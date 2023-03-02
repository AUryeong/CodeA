using UnityEngine;
using UnityEngine.EventSystems;

public class TalkClickEvent : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TalkManager.Instance.CheckClick(eventData);
    }
}