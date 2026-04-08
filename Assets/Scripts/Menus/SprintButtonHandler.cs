using UnityEngine;
using UnityEngine.EventSystems;

public class SprintButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerControl.Instance.isSprinting = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerControl.Instance.isSprinting = false;
    }
}